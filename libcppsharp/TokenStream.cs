﻿//
//  TokenStream.cs
//
//  Author:
//       Roy Merkel <merkel-roy@comcast.net>
//
//  Copyright (c) 2018 Roy Merkel
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Ude;

namespace libcppsharp
{
    public class TokenStream
    {
        private delegate List<Token> HandleStateEOF(ref EnumeratorState state);
        private delegate List<Token> HandleStateNewChar(char ch, ref EnumeratorState state);

        private enum State
        {
            DEFAULT,
            WHITESPACE,
            DIGRAPH,
            STRINGPREFIX,
            STRING,
            STRINGPOSTFIX,
            RAWSTRINGPREFIX,
            RAWSTRING,
            RAWSTRINGPOSTFIX,
            IDENTIFIER
        }

        private struct StateCallbacks
        {
            public HandleStateEOF eofHandler;
            public HandleStateNewChar charHandler;

            public StateCallbacks(HandleStateEOF eofHandler, HandleStateNewChar charHandler)
            {
                this.eofHandler = eofHandler;
                this.charHandler = charHandler;
            }
        }

        private Dictionary<State, StateCallbacks> callbacks;

        private State state;
        private const int bufSize = 1023;
        private char[] charReadBuffer;
        private const int putBackBufSize = 10;
        private int putBackFront;
        private int putBackEnd;
        private int putBackCount;
        private char[] putBackBuffer;
        private int charBufDatSize;
        private int charBufPtr;
        private Stream inStr;
        private StringBuilder curTokVal;
        private bool digraphs;
        private int readResult;

        private TrigraphStream charStream;
        private IEnumerable<char> charEnumerable;
        private IEnumerator<char> charEnumerator;
        private bool eofEncountered;
        private bool ignoreStrayBackslash;
        private bool allowSlashNInString;
        private bool treatStringSlashNAsNothing;

        private readonly Dictionary<char, TokenType> punctuation = new Dictionary<char, TokenType>() {
            { '!', TokenType.EXCLAIMATION_MARK },
            { '^', TokenType.CARROT },
            { '&', TokenType.AMPERSTAND },
            { '*', TokenType.ASTERISK },
            { '(', TokenType.L_PAREN },
            { ')', TokenType.R_PAREN },
            { '-', TokenType.MINUS_SIGN },
            { '+', TokenType.PLUS_SIGN },
            { '=', TokenType.EQUAL_SIGN },
            { '{', TokenType.L_CURLY_BRACE },
            { '}', TokenType.R_CURLY_BRACE },
            { '|', TokenType.PIPE },
            { '~', TokenType.TILDE },
            { '[', TokenType.L_SQ_BRACKET },
            { ']', TokenType.R_SQ_BRACKET },
            { ':', TokenType.COLON },
            { ';', TokenType.SEMI_COLON },
            { '<', TokenType.LESS_THEN },
            { '>', TokenType.GREATER_THEN },
            { ',', TokenType.COMMA },
            { '.', TokenType.PERIOD },
            { '/', TokenType.FORWARD_SLASH },
            { '%', TokenType.PERCENT },
            { '#', TokenType.HASH },
            { '`', TokenType.GRAVE },
            { '@', TokenType.AT },
            { '?', TokenType.QUESTION_MARK },
        };

        public TokenStream(Stream inStream, bool handleTrigraphs = false, bool handleDigraphs = false, bool allowSlashNInString = true, bool treatStringSlashNAsNothing = true)
        {
            charStream = new TrigraphStream(inStream, handleTrigraphs, handleDigraphs);
            charEnumerable = charStream.GetCharEnumerable();
            charEnumerator = charEnumerable.GetEnumerator();
            eofEncountered = !charEnumerator.MoveNext();
            ignoreStrayBackslash = false;
            this.allowSlashNInString = allowSlashNInString;
            this.treatStringSlashNAsNothing = treatStringSlashNAsNothing;

            inStr = inStream;
            digraphs = handleDigraphs;
            curTokVal = new StringBuilder();
            charReadBuffer = new char[bufSize];
            charBufPtr = 0;
            charBufDatSize = 0;
            readResult = 0;

            putBackFront = 0;
            putBackEnd = 0;
            putBackCount = 0;
            putBackBuffer = new char[putBackBufSize];

            state = State.DEFAULT;
            callbacks = new Dictionary<State, StateCallbacks>() {
                {State.DEFAULT, new StateCallbacks(HandleDefaultStateEOFTokens, HandleDefaultNewChar) },
                {State.WHITESPACE, new StateCallbacks(HandleWhiteSpaceStateEOFTokens, HandleWhiteSpaceNewChar) },
                {State.DIGRAPH, new StateCallbacks(HandleDigraphStateEOFTokens, HandleDigraphNewChar) },
                {State.STRINGPREFIX, new StateCallbacks(HandleStringPrefixStateEOFTokens, HandleStringPrefixNewChar) },
                {State.STRING, new StateCallbacks(HandleStringStateEOFTokens, HandleStringNewChar) },
                {State.STRINGPOSTFIX, new StateCallbacks(HandleStringPostfixStateEOFTokens, HandleStringPostfixNewChar) },
            };
        }


