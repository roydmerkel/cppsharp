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
using System.IO;
using System.Text;
using Ude;

namespace libcppsharp
{
    public class TokenStream
    {
        private char[] charReadBuffer;
        private byte[] byteReadBuffer;
        private const int bufSize = 1023;
        private const int saveBufSize = 2;
        private int bufDatSize;
        private int charBufDatSize;
        private int bufPtr;
        private int charBufPtr;
        private Stream inStr;
        private StringBuilder curTokVal;
        private bool digraphs;
        private bool trigraphs;
        private int readResult;
        private Encoding encoding;
        private Decoder decoder;
        private Encoder encoder;
        private ulong column;
        private ulong line;

        public TokenStream(Stream inStream, bool handleTrigraphs = false, bool handleDigraphs = false)
        {
            inStr = inStream;
            digraphs = handleDigraphs;
            trigraphs = handleTrigraphs;
            curTokVal = new StringBuilder();
            byteReadBuffer = new byte[bufSize];
            charReadBuffer = new char[bufSize];
            bufPtr = 0;
            bufDatSize = 0;
            charBufPtr = 0;
            charBufDatSize = 0;
            readResult = 0;
            encoding = null;
            column = 1;
            line = 1;
        }

        private int RefillByteArray()
        {
            int i;
            // move any unprocessed bytes down.
            for (i = 0; i + bufPtr < bufDatSize; i++)
            {
                byteReadBuffer[i] = byteReadBuffer[i + bufPtr];
            }

            readResult = inStr.Read(byteReadBuffer, i, bufSize - i);

            if (readResult > 0)
            {
                bufDatSize = readResult + i;
            }

            bufPtr = 0;

            return readResult;
        }

        private bool SetEncoder()
        {
            // try to determine encoding.
            CharsetDetector charsetDetector = new CharsetDetector();
            charsetDetector.Feed(byteReadBuffer, 0, bufSize);
            charsetDetector.DataEnd();

            if (charsetDetector.Charset != null)
            {
                switch (charsetDetector.Charset)
                {
                    case Ude.Charsets.ASCII:
                        encoding = Encoding.GetEncoding("us-ascii");
                        break;
                    case Ude.Charsets.BIG5:
                        encoding = Encoding.GetEncoding("big5");
                        break;
                    case Ude.Charsets.EUCJP:
                        encoding = Encoding.GetEncoding("EUC-JP");
                        break;
                    case Ude.Charsets.EUCKR:
                        encoding = Encoding.GetEncoding("euc-kr");
                        break;
                    case Ude.Charsets.EUCTW:
                        // windows uses 51950 codepage for Traditional chinese writing.
                        encoding = Encoding.GetEncoding(51950);
                        break;
                    case Ude.Charsets.GB18030:
                        encoding = Encoding.GetEncoding("GB18030");
                        break;
                    case Ude.Charsets.HZ_GB_2312:
                        encoding = Encoding.GetEncoding("hz-gb-2312");
                        break;
                    case Ude.Charsets.IBM855:
                        encoding = Encoding.GetEncoding("IBM855");
                        break;
                    case Ude.Charsets.IBM866:
                        encoding = Encoding.GetEncoding("cp866");
                        break;
                    case Ude.Charsets.ISO2022_CN:
                        encoding = Encoding.GetEncoding(50229);
                        break;
                    case Ude.Charsets.ISO2022_JP:
                        encoding = Encoding.GetEncoding("iso-2022-jp");
                        break;
                    case Ude.Charsets.ISO2022_KR:
                        encoding = Encoding.GetEncoding("iso-2022-kr");
                        break;
                    case Ude.Charsets.ISO8859_2:
                        encoding = Encoding.GetEncoding("iso-8859-2");
                        break;
                    case Ude.Charsets.ISO8859_5:
                        encoding = Encoding.GetEncoding("iso-8859-5");
                        break;
                    case Ude.Charsets.ISO8859_8:
                        encoding = Encoding.GetEncoding("iso-8859-8");
                        break;
                    case Ude.Charsets.ISO_8859_7:
                        encoding = Encoding.GetEncoding("iso-8859-7");
                        break;
                    case Ude.Charsets.KOI8R:
                        encoding = Encoding.GetEncoding("koi8-r");
                        break;
                    case Ude.Charsets.MAC_CYRILLIC:
                        encoding = Encoding.GetEncoding("x-mac-cyrillic");
                        break;
                    case Ude.Charsets.SHIFT_JIS:
                        encoding = Encoding.GetEncoding("shift_jis");
                        break;
                    case Ude.Charsets.TIS620:
                        encoding = Encoding.GetEncoding("windows-874");
                        break;
                    case Ude.Charsets.UCS4_2413:
                        encoding = Encoding.GetEncoding("utf-32");
                        break;
                    case Ude.Charsets.UCS4_3412:
                        encoding = Encoding.GetEncoding("utf-32");
                        break;
                    case Ude.Charsets.UTF16_BE:
                        encoding = Encoding.GetEncoding("unicodeFFFE");
                        break;
                    case Ude.Charsets.UTF16_LE:
                        encoding = Encoding.GetEncoding("utf-16");
                        break;
                    case Ude.Charsets.UTF32_BE:
                        encoding = Encoding.GetEncoding("utf-32BE");
                        break;
                    case Ude.Charsets.UTF32_LE:
                        encoding = Encoding.GetEncoding("utf-32");
                        break;
                    case Ude.Charsets.UTF8:
                        encoding = Encoding.GetEncoding("utf-8");
                        break;
                    case Ude.Charsets.WIN1251:
                        encoding = Encoding.GetEncoding("windows-1251");
                        break;
                    case Ude.Charsets.WIN1252:
                        encoding = Encoding.GetEncoding("windows-1252");
                        break;
                    case Ude.Charsets.WIN1253:
                        encoding = Encoding.GetEncoding("windows-1253");
                        break;
                    case Ude.Charsets.WIN1255:
                        encoding = Encoding.GetEncoding("windows-1255");
                        break;
                    default:
                        encoding = Encoding.UTF8;
                        break;
                }

                if (encoding == null)
                {
                    return false;
                }
            }
            else
            {
                encoding = Encoding.UTF8;
            }

            encoder = encoding.GetEncoder();
            decoder = encoding.GetDecoder();

            return true;
        }

