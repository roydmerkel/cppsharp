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
        private delegate List<Token> HandleStateEOF(ref EnumeratorState state);
        private delegate List<Token> HandleStateNewChar(char ch, ref EnumeratorState state);

        private enum State
        {
            DEFAULT,
            WHITESPACE,
            DIGRAPH,
            STRING,
            STRINGPOSTFIX,
            RAWSTRINGPREFIX,
            RAWSTRING,
            CHAR,
            IDENTIFIER,
            INTEGER,
            DECIMAL,
            EXPONENT,
            NUMBERPOSTFIX,
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
                {State.STRING, new StateCallbacks(HandleStringStateEOFTokens, HandleStringNewChar) },
                {State.STRINGPOSTFIX, new StateCallbacks(HandleStringPostfixStateEOFTokens, HandleStringPostfixNewChar) },
                {State.RAWSTRINGPREFIX, new StateCallbacks(HandleRawStringPrefixStateEOFTokens, HandleRawStringPrefixNewChar) },
                {State.RAWSTRING, new StateCallbacks(HandleRawStringStateEOFTokens, HandleRawStringNewChar) },
                {State.CHAR, new StateCallbacks(HandleCharEOFTokens, HandleCharNewChar ) },
                {State.IDENTIFIER, new StateCallbacks(HandleIdentifierStateEOFTokens, HandleIdentifiersNewChar) },
                {State.INTEGER, new StateCallbacks(HandleIntegerStateEOFTokens, HandleIntegerNewChar) },
                {State.DECIMAL, new StateCallbacks(HandleDecimalStateEOFTokens, HandleDecimalNewChar) },
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
            public StringBuilder prefixVal;
            public StringBuilder postfixVal;
            public int prefixValIdx;
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

            ret.Add(tok);

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

            if (state.escaped)
            {
                throw new InvalidDataException("stray \\ found at EOF.");
            }

            if (state.postfixVal.ToString().Equals("s"))
            {
                curTokVal.Append(state.postfixVal);

                Token tok;
                tok.tokenType = TokenType.STRING;
                tok.value = curTokVal.ToString();

                ret.Add(tok);
            }
            else
            {
                Token tok;
                tok.tokenType = TokenType.STRING;
                tok.value = curTokVal.ToString();

                ret.Add(tok);

                tok.tokenType = TokenType.IDENTIFIER;
                tok.value = state.postfixVal.ToString();
            }

            return ret;
        }

        private List<Token> HandleRawStringPrefixStateEOFTokens(ref EnumeratorState state)
        {
            if (state.escaped)
            {
                throw new InvalidDataException("stray \\ found at EOF.");
            }

            throw new InvalidDataException("Unfinished raw string...");
        }

        private List<Token> HandleRawStringStateEOFTokens(ref EnumeratorState state)
        {
            if (state.escaped)
            {
                throw new InvalidDataException("stray \\ found at EOF.");
            }

            throw new InvalidDataException("Unfinished raw string...");
        }

        private List<Token> HandleCharEOFTokens(ref EnumeratorState state)
        {
            if (state.escaped)
            {
                throw new InvalidDataException("stray \\ found at EOF.");
            }

            throw new InvalidDataException("Unfinished char literal...");
        }

        private List<Token> HandleIdentifierStateEOFTokens(ref EnumeratorState state)
        {
            List<Token> ret = new List<Token>();

            if (state.escaped)
            {
                throw new InvalidDataException("stray \\ found at EOF.");
            }

            Token tok;
            tok.tokenType = TokenType.IDENTIFIER;
            tok.value = curTokVal.ToString();

            ret.Add(tok);

            return ret;
        }

        private List<Token> HandleIntegerStateEOFTokens(ref EnumeratorState state)
        {
            List<Token> ret = new List<Token>();

            if (state.escaped)
            {
                throw new InvalidDataException("stray \\ found at EOF.");
            }

            Token tok;
            tok.tokenType = TokenType.NUMBER;
            tok.value = curTokVal.ToString();

            ret.Add(tok);

            return ret;
        }

        private List<Token> HandleDecimalStateEOFTokens(ref EnumeratorState state)
        {
            List<Token> ret = new List<Token>();

            if (state.escaped)
            {
                throw new InvalidDataException("stray \\ found at EOF.");
            }

            Token tok;
            tok.tokenType = TokenType.NUMBER;
            tok.value = curTokVal.ToString();

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
                case '.':
                    curTokVal.Clear();
                    curTokVal.Append('.');

                    this.state = State.DECIMAL;

                    MoveNextChar();
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

                            MoveNextChar();
                        }
                    }
                    break;
                case '_':
                case 'a':
                case 'b':
                case 'c':
                case 'd':
                case 'e':
                case 'f':
                case 'g':
                case 'h':
                case 'i':
                case 'j':
                case 'k':
                case 'l':
                case 'm':
                case 'n':
                case 'o':
                case 'p':
                case 'q':
                case 'r':
                case 's':
                case 't':
                case 'u':
                case 'v':
                case 'w':
                case 'x':
                case 'y':
                case 'z':
                case 'A':
                case 'B':
                case 'C':
                case 'D':
                case 'E':
                case 'F':
                case 'G':
                case 'H':
                case 'I':
                case 'J':
                case 'K':
                case 'L':
                case 'M':
                case 'N':
                case 'O':
                case 'P':
                case 'Q':
                case 'R':
                case 'S':
                case 'T':
                case 'U':
                case 'V':
                case 'W':
                case 'X':
                case 'Y':
                case 'Z':
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
                        this.state = State.IDENTIFIER;
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
                case '\'':
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

                        this.state = State.CHAR;
                    }
                    break;
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
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

                        this.state = State.INTEGER;
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
                        if (!treatStringSlashNAsNothing)
                        {
                            curTokVal.Append('\\');
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
                        state.postfixVal.Clear();
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
                case '_':
                case 'a':
                case 'b':
                case 'c':
                case 'd':
                case 'e':
                case 'f':
                case 'g':
                case 'h':
                case 'i':
                case 'j':
                case 'k':
                case 'l':
                case 'm':
                case 'n':
                case 'o':
                case 'p':
                case 'q':
                case 'r':
                case 's':
                case 't':
                case 'u':
                case 'v':
                case 'w':
                case 'x':
                case 'y':
                case 'z':
                case 'A':
                case 'B':
                case 'C':
                case 'D':
                case 'E':
                case 'F':
                case 'G':
                case 'H':
                case 'I':
                case 'J':
                case 'K':
                case 'L':
                case 'M':
                case 'N':
                case 'O':
                case 'P':
                case 'Q':
                case 'R':
                case 'S':
                case 'T':
                case 'U':
                case 'V':
                case 'W':
                case 'X':
                case 'Y':
                case 'Z':
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    if (state.escaped)
                    {
                        tok.value = curTokVal.ToString();
                        tok.tokenType = TokenType.STRING;
                        ret.Add(tok);

                        PushPutBackArray('\\');
                        this.state = State.DEFAULT;
                    }
                    else
                    {
                        state.postfixVal.Append(ch);
                        MoveNextChar();
                    }

                    break;
                case '\\':
                    if (state.escaped)
                    {
                        switch (state.postfixVal.ToString())
                        {
                            case "s":
                                curTokVal.Append(state.postfixVal);

                                tok.value = curTokVal.ToString();
                                tok.tokenType = TokenType.STRING;
                                ret.Add(tok);
                                break;
                            default:
                                tok.value = curTokVal.ToString();
                                tok.tokenType = TokenType.STRING;
                                ret.Add(tok);

                                curTokVal.Clear();

                                if (state.postfixVal.Length > 0)
                                {
                                    curTokVal.Append(state.postfixVal);

                                    tok.value = curTokVal.ToString();
                                    tok.tokenType = TokenType.IDENTIFIER;
                                    ret.Add(tok);
                                }
                                break;
                        }

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
                        switch (state.postfixVal.ToString())
                        {
                            case "s":
                                curTokVal.Append(state.postfixVal);

                                tok.value = curTokVal.ToString();
                                tok.tokenType = TokenType.STRING;
                                ret.Add(tok);
                                break;
                            default:
                                tok.value = curTokVal.ToString();
                                tok.tokenType = TokenType.STRING;
                                ret.Add(tok);

                                curTokVal.Clear();

                                if (state.postfixVal.Length > 0)
                                {
                                    curTokVal.Append(state.postfixVal);

                                    tok.value = curTokVal.ToString();
                                    tok.tokenType = TokenType.IDENTIFIER;
                                    ret.Add(tok);
                                }
                                break;
                        }

                        this.state = State.DEFAULT;
                    }
                    else
                    {
                        MoveNextChar();
                    }
                    break;
                default:
                    switch (state.postfixVal.ToString())
                    {
                        case "s":
                            curTokVal.Append(state.postfixVal);

                            tok.value = curTokVal.ToString();
                            tok.tokenType = TokenType.STRING;
                            ret.Add(tok);
                            break;
                        default:
                            tok.value = curTokVal.ToString();
                            tok.tokenType = TokenType.STRING;
                            ret.Add(tok);

                            curTokVal.Clear();

                            if (state.postfixVal.Length > 0)
                            {
                                curTokVal.Append(state.postfixVal);

                                tok.value = curTokVal.ToString();
                                tok.tokenType = TokenType.IDENTIFIER;
                                ret.Add(tok);
                            }
                            break;
                    }

                    this.state = State.DEFAULT;
                    break;
            }
            return ret;
        }

        private List<Token> HandleRawStringPrefixNewChar(char ch, ref EnumeratorState state)
        {
            List<Token> ret = new List<Token>();

            switch (ch)
            {
                case ' ':
                case '\t':
                case '\v':
                case '\f':
                    throw new InvalidDataException("Unterminated raw string!");
                case '"':
                    throw new InvalidDataException("Unterminated raw string!");
                case '\\':
                    if (state.escaped)
                    {
                        throw new InvalidDataException("Unterminated raw string!");
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
                        throw new InvalidDataException("Unterminated raw string!");
                    }
                    else
                    {
                        state.escaped = false;
                        MoveNextChar();
                    }
                    break;
                case '(':
                    curTokVal.Append(ch);
                    MoveNextChar();
                    this.state = State.RAWSTRING;
                    state.prefixValIdx = -1;
                    break;
                default:
                    if (state.escaped)
                    {
                        throw new InvalidDataException("Unterminated raw string!");
                    }

                    state.prefixVal.Append(ch);
                    curTokVal.Append(ch);
                    MoveNextChar();
                    break;
            }
            return ret;
        }

        private List<Token> HandleRawStringNewChar(char ch, ref EnumeratorState state)
        {
            List<Token> ret = new List<Token>();

            if (ch == ')')
            {
                if (state.prefixValIdx == -1)
                {
                    state.prefixValIdx++;
                }
                else
                {
                    state.prefixValIdx = 0;
                }
            }
            else if (ch == '"')
            {
                if (state.prefixValIdx == state.prefixVal.Length)
                {
                    state.postfixVal.Clear();
                    this.state = State.STRINGPOSTFIX;
                }
                else
                {
                    state.prefixValIdx = -1;
                }
            }
            else
            {
                if (state.prefixValIdx >= 0 && ch == state.prefixVal[state.prefixValIdx])
                {
                    state.prefixValIdx++;
                }
                else
                {
                    state.prefixValIdx = -1;
                }
            }

            curTokVal.Append(ch);
            MoveNextChar();

            return ret;
        }

        private List<Token> HandleCharNewChar(char ch, ref EnumeratorState state)
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
                    if (!state.escaped)
                    {
                        throw new InvalidDataException("unterminated newline found in char literal...");
                    }
                    else
                    {
                        state.escaped = false;
                        MoveNextChar();
                    }
                    break;
                case '\'':
                    if (state.escaped)
                    {
                        state.escaped = false;
                        curTokVal.Append('\\');
                        curTokVal.Append(ch);
                    }
                    else
                    {
                        Token tok;
                        tok.tokenType = TokenType.CHAR;
                        curTokVal.Append(ch);
                        tok.value = curTokVal.ToString();
                        ret.Add(tok);

                        this.state = State.DEFAULT;
                    }

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

        private List<Token> HandleIdentifiersNewChar(char ch, ref EnumeratorState state)
        {
            List<Token> ret = new List<Token>();
            Token tok;

            switch (ch)
            {
                case '_':
                case 'a':
                case 'b':
                case 'c':
                case 'd':
                case 'e':
                case 'f':
                case 'g':
                case 'h':
                case 'i':
                case 'j':
                case 'k':
                case 'l':
                case 'm':
                case 'n':
                case 'o':
                case 'p':
                case 'q':
                case 'r':
                case 's':
                case 't':
                case 'u':
                case 'v':
                case 'w':
                case 'x':
                case 'y':
                case 'z':
                case 'A':
                case 'B':
                case 'C':
                case 'D':
                case 'E':
                case 'F':
                case 'G':
                case 'H':
                case 'I':
                case 'J':
                case 'K':
                case 'L':
                case 'M':
                case 'N':
                case 'O':
                case 'P':
                case 'Q':
                case 'R':
                case 'S':
                case 'T':
                case 'U':
                case 'V':
                case 'W':
                case 'X':
                case 'Y':
                case 'Z':
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    if (state.escaped)
                    {
                        PushPutBackArray('\\');

                        tok.value = curTokVal.ToString();
                        tok.tokenType = TokenType.IDENTIFIER;
                        ret.Add(tok);

                        this.state = State.DEFAULT;
                    }
                    else
                    {
                        curTokVal.Append(ch);
                        MoveNextChar();
                    }

                    break;
                case '"':
                    if (state.escaped)
                    {
                        PushPutBackArray('\\');

                        tok.value = curTokVal.ToString();
                        tok.tokenType = TokenType.IDENTIFIER;
                        ret.Add(tok);

                        this.state = State.DEFAULT;
                    }
                    else
                    {
                        switch (curTokVal.ToString())
                        {
                            case "u":
                            case "u8":
                            case "U":
                            case "L":
                                curTokVal.Append('"');
                                this.state = State.STRING;
                                MoveNextChar();
                                break;
                            case "R":
                            case "uR":
                            case "u8R":
                            case "UR":
                            case "LR":
                                curTokVal.Append('"');
                                state.prefixVal.Clear();
                                this.state = State.RAWSTRINGPREFIX;
                                MoveNextChar();
                                break;
                            default:
                                tok.value = curTokVal.ToString();
                                tok.tokenType = TokenType.IDENTIFIER;

                                ret.Add(tok);

                                curTokVal.Clear();
                                curTokVal.Append('"');
                                MoveNextChar();

                                this.state = State.STRING;

                                break;
                        }
                    }
                    break;
                case '\'':
                    if (state.escaped)
                    {
                        PushPutBackArray('\\');

                        tok.value = curTokVal.ToString();
                        tok.tokenType = TokenType.IDENTIFIER;
                        ret.Add(tok);

                        this.state = State.DEFAULT;
                    }
                    else
                    {
                        switch (curTokVal.ToString())
                        {
                            case "u":
                            case "u8":
                            case "U":
                            case "L":
                                curTokVal.Append('\'');
                                this.state = State.CHAR;
                                MoveNextChar();
                                break;
                            default:
                                tok.value = curTokVal.ToString();
                                tok.tokenType = TokenType.IDENTIFIER;

                                ret.Add(tok);

                                curTokVal.Clear();
                                curTokVal.Append('\'');
                                MoveNextChar();

                                this.state = State.CHAR;

                                break;
                        }
                    }
                    break;
                case '\\':
                    if (state.escaped)
                    {
                        PushPutBackArray('\\');

                        tok.value = curTokVal.ToString();
                        tok.tokenType = TokenType.IDENTIFIER;
                        ret.Add(tok);

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
                        tok.tokenType = TokenType.IDENTIFIER;
                        ret.Add(tok);

                        this.state = State.DEFAULT;
                    }
                    else
                    {
                        state.escaped = false;
                        MoveNextChar();
                    }
                    break;
                default:
                    tok.value = curTokVal.ToString();
                    tok.tokenType = TokenType.IDENTIFIER;
                    ret.Add(tok);

                    this.state = State.DEFAULT;
                    break;
            }
            return ret;
        }

        private List<Token> HandleIntegerNewChar(char ch, ref EnumeratorState state)
        {
            List<Token> ret = new List<Token>();
            Token tok;

            switch (ch)
            {
                case 'a':
                case 'A':
                case 'c':
                case 'C':
                case 'd':
                case 'D':
                    curTokVal.Append(ch);
                    MoveNextChar();
                    break;
                case 'x':
                case 'X':
                    if (curTokVal.Length == 1 && curTokVal[0] == '0')
                    {
                        curTokVal.Append(ch);
                        MoveNextChar();
                    }
                    else
                    {
                        tok.tokenType = TokenType.NUMBER;
                        tok.value = curTokVal.ToString();

                        ret.Add(tok);

                        curTokVal.Clear();
                        curTokVal.Append(ch);

                        this.state = State.IDENTIFIER;

                        MoveNextChar();
                    }
                    break;
                case 'b':
                case 'B':
                    if ((curTokVal.Length == 1 && curTokVal[0] == '0') || 
                        (curTokVal.Length >= 2 && (curTokVal[1] == 'x' || curTokVal[1] == 'X')))
                    {
                        curTokVal.Append(ch);
                        MoveNextChar();
                    }
                    else
                    {
                        tok.tokenType = TokenType.NUMBER;
                        tok.value = curTokVal.ToString();

                        ret.Add(tok);

                        curTokVal.Clear();
                        curTokVal.Append(ch);

                        this.state = State.IDENTIFIER;

                        MoveNextChar();
                    }
                    break;
                case 'e':
                case 'E':
                    if (curTokVal.Length >= 2 && (curTokVal[1] == 'x' || curTokVal[1] == 'X'))
                    {
                        curTokVal.Append(ch);
                        MoveNextChar();
                    }
                    else
                    {
                        this.state = State.EXPONENT;
                    }
                    break;
                case '.':
                    if (curTokVal.Length >= 2 && curTokVal[2] == 'b')
                    {
                        tok.tokenType = TokenType.NUMBER;
                        tok.value = curTokVal.ToString();

                        ret.Add(tok);

                        curTokVal.Clear();

                        this.state = State.DEFAULT;
                    }
                    else
                    {
                        curTokVal.Append(ch);
                        MoveNextChar();

                        this.state = State.DECIMAL;
                    }

                    break;
                case '\\':
                    if (state.escaped)
                    {
                        PushPutBackArray('\\');

                        tok.value = curTokVal.ToString();
                        tok.tokenType = TokenType.IDENTIFIER;
                        ret.Add(tok);

                        ret.Add(tok);

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
                        tok.tokenType = TokenType.NUMBER;
                        ret.Add(tok);

                        ret.Add(tok);

                        this.state = State.DEFAULT;
                    }
                    else
                    {
                        state.escaped = false;
                        MoveNextChar();
                    }
                    break;
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    curTokVal.Append(ch);
                    MoveNextChar();

                    break;
                default:
                    tok.value = curTokVal.ToString();
                    tok.tokenType = TokenType.NUMBER;
                    ret.Add(tok);

                    this.state = State.DEFAULT;
                    break;
            }
            return ret;
        }

        private List<Token> HandleDecimalNewChar(char ch, ref EnumeratorState state)
        {
            List<Token> ret = new List<Token>();
            Token tok;

            switch (ch)
            {
                case 'a':
                case 'A':
                case 'c':
                case 'C':
                case 'd':
                case 'D':
                    curTokVal.Append(ch);
                    MoveNextChar();
                    break;
                case 'b':
                case 'B':
                    if (curTokVal.Length >= 2 && (curTokVal[1] == 'x' || curTokVal[1] == 'X'))
                    {
                        curTokVal.Append(ch);
                        MoveNextChar();
                    }
                    else
                    {
                        tok.tokenType = TokenType.NUMBER;
                        tok.value = curTokVal.ToString();

                        ret.Add(tok);

                        curTokVal.Clear();
                        curTokVal.Append(ch);

                        this.state = State.IDENTIFIER;

                        MoveNextChar();
                    }
                    break;
                case 'e':
                case 'E':
                    if (curTokVal.Length >= 2 && (curTokVal[1] == 'x' || curTokVal[1] == 'X'))
                    {
                        curTokVal.Append(ch);
                        MoveNextChar();
                    }
                    else
                    {
                        this.state = State.EXPONENT;
                    }
                    break;
                case ' ':
                case '\t':
                case '\v':
                case '\f':
                    if (curTokVal.Length >= 2 && (curTokVal[1] == 'x' || curTokVal[1] == 'X'))
                    {
                        this.state = State.EXPONENT;
                    }
                    else
                    {
                        tok.tokenType = TokenType.NUMBER;
                        tok.value = curTokVal.ToString();

                        ret.Add(tok);

                        curTokVal.Clear();
                        curTokVal.Append(ch);

                        this.state = State.WHITESPACE;

                        MoveNextChar();
                    }
                    break;
                case '\\':
                    if (state.escaped)
                    {
                        PushPutBackArray('\\');

                        tok.value = curTokVal.ToString();
                        tok.tokenType = TokenType.IDENTIFIER;
                        ret.Add(tok);

                        ret.Add(tok);

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
                        tok.tokenType = TokenType.NUMBER;
                        ret.Add(tok);

                        ret.Add(tok);

                        this.state = State.DEFAULT;
                    }
                    else
                    {
                        state.escaped = false;
                        MoveNextChar();
                    }
                    break;
                default:
                    if (curTokVal.Length == 1 && curTokVal[0] == '.')
                    {
                        tok.value = "";
                        tok.tokenType = TokenType.PERIOD;
                        ret.Add(tok);
                    }
                    else
                    {
                        tok.value = curTokVal.ToString();
                        tok.tokenType = TokenType.NUMBER;
                        ret.Add(tok);
                    }

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
            enumeratorState.prefixVal = new StringBuilder();
            enumeratorState.postfixVal = new StringBuilder();
            enumeratorState.prefixValIdx = 0;

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
