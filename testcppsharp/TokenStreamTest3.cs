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
            Assert.AreEqual(t.value, "");

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
            Assert.AreEqual(t.value, "");

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.IDENTIFIER);
            Assert.AreEqual(t.value, "stdio");

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.PERIOD);
            Assert.AreEqual(t.value, "");

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.IDENTIFIER);
            Assert.AreEqual(t.value, "h");

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.GREATER_THEN);
            Assert.AreEqual(t.value, "");
        }
    }
}