        private bool PutBackArrayEmpty()
        {
            return putBackCount == 0;
        }

        private bool PushPutBackArray(char ch)
        {
            if (putBackCount == putBackBufSize)
            {
                return false;
            }
            else
            {
                putBackBuffer[putBackEnd] = ch;
                putBackEnd++;
                putBackCount++;

                if (putBackEnd == putBackBufSize)
                {
                    putBackEnd = 0;
                }

                return true;
            }
        }

        private char? PeekPutBackArray()
        {
            if (putBackCount == 0)
            {
                return null;
            }
            else
            {
                char ret = putBackBuffer[putBackFront];

                return ret;
            }
        }

        private char? PopPutBackArray()
        {
            if (putBackCount == 0)
            {
                return null;
            }
            else
            {
                char ret = putBackBuffer[putBackFront];
                putBackFront++;
                putBackCount--;

                if (putBackFront == putBackBufSize)
                {
                    putBackFront = 0;
                }

                return ret;
            }
        }

        private int RefillCharArray()
        {
            int read = 0;
            while (!eofEncountered && read < bufSize)
            {
                charReadBuffer[read] = charEnumerator.Current;

                eofEncountered = !charEnumerator.MoveNext();
                read++;
            }

            charBufDatSize = read;
            charBufPtr = 0;

            return read;
        }

        private struct EnumeratorState
        {
            public bool escaped;
        }

        private List<Token> HandleDefaultStateEOFTokens(ref EnumeratorState state)
        {
            List<Token> ret = new List<Token>();

            if (state.escaped && !ignoreStrayBackslash)
            {
                throw new InvalidDataException("Stray \\ in source");
            }
            else if (state.escaped)
            {
                state.escaped = false;

                Token tok;
                tok.tokenType = TokenType.BACK_SLASH;
                tok.value = "";

                ret.Add(tok);
            }

            return ret;
        }

        private List<Token> HandleWhiteSpaceStateEOFTokens(ref EnumeratorState state)
        {
            Token tok;
            List<Token> ret = new List<Token>();

            if (state.escaped && !ignoreStrayBackslash)
            {
                throw new InvalidDataException("Stray \\ in source");
            }
            else if (state.escaped)
            {
                state.escaped = false;

                tok.tokenType = TokenType.BACK_SLASH;
                tok.value = "";

                ret.Add(tok);
            }

            tok.tokenType = TokenType.WHITESPACE;
            tok.value = curTokVal.ToString();

            return ret;
        }

        private List<Token> HandleDigraphStateEOFTokens(ref EnumeratorState state)
        {
            List<Token> ret = new List<Token>();

            foreach (char ch in curTokVal.ToString())
            {
                Token tok;
                punctuation.TryGetValue(ch, out tok.tokenType);
                tok.value = "";

                ret.Add(tok);
            }

            if (state.escaped)
            {
                throw new InvalidDataException("stray \\ found at EOF.");
            }

            return ret;
        }

        private List<Token> HandleStringPrefixStateEOFTokens(ref EnumeratorState state)
        {
            List<Token> ret = new List<Token>();

            if (curTokVal.Length > 0)
            {
                string value = curTokVal.ToString();

                Token tok;
                tok.tokenType = TokenType.IDENTIFIER;
                tok.value = value;

                ret.Add(tok);
            }

            if (state.escaped)
            {
                throw new InvalidDataException("stray \\ found at EOF.");
            }

            return ret;
        }

        private List<Token> HandleStringStateEOFTokens(ref EnumeratorState state)
        {
            if (state.escaped)
            {
                throw new InvalidDataException("stray \\ found at EOF.");
            }

            throw new InvalidDataException("Unfinished string...");
        }

