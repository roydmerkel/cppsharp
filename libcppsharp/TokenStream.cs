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
        ulong column;
        ulong line;

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

        public System.Collections.Generic.IEnumerable<Token>  GetNextToken()
        {
            bool refillBuffer = false;

            readResult = RefillCharArray();

            if (readResult <= 0)
            {
                Token eofTok;
                eofTok.tokenType = TokenType.EOF;
                eofTok.value = null;
                eofTok.column = column;
                eofTok.line = line;

                yield return eofTok;
            }

            while(readResult > 0)
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
                        Token eofTok;
                        eofTok.tokenType = TokenType.EOF;
                        eofTok.value = null;
                        eofTok.column = column;
                        eofTok.line = line;

                        yield return eofTok;
                        break;
                    }
                }

                switch (charReadBuffer[charBufPtr])
                {
                    case ' ':
                    case '\t':
                        column++;
                        charBufPtr++;
                        break;
                    case '\r':
                    case '\n':
                        column = 1;
                        line++;
                        charBufPtr++;
                        break;
                    case '#':
                        {
                            Token hashToken;
                            hashToken.tokenType = TokenType.HASH;
                            hashToken.value = null;
                            hashToken.column = column;
                            hashToken.line = line;

                            yield return hashToken;

                            column++;
                            charBufPtr++;
                        }
                        break;
                    case '?':
                        {
                            Token questionToken;
                            questionToken.tokenType = TokenType.QUESTION_MARK;
                            questionToken.value = null;
                            questionToken.column = column;
                            questionToken.line = line;

                            yield return questionToken;

                            column++;
                            charBufPtr++;
                        }
                        break;
                    case '=':
                        {
                            Token equalSignToken;
                            equalSignToken.tokenType = TokenType.EQUAL_SIGN;
                            equalSignToken.value = null;
                            equalSignToken.column = column;
                            equalSignToken.line = line;

                            yield return equalSignToken;

                            column++;
                            charBufPtr++;
                        }
                        break;
                    default:
                        throw new NotImplementedException(new String(new char[] { charReadBuffer[charBufPtr] }) + " not yet handled.");
                }
            }
        }
    }
}