        public System.Collections.Generic.IEnumerable<Token>  GetNextToken()
        {
            bool refillBuffer = false;

            readResult = RefillByteArray();

            if (readResult <= 0)
            {
                Token eofTok;
                eofTok.tokenType = TokenType.EOF;
                eofTok.isDigraph = false;
                eofTok.isTrigraph = false;
                eofTok.value = null;
                eofTok.column = column;
                eofTok.line = line;

                yield return eofTok;
            }
            else
            {
                if (!SetEncoder())
                {
                    throw new ArgumentException("Encoding of stream is not supported by C#.");
                }

                charBufDatSize = decoder.GetChars(byteReadBuffer, 0, bufDatSize, charReadBuffer, 0, false);
                bufPtr = encoder.GetByteCount(charReadBuffer, 0, charBufDatSize, false);
                decoder.Reset();
                encoder.Reset();
            }

            do
            {
                if (readResult <= 0)
                {
                    break;
                }


                if (charBufPtr >= charBufDatSize)
                {
                    refillBuffer = true;
                }

                if (refillBuffer)
                {
                    refillBuffer = false;
                    readResult = RefillByteArray();

                    if (readResult <= 0)
                    {
                        Token eofTok;
                        eofTok.tokenType = TokenType.EOF;
                        eofTok.isDigraph = false;
                        eofTok.isTrigraph = false;
                        eofTok.value = null;
                        eofTok.column = column;
                        eofTok.line = line;

                        yield return eofTok;
                        break;
                    }
                    else
                    {
                        charBufDatSize = decoder.GetChars(byteReadBuffer, 0, bufDatSize, charReadBuffer, 0, false);
                        bufPtr = encoder.GetByteCount(charReadBuffer, 0, charBufDatSize, false);
                        decoder.Reset();
                        encoder.Reset();
                        charBufPtr = 0;
                    }
                }

                Console.WriteLine(charReadBuffer[charBufPtr]);
                switch (charReadBuffer[charBufPtr])
                {
                    case '\xFEFF':
                        charBufPtr++;
                        break;
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
                            hashToken.isDigraph = false;
                            hashToken.isTrigraph = false;
                            hashToken.value = null;
                            hashToken.column = column;
                            hashToken.line = line;

                            yield return hashToken;

                            column++;
                            charBufPtr++;
                        }
                        break;
                }
            } while (true);
        }
    }
}