        private List<Token> HandleStringPostfixStateEOFTokens(ref EnumeratorState state)
        {
            List<Token> ret = new List<Token>();
            String val = curTokVal.ToString();

            if (state.escaped)
            {
                throw new InvalidDataException("stray \\ found at EOF.");
            }

            Token tok;
            tok.tokenType = TokenType.STRING;
            tok.value = val;

            ret.Add(tok);

            return ret;
        }

        private List<Token> HandleDefaultNewChar(char ch, ref EnumeratorState state)
        {
            List<Token> ret = new List<Token>();
            Token tok;

            switch (ch)
            {
                case ' ':
                case '\t':
                case '\v':
                case '\f':
                    curTokVal.Clear();
                    curTokVal.Append(ch);

                    this.state = State.WHITESPACE;
                    MoveNextChar();
                    break;
                case '\r':
                case '\n':
                    if (state.escaped)
                    {
                        curTokVal.Clear();
                        curTokVal.Append(ch);
                        this.state = State.WHITESPACE;
                    }
                    else
                    {
                        tok.tokenType = TokenType.NEWLINE;
                        tok.value = "";

                        ret.Add(tok);
                    }

                    MoveNextChar();
                    break;
                case '<':
                    if (state.escaped)
                    {
                        if (!ignoreStrayBackslash)
                        {
                            throw new InvalidDataException("Stray \\ found in code.");
                        }
                        else
                        {
                            tok.tokenType = TokenType.BACK_SLASH;
                            tok.value = "";

                            state.escaped = false;
                            ret.Add(tok);
                        }
                    }
                    else
                    {
                        if (digraphs)
                        {
                            curTokVal.Clear();
                            curTokVal.Append('<');

                            this.state = State.DIGRAPH;
                        }
                        else
                        {
                            tok.tokenType = TokenType.LESS_THEN;
                            tok.value = "";

                            ret.Add(tok);
                        }

                        MoveNextChar();
                    }
                    break;
                case ':':
                    if (state.escaped)
                    {
                        if (!ignoreStrayBackslash)
                        {
                            throw new InvalidDataException("Stray \\ found in code.");
                        }
                        else
                        {
                            tok.tokenType = TokenType.BACK_SLASH;
                            tok.value = "";

                            state.escaped = false;

                            ret.Add(tok);
                        }
                    }
                    else
                    {
                        if (digraphs)
                        {
                            curTokVal.Clear();
                            curTokVal.Append(':');

                            this.state = State.DIGRAPH;
                        }
                        else
                        {
                            tok.tokenType = TokenType.COLON;
                            tok.value = "";

                            ret.Add(tok);

                            tok.tokenType = TokenType.UNKNOWN;
                        }

                        MoveNextChar();
                    }
                    break;
                case '%':
                    if (state.escaped)
                    {
                        if (!ignoreStrayBackslash)
                        {
                            throw new InvalidDataException("Stray \\ found in code.");
                        }
                        else
                        {
                            tok.tokenType = TokenType.BACK_SLASH;
                            tok.value = "";

                            state.escaped = false;

                            ret.Add(tok);
                        }
                    }
                    else
                    {
                        if (digraphs)
                        {
                            curTokVal.Clear();
                            curTokVal.Append('%');

                            this.state = State.DIGRAPH;
                        }
                        else
                        {
                            tok.tokenType = TokenType.PERCENT;
                            tok.value = "";

                            ret.Add(tok);
                        }

                        MoveNextChar();
                    }
                    break;
                case '\\':
                    if (state.escaped)
                    {
                        if (!ignoreStrayBackslash)
                        {
                            throw new InvalidDataException("Stray \\ found in code.");
                        }
                        else
                        {
                            tok.tokenType = TokenType.BACK_SLASH;
                            tok.value = "";

                            state.escaped = false;

                            ret.Add(tok);
                        }
                    }
                    else
                    {
                        state.escaped = true;
                        MoveNextChar();
                    }
                    break;
                case '!':
                case '^':
                case '&':
                case '*':
                case '(':
                case ')':
                case '-':
                case '+':
                case '=':
                case '{':
                case '}':
                case '|':
                case '~':
                case '[':
                case ']':
                case ';':
                case '>':
                case ',':
                case '.':
                case '/':
                case '#':
                case '`':
                case '@':
                case '?':
                    {
                        if (state.escaped)
                        {
                            if (!ignoreStrayBackslash)
                            {
                                throw new InvalidDataException("Stray \\ found in code.");
                            }
                            else
                            {
                                tok.tokenType = TokenType.BACK_SLASH;
                                tok.value = "";

                                state.escaped = false;

                                ret.Add(tok);
                            }
                        }
                        else
                        {
                            punctuation.TryGetValue(ch, out tok.tokenType);
                            tok.value = "";
                            ret.Add(tok);

                            tok.tokenType = TokenType.UNKNOWN;

                            MoveNextChar();
                        }
                    }
                    break;
                case 'u':
                case 'U':
                case 'L':
                    if (state.escaped)
                    {
                        if (!ignoreStrayBackslash)
                        {
                            throw new InvalidDataException("Stray \\ found in code.");
                        }
                        else
                        {
                            tok.tokenType = TokenType.BACK_SLASH;
                            tok.value = "";

                            state.escaped = false;

                            ret.Add(tok);
                        }
                    }
                    else
                    {
                        curTokVal.Clear();
                        curTokVal.Append(ch);

                        MoveNextChar();
                        this.state = State.STRINGPREFIX;
                    }
                    break;
                case '"':
                    if (state.escaped)
                    {
                        if (!ignoreStrayBackslash)
                        {
                            throw new InvalidDataException("Stray \\ found in code.");
                        }
                        else
                        {
                            tok.tokenType = TokenType.BACK_SLASH;
                            tok.value = "";

                            state.escaped = false;

                            ret.Add(tok);
                        }
                    }
                    else
                    {
                        curTokVal.Clear();
                        curTokVal.Append(ch);
                        MoveNextChar();

                        this.state = State.STRING;
                    }
                    break;
                default:
                    throw new NotImplementedException(new String(new char[] { ch }) + " not yet handled.");
            }

            return ret;
        }

