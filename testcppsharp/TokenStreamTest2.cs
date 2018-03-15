//
//  TokenStreamTest2.cs
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
    public class TokenStreamTest2
    {
        public TokenStreamTest2()
        {
        }

        [Test]
        public void TestDigram1()
        {
            // test # digram.
            string code = @"%:#include <stdio.h>

int main(void)
{
    return 0;
}";
            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(code));
            TokenStream ts = new TokenStream(ms, true, true);

            IEnumerable<Token> enumerable = ts.GetTokenEnumerable();
            IEnumerator<Token> i = enumerable.GetEnumerator();
            i.MoveNext();
            Token t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.HASH);

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.HASH);
        }

        [Test]
        public void TestDigram2()
        {
            // test %: with digrams turned off.
            string code = @"%:#include <stdio.h>

int main(void)
{
    return 0;
}";
            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(code));
            TokenStream ts = new TokenStream(ms, true, false);

            IEnumerable<Token> enumerable = ts.GetTokenEnumerable();
            IEnumerator<Token> i = enumerable.GetEnumerator();
            i.MoveNext();
            Token t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.PERCENT);

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.COLON);

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.HASH);
        }

        [Test]
        public void TestDigram3()
        {
            // test [ digram.
            string code = @"<:#include <stdio.h>

int main(void)
{
    return 0;
}";
            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(code));
            TokenStream ts = new TokenStream(ms, true, true);

            IEnumerable<Token> enumerable = ts.GetTokenEnumerable();
            IEnumerator<Token> i = enumerable.GetEnumerator();
            i.MoveNext();
            Token t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.L_SQ_BRACKET);

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.HASH);
        }

        [Test]
        public void TestDigram4()
        {
            // test ] digram.
            string code = @":>#include <stdio.h>

int main(void)
{
    return 0;
}";
            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(code));
            TokenStream ts = new TokenStream(ms, true, true);

            IEnumerable<Token> enumerable = ts.GetTokenEnumerable();
            IEnumerator<Token> i = enumerable.GetEnumerator();
            i.MoveNext();
            Token t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.R_SQ_BRACKET);

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.HASH);
        }

        [Test]
        public void TestDigram5()
        {
            // test { digram.
            string code = @"<%#include <stdio.h>

int main(void)
{
    return 0;
}";
            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(code));
            TokenStream ts = new TokenStream(ms, true, true);

            IEnumerable<Token> enumerable = ts.GetTokenEnumerable();
            IEnumerator<Token> i = enumerable.GetEnumerator();
            i.MoveNext();
            Token t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.L_CURLY_BRACE);

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.HASH);
        }

        [Test]
        public void TestDigram6()
        {
            // test } digram.
            string code = @"%>#include <stdio.h>

int main(void)
{
    return 0;
}";
            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(code));
            TokenStream ts = new TokenStream(ms, true, true);

            IEnumerable<Token> enumerable = ts.GetTokenEnumerable();
            IEnumerator<Token> i = enumerable.GetEnumerator();
            i.MoveNext();
            Token t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.R_CURLY_BRACE);

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.HASH);
        }

        [Test]
        public void TestDigram7()
        {
            // test <: with digrams turned off.
            string code = @"<:#include <stdio.h>

int main(void)
{
    return 0;
}";
            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(code));
            TokenStream ts = new TokenStream(ms, true, false);

            IEnumerable<Token> enumerable = ts.GetTokenEnumerable();
            IEnumerator<Token> i = enumerable.GetEnumerator();
            i.MoveNext();
            Token t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.LESS_THEN);

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.COLON);

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.HASH);
        }

        [Test]
        public void TestDigram8()
        {
            // test :> with digrams turned off.
            string code = @":>#include <stdio.h>

int main(void)
{
    return 0;
}";
            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(code));
            TokenStream ts = new TokenStream(ms, true, false);

            IEnumerable<Token> enumerable = ts.GetTokenEnumerable();
            IEnumerator<Token> i = enumerable.GetEnumerator();
            i.MoveNext();
            Token t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.COLON);

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.GREATER_THEN);

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.HASH);
        }

        [Test]
        public void TestDigram9()
        {
            // test <% with digrams turned off.
            string code = @"<%#include <stdio.h>

int main(void)
{
    return 0;
}";
            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(code));
            TokenStream ts = new TokenStream(ms, true, false);

            IEnumerable<Token> enumerable = ts.GetTokenEnumerable();
            IEnumerator<Token> i = enumerable.GetEnumerator();
            i.MoveNext();
            Token t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.LESS_THEN);

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.PERCENT);

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.HASH);
        }

        [Test]
        public void TestDigram10()
        {
            // test %> with digrams turned off.
            string code = @"%>#include <stdio.h>

int main(void)
{
    return 0;
}";
            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(code));
            TokenStream ts = new TokenStream(ms, true, false);

            IEnumerable<Token> enumerable = ts.GetTokenEnumerable();
            IEnumerator<Token> i = enumerable.GetEnumerator();
            i.MoveNext();
            Token t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.PERCENT);

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.GREATER_THEN);

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.HASH);
        }

        [Test]
        public void TestDigram11()
        {
            // test incomplete < digram.
            string code = @"<##include <stdio.h>

int main(void)
{
    return 0;
}";
            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(code));
            TokenStream ts = new TokenStream(ms, true, true);

            IEnumerable<Token> enumerable = ts.GetTokenEnumerable();
            IEnumerator<Token> i = enumerable.GetEnumerator();
            i.MoveNext();
            Token t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.LESS_THEN);

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.HASH);

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.HASH);
        }

        [Test]
        public void TestDigram12()
        {
            // test incomplete : digram.
            string code = @":##include <stdio.h>

int main(void)
{
    return 0;
}";
            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(code));
            TokenStream ts = new TokenStream(ms, true, true);

            IEnumerable<Token> enumerable = ts.GetTokenEnumerable();
            IEnumerator<Token> i = enumerable.GetEnumerator();
            i.MoveNext();
            Token t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.COLON);

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.HASH);

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.HASH);
        }

        [Test]
        public void TestDigram13()
        {
            // test incomplete % digram.
            string code = @"%##include <stdio.h>

int main(void)
{
    return 0;
}";
            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(code));
            TokenStream ts = new TokenStream(ms, true, true);

            IEnumerable<Token> enumerable = ts.GetTokenEnumerable();
            IEnumerator<Token> i = enumerable.GetEnumerator();
            i.MoveNext();
            Token t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.PERCENT);

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.HASH);

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.HASH);
        }

        [Test]
        public void TestDigram14()
        {
            // test <: after a buffer change at the end of input.
            FieldInfo field = typeof(TokenStream).GetField("bufSize", BindingFlags.NonPublic | BindingFlags.Static);
            string bufSizeStr = field.GetRawConstantValue().ToString();
            int bufSize = int.Parse(bufSizeStr);

            string code = new string('#', bufSize - 1) + "<:";
            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(code));
            TokenStream ts = new TokenStream(ms, true, true);

            IEnumerable<Token> enumerable = ts.GetTokenEnumerable();
            IEnumerator<Token> i = enumerable.GetEnumerator();

            Token t;

            for (int iter = 0; iter < bufSize - 1; iter++)
            {
                Assert.IsTrue(i.MoveNext());
                t = i.Current;
                Assert.AreEqual((int)t.tokenType, (int)TokenType.HASH);
            }

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.L_SQ_BRACKET);

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.EOF);
        }

        [Test]
        public void TestDigram15()
        {
            // test :> after a buffer change at the end of input.
            FieldInfo field = typeof(TokenStream).GetField("bufSize", BindingFlags.NonPublic | BindingFlags.Static);
            string bufSizeStr = field.GetRawConstantValue().ToString();
            int bufSize = int.Parse(bufSizeStr);

            string code = new string('#', bufSize - 1) + ":>";
            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(code));
            TokenStream ts = new TokenStream(ms, true, true);

            IEnumerable<Token> enumerable = ts.GetTokenEnumerable();
            IEnumerator<Token> i = enumerable.GetEnumerator();

            Token t;

            for (int iter = 0; iter < bufSize - 1; iter++)
            {
                Assert.IsTrue(i.MoveNext());
                t = i.Current;
                Assert.AreEqual((int)t.tokenType, (int)TokenType.HASH);
            }

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.R_SQ_BRACKET);

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.EOF);
        }

        [Test]
        public void TestDigram16()
        {
            // test <% after a buffer change at the end of input.
            FieldInfo field = typeof(TokenStream).GetField("bufSize", BindingFlags.NonPublic | BindingFlags.Static);
            string bufSizeStr = field.GetRawConstantValue().ToString();
            int bufSize = int.Parse(bufSizeStr);

            string code = new string('#', bufSize - 1) + "<%";
            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(code));
            TokenStream ts = new TokenStream(ms, true, true);

            IEnumerable<Token> enumerable = ts.GetTokenEnumerable();
            IEnumerator<Token> i = enumerable.GetEnumerator();

            Token t;

            for (int iter = 0; iter < bufSize - 1; iter++)
            {
                Assert.IsTrue(i.MoveNext());
                t = i.Current;
                Assert.AreEqual((int)t.tokenType, (int)TokenType.HASH);
            }

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.L_CURLY_BRACE);

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.EOF);
        }

        [Test]
        public void TestDigram17()
        {
            // test %> after a buffer change at the end of input.
            FieldInfo field = typeof(TokenStream).GetField("bufSize", BindingFlags.NonPublic | BindingFlags.Static);
            string bufSizeStr = field.GetRawConstantValue().ToString();
            int bufSize = int.Parse(bufSizeStr);

            string code = new string('#', bufSize - 1) + "%>";
            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(code));
            TokenStream ts = new TokenStream(ms, true, true);

            IEnumerable<Token> enumerable = ts.GetTokenEnumerable();
            IEnumerator<Token> i = enumerable.GetEnumerator();

            Token t;

            for (int iter = 0; iter < bufSize - 1; iter++)
            {
                Assert.IsTrue(i.MoveNext());
                t = i.Current;
                Assert.AreEqual((int)t.tokenType, (int)TokenType.HASH);
            }

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.R_CURLY_BRACE);

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.EOF);
        }

        [Test]
        public void TestDigram18()
        {
            // test %> after a buffer change at the end of input.
            FieldInfo field = typeof(TokenStream).GetField("bufSize", BindingFlags.NonPublic | BindingFlags.Static);
            string bufSizeStr = field.GetRawConstantValue().ToString();
            int bufSize = int.Parse(bufSizeStr);

            string code = new string('#', bufSize - 1) + "%>";
            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(code));
            TokenStream ts = new TokenStream(ms, true, true);

            IEnumerable<Token> enumerable = ts.GetTokenEnumerable();
            IEnumerator<Token> i = enumerable.GetEnumerator();

            Token t;

            for (int iter = 0; iter < bufSize - 1; iter++)
            {
                Assert.IsTrue(i.MoveNext());
                t = i.Current;
                Assert.AreEqual((int)t.tokenType, (int)TokenType.HASH);
            }

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.R_CURLY_BRACE);

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.EOF);
        }

        [Test]
        public void TestDigram19()
        {
            // test invalid < digram after a buffer change at the end of input.
            FieldInfo field = typeof(TokenStream).GetField("bufSize", BindingFlags.NonPublic | BindingFlags.Static);
            string bufSizeStr = field.GetRawConstantValue().ToString();
            int bufSize = int.Parse(bufSizeStr);

            string code = new string('#', bufSize - 1) + "<#";
            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(code));
            TokenStream ts = new TokenStream(ms, true, true);

            IEnumerable<Token> enumerable = ts.GetTokenEnumerable();
            IEnumerator<Token> i = enumerable.GetEnumerator();

            Token t;

            for (int iter = 0; iter < bufSize - 1; iter++)
            {
                Assert.IsTrue(i.MoveNext());
                t = i.Current;
                Assert.AreEqual((int)t.tokenType, (int)TokenType.HASH);
            }

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.LESS_THEN);

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.HASH);

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.EOF);
        }

        [Test]
        public void TestDigram20()
        {
            // test invalid : digram after a buffer change at the end of input.
            FieldInfo field = typeof(TokenStream).GetField("bufSize", BindingFlags.NonPublic | BindingFlags.Static);
            string bufSizeStr = field.GetRawConstantValue().ToString();
            int bufSize = int.Parse(bufSizeStr);

            string code = new string('#', bufSize - 1) + ":#";
            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(code));
            TokenStream ts = new TokenStream(ms, true, true);

            IEnumerable<Token> enumerable = ts.GetTokenEnumerable();
            IEnumerator<Token> i = enumerable.GetEnumerator();

            Token t;

            for (int iter = 0; iter < bufSize - 1; iter++)
            {
                Assert.IsTrue(i.MoveNext());
                t = i.Current;
                Assert.AreEqual((int)t.tokenType, (int)TokenType.HASH);
            }

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.COLON);

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.HASH);

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.EOF);
        }

        [Test]
        public void TestDigram21()
        {
            // test invalid % digram after a buffer change at the end of input.
            FieldInfo field = typeof(TokenStream).GetField("bufSize", BindingFlags.NonPublic | BindingFlags.Static);
            string bufSizeStr = field.GetRawConstantValue().ToString();
            int bufSize = int.Parse(bufSizeStr);

            string code = new string('#', bufSize - 1) + "%#";
            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(code));
            TokenStream ts = new TokenStream(ms, true, true);

            IEnumerable<Token> enumerable = ts.GetTokenEnumerable();
            IEnumerator<Token> i = enumerable.GetEnumerator();

            Token t;

            for (int iter = 0; iter < bufSize - 1; iter++)
            {
                Assert.IsTrue(i.MoveNext());
                t = i.Current;
                Assert.AreEqual((int)t.tokenType, (int)TokenType.HASH);
            }

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.PERCENT);

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.HASH);

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.EOF);
        }

        [Test]
        public void TestDigram22()
        {
            // test incomplete % digram.
            string code = @"%\
:#include <stdio.h>

int main(void)
{
    return 0;
}";
            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(code));
            TokenStream ts = new TokenStream(ms, true, true);

            IEnumerable<Token> enumerable = ts.GetTokenEnumerable();
            IEnumerator<Token> i = enumerable.GetEnumerator();
            i.MoveNext();
            Token t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.HASH);

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.HASH);
        }

        [Test]
        public void TestDigram23()
        {
            // test incomplete % digram.
            string code = @"%\ :#include <stdio.h>

int main(void)
{
    return 0;
}";
            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(code));
            TokenStream ts = new TokenStream(ms, true, true);
            FieldInfo fieldInfo = ts.GetType().GetField("ignoreStrayBackslash", BindingFlags.NonPublic | BindingFlags.Instance);
            fieldInfo.SetValue(ts, true);

            IEnumerable<Token> enumerable = ts.GetTokenEnumerable();
            IEnumerator<Token> i = enumerable.GetEnumerator();
            i.MoveNext();
            Token t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.PERCENT);

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.WHITESPACE);
            Assert.AreEqual(t.value, " ");

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.BACK_SLASH);
            Assert.AreEqual(t.value, "");

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.COLON);

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.HASH);
        }

        [Test]
        public void TestPunctuation()
        {
            string code = "!^&*()-+={}|~[];>,./#@`";
            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(code));
            TokenStream ts = new TokenStream(ms, true, true);

            IEnumerable<Token> enumerable = ts.GetTokenEnumerable();
            IEnumerator<Token> i = enumerable.GetEnumerator();

            Token t;

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.EXCLAIMATION_MARK);

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.CARROT);

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.AMPERSTAND);

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.ASTERISK);

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.L_PAREN);

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.R_PAREN);

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.MINUS_SIGN);

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.PLUS_SIGN);

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.EQUAL_SIGN);

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.L_CURLY_BRACE);

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.R_CURLY_BRACE);

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.PIPE);

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.TILDE);

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.L_SQ_BRACKET);

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.R_SQ_BRACKET);

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.SEMI_COLON);

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.GREATER_THEN);

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.COMMA);

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.PERIOD);

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.FORWARD_SLASH);

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.HASH);


            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.AT);


            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.GRAVE);

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.EOF);
        }

        [Test]
        public void TestPutBackQueuePush()
        {
            FieldInfo field = typeof(TokenStream).GetField("putBackBufSize", BindingFlags.NonPublic | BindingFlags.Static);
            string putBackBufSizeStr = field.GetRawConstantValue().ToString();
            int putBackBufSize = int.Parse(putBackBufSizeStr);

            String code = "#";

            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(code));
            TokenStream ts = new TokenStream(ms, false, false);

            MethodInfo PutBackArrayEmpty = ts.GetType().GetMethod("PutBackArrayEmpty", BindingFlags.NonPublic | BindingFlags.Instance);
            MethodInfo PushPutBackArray = ts.GetType().GetMethod("PushPutBackArray", BindingFlags.NonPublic | BindingFlags.Instance);

            var isEmpty = PutBackArrayEmpty.Invoke(ts, new object[] { });

            Assert.AreEqual(true, isEmpty);

            for (int i = 1; i <= putBackBufSize; i++)
            {
                var pushSuccess = PushPutBackArray.Invoke(ts, new object[] { Convert.ToChar('a' + i) });

                Assert.AreEqual(true, pushSuccess);

                isEmpty = PutBackArrayEmpty.Invoke(ts, new object[] { });

                Assert.AreEqual(false, isEmpty);
            }

            var finalPushSuccess = PushPutBackArray.Invoke(ts, new object[] { Convert.ToChar('a' + putBackBufSize + 1) });

            Assert.AreEqual(false, finalPushSuccess);

            isEmpty = PutBackArrayEmpty.Invoke(ts, new object[] { });

            Assert.AreEqual(false, isEmpty);
        }

        [Test]
        public void TestPutBackQueuePop()
        {
            FieldInfo field = typeof(TokenStream).GetField("putBackBufSize", BindingFlags.NonPublic | BindingFlags.Static);
            string putBackBufSizeStr = field.GetRawConstantValue().ToString();
            int putBackBufSize = int.Parse(putBackBufSizeStr);

            String code = "#";

            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(code));
            TokenStream ts = new TokenStream(ms, false, false);

            MethodInfo PutBackArrayEmpty = ts.GetType().GetMethod("PutBackArrayEmpty", BindingFlags.NonPublic | BindingFlags.Instance);
            MethodInfo PushPutBackArray = ts.GetType().GetMethod("PushPutBackArray", BindingFlags.NonPublic | BindingFlags.Instance);
            MethodInfo PopPutBackArray = ts.GetType().GetMethod("PopPutBackArray", BindingFlags.NonPublic | BindingFlags.Instance);

            var isEmpty = PutBackArrayEmpty.Invoke(ts, new object[] { });

            Assert.AreEqual(true, isEmpty);

            for (int i = 1; i <= putBackBufSize; i++)
            {
                var pushSuccess = PushPutBackArray.Invoke(ts, new object[] { Convert.ToChar('a' + i) });

                Assert.AreEqual(true, pushSuccess);

                isEmpty = PutBackArrayEmpty.Invoke(ts, new object[] { });

                Assert.AreEqual(false, isEmpty);
            }

            var finalPushSuccess = PushPutBackArray.Invoke(ts, new object[] { Convert.ToChar('a' + putBackBufSize + 1) });

            Assert.AreEqual(false, finalPushSuccess);

            isEmpty = PutBackArrayEmpty.Invoke(ts, new object[] { });

            Assert.AreEqual(false, isEmpty);

            for (int i = 1; i < putBackBufSize; i++)
            {
                var ch = PopPutBackArray.Invoke(ts, new object[] { });

                Assert.AreEqual(Convert.ToChar('a' + i), ch);

                isEmpty = PutBackArrayEmpty.Invoke(ts, new object[] { });

                Assert.AreEqual(false, isEmpty);
            }

            var ch2 = PopPutBackArray.Invoke(ts, new object[] { });

            Assert.AreEqual(Convert.ToChar('a' + putBackBufSize), ch2);

            isEmpty = PutBackArrayEmpty.Invoke(ts, new object[] { });

            Assert.AreEqual(true, isEmpty);
        }


        [Test]
        public void TestPutBackQueuePop2()
        {
            FieldInfo field = typeof(TokenStream).GetField("putBackBufSize", BindingFlags.NonPublic | BindingFlags.Static);
            string putBackBufSizeStr = field.GetRawConstantValue().ToString();
            int putBackBufSize = int.Parse(putBackBufSizeStr);

            String code = "#";

            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(code));
            TokenStream ts = new TokenStream(ms, false, false);

            MethodInfo PutBackArrayEmpty = ts.GetType().GetMethod("PutBackArrayEmpty", BindingFlags.NonPublic | BindingFlags.Instance);
            MethodInfo PushPutBackArray = ts.GetType().GetMethod("PushPutBackArray", BindingFlags.NonPublic | BindingFlags.Instance);
            MethodInfo PopPutBackArray = ts.GetType().GetMethod("PopPutBackArray", BindingFlags.NonPublic | BindingFlags.Instance);

            var isEmpty = PutBackArrayEmpty.Invoke(ts, new object[] { });

            Assert.AreEqual(true, isEmpty);

            for (int i = 1; i <= putBackBufSize; i++)
            {
                var pushSuccess = PushPutBackArray.Invoke(ts, new object[] { Convert.ToChar('a' + i) });

                Assert.AreEqual(true, pushSuccess);

                isEmpty = PutBackArrayEmpty.Invoke(ts, new object[] { });

                Assert.AreEqual(false, isEmpty);
            }

            var finalPushSuccess = PushPutBackArray.Invoke(ts, new object[] { Convert.ToChar('a' + putBackBufSize + 1) });

            Assert.AreEqual(false, finalPushSuccess);

            isEmpty = PutBackArrayEmpty.Invoke(ts, new object[] { });

            Assert.AreEqual(false, isEmpty);

            var ch = PopPutBackArray.Invoke(ts, new object[] { });

                Assert.AreEqual(Convert.ToChar('a' + 1), ch);

                isEmpty = PutBackArrayEmpty.Invoke(ts, new object[] { });

                Assert.AreEqual(false, isEmpty);

            finalPushSuccess = PushPutBackArray.Invoke(ts, new object[] { Convert.ToChar('a' + putBackBufSize + 1) });

            Assert.AreEqual(true, finalPushSuccess);

            for (int i = 1; i < putBackBufSize; i++)
            {
                ch = PopPutBackArray.Invoke(ts, new object[] { });

                Assert.AreEqual(Convert.ToChar('a' + i + 1), ch);

                isEmpty = PutBackArrayEmpty.Invoke(ts, new object[] { });

                Assert.AreEqual(false, isEmpty);
            }

            ch = PopPutBackArray.Invoke(ts, new object[] { });

            Assert.AreEqual(Convert.ToChar('a' + putBackBufSize + 1), ch);

            isEmpty = PutBackArrayEmpty.Invoke(ts, new object[] { });

            Assert.AreEqual(true, isEmpty);
        }
    }
}
