//
//  TrigraphStream.cs
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
    public class TrigraphStream : Stream
    {
        private bool trigraphs;
        private const string TrigraphChars = "=/'()!<>-";
        private const string TrigraphStandins = "#\\^[]|{}~";
        private bool escaped;
        private Encoder encoder;
        private Decoder decoder;
        private String data;
        private const int bufSize = 1023;
        private MemoryStream ms;

        public override bool CanRead { get { return ms.CanRead; } }

        public override bool CanSeek { get { return ms.CanSeek; } }

        public override bool CanWrite { get { return false; } }

        public override long Length { get { throw new NotSupportedException(); } }

        public override long Position { get { throw new NotSupportedException(); } set { throw new NotSupportedException(); } }

        public TrigraphStream(Stream inStream, bool handleTrigraphs = false, bool handleDigraphs = false)
        {
            List<byte> bytesList = new List<byte>();
            byte[] readBuf = new byte[1024];
            int numRead = 0;
            bool escaped = false;

            trigraphs = handleTrigraphs;

            escaped = false;
            encoder = null;
            decoder = null;

            do
            {
                numRead = inStream.Read(readBuf, 0, 1024);

                for (int i = 0; i < numRead; i++)
                {
                    bytesList.Add(readBuf[i]);
                }
            }
            while (numRead > 0);

            byte[] bytes = bytesList.ToArray();

            CharsetDetector charsetDetector = new CharsetDetector();
            charsetDetector.Feed(bytes, 0, bytes.Length);
            charsetDetector.DataEnd();

            Encoding encoding = null;

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
            }
            else
            {
                encoding = Encoding.UTF8;
            }

            if (encoding == null)
            {
                throw new InvalidDataException("Can't find the type of data passed in via the stream.");
            }

            encoder = encoding.GetEncoder();
            decoder = encoding.GetDecoder();

            int charCount = decoder.GetCharCount(bytes, 0, bytes.Length);
            char[] chars = new char[charCount];

            decoder.GetChars(bytes, 0, bytes.Length, chars, 0);

            String s = new String(chars);
            StringBuilder sb = new StringBuilder();

            int sLen = s.Length;
            for (int i = 0; i < sLen; i++)
            {
                if (s[i] == '\xFEFF')
                {
                }
                else if (s[i] == '\n')
                {
                    if (i + 1 < sLen && s[i + 1] == '\r')
                    {
                        sb.Append('\n');
                        i++;
                    }
                    else
                    {
                        sb.Append('\n');
                    }
                }
                else if (s[i] == '\r')
                {
                    if (i + 1 < sLen && s[i + 1] == '\n')
                    {
                        sb.Append('\n');
                        i++;
                    }
                    else
                    {
                        sb.Append('\n');
                    }
                }
                else if (!trigraphs)
                {
                    sb.Append(s[i]);
                }
                else if (s[i] == '\\')
                {
                    escaped = true;
                    sb.Append(s[i]);
                }
                else if (s[i] == '?')
                {
                    if (escaped)
                    {
                        sb.Append(s[i]);
                        escaped = false;
                    }
                    else if (i + 1 < sLen && s[i + 1] == '?')
                    {
                        if (i + 2 < sLen)
                        {
                            int idx = TrigraphChars.IndexOf(s[i + 2]);

                            if (idx >= 0)
                            {
                                char ch = TrigraphStandins[idx];

                                if (ch == '\\')
                                {
                                    escaped = true;
                                }
                                sb.Append(ch);
                                i += 2;
                            }
                            else
                            {
                                sb.Append(s[i]);
                            }
                        }
                        else
                        {
                            sb.Append(s[i]);
                        }
                    }
                    else
                    {
                        sb.Append(s[i]);
                    }
                }
                else
                {
                    escaped = false;
                    sb.Append(s[i]);
                }
            }

            data = sb.ToString();

            chars = data.ToCharArray();

            int byteCount = encoder.GetByteCount(chars, 0, chars.Length, true);

            bytes = new byte[byteCount];

            encoder.GetBytes(chars, 0, chars.Length, bytes, 0, true);

            ms = new MemoryStream(bytes);
        }

        public IEnumerable<char> GetCharEnumerable()
        {
            foreach (char ch in data)
            {
                yield return ch;
            }
        }

        public override void Flush()
        {
            ms.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return ms.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return ms.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            ms.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            ms.Write(buffer, offset, count);
        }
    }
}