        private List<Token> HandleWhiteSpaceNewChar(char ch, ref EnumeratorState state)
        {
            List<Token> ret = new List<Token>();
            Token tok;

            switch (ch)
            {
                case '\\':
                    if (state.escaped)
                    {
                        tok.value = curTokVal.ToString();
                        tok.tokenType = TokenType.WHITESPACE;

                        ret.Add(tok);

                        PushPutBackArray('\\');
                        this.state = State.DEFAULT;
                    }
                    else
                    {
                        state.escaped = true;
                        MoveNextChar();
                    }

                    break;
                case '\n':
                    if (state.escaped)
                    {
                        curTokVal.Append(ch);
                        MoveNextChar();
                    }
                    else
                    {
                        tok.value = curTokVal.ToString();
                        tok.tokenType = TokenType.WHITESPACE;

                        ret.Add(tok);

                        this.state = State.DEFAULT;
                    }
                    break;
                case ' ':
                case '\t':
                case '\v':
                case '\f':
                    curTokVal.Append(ch);

                    MoveNextChar();
                    break;
                default:
                    {
                        tok.value = curTokVal.ToString();
                        tok.tokenType = TokenType.WHITESPACE;

                        ret.Add(tok);

                        this.state = State.DEFAULT;
                    }
                    break;
            }

            return ret;
        }

