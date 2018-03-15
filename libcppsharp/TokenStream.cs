//
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
        private delegate List<Token> HandleStateEOF(ref Token lastTok, ref EnumeratorState state);
        private delegate List<Token> HandleStateNewChar(char ch, ref Token lastTok, ref EnumeratorState state);

        private enum State
        {
            DEFAULT,
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

        public TokenStream(Stream inStream, bool handleTrigraphs = false, bool handleDigraphs = false)
        {
            charStream = new TrigraphStream(inStream, handleTrigraphs, handleDigraphs);
            charEnumerable = charStream.GetCharEnumerable();
            charEnumerator = charEnumerable.GetEnumerator();
            eofEncountered = !charEnumerator.MoveNext();
            ignoreStrayBackslash = false;

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
                {State.DIGRAPH, new StateCallbacks(HandleDigraphStateEOFTokens, HandleDigraphNewChar) }
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

        private List<Token> HandleDefaultStateEOFTokens(ref Token lastTok, ref EnumeratorState state)
        {
            List<Token> ret = new List<Token>();

            if (state.escaped && !ignoreStrayBackslash)
            {
                throw new InvalidDataException("Stray \\ in source");
            }
            else if (state.escaped)
            {
                Token? retTok = ReturnLastToken(ref lastTok);
                if (retTok != null)
                {
                    ret.Add(lastTok);
                }

                state.escaped = false;

                lastTok.tokenType = TokenType.BACK_SLASH;
                lastTok.value = "";

                ret.Add(lastTok);

                lastTok.tokenType = TokenType.UNKNOWN;
            }

            return ret;
        }

        private List<Token> HandleDigraphStateEOFTokens(ref Token lastTok, ref EnumeratorState state)
        {
            List<Token> ret = new List<Token>();

            Token? retTok = ReturnLastToken(ref lastTok);
            if (retTok != null)
            {
                ret.Add(lastTok);
            }

            foreach (char ch in curTokVal.ToString())
            {
                punctuation.TryGetValue(ch, out lastTok.tokenType);
                lastTok.value = "";

                ret.Add(lastTok);
            }

            if (state.escaped)
            {
                throw new InvalidDataException("stray \\ found at EOF.");
            }

            return ret;
        }

        private List<Token> HandleStringPrefixStateEOFTokens(ref Token lastTok, ref EnumeratorState state)
        {
            List<Token> ret = new List<Token>();

            Token? retTok = ReturnLastToken(ref lastTok);
            if (retTok != null)
            {
                ret.Add(lastTok);
            }

            lastTok.value = curTokVal.ToString();
            lastTok.tokenType = TokenType.IDENTIFIER;

            ret.Add(lastTok);

            return ret;
        }

        private List<Token> HandleDefaultNewChar(char ch, ref Token lastTok, ref EnumeratorState state)
        {
            List<Token> ret = new List<Token>();
            Token? retTok;

            if (" \t\v\f\r\n".IndexOf(ch) < 0 && state.escaped && !ignoreStrayBackslash)
            {
                throw new InvalidDataException("Stray \\ in source");
            }
            else if (" \t\v\f\r\n".IndexOf(ch) < 0 && state.escaped)
            {
                state.escaped = false;

                retTok = ReturnLastToken(ref lastTok);
                if (retTok != null)
                {
                    ret.Add(lastTok);
                }

                lastTok.tokenType = TokenType.BACK_SLASH;
                lastTok.value = "";

                ret.Add(lastTok);

                lastTok.tokenType = TokenType.UNKNOWN;
            }

            switch (ch)
            {
                case ' ':
                case '\t':
                case '\v':
                case '\f':
                    if (lastTok.tokenType != TokenType.WHITESPACE)
                    {
                        retTok = ReturnLastToken(ref lastTok);
                        if (retTok != null)
                        {
                            ret.Add(lastTok);
                        }

                        lastTok.tokenType = TokenType.WHITESPACE;
                    }

                    curTokVal.Append(ch);
                    MoveNextChar();
                    break;
                case '\r':
                case '\n':
                    retTok = ReturnLastToken(ref lastTok);
                    if (retTok != null)
                    {
                        ret.Add(lastTok);
                    }

                    lastTok.tokenType = TokenType.NEWLINE;
                    lastTok.value = "";

                    ret.Add(lastTok);

                    lastTok.tokenType = TokenType.UNKNOWN;

                    MoveNextChar();
                    break;
                case '<':
                    retTok = ReturnLastToken(ref lastTok);
                    if (retTok != null)
                    {
                        ret.Add(lastTok);
                    }

                    if (digraphs)
                    {
                        curTokVal.Clear();
                        curTokVal.Append('<');

                        this.state = State.DIGRAPH;
                    }
                    else
                    {
                        lastTok.tokenType = TokenType.LESS_THEN;

                        ret.Add(lastTok);

                        lastTok.tokenType = TokenType.UNKNOWN;

                    }

                    MoveNextChar();
                    break;
                case ':':
                    retTok = ReturnLastToken(ref lastTok);
                    if (retTok != null)
                    {
                        ret.Add(lastTok);
                    }

                    if (digraphs)
                    {
                        curTokVal.Clear();
                        curTokVal.Append(':');

                        this.state = State.DIGRAPH;
                    }
                    else
                    {
                        lastTok.tokenType = TokenType.COLON;

                        ret.Add(lastTok);

                        lastTok.tokenType = TokenType.UNKNOWN;
                    }

                    MoveNextChar();
                    break;
                case '%':
                    retTok = ReturnLastToken(ref lastTok);
                    if (retTok != null)
                    {
                        ret.Add(lastTok);
                    }

                    if (digraphs)
                    {
                        curTokVal.Clear();
                        curTokVal.Append('%');

                        this.state = State.DIGRAPH;
                    }
                    else
                    {
                        lastTok.tokenType = TokenType.PERCENT;

                        ret.Add(lastTok);

                        lastTok.tokenType = TokenType.UNKNOWN;
                    }

                    MoveNextChar();
                    break;
                case '\\':
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
                        retTok = ReturnLastToken(ref lastTok);
                        if (retTok != null)
                        {
                            ret.Add(lastTok);
                        }

                        punctuation.TryGetValue(ch, out lastTok.tokenType);
                        ret.Add(lastTok);

                        lastTok.tokenType = TokenType.UNKNOWN;

                        MoveNextChar();
                    }
                    break;
                case 'u':
                case 'U':
                case 'L':
                    curTokVal.Clear();
                    curTokVal.Append(ch);

                    this.state = State.STRINGPREFIX;
                    break;
                default:
                    retTok = ReturnLastToken(ref lastTok);
                    if (retTok != null)
                    {
                        ret.Add(lastTok);
                    }

                    throw new NotImplementedException(new String(new char[] { ch }) + " not yet handled.");
            }

            return ret;
        }

        private List<Token> HandleDigraphNewChar(char ch, ref Token lastTok, ref EnumeratorState state)
        {
            // TODO: Handle escaped newline between two pieces of the digram!
            bool invalidDigram = false;
            List<Token> ret = new List<Token>();

            Token? retTok = ReturnLastToken(ref lastTok);
            if (retTok != null)
            {
                ret.Add(lastTok);
            }

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
                                    lastTok.tokenType = TokenType.L_SQ_BRACKET;
                                    lastTok.value = "";

                                    ret.Add(lastTok);

                                    MoveNextChar();

                                    lastTok.tokenType = TokenType.UNKNOWN;
                                    this.state = State.DEFAULT;
                                    break;
                                case '%':
                                    lastTok.value = "";

                                    lastTok.tokenType = TokenType.HASH;
                                    ret.Add(lastTok);

                                    MoveNextChar();

                                    lastTok.tokenType = TokenType.UNKNOWN;
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
                                    lastTok.tokenType = TokenType.R_SQ_BRACKET;
                                    lastTok.value = "";

                                    ret.Add(lastTok);

                                    MoveNextChar();

                                    lastTok.tokenType = TokenType.UNKNOWN;
                                    this.state = State.DEFAULT;
                                    break;
                                case '%':
                                    lastTok.tokenType = TokenType.R_CURLY_BRACE;
                                    lastTok.value = "";

                                    ret.Add(lastTok);

                                    MoveNextChar();

                                    lastTok.tokenType = TokenType.UNKNOWN;
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
                                    lastTok.tokenType = TokenType.L_CURLY_BRACE;
                                    lastTok.value = "";

                                    ret.Add(lastTok);

                                    MoveNextChar();

                                    lastTok.tokenType = TokenType.UNKNOWN;
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
                lastTok.value = "";

                punctuation.TryGetValue(curTokVal[0], out lastTok.tokenType);
                ret.Add(lastTok);

                curTokVal.Clear();

                lastTok.tokenType = TokenType.UNKNOWN;
                this.state = State.DEFAULT;
            }

            return ret;
        }

        private List<Token> HandleStringPrefixNewChar(char ch, ref Token lastTok, ref EnumeratorState state)
        {
            List<Token> ret = new List<Token>();

            switch (ch)
            {
                case '\\':
                    if (state.escaped)
                    {
                        this.state = State.IDENTIFIER;
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
                    MoveNextChar();
                    curTokVal.Append(ch);
                    break;
                default:
                    this.state = State.IDENTIFIER;
                    break;
            }
            return ret;
        }

        private Token? ReturnLastToken(ref Token lastTok)
        {
            Token? ret = null;

            if (lastTok.tokenType != TokenType.UNKNOWN)
            {
                if (curTokVal.Length > 0)
                {
                    lastTok.value = curTokVal.ToString();
                    curTokVal.Clear();
                }
                else
                {
                    lastTok.value = "";
                }

                ret = lastTok;
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

        public System.Collections.Generic.IEnumerable<Token> GetTokenEnumerable()
        {
            StateCallbacks sc;
            EnumeratorState enumeratorState = new EnumeratorState();
            bool refillBuffer = false;
            Token lastTok;
            lastTok.tokenType = TokenType.UNKNOWN;
            lastTok.value = null;
            enumeratorState.escaped = false;

            readResult = RefillCharArray();

            if (readResult <= 0)
            {
                callbacks.TryGetValue(state, out sc);
                foreach (Token tok in sc.eofHandler(ref lastTok, ref enumeratorState))
                {
                    yield return tok;
                }

                lastTok.tokenType = TokenType.EOF;

                yield return lastTok;
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
                        foreach (Token tok in sc.eofHandler(ref lastTok, ref enumeratorState))
                        {
                            yield return tok;
                        }

                        lastTok.tokenType = TokenType.EOF;

                        yield return lastTok;
                        break;
                    }
                }

                char ch = GetNextChar();

                callbacks.TryGetValue(state, out sc);
                foreach (Token tok in sc.charHandler(ch, ref lastTok, ref enumeratorState))
                {
                    yield return tok;
                }
            }
        }
    }
}
