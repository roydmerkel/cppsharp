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
            DIGRAPH
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
        private int charBufDatSize;
        private int charBufPtr;
        private Stream inStr;
        private StringBuilder curTokVal;
        private bool digraphs;
        private int readResult;
        private ulong column;
        private ulong line;
        private ulong endcolumn;
        private ulong endline;

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
            column = 1;
            line = 1;

            state = State.DEFAULT;
            callbacks = new Dictionary<State, StateCallbacks>() {
                {State.DEFAULT, new StateCallbacks(HandleDefaultStateEOFTokens, HandleDefaultNewChar) },
                {State.DIGRAPH, new StateCallbacks(HandleDigraphStateEOFTokens, HandleDigraphNewChar) }
        };
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
                throw new InvalidDataException("Stray \\ in source, at column " + column.ToString() + " line " + line.ToString());
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
                lastTok.column = column;
                lastTok.line = line;

                ret.Add(lastTok);

                lastTok.tokenType = TokenType.UNKNOWN;
                column++;
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
                lastTok.column = column;
                lastTok.line = line;
                lastTok.value = "";

                ret.Add(lastTok);
            }

            return ret;
        }

        private List<Token> HandleDefaultNewChar(char ch, ref Token lastTok, ref EnumeratorState state)
        {
            List<Token> ret = new List<Token>();
            Token? retTok;

            if (" \t\v\f\r\n".IndexOf(ch) < 0 && state.escaped && !ignoreStrayBackslash)
            {
                throw new InvalidDataException("Stray \\ in source, at column " + column.ToString() + " line " + line.ToString());
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
                lastTok.column = column;
                lastTok.line = line;

                column++;
            }
            else if (state.escaped)
            {
                column++;
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
                        lastTok.column = column;
                        lastTok.line = line;
                    }

                    curTokVal.Append(ch);
                    column++;
                    charBufPtr++;
                    break;
                case '\r':
                case '\n':
                    retTok = ReturnLastToken(ref lastTok);
                    if (retTok != null)
                    {
                        ret.Add(lastTok);
                    }

                    lastTok.tokenType = TokenType.NEWLINE;
                    lastTok.column = column;
                    lastTok.line = line;
                    lastTok.value = "";

                    ret.Add(lastTok);

                    lastTok.tokenType = TokenType.UNKNOWN;

                    column = 1;
                    line++;
                    charBufPtr++;
                    break;
                case '<':
                    if (digraphs)
                    {
                        curTokVal.Clear();
                        curTokVal.Append('<');

                        endcolumn = column + 1;
                        endline = line;

                        this.state = State.DIGRAPH;
                    }
                    else
                    {
                        retTok = ReturnLastToken(ref lastTok);
                        if (retTok != null)
                        {
                            ret.Add(lastTok);
                        }

                        lastTok.tokenType = TokenType.LESS_THEN;
                        lastTok.column = column;
                        lastTok.line = line;

                        ret.Add(lastTok);

                        lastTok.tokenType = TokenType.UNKNOWN;

                        column++;
                    }

                    charBufPtr++;
                    break;
                case ':':
                    if (digraphs)
                    {
                        curTokVal.Clear();
                        curTokVal.Append(':');

                        endcolumn = column + 1;
                        endline = line;

                        this.state = State.DIGRAPH;
                    }
                    else
                    {
                        retTok = ReturnLastToken(ref lastTok);
                        if (retTok != null)
                        {
                            ret.Add(lastTok);
                        }

                        lastTok.tokenType = TokenType.COLON;
                        lastTok.column = column;
                        lastTok.line = line;

                        ret.Add(lastTok);

                        lastTok.tokenType = TokenType.UNKNOWN;

                        column++;
                    }

                    charBufPtr++;
                    break;
                case '%':
                    if (digraphs)
                    {
                        curTokVal.Clear();
                        curTokVal.Append('%');

                        endcolumn = column + 1;
                        endline = line;

                        this.state = State.DIGRAPH;
                    }
                    else
                    {
                        retTok = ReturnLastToken(ref lastTok);
                        if (retTok != null)
                        {
                            ret.Add(lastTok);
                        }

                        lastTok.tokenType = TokenType.PERCENT;
                        lastTok.column = column;
                        lastTok.line = line;

                        ret.Add(lastTok);

                        lastTok.tokenType = TokenType.UNKNOWN;

                        column++;
                    }

                    charBufPtr++;
                    break;
                case '\\':
                    {
                        state.escaped = true;
                        charBufPtr++;
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
                        lastTok.column = column;
                        lastTok.line = line;

                        ret.Add(lastTok);

                        lastTok.tokenType = TokenType.UNKNOWN;

                        column++;
                        charBufPtr++;
                    }
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
                    throw new InvalidDataException("Stray \\ at column " + endcolumn.ToString() + ", line " + endline.ToString());
                }

                switch (ch)
                {
                    case '\n':
                        if (state.escaped)
                        {
                            endcolumn = 1;
                            endline++;
                            state.escaped = false;
                            charBufPtr++;
                        }
                        else
                        {
                            invalidDigram = true;
                        }
                        break;
                    case '\\':
                        if (state.escaped)
                        {
                            throw new InvalidDataException("Stray \\ at column " + endcolumn.ToString() + ", line " + endline.ToString());
                        }
                        else
                        {
                            state.escaped = true;
                            endcolumn++;
                            charBufPtr++;
                        }
                        break;
                    case ':':
                        switch (curTokVal[0])
                        {
                            case '<':
                                lastTok.tokenType = TokenType.L_SQ_BRACKET;
                                break;
                            case '%':
                                lastTok.tokenType = TokenType.HASH;
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
                                break;
                            case '%':
                                lastTok.tokenType = TokenType.R_CURLY_BRACE;
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

                if (ch != '\\' && ch != '\n')
                {
                    if (!invalidDigram)
                    {
                        lastTok.column = column;
                        lastTok.line = line;
                        lastTok.value = "";
                        ret.Add(lastTok);

                        endcolumn++;
                        column = endcolumn;
                        line = endline;

                        charBufPtr++;
                    }
                    else
                    {
                        lastTok.line = line;
                        lastTok.value = "";

                        lastTok.column = column;
                        punctuation.TryGetValue(curTokVal[0], out lastTok.tokenType);
                        ret.Add(lastTok);

                        column = endcolumn;
                        line = endline;

                        if (state.escaped)
                        {
                            lastTok.line = line;
                            lastTok.value = "";

                            if (ignoreStrayBackslash)
                            {
                                lastTok.column = column;
                                lastTok.tokenType = TokenType.BACK_SLASH;
                                ret.Add(lastTok);
                            }
                            else
                            {
                                throw new InvalidDataException("Stray backslash found at " + column.ToString());
                            }

                            column++;
                        }
                    }

                    lastTok.tokenType = TokenType.UNKNOWN;
                    this.state = State.DEFAULT;
                }
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

        public System.Collections.Generic.IEnumerable<Token> GetTokenEnumerable()
        {
            StateCallbacks sc;
            EnumeratorState enumeratorState = new EnumeratorState();
            bool refillBuffer = false;
            Token lastTok;
            lastTok.tokenType = TokenType.UNKNOWN;
            lastTok.value = null;
            lastTok.column = 0;
            lastTok.line = 0;
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
                lastTok.column = column;
                lastTok.line = line;

                yield return lastTok;
            }

            while (readResult > 0)
            {
                if (charBufPtr >= charBufDatSize)
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
                        lastTok.column = column;
                        lastTok.line = line;

                        yield return lastTok;
                        break;
                    }
                }

                char ch = charReadBuffer[charBufPtr];

                callbacks.TryGetValue(state, out sc);
                foreach (Token tok in sc.charHandler(ch, ref lastTok, ref enumeratorState))
                {
                    yield return tok;
                }
            }
        }
    }
}