        private List<Token> HandleDigraphNewChar(char ch, ref EnumeratorState state)
        {
            // TODO: Handle escaped newline between two pieces of the digram!
            bool invalidDigram = false;
            Token tok;
            List<Token> ret = new List<Token>();

            if (curTokVal.Length != 1)
            {
                throw new InvalidDataException("Somehow got into the digraph state without the right number of characters in the buffer (should be 1.)");
            }
            else if ("<:%".IndexOf(curTokVal[0]) < 0)
            {
                throw new InvalidDataException("Somehow got into the digraph state with the wrong character in the digraph buffer, should be '<', ':', or '%'");
            }
            else
            {
                if (state.escaped && ch != '\n')
                {
                    PushPutBackArray('\\');
                    invalidDigram = true;
                    state.escaped = false;
                }
                else
                {
                    switch (ch)
                    {
                        case '\n':
                            if (state.escaped)
                            {
                                state.escaped = false;
                                MoveNextChar();
                            }
                            else
                            {
                                invalidDigram = true;
                            }
                            break;
                        case '\\':
                            if (state.escaped)
                            {
                                PushPutBackArray('\\');
                                invalidDigram = true;
                            }
                            else
                            {
                                state.escaped = true;
                                MoveNextChar();
                            }
                            break;
                        case ':':
                            switch (curTokVal[0])
                            {
                                case '<':
                                    tok.tokenType = TokenType.L_SQ_BRACKET;
                                    tok.value = "";

                                    ret.Add(tok);

                                    MoveNextChar();
                                    this.state = State.DEFAULT;
                                    break;
                                case '%':
                                    tok.value = "";

                                    tok.tokenType = TokenType.HASH;
                                    ret.Add(tok);

                                    MoveNextChar();
                                    this.state = State.DEFAULT;
                                    break;
                                default:
                                    invalidDigram = true;
                                    break;
                            }
                            break;
                        case '>':
                            switch (curTokVal[0])
                            {
                                case ':':
                                    tok.tokenType = TokenType.R_SQ_BRACKET;
                                    tok.value = "";

                                    ret.Add(tok);

                                    MoveNextChar();
                                    this.state = State.DEFAULT;
                                    break;
                                case '%':
                                    tok.tokenType = TokenType.R_CURLY_BRACE;
                                    tok.value = "";

                                    ret.Add(tok);

                                    MoveNextChar();
                                    this.state = State.DEFAULT;
                                    break;
                                default:
                                    invalidDigram = true;
                                    break;
                            }
                            break;
                        case '%':
                            switch (curTokVal[0])
                            {
                                case '<':
                                    tok.tokenType = TokenType.L_CURLY_BRACE;
                                    tok.value = "";

                                    ret.Add(tok);

                                    MoveNextChar();

                                    this.state = State.DEFAULT;
                                    break;
                                default:
                                    invalidDigram = true;
                                    break;
                            }
                            break;
                        default:
                            invalidDigram = true;
                            break;
                    }
                }
            }

            if (invalidDigram)
            {
                tok.value = "";

                punctuation.TryGetValue(curTokVal[0], out tok.tokenType);
                ret.Add(tok);

                curTokVal.Clear();
                this.state = State.DEFAULT;
            }

            return ret;
        }

        private List<Token> HandleStringPrefixNewChar(char ch, ref EnumeratorState state)
        {
            List<Token> ret = new List<Token>();

            if (state.escaped && ch != '\n')
            {
                PushPutBackArray('\\');

                if (curTokVal.Length > 0)
                {
                    string value = curTokVal.ToString();

                    Token tok;
                    tok.tokenType = TokenType.IDENTIFIER;
                    tok.value = value;

                    ret.Add(tok);
                }

                state.escaped = false;
                this.state = State.DEFAULT;
            }

            switch (ch)
            {
                case '\\':
                    if (state.escaped)
                    {
                        if (curTokVal.Length > 0)
                        {
                            string value = curTokVal.ToString();

                            Token tok;
                            tok.tokenType = TokenType.IDENTIFIER;
                            tok.value = value;

                            ret.Add(tok);
                        }

                        PushPutBackArray('\\');
                        state.escaped = false;
                        this.state = State.DEFAULT;
                    }
                    else
                    {
                        state.escaped = true;
                        MoveNextChar();
                    }
                    break;
                case '\n':
                    if (!state.escaped)
                    {
                        this.state = State.IDENTIFIER;
                    }
                    else
                    {
                        state.escaped = false;
                        MoveNextChar();
                    }
                    break;
                case '8':
                    if (curTokVal[0] != 'u')
                    {
                        this.state = State.IDENTIFIER;
                    }
                    else
                    {
                        MoveNextChar();
                    }
                    curTokVal.Append(ch);
                    break;
                case 'R':
                    switch (curTokVal.ToString())
                    {
                        case "":
                        case "u8":
                        case "L":
                        case "u":
                        case "U":
                            this.state = State.RAWSTRINGPREFIX;
                            MoveNextChar();
                            break;
                        default:
                            this.state = State.IDENTIFIER;
                            break;
                    }
                    curTokVal.Append(ch);
                    break;
                case '"':
                    this.state = State.STRING;
                    curTokVal.Append(ch);
                    MoveNextChar();
                    break;
                default:
                    this.state = State.IDENTIFIER;
                    break;
            }
            return ret;
        }

