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

        private TrigraphStream charStream;
        private IEnumerable<char> charEnumerable;
        private IEnumerator<char> charEnumerator;
        private bool eofEncountered;
 
        public TokenStream(Stream inStream, bool handleTrigraphs = false, bool handleDigraphs = false)
        {
            charStream = new TrigraphStream(inStream, handleTrigraphs, handleDigraphs);
            charEnumerable = charStream.GetCharEnumerable();
            charEnumerator = charEnumerable.GetEnumerator();
            eofEncountered = !charEnumerator.MoveNext();

            inStr = inStream;
            digraphs = handleDigraphs;
            curTokVal = new StringBuilder();
            charReadBuffer = new char[bufSize];
            charBufPtr = 0;
            charBufDatSize = 0;
            readResult = 0;
            column = 1;
            line = 1;
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

        public System.Collections.Generic.IEnumerable<Token> GetTokenEnumerable()
        {
            char digraphCh = '\0';
            bool refillBuffer = false;
            Token lastTok;
            lastTok.tokenType = TokenType.UNKNOWN;
            lastTok.value = null;
            lastTok.column = 0;
            lastTok.line = 0;

            readResult = RefillCharArray();

            if (readResult <= 0)
            {
                if (digraphCh != '\0')
                {
                    switch (digraphCh)
                    {
                        case '<':
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

                                yield return lastTok;
                            }

                            lastTok.tokenType = TokenType.LESS_THEN;
                            lastTok.column = column;
                            lastTok.line = line;
                            column++;
                            break;
                        case ':':
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

                                yield return lastTok;
                            }

                            lastTok.tokenType = TokenType.COLON;
                            lastTok.column = column;
                            lastTok.line = line;
                            column++;
                            break;
                        case '%':
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

                                yield return lastTok;
                            }

                            lastTok.tokenType = TokenType.PERCENT;
                            lastTok.column = column;
                            lastTok.line = line;
                            column++;
                            break;
                    }
                }

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

                    yield return lastTok;
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
                        if (digraphCh != '\0')
                        {
                            switch (digraphCh)
                            {
                                case '<':
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

                                        yield return lastTok;
                                    }

                                    lastTok.tokenType = TokenType.LESS_THEN;
                                    lastTok.column = column;
                                    lastTok.line = line;
                                    column++;
                                    break;
                                case ':':
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

                                        yield return lastTok;
                                    }

                                    lastTok.tokenType = TokenType.COLON;
                                    lastTok.column = column;
                                    lastTok.line = line;
                                    column++;
                                    break;
                                case '%':
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

                                        yield return lastTok;
                                    }

                                    lastTok.tokenType = TokenType.PERCENT;
                                    lastTok.column = column;
                                    lastTok.line = line;
                                    column++;
                                    break;
                            }
                        }

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

                            yield return lastTok;
                            lastTok.tokenType = TokenType.UNKNOWN;
                        }

                        lastTok.tokenType = TokenType.EOF;
                        lastTok.column = column;
                        lastTok.line = line;

                        yield return lastTok;
                        break;
                    }
                }

                char ch = charReadBuffer[charBufPtr];

                if (digraphCh != '\0')
                {
                    switch (digraphCh)
                    {
                        case '<':
                            if (ch == ':')
                            {
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

                                    yield return lastTok;
                                    lastTok.tokenType = TokenType.UNKNOWN;
                                }

                                lastTok.tokenType = TokenType.L_SQ_BRACKET;
                                lastTok.column = column;
                                lastTok.line = line;

                                column += 2;
                                charBufPtr++;
                            }
                            else if (ch == '%')
                            {
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

                                    yield return lastTok;
                                    lastTok.tokenType = TokenType.UNKNOWN;
                                }

                                lastTok.tokenType = TokenType.L_CURLY_BRACE;
                                lastTok.column = column;
                                lastTok.line = line;

                                column += 2;
                                charBufPtr++;
                            }
                            else
                            {
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

                                    yield return lastTok;
                                    lastTok.tokenType = TokenType.UNKNOWN;
                                }

                                lastTok.tokenType = TokenType.LESS_THEN;
                                lastTok.column = column;
                                lastTok.line = line;

                                column++;
                            }

                            break;
                        case ':':
                            if (ch == '>')
                            {
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

                                    yield return lastTok;
                                    lastTok.tokenType = TokenType.UNKNOWN;
                                }

                                lastTok.tokenType = TokenType.R_SQ_BRACKET;
                                lastTok.column = column;
                                lastTok.line = line;

                                column += 2;
                                charBufPtr ++;
                            }
                            else
                            {
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

                                    yield return lastTok;
                                    lastTok.tokenType = TokenType.UNKNOWN;
                                }

                                lastTok.tokenType = TokenType.COLON;
                                lastTok.column = column;
                                lastTok.line = line;

                                column++;
                            }
                            break;
                        case '%':
                            if (ch == '>')
                            {
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

                                    yield return lastTok;
                                    lastTok.tokenType = TokenType.UNKNOWN;
                                }

                                lastTok.tokenType = TokenType.R_CURLY_BRACE;
                                lastTok.column = column;
                                lastTok.line = line;

                                column += 2;
                                charBufPtr++;
                            }
                            else if (ch == ':')
                            {
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

                                    yield return lastTok;
                                    lastTok.tokenType = TokenType.UNKNOWN;
                                }

                                lastTok.tokenType = TokenType.HASH;
                                lastTok.column = column;
                                lastTok.line = line;

                                column += 2;
                                charBufPtr++;
                            }
                            else
                            {
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

                                    yield return lastTok;
                                    lastTok.tokenType = TokenType.UNKNOWN;
                                }

                                lastTok.tokenType = TokenType.PERCENT;
                                lastTok.column = column;
                                lastTok.line = line;

                                column++;
                            }
                            break;
                    }

                    digraphCh = '\0';
                    continue;
                }

                switch (ch)
                {
                    case ' ':
                    case '\t':
                    case '\v':
                    case '\f':
                        if (lastTok.tokenType != TokenType.WHITESPACE)
                        {
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

                                yield return lastTok;
                                lastTok.tokenType = TokenType.UNKNOWN;
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

                            yield return lastTok;
                            lastTok.tokenType = TokenType.UNKNOWN;
                        }

                        lastTok.tokenType = TokenType.NEWLINE;
                        lastTok.column = column;
                        lastTok.line = line;

                        column = 1;
                        line++;
                        charBufPtr++;
                        break;
                    case '#':
                        {
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

                                yield return lastTok;
                                lastTok.tokenType = TokenType.UNKNOWN;
                            }

                            lastTok.tokenType = TokenType.HASH;
                            lastTok.column = column;
                            lastTok.line = line;

                            column++;
                            charBufPtr++;
                        }
                        break;
                    case '?':
                        {
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

                                yield return lastTok;
                                lastTok.tokenType = TokenType.UNKNOWN;
                            }

                            lastTok.tokenType = TokenType.QUESTION_MARK;
                            lastTok.column = column;
                            lastTok.line = line;

                            column++;
                            charBufPtr++;
                        }
                        break;
                    case '=':
                        {
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

                                yield return lastTok;
                                lastTok.tokenType = TokenType.UNKNOWN;
                            }

                            lastTok.tokenType = TokenType.EQUAL_SIGN;
                            lastTok.column = column;
                            lastTok.line = line;

                            column++;
                            charBufPtr++;
                        }
                        break;
                    case '<':
                        if (digraphs)
                        {
                            //charReadBuffer[charBufPtr]
                            if (charBufPtr + 1 < charBufDatSize)
                            {
                                if (charReadBuffer[charBufPtr + 1] == ':')
                                {
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

                                        yield return lastTok;
                                        lastTok.tokenType = TokenType.UNKNOWN;
                                    }

                                    lastTok.tokenType = TokenType.L_SQ_BRACKET;
                                    lastTok.column = column;
                                    lastTok.line = line;

                                    column += 2;
                                    charBufPtr += 2;
                                }
                                else if (charReadBuffer[charBufPtr + 1] == '%')
                                {
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

                                        yield return lastTok;
                                        lastTok.tokenType = TokenType.UNKNOWN;
                                    }

                                    lastTok.tokenType = TokenType.L_CURLY_BRACE;
                                    lastTok.column = column;
                                    lastTok.line = line;

                                    column += 2;
                                    charBufPtr += 2;
                                }
                                else
                                {
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

                                        yield return lastTok;
                                        lastTok.tokenType = TokenType.UNKNOWN;
                                    }

                                    lastTok.tokenType = TokenType.LESS_THEN;
                                    lastTok.column = column;
                                    lastTok.line = line;

                                    column++;
                                    charBufPtr++;
                                }
                            }
                            else
                            {
                                digraphCh = ch;
                                refillBuffer = true;
                            }
                        }
                        else
                        {
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

                                yield return lastTok;
                                lastTok.tokenType = TokenType.UNKNOWN;
                            }

                            lastTok.tokenType = TokenType.LESS_THEN;
                            lastTok.column = column;
                            lastTok.line = line;

                            column++;
                            charBufPtr++;
                        }
                        break;
                    case ':':
                        if (digraphs)
                        {
                            //charReadBuffer[charBufPtr]
                            if (charBufPtr + 1 < charBufDatSize)
                            {
                                if (charReadBuffer[charBufPtr + 1] == '>')
                                {
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

                                        yield return lastTok;
                                        lastTok.tokenType = TokenType.UNKNOWN;
                                    }

                                    lastTok.tokenType = TokenType.R_SQ_BRACKET;
                                    lastTok.column = column;
                                    lastTok.line = line;

                                    column += 2;
                                    charBufPtr += 2;
                                }
                                else
                                {
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

                                        yield return lastTok;
                                        lastTok.tokenType = TokenType.UNKNOWN;
                                    }

                                    lastTok.tokenType = TokenType.COLON;
                                    lastTok.column = column;
                                    lastTok.line = line;

                                    column++;
                                    charBufPtr++;
                                }
                            }
                            else
                            {
                                digraphCh = ch;
                                refillBuffer = true;
                            }
                        }
                        else
                        {
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

                                yield return lastTok;
                                lastTok.tokenType = TokenType.UNKNOWN;
                            }

                            lastTok.tokenType = TokenType.COLON;
                            lastTok.column = column;
                            lastTok.line = line;

                            column++;
                            charBufPtr++;
                        }
                        break;
                    case '%':
                        if (digraphs)
                        {
                            if (charBufPtr + 1 < charBufDatSize)
                            {
                                if (charReadBuffer[charBufPtr + 1] == '>')
                                {
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

                                        yield return lastTok;
                                        lastTok.tokenType = TokenType.UNKNOWN;
                                    }

                                    lastTok.tokenType = TokenType.R_CURLY_BRACE;
                                    lastTok.column = column;
                                    lastTok.line = line;

                                    column += 2;
                                    charBufPtr += 2;
                                }
                                else if (charReadBuffer[charBufPtr + 1] == ':')
                                {
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

                                        yield return lastTok;
                                        lastTok.tokenType = TokenType.UNKNOWN;
                                    }

                                    lastTok.tokenType = TokenType.HASH;
                                    lastTok.column = column;
                                    lastTok.line = line;

                                    column += 2;
                                    charBufPtr += 2;
                                }
                                else
                                {
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

                                        yield return lastTok;
                                        lastTok.tokenType = TokenType.UNKNOWN;
                                    }

                                    lastTok.tokenType = TokenType.PERCENT;
                                    lastTok.column = column;
                                    lastTok.line = line;

                                    column ++;
                                    charBufPtr ++;
                                }
                            }
                            else
                            {
                                digraphCh = ch;
                                refillBuffer = true;
                            }
                        }
                        else
                        {
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

                                yield return lastTok;
                                lastTok.tokenType = TokenType.UNKNOWN;
                            }

                            lastTok.tokenType = TokenType.PERCENT;
                            lastTok.column = column;
                            lastTok.line = line;

                            column++;
                            charBufPtr++;
                        }
                        break;
                    case '>':
                        {
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

                                yield return lastTok;
                                lastTok.tokenType = TokenType.UNKNOWN;
                            }

                            lastTok.tokenType = TokenType.GREATER_THEN;
                            lastTok.column = column;
                            lastTok.line = line;

                            column++;
                            charBufPtr++;
                        }
                        break;
                    default:
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

                            yield return lastTok;
                        }

                        throw new NotImplementedException(new String(new char[] { ch }) + " not yet handled.");
                }
            }
        }
    }
}
