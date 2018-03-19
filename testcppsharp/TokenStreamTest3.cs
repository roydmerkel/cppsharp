//
//  TokenStreamTest3.cs
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
using libcppsharp;
using System;
using System.IO;
using System.Text;
using NUnit.Framework;
using System.Collections.Generic;
using System.Reflection;

namespace testcppsharp
{
    [TestFixture]
    public class TokenStreamTest3
    {
        public TokenStreamTest3()
        {
        }

        [Test]
        public void TestString1()
        {
            // test # digram.
            string code = "\"ABC123\"#";
            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(code));
            TokenStream ts = new TokenStream(ms, true, true);

            IEnumerable<Token> enumerable = ts.GetTokenEnumerable();
            IEnumerator<Token> i = enumerable.GetEnumerator();
            i.MoveNext();
            Token t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.STRING);
            Assert.AreEqual(t.value, "\"ABC123\"");

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.HASH);
        }

        [Test]
        public void TestString2()
        {
            // test # digram.
            string code = "u\"ABC123\"#";
            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(code));
            TokenStream ts = new TokenStream(ms, true, true);

            IEnumerable<Token> enumerable = ts.GetTokenEnumerable();
            IEnumerator<Token> i = enumerable.GetEnumerator();
            i.MoveNext();
            Token t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.STRING);
            Assert.AreEqual(t.value, "u\"ABC123\"");

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.HASH);
        }

        [Test]
        public void TestString3()
        {
            // test # digram.
            string code = "u8\"ABC123\"#";
            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(code));
            TokenStream ts = new TokenStream(ms, true, true);

            IEnumerable<Token> enumerable = ts.GetTokenEnumerable();
            IEnumerator<Token> i = enumerable.GetEnumerator();
            i.MoveNext();
            Token t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.STRING);
            Assert.AreEqual(t.value, "u8\"ABC123\"");

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.HASH);
        }

        [Test]
        public void TestString4()
        {
            // test # digram.
            string code = "U\"ABC123\"#";
            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(code));
            TokenStream ts = new TokenStream(ms, true, true);

            IEnumerable<Token> enumerable = ts.GetTokenEnumerable();
            IEnumerator<Token> i = enumerable.GetEnumerator();
            i.MoveNext();
            Token t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.STRING);
            Assert.AreEqual(t.value, "U\"ABC123\"");

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.HASH);
        }

        [Test]
        public void TestString5()
        {
            // test # digram.
            string code = "L\"ABC123\"#";
            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(code));
            TokenStream ts = new TokenStream(ms, true, true);

            IEnumerable<Token> enumerable = ts.GetTokenEnumerable();
            IEnumerator<Token> i = enumerable.GetEnumerator();
            i.MoveNext();
            Token t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.STRING);
            Assert.AreEqual(t.value, "L\"ABC123\"");

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.HASH);
        }

        [Test]
        public void TestString6()
        {
            // test # digram.
            string code = "\"ABC123\"s#";
            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(code));
            TokenStream ts = new TokenStream(ms, true, true);

            IEnumerable<Token> enumerable = ts.GetTokenEnumerable();
            IEnumerator<Token> i = enumerable.GetEnumerator();
            i.MoveNext();
            Token t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.STRING);
            Assert.AreEqual(t.value, "\"ABC123\"s");

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.HASH);
        }

        [Test]
        public void TestString7()
        {
            // test # digram.
            string code = "u\"ABC123\"s#";
            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(code));
            TokenStream ts = new TokenStream(ms, true, true);

            IEnumerable<Token> enumerable = ts.GetTokenEnumerable();
            IEnumerator<Token> i = enumerable.GetEnumerator();
            i.MoveNext();
            Token t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.STRING);
            Assert.AreEqual(t.value, "u\"ABC123\"s");

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.HASH);
        }

        [Test]
        public void TestString8()
        {
            // test # digram.
            string code = "u8\"ABC123\"s#";
            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(code));
            TokenStream ts = new TokenStream(ms, true, true);

            IEnumerable<Token> enumerable = ts.GetTokenEnumerable();
            IEnumerator<Token> i = enumerable.GetEnumerator();
            i.MoveNext();
            Token t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.STRING);
            Assert.AreEqual(t.value, "u8\"ABC123\"s");

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.HASH);
        }

        [Test]
        public void TestString9()
        {
            // test # digram.
            string code = "U\"ABC123\"s#";
            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(code));
            TokenStream ts = new TokenStream(ms, true, true);

            IEnumerable<Token> enumerable = ts.GetTokenEnumerable();
            IEnumerator<Token> i = enumerable.GetEnumerator();
            i.MoveNext();
            Token t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.STRING);
            Assert.AreEqual(t.value, "U\"ABC123\"s");

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.HASH);
        }

        [Test]
        public void TestString10()
        {
            // test # digram.
            string code = "L\"ABC123\"s#";
            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(code));
            TokenStream ts = new TokenStream(ms, true, true);

            IEnumerable<Token> enumerable = ts.GetTokenEnumerable();
            IEnumerator<Token> i = enumerable.GetEnumerator();
            i.MoveNext();
            Token t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.STRING);
            Assert.AreEqual(t.value, "L\"ABC123\"s");

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.HASH);
        }

        [Test]
        public void TestString11()
        {
            // test # digram.
            string code = "Lu8\"ABC123\"s#";
            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(code));
            TokenStream ts = new TokenStream(ms, true, true);

            IEnumerable<Token> enumerable = ts.GetTokenEnumerable();
            IEnumerator<Token> i = enumerable.GetEnumerator();
            i.MoveNext();
            Token t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.IDENTIFIER);
            Assert.AreEqual(t.value, "Lu8");

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.STRING);
            Assert.AreEqual(t.value, "\"ABC123\"s");

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.HASH);
        }

        [Test]
        public void TestString12()
        {
            // test # digram.
            string code = "Lu\\\n8\"ABC\\\n123\"s#";
            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(code));
            TokenStream ts = new TokenStream(ms, true, true, true, true);

            IEnumerable<Token> enumerable = ts.GetTokenEnumerable();
            IEnumerator<Token> i = enumerable.GetEnumerator();
            i.MoveNext();
            Token t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.IDENTIFIER);
            Assert.AreEqual(t.value, "Lu8");

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.STRING);
            Assert.AreEqual(t.value, "\"ABC123\"s");

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.HASH);
        }

        [Test]
        public void TestString13()
        {
            // test # digram.
            string code = "Lu\\\n8\"ABC\\\n123\"sa#";
            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(code));
            TokenStream ts = new TokenStream(ms, true, true, true, true);

            IEnumerable<Token> enumerable = ts.GetTokenEnumerable();
            IEnumerator<Token> i = enumerable.GetEnumerator();
            i.MoveNext();
            Token t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.IDENTIFIER);
            Assert.AreEqual(t.value, "Lu8");

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.STRING);
            Assert.AreEqual(t.value, "\"ABC123\"");

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.IDENTIFIER);
            Assert.AreEqual(t.value, "sa");

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.HASH);
        }

        [Test]
        public void TestIdentifer()
        {
            // test # digram.
            string code = @"#include <stdio.h>
            
int main(void)
{
    return 0;
}";
            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(code));
            TokenStream ts = new TokenStream(ms, true, true, true, true);

            IEnumerable<Token> enumerable = ts.GetTokenEnumerable();
            IEnumerator<Token> i = enumerable.GetEnumerator();
            i.MoveNext();
            Token t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.HASH);
            Assert.AreEqual(t.value, "#");

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.IDENTIFIER);
            Assert.AreEqual(t.value, "include");

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.WHITESPACE);
            Assert.AreEqual(t.value, " ");

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.LESS_THEN);
            Assert.AreEqual(t.value, "<");

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.IDENTIFIER);
            Assert.AreEqual(t.value, "stdio");

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.PERIOD);
            Assert.AreEqual(t.value, ".");

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.IDENTIFIER);
            Assert.AreEqual(t.value, "h");

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.GREATER_THEN);
            Assert.AreEqual(t.value, ">");
        }

        [Test]
        public void TestRawString1()
        {
            // test # digram.
            string code = @"R""ABC(DEF)ABC""";
            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(code));
            TokenStream ts = new TokenStream(ms, true, true, true, true);

            IEnumerable<Token> enumerable = ts.GetTokenEnumerable();
            IEnumerator<Token> i = enumerable.GetEnumerator();
            i.MoveNext();
            Token t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.STRING);
            Assert.AreEqual(t.value, "R\"ABC(DEF)ABC\"");
        }

        [Test]
        public void TestRawString2()
        {
            // test # digram.
            string code = @"R""ABC(DEF)ABC)ABC""";
            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(code));
            TokenStream ts = new TokenStream(ms, true, true, true, true);

            IEnumerable<Token> enumerable = ts.GetTokenEnumerable();
            IEnumerator<Token> i = enumerable.GetEnumerator();
            i.MoveNext();
            Token t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.STRING);
            Assert.AreEqual(t.value, "R\"ABC(DEF)ABC)ABC\"");
        }

        [Test]
        public void TestRawString3()
        {
            // test # digram.
            string code = @"R""(DEF))""";
            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(code));
            TokenStream ts = new TokenStream(ms, true, true, true, true);

            IEnumerable<Token> enumerable = ts.GetTokenEnumerable();
            IEnumerator<Token> i = enumerable.GetEnumerator();
            i.MoveNext();
            Token t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.STRING);
            Assert.AreEqual(t.value, "R\"(DEF))\"");
        }

        [Test]
        public void TestRawString4()
        {
            // test # digram.
            string code = @"R""(DEF))""s";
            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(code));
            TokenStream ts = new TokenStream(ms, true, true, true, true);

            IEnumerable<Token> enumerable = ts.GetTokenEnumerable();
            IEnumerator<Token> i = enumerable.GetEnumerator();
            i.MoveNext();
            Token t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.STRING);
            Assert.AreEqual(t.value, "R\"(DEF))\"s");
        }

        [Test]
        public void TestRawString5()
        {
            // test # digram.
            string code = @"R\
""\
(DEF))""s";
            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(code));
            TokenStream ts = new TokenStream(ms, true, true, true, true);

            IEnumerable<Token> enumerable = ts.GetTokenEnumerable();
            IEnumerator<Token> i = enumerable.GetEnumerator();
            i.MoveNext();
            Token t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.STRING);
            Assert.AreEqual(t.value, "R\"(DEF))\"s");
        }


        [Test]
        public void TestRawString6()
        {
            // test # digram.
            string code = @"R""\A\
(DEF))""s";
            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(code));
            TokenStream ts = new TokenStream(ms, true, true, true, true);

            IEnumerable<Token> enumerable = ts.GetTokenEnumerable();
            IEnumerator<Token> i = enumerable.GetEnumerator();

            try
            {
                i.MoveNext();
                Token tok = i.Current;

                Assert.Fail();
            }
            catch (InvalidDataException)
            {
                Assert.Pass();
            }
        }

        [Test]
        public void TestRawString7()
        {
            // test # digram.
            string code = @"R"" A(DEF))""s";
            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(code));
            TokenStream ts = new TokenStream(ms, true, true, true, true);

            IEnumerable<Token> enumerable = ts.GetTokenEnumerable();
            IEnumerator<Token> i = enumerable.GetEnumerator();

            try
            {
                i.MoveNext();
                Token tok = i.Current;

                Assert.Fail();
            }
            catch (InvalidDataException)
            {
                Assert.Pass();
            }
        }

        [Test]
        public void TestRawString8()
        {
            // test # digram.
            string code = @"R"" A(DEF))""s";
            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(code));
            TokenStream ts = new TokenStream(ms, true, true, true, true);

            IEnumerable<Token> enumerable = ts.GetTokenEnumerable();
            IEnumerator<Token> i = enumerable.GetEnumerator();

            try
            {
                i.MoveNext();
                Token tok = i.Current;

                Assert.Fail();
            }
            catch (InvalidDataException)
            {
                Assert.Pass();
            }
        }

        [Test]
        public void TestRawString9()
        {
            // test # digram.
            string code = @"R"")A(DEF))""s";
            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(code));
            TokenStream ts = new TokenStream(ms, true, true, true, true);

            IEnumerable<Token> enumerable = ts.GetTokenEnumerable();
            IEnumerator<Token> i = enumerable.GetEnumerator();

            try
            {
                i.MoveNext();
                Token tok = i.Current;

                Assert.Fail();
            }
            catch (InvalidDataException)
            {
                Assert.Pass();
            }
        }

        [Test]
        public void TestRawString10()
        {
            // test # digram.
            string code = @"R""""A(DEF))""s";
            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(code));
            TokenStream ts = new TokenStream(ms, true, true, true, true);

            IEnumerable<Token> enumerable = ts.GetTokenEnumerable();
            IEnumerator<Token> i = enumerable.GetEnumerator();

            try
            {
                i.MoveNext();
                Token tok = i.Current;

                Assert.Fail();
            }
            catch (InvalidDataException)
            {
                Assert.Pass();
            }
        }

        [Test]
        public void TestChar1()
        {
            // test # digram.
            string code = "'A'#";
            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(code));
            TokenStream ts = new TokenStream(ms, true, true);

            IEnumerable<Token> enumerable = ts.GetTokenEnumerable();
            IEnumerator<Token> i = enumerable.GetEnumerator();
            i.MoveNext();
            Token t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.CHAR);
            Assert.AreEqual(t.value, "'A'");

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.HASH);
        }

        [Test]
        public void TestChar()
        {
            // test # digram.
            string code = "u'A'#";
            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(code));
            TokenStream ts = new TokenStream(ms, true, true);

            IEnumerable<Token> enumerable = ts.GetTokenEnumerable();
            IEnumerator<Token> i = enumerable.GetEnumerator();
            i.MoveNext();
            Token t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.CHAR);
            Assert.AreEqual(t.value, "u'A'");

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.HASH);
        }

        [Test]
        public void TestChar3()
        {
            // test # digram.
            string code = "u8'A'#";
            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(code));
            TokenStream ts = new TokenStream(ms, true, true);

            IEnumerable<Token> enumerable = ts.GetTokenEnumerable();
            IEnumerator<Token> i = enumerable.GetEnumerator();
            i.MoveNext();
            Token t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.CHAR);
            Assert.AreEqual(t.value, "u8'A'");

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.HASH);
        }

        [Test]
        public void TestChar4()
        {
            // test # digram.
            string code = "U'A'#";
            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(code));
            TokenStream ts = new TokenStream(ms, true, true);

            IEnumerable<Token> enumerable = ts.GetTokenEnumerable();
            IEnumerator<Token> i = enumerable.GetEnumerator();
            i.MoveNext();
            Token t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.CHAR);
            Assert.AreEqual(t.value, "U'A'");

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.HASH);
        }

        [Test]
        public void TestChar5()
        {
            // test # digram.
            string code = "L'A'#";
            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(code));
            TokenStream ts = new TokenStream(ms, true, true);

            IEnumerable<Token> enumerable = ts.GetTokenEnumerable();
            IEnumerator<Token> i = enumerable.GetEnumerator();
            i.MoveNext();
            Token t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.CHAR);
            Assert.AreEqual(t.value, "L'A'");

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.HASH);
        }

        [Test]
        public void TestChar6()
        {
            // test # digram.
            string code = "'A's#";
            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(code));
            TokenStream ts = new TokenStream(ms, true, true);

            IEnumerable<Token> enumerable = ts.GetTokenEnumerable();
            IEnumerator<Token> i = enumerable.GetEnumerator();
            i.MoveNext();
            Token t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.CHAR);
            Assert.AreEqual(t.value, "'A'");

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.IDENTIFIER);
            Assert.AreEqual(t.value, "s");

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.HASH);
        }

        [Test]
        public void TestChar7()
        {
            // test # digram.
            string code = "u'A's#";
            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(code));
            TokenStream ts = new TokenStream(ms, true, true);

            IEnumerable<Token> enumerable = ts.GetTokenEnumerable();
            IEnumerator<Token> i = enumerable.GetEnumerator();
            i.MoveNext();
            Token t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.CHAR);
            Assert.AreEqual(t.value, "u'A'");

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.IDENTIFIER);
            Assert.AreEqual(t.value, "s");

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.HASH);
        }

        [Test]
        public void TestChar8()
        {
            // test # digram.
            string code = "u8'A's#";
            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(code));
            TokenStream ts = new TokenStream(ms, true, true);

            IEnumerable<Token> enumerable = ts.GetTokenEnumerable();
            IEnumerator<Token> i = enumerable.GetEnumerator();
            i.MoveNext();
            Token t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.CHAR);
            Assert.AreEqual(t.value, "u8'A'");

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.IDENTIFIER);
            Assert.AreEqual(t.value, "s");

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.HASH);
        }

        [Test]
        public void TestChar9()
        {
            // test # digram.
            string code = "U'A's#";
            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(code));
            TokenStream ts = new TokenStream(ms, true, true);

            IEnumerable<Token> enumerable = ts.GetTokenEnumerable();
            IEnumerator<Token> i = enumerable.GetEnumerator();
            i.MoveNext();
            Token t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.CHAR);
            Assert.AreEqual(t.value, "U'A'");

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.IDENTIFIER);
            Assert.AreEqual(t.value, "s");

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.HASH);
        }

        [Test]
        public void TestChar10()
        {
            // test # digram.
            string code = "L'A's#";
            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(code));
            TokenStream ts = new TokenStream(ms, true, true);

            IEnumerable<Token> enumerable = ts.GetTokenEnumerable();
            IEnumerator<Token> i = enumerable.GetEnumerator();
            i.MoveNext();
            Token t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.CHAR);
            Assert.AreEqual(t.value, "L'A'");

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.IDENTIFIER);
            Assert.AreEqual(t.value, "s");

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.HASH);
        }

        [Test]
        public void TestChar11()
        {
            // test # digram.
            string code = "Lu8'A's#";
            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(code));
            TokenStream ts = new TokenStream(ms, true, true);

            IEnumerable<Token> enumerable = ts.GetTokenEnumerable();
            IEnumerator<Token> i = enumerable.GetEnumerator();
            i.MoveNext();
            Token t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.IDENTIFIER);
            Assert.AreEqual(t.value, "Lu8");

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.CHAR);
            Assert.AreEqual(t.value, "'A'");

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.IDENTIFIER);
            Assert.AreEqual(t.value, "s");

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.HASH);
        }

        [Test]
        public void TestChar12()
        {
            // test # digram.
            string code = "Lu\\\n8'\\\nA's#";
            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(code));
            TokenStream ts = new TokenStream(ms, true, true, true, true);

            IEnumerable<Token> enumerable = ts.GetTokenEnumerable();
            IEnumerator<Token> i = enumerable.GetEnumerator();
            i.MoveNext();
            Token t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.IDENTIFIER);
            Assert.AreEqual(t.value, "Lu8");

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.CHAR);
            Assert.AreEqual(t.value, "'A'");

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.IDENTIFIER);
            Assert.AreEqual(t.value, "s");

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.HASH);
        }

        [Test]
        public void TestChar13()
        {
            // test # digram.
            string code = "Lu\\\n8'\\\nA'sa#";
            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(code));
            TokenStream ts = new TokenStream(ms, true, true, true, true);

            IEnumerable<Token> enumerable = ts.GetTokenEnumerable();
            IEnumerator<Token> i = enumerable.GetEnumerator();
            i.MoveNext();
            Token t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.IDENTIFIER);
            Assert.AreEqual(t.value, "Lu8");

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.CHAR);
            Assert.AreEqual(t.value, "'A'");

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.IDENTIFIER);
            Assert.AreEqual(t.value, "sa");

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.HASH);
        }

        [Test]
        public void TestInteger1()
        {
            // test # digram.
            string code = "0123 ";
            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(code));
            TokenStream ts = new TokenStream(ms, true, true, true, true);

            IEnumerable<Token> enumerable = ts.GetTokenEnumerable();
            IEnumerator<Token> i = enumerable.GetEnumerator();
            i.MoveNext();
            Token t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.NUMBER);
            Assert.AreEqual(t.value, "0123");

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.WHITESPACE);
            Assert.AreEqual(t.value, " ");
        }

        [Test]
        public void TestInteger2()
        {
            // test # digram.
            string code = "0x123#";
            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(code));
            TokenStream ts = new TokenStream(ms, true, true, true, true);

            IEnumerable<Token> enumerable = ts.GetTokenEnumerable();
            IEnumerator<Token> i = enumerable.GetEnumerator();
            i.MoveNext();
            Token t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.NUMBER);
            Assert.AreEqual(t.value, "0x123");

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.HASH);
            Assert.AreEqual(t.value, "#");
        }

        [Test]
        public void TestInteger3()
        {
            // test # digram.
            string code = "0x123B#";
            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(code));
            TokenStream ts = new TokenStream(ms, true, true, true, true);

            IEnumerable<Token> enumerable = ts.GetTokenEnumerable();
            IEnumerator<Token> i = enumerable.GetEnumerator();
            i.MoveNext();
            Token t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.NUMBER);
            Assert.AreEqual(t.value, "0x123B");

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.HASH);
            Assert.AreEqual(t.value, "#");
        }

        [Test]
        public void TestInteger4()
        {
            // test # digram.
            string code = "0B123B#";
            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(code));
            TokenStream ts = new TokenStream(ms, true, true, true, true);

            IEnumerable<Token> enumerable = ts.GetTokenEnumerable();
            IEnumerator<Token> i = enumerable.GetEnumerator();
            i.MoveNext();
            Token t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.NUMBER);
            Assert.AreEqual(t.value, "0B123");

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.IDENTIFIER);
            Assert.AreEqual(t.value, "B");

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.HASH);
            Assert.AreEqual(t.value, "#");
        }

        [Test]
        public void TestInteger5()
        {
            // test # digram.
            string code = "123 ";
            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(code));
            TokenStream ts = new TokenStream(ms, true, true, true, true);

            IEnumerable<Token> enumerable = ts.GetTokenEnumerable();
            IEnumerator<Token> i = enumerable.GetEnumerator();
            i.MoveNext();
            Token t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.NUMBER);
            Assert.AreEqual(t.value, "123");

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.WHITESPACE);
            Assert.AreEqual(t.value, " ");
        }

        [Test]
        public void TestInteger6()
        {
            // test # digram.
            string code = "123E+123 ";
            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(code));
            TokenStream ts = new TokenStream(ms, true, true, true, true);

            IEnumerable<Token> enumerable = ts.GetTokenEnumerable();
            IEnumerator<Token> i = enumerable.GetEnumerator();
            i.MoveNext();
            Token t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.NUMBER);
            Assert.AreEqual(t.value, "123E+123");

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.WHITESPACE);
            Assert.AreEqual(t.value, " ");
        }

        [Test]
        public void TestInteger7()
        {
            // test # digram.
            string code = "123E-123 ";
            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(code));
            TokenStream ts = new TokenStream(ms, true, true, true, true);

            IEnumerable<Token> enumerable = ts.GetTokenEnumerable();
            IEnumerator<Token> i = enumerable.GetEnumerator();
            i.MoveNext();
            Token t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.NUMBER);
            Assert.AreEqual(t.value, "123E-123");

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.WHITESPACE);
            Assert.AreEqual(t.value, " ");
        }

        [Test]
        public void TestInteger8()
        {
            // test # digram.
            string code = "123P+123 ";
            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(code));
            TokenStream ts = new TokenStream(ms, true, true, true, true);

            IEnumerable<Token> enumerable = ts.GetTokenEnumerable();
            IEnumerator<Token> i = enumerable.GetEnumerator();
            i.MoveNext();
            Token t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.NUMBER);
            Assert.AreEqual(t.value, "123P+123");

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.WHITESPACE);
            Assert.AreEqual(t.value, " ");
        }

        [Test]
        public void TestInteger9()
        {
            // test # digram.
            string code = "123P-123 ";
            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(code));
            TokenStream ts = new TokenStream(ms, true, true, true, true);

            IEnumerable<Token> enumerable = ts.GetTokenEnumerable();
            IEnumerator<Token> i = enumerable.GetEnumerator();
            i.MoveNext();
            Token t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.NUMBER);
            Assert.AreEqual(t.value, "123P-123");

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.WHITESPACE);
            Assert.AreEqual(t.value, " ");
        }

        [Test]
        public void TestInteger10()
        {
            // test # digram.
            string code = "123e+123 ";
            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(code));
            TokenStream ts = new TokenStream(ms, true, true, true, true);

            IEnumerable<Token> enumerable = ts.GetTokenEnumerable();
            IEnumerator<Token> i = enumerable.GetEnumerator();
            i.MoveNext();
            Token t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.NUMBER);
            Assert.AreEqual(t.value, "123e+123");

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.WHITESPACE);
            Assert.AreEqual(t.value, " ");
        }

        [Test]
        public void TestInteger11()
        {
            // test # digram.
            string code = "123e-123 ";
            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(code));
            TokenStream ts = new TokenStream(ms, true, true, true, true);

            IEnumerable<Token> enumerable = ts.GetTokenEnumerable();
            IEnumerator<Token> i = enumerable.GetEnumerator();
            i.MoveNext();
            Token t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.NUMBER);
            Assert.AreEqual(t.value, "123e-123");

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.WHITESPACE);
            Assert.AreEqual(t.value, " ");
        }

        [Test]
        public void TestInteger12()
        {
            // test # digram.
            string code = "123p+123 ";
            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(code));
            TokenStream ts = new TokenStream(ms, true, true, true, true);

            IEnumerable<Token> enumerable = ts.GetTokenEnumerable();
            IEnumerator<Token> i = enumerable.GetEnumerator();
            i.MoveNext();
            Token t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.NUMBER);
            Assert.AreEqual(t.value, "123p+123");

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.WHITESPACE);
            Assert.AreEqual(t.value, " ");
        }

        [Test]
        public void TestInteger13()
        {
            // test # digram.
            string code = "123p-123 ";
            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(code));
            TokenStream ts = new TokenStream(ms, true, true, true, true);

            IEnumerable<Token> enumerable = ts.GetTokenEnumerable();
            IEnumerator<Token> i = enumerable.GetEnumerator();
            i.MoveNext();
            Token t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.NUMBER);
            Assert.AreEqual(t.value, "123p-123");

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.WHITESPACE);
            Assert.AreEqual(t.value, " ");
        }

        [Test]
        public void TestInteger14()
        {
            // test # digram.
            string code = "123p123 ";
            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(code));
            TokenStream ts = new TokenStream(ms, true, true, true, true);

            IEnumerable<Token> enumerable = ts.GetTokenEnumerable();
            IEnumerator<Token> i = enumerable.GetEnumerator();
            i.MoveNext();
            Token t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.NUMBER);
            Assert.AreEqual(t.value, "123p123");

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.WHITESPACE);
            Assert.AreEqual(t.value, " ");
        }

        [Test]
        public void TestInteger15()
        {
            // test # digram.
            string code = "123P123 ";
            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(code));
            TokenStream ts = new TokenStream(ms, true, true, true, true);

            IEnumerable<Token> enumerable = ts.GetTokenEnumerable();
            IEnumerator<Token> i = enumerable.GetEnumerator();
            i.MoveNext();
            Token t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.NUMBER);
            Assert.AreEqual(t.value, "123P123");

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.WHITESPACE);
            Assert.AreEqual(t.value, " ");
        }

        [Test]
        public void TestInteger16()
        {
            // test # digram.
            string code = "123E123 ";
            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(code));
            TokenStream ts = new TokenStream(ms, true, true, true, true);

            IEnumerable<Token> enumerable = ts.GetTokenEnumerable();
            IEnumerator<Token> i = enumerable.GetEnumerator();
            i.MoveNext();
            Token t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.NUMBER);
            Assert.AreEqual(t.value, "123E123");

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.WHITESPACE);
            Assert.AreEqual(t.value, " ");
        }

        [Test]
        public void TestInteger17()
        {
            // test # digram.
            string code = "123e123 ";
            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(code));
            TokenStream ts = new TokenStream(ms, true, true, true, true);

            IEnumerable<Token> enumerable = ts.GetTokenEnumerable();
            IEnumerator<Token> i = enumerable.GetEnumerator();
            i.MoveNext();
            Token t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.NUMBER);
            Assert.AreEqual(t.value, "123e123");

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.WHITESPACE);
            Assert.AreEqual(t.value, " ");
        }

        [Test]
        public void TestInteger18()
        {
            // test # digram.
            string code = "123.e123 ";
            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(code));
            TokenStream ts = new TokenStream(ms, true, true, true, true);

            IEnumerable<Token> enumerable = ts.GetTokenEnumerable();
            IEnumerator<Token> i = enumerable.GetEnumerator();
            i.MoveNext();
            Token t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.NUMBER);
            Assert.AreEqual(t.value, "123.e123");

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.WHITESPACE);
            Assert.AreEqual(t.value, " ");
        }

        [Test]
        public void TestInteger19()
        {
            // test # digram.
            string code = "123.e+123 ";
            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(code));
            TokenStream ts = new TokenStream(ms, true, true, true, true);

            IEnumerable<Token> enumerable = ts.GetTokenEnumerable();
            IEnumerator<Token> i = enumerable.GetEnumerator();
            i.MoveNext();
            Token t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.NUMBER);
            Assert.AreEqual(t.value, "123.e+123");

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.WHITESPACE);
            Assert.AreEqual(t.value, " ");
        }

        [Test]
        public void TestInteger20()
        {
            // test # digram.
            string code = "123.p123 ";
            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(code));
            TokenStream ts = new TokenStream(ms, true, true, true, true);

            IEnumerable<Token> enumerable = ts.GetTokenEnumerable();
            IEnumerator<Token> i = enumerable.GetEnumerator();
            i.MoveNext();
            Token t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.NUMBER);
            Assert.AreEqual(t.value, "123.p123");

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.WHITESPACE);
            Assert.AreEqual(t.value, " ");
        }

        [Test]
        public void TestInteger21()
        {
            // test # digram.
            string code = "123.p+123 ";
            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(code));
            TokenStream ts = new TokenStream(ms, true, true, true, true);

            IEnumerable<Token> enumerable = ts.GetTokenEnumerable();
            IEnumerator<Token> i = enumerable.GetEnumerator();
            i.MoveNext();
            Token t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.NUMBER);
            Assert.AreEqual(t.value, "123.p+123");

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.WHITESPACE);
            Assert.AreEqual(t.value, " ");
        }

        [Test]
        public void TestInteger22()
        {
            // test # digram.
            string code = "123. ";
            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(code));
            TokenStream ts = new TokenStream(ms, true, true, true, true);

            IEnumerable<Token> enumerable = ts.GetTokenEnumerable();
            IEnumerator<Token> i = enumerable.GetEnumerator();
            i.MoveNext();
            Token t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.NUMBER);
            Assert.AreEqual(t.value, "123.");

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.WHITESPACE);
            Assert.AreEqual(t.value, " ");
        }

        [Test]
        public void TestInteger23()
        {
            // test # digram.
            string code = "123.123 ";
            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(code));
            TokenStream ts = new TokenStream(ms, true, true, true, true);

            IEnumerable<Token> enumerable = ts.GetTokenEnumerable();
            IEnumerator<Token> i = enumerable.GetEnumerator();
            i.MoveNext();
            Token t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.NUMBER);
            Assert.AreEqual(t.value, "123.123");

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.WHITESPACE);
            Assert.AreEqual(t.value, " ");
        }

        [Test]
        public void TestInteger24()
        {
            // test # digram.
            string code = "123.123e123 ";
            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(code));
            TokenStream ts = new TokenStream(ms, true, true, true, true);

            IEnumerable<Token> enumerable = ts.GetTokenEnumerable();
            IEnumerator<Token> i = enumerable.GetEnumerator();
            i.MoveNext();
            Token t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.NUMBER);
            Assert.AreEqual(t.value, "123.123e123");

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.WHITESPACE);
            Assert.AreEqual(t.value, " ");
        }

        [Test]
        public void TestInteger25()
        {
            // test # digram.
            string code = "123.123e+123 ";
            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(code));
            TokenStream ts = new TokenStream(ms, true, true, true, true);

            IEnumerable<Token> enumerable = ts.GetTokenEnumerable();
            IEnumerator<Token> i = enumerable.GetEnumerator();
            i.MoveNext();
            Token t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.NUMBER);
            Assert.AreEqual(t.value, "123.123e+123");

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.WHITESPACE);
            Assert.AreEqual(t.value, " ");
        }

        [Test]
        public void TestInteger26()
        {
            // test # digram.
            string code = "123.123p123 ";
            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(code));
            TokenStream ts = new TokenStream(ms, true, true, true, true);

            IEnumerable<Token> enumerable = ts.GetTokenEnumerable();
            IEnumerator<Token> i = enumerable.GetEnumerator();
            i.MoveNext();
            Token t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.NUMBER);
            Assert.AreEqual(t.value, "123.123p123");

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.WHITESPACE);
            Assert.AreEqual(t.value, " ");
        }

        [Test]
        public void TestInteger27()
        {
            // test # digram.
            string code = "123.123p+123 ";
            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(code));
            TokenStream ts = new TokenStream(ms, true, true, true, true);

            IEnumerable<Token> enumerable = ts.GetTokenEnumerable();
            IEnumerator<Token> i = enumerable.GetEnumerator();
            i.MoveNext();
            Token t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.NUMBER);
            Assert.AreEqual(t.value, "123.123p+123");

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.WHITESPACE);
            Assert.AreEqual(t.value, " ");
        }

        [Test]
        public void TestInteger28()
        {
            // test # digram.
            string code = "1b23.123p+123 ";
            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(code));
            TokenStream ts = new TokenStream(ms, true, true, true, true);

            IEnumerable<Token> enumerable = ts.GetTokenEnumerable();
            IEnumerator<Token> i = enumerable.GetEnumerator();
            i.MoveNext();
            Token t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.NUMBER);
            Assert.AreEqual(t.value, "1");

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.IDENTIFIER);
            Assert.AreEqual(t.value, "b23");

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.NUMBER);
            Assert.AreEqual(t.value, ".123p+123");

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.WHITESPACE);
            Assert.AreEqual(t.value, " ");
        }

        [Test]
        public void TestInteger29()
        {
            // test # digram.
            string code = "0bb23.123p+123 ";
            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(code));
            TokenStream ts = new TokenStream(ms, true, true, true, true);

            IEnumerable<Token> enumerable = ts.GetTokenEnumerable();
            IEnumerator<Token> i = enumerable.GetEnumerator();
            i.MoveNext();
            Token t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.NUMBER);
            Assert.AreEqual(t.value, "0b");

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.IDENTIFIER);
            Assert.AreEqual(t.value, "b23");

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.NUMBER);
            Assert.AreEqual(t.value, ".123p+123");

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.WHITESPACE);
            Assert.AreEqual(t.value, " ");
        }

        [Test]
        public void TestInteger30()
        {
            // test # digram.
            string code = "0x1b23.123p+123 ";
            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(code));
            TokenStream ts = new TokenStream(ms, true, true, true, true);

            IEnumerable<Token> enumerable = ts.GetTokenEnumerable();
            IEnumerator<Token> i = enumerable.GetEnumerator();
            i.MoveNext();
            Token t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.NUMBER);
            Assert.AreEqual(t.value, "0x1b23.123p+123");

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.WHITESPACE);
            Assert.AreEqual(t.value, " ");
        }

        [Test]
        public void TestInteger31()
        {
            // test # digram.
            string code = "123.+123p+123 ";
            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(code));
            TokenStream ts = new TokenStream(ms, true, true, true, true);

            IEnumerable<Token> enumerable = ts.GetTokenEnumerable();
            IEnumerator<Token> i = enumerable.GetEnumerator();
            i.MoveNext();
            Token t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.NUMBER);
            Assert.AreEqual(t.value, "123.");

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.PLUS_SIGN);
            Assert.AreEqual(t.value, "+");

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.NUMBER);
            Assert.AreEqual(t.value, "123p+123");

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.WHITESPACE);
            Assert.AreEqual(t.value, " ");
        }

        [Test]
        public void TestInteger32()
        {
            // test # digram.
            string code = "123.123+p+123 ";
            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(code));
            TokenStream ts = new TokenStream(ms, true, true, true, true);

            IEnumerable<Token> enumerable = ts.GetTokenEnumerable();
            IEnumerator<Token> i = enumerable.GetEnumerator();
            i.MoveNext();
            Token t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.NUMBER);
            Assert.AreEqual(t.value, "123.123");

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.PLUS_SIGN);
            Assert.AreEqual(t.value, "+");

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.IDENTIFIER);
            Assert.AreEqual(t.value, "p");

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.PLUS_SIGN);
            Assert.AreEqual(t.value, "+");

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.NUMBER);
            Assert.AreEqual(t.value, "123");

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.WHITESPACE);
            Assert.AreEqual(t.value, " ");
        }

        [Test]
        public void TestInteger33()
        {
            // test # digram.
            string code = "123.123p++123 ";
            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(code));
            TokenStream ts = new TokenStream(ms, true, true, true, true);

            IEnumerable<Token> enumerable = ts.GetTokenEnumerable();
            IEnumerator<Token> i = enumerable.GetEnumerator();
            i.MoveNext();
            Token t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.NUMBER);
            Assert.AreEqual(t.value, "123.123p+");

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.PLUS_SIGN);
            Assert.AreEqual(t.value, "+");

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.NUMBER);
            Assert.AreEqual(t.value, "123");

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.WHITESPACE);
            Assert.AreEqual(t.value, " ");
        }

        [Test]
        public void TestIdentifierUnderscores()
        {
            // test # digram.
            string code = "__VA_ARGS__ ";
            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(code));
            TokenStream ts = new TokenStream(ms, true, true, true, true);

            IEnumerable<Token> enumerable = ts.GetTokenEnumerable();
            IEnumerator<Token> i = enumerable.GetEnumerator();
            i.MoveNext();
            Token t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.IDENTIFIER);
            Assert.AreEqual(t.value, "__VA_ARGS__");

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.WHITESPACE);
            Assert.AreEqual(t.value, " ");
        }

        [Test]
        public void TestComment1()
        {
            // test # digram.
            string code = @"#include ""stdio.h""

/* Does this work? *//* it had better! */
int main(void)
{
    return 0;
}";
            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(code));
            TokenStream ts = new TokenStream(ms, true, true, true, true);

            IEnumerable<Token> enumerable = ts.GetTokenEnumerable();
            IEnumerator<Token> i = enumerable.GetEnumerator();
            i.MoveNext();
            Token t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.HASH);
            Assert.AreEqual(t.value, "#");

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.IDENTIFIER);
            Assert.AreEqual(t.value, "include");

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.WHITESPACE);
            Assert.AreEqual(t.value, " ");

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.STRING);
            Assert.AreEqual(t.value, "\"stdio.h\"");

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.NEWLINE);
            Assert.AreEqual(t.value, "\n");

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.NEWLINE);
            Assert.AreEqual(t.value, "\n");

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.COMMENT);
            Assert.AreEqual(t.value, "/* Does this work? */");

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.COMMENT);
            Assert.AreEqual(t.value, "/* it had better! */");

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.NEWLINE);
            Assert.AreEqual(t.value, "\n");

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.IDENTIFIER);
            Assert.AreEqual(t.value, "int");
        }

        [Test]
        public void TestComment2()
        {
            // test # digram.
            string code = @"#include ""stdio.h""

// Does this work?
// it had better!
int main(void)
{
    return 0;
}";
            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(code));
            TokenStream ts = new TokenStream(ms, true, true, true, true);

            IEnumerable<Token> enumerable = ts.GetTokenEnumerable();
            IEnumerator<Token> i = enumerable.GetEnumerator();
            i.MoveNext();
            Token t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.HASH);
            Assert.AreEqual(t.value, "#");

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.IDENTIFIER);
            Assert.AreEqual(t.value, "include");

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.WHITESPACE);
            Assert.AreEqual(t.value, " ");

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.STRING);
            Assert.AreEqual(t.value, "\"stdio.h\"");

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.NEWLINE);
            Assert.AreEqual(t.value, "\n");

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.NEWLINE);
            Assert.AreEqual(t.value, "\n");

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.COMMENT);
            Assert.AreEqual(t.value, "// Does this work?\n");

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.COMMENT);
            Assert.AreEqual(t.value, "// it had better!\n");

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.IDENTIFIER);
            Assert.AreEqual(t.value, "int");
        }

        [Test]
        public void TestComment3()
        {
            // test # digram.
            string code = @"#include ""stdio.h""

// Does this work?\
// it had better!
int main(void)
{
    return 0;
}";
            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(code));
            TokenStream ts = new TokenStream(ms, true, true, true, true);

            IEnumerable<Token> enumerable = ts.GetTokenEnumerable();
            IEnumerator<Token> i = enumerable.GetEnumerator();
            i.MoveNext();
            Token t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.HASH);
            Assert.AreEqual(t.value, "#");

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.IDENTIFIER);
            Assert.AreEqual(t.value, "include");

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.WHITESPACE);
            Assert.AreEqual(t.value, " ");

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.STRING);
            Assert.AreEqual(t.value, "\"stdio.h\"");

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.NEWLINE);
            Assert.AreEqual(t.value, "\n");

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.NEWLINE);
            Assert.AreEqual(t.value, "\n");

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.COMMENT);
            Assert.AreEqual(t.value, "// Does this work?\\\n// it had better!\n");

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.IDENTIFIER);
            Assert.AreEqual(t.value, "int");
        }
    }
}