        private List<Token> HandleStringNewChar(char ch, ref EnumeratorState state)
        {
            List<Token> ret = new List<Token>();

            switch (ch)
            {
                case '\\':
                    if (state.escaped)
                    {
                        curTokVal.Append('\\');
                        curTokVal.Append('\\');
                    }
                    else
                    {
                        state.escaped = true;
                    }
                    MoveNextChar();
                    break;
                case '\0':
                    curTokVal.Append('\\');
                    curTokVal.Append('0');

                    MoveNextChar();
                    break;
                case '\n':
                    if (!state.escaped || !allowSlashNInString)
                    {
                        throw new InvalidDataException("unterminated newline found in string...");
                    }
                    else
                    {
                        if (treatStringSlashNAsNothing)
                        {
                            curTokVal.Append('n');
                        }
                        state.escaped = false;
                        MoveNextChar();
                    }
                    break;
                case '"':
                    if (state.escaped)
                    {
                        state.escaped = false;
                        curTokVal.Append('\\');
                    }
                    else
                    {
                        this.state = State.STRINGPOSTFIX;
                    }

                    curTokVal.Append(ch);
                    MoveNextChar();
                    break;
                default:
                    if (state.escaped)
                    {
                        curTokVal.Append('\\');
                        state.escaped = false;
                    }

                    curTokVal.Append(ch);
                    MoveNextChar();
                    break;
            }
            return ret;
        }

        private List<Token> HandleStringPostfixNewChar(char ch, ref EnumeratorState state)
        {
            List<Token> ret = new List<Token>();
            Token tok;

            switch (ch)
            {
                case 's':
                    if (state.escaped)
                    {
                        tok.value = curTokVal.ToString();
                        tok.tokenType = TokenType.STRING;
                        ret.Add(tok);

                        this.state = State.DEFAULT;
                    }
                    else
                    {
                        curTokVal.Append(ch);
                        MoveNextChar();
                    }

                    break;
                case '\\':
                    if (state.escaped)
                    {
                        tok.value = curTokVal.ToString();
                        tok.tokenType = TokenType.STRING;
                        ret.Add(tok);

                        tok.tokenType = TokenType.UNKNOWN;

                        this.state = State.DEFAULT;
                    }
                    else
                    {
                        state.escaped = true;
                        MoveNextChar();
                    }
                    break;
                case '\n':
                    if (!state.escaped)
                    {
                        tok.value = curTokVal.ToString();
                        tok.tokenType = TokenType.STRING;
                        ret.Add(tok);

                        tok.tokenType = TokenType.UNKNOWN;

                        this.state = State.DEFAULT;
                    }
                    else
                    {
                        MoveNextChar();
                    }
                    break;
                default:
                    tok.value = curTokVal.ToString();
                    tok.tokenType = TokenType.STRING;
                    ret.Add(tok);

                    this.state = State.DEFAULT;
                    break;
            }
            return ret;
        }

        private void MoveNextChar()
        {
            if (!PutBackArrayEmpty())
            {
                PopPutBackArray();
            }
            else
            {
                charBufPtr++;
            }
        }

        private char GetNextChar()
        {
            char ch;

            if (!PutBackArrayEmpty())
            {
                ch = PeekPutBackArray().Value;
            }
            else
            {
                ch = charReadBuffer[charBufPtr];
            }

            return ch;
        }

        public IEnumerable<Token> GetTokenEnumerable()
        {
            StateCallbacks sc;
            EnumeratorState enumeratorState = new EnumeratorState();
            bool refillBuffer = false;
            enumeratorState.escaped = false;

            readResult = RefillCharArray();

            if (readResult <= 0)
            {
                callbacks.TryGetValue(state, out sc);
                foreach (Token curtok in sc.eofHandler(ref enumeratorState))
                {
                    yield return curtok;
                }

                Token tok;
                tok.tokenType = TokenType.EOF;
                tok.value = "";

                yield return tok;
            }

            while (readResult > 0)
            {
                if (charBufPtr >= charBufDatSize && PutBackArrayEmpty())
                {
                    refillBuffer = true;
                }

                if (refillBuffer)
                {
                    refillBuffer = false;
                    readResult = RefillCharArray();

                    if (readResult <= 0)
                    {
                        callbacks.TryGetValue(state, out sc);
                        foreach (Token curtok in sc.eofHandler(ref enumeratorState))
                        {
                            yield return curtok;
                        }

                        Token tok;
                        tok.tokenType = TokenType.EOF;
                        tok.value = "";

                        yield return tok;
                        break;
                    }
                }

                char ch = GetNextChar();

                callbacks.TryGetValue(state, out sc);
                foreach (Token tok in sc.charHandler(ch, ref enumeratorState))
                {
                    yield return tok;
                }
            }
        }
    }
}
