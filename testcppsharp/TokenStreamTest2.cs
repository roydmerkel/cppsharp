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
            MemoryStream ms = new MemoryStream(System.Text.Encoding.ASCII.GetBytes(code));
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
            MemoryStream ms = new MemoryStream(System.Text.Encoding.ASCII.GetBytes(code));
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
            MemoryStream ms = new MemoryStream(System.Text.Encoding.ASCII.GetBytes(code));
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
            MemoryStream ms = new MemoryStream(System.Text.Encoding.ASCII.GetBytes(code));
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
            MemoryStream ms = new MemoryStream(System.Text.Encoding.ASCII.GetBytes(code));
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
            MemoryStream ms = new MemoryStream(System.Text.Encoding.ASCII.GetBytes(code));
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
            MemoryStream ms = new MemoryStream(System.Text.Encoding.ASCII.GetBytes(code));
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
            MemoryStream ms = new MemoryStream(System.Text.Encoding.ASCII.GetBytes(code));
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
            MemoryStream ms = new MemoryStream(System.Text.Encoding.ASCII.GetBytes(code));
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
            MemoryStream ms = new MemoryStream(System.Text.Encoding.ASCII.GetBytes(code));
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
            MemoryStream ms = new MemoryStream(System.Text.Encoding.ASCII.GetBytes(code));
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
            MemoryStream ms = new MemoryStream(System.Text.Encoding.ASCII.GetBytes(code));
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
            MemoryStream ms = new MemoryStream(System.Text.Encoding.ASCII.GetBytes(code));
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

            //string oneThousandTwentyFourBlanks = 
            string code = new string('#', bufSize - 1) + "<:";
            MemoryStream ms = new MemoryStream(System.Text.Encoding.ASCII.GetBytes(code));
            TokenStream ts = new TokenStream(ms, true, true);

            IEnumerable<Token> enumerable = ts.GetTokenEnumerable();
            IEnumerator<Token> i = enumerable.GetEnumerator();

            Token t;

            for (int iter = 0; iter < bufSize - 1; iter++)
            {
                Assert.IsTrue(i.MoveNext());
                t = i.Current;
                Assert.AreEqual((int)t.tokenType, (int)TokenType.HASH);
                Assert.AreEqual(iter + 1, t.column);
            }

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.L_SQ_BRACKET);
            Assert.AreEqual(bufSize, t.column);

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.EOF);
            Assert.AreEqual(bufSize + 2, t.column);
        }

        [Test]
        public void TestDigram15()
        {
            // test :> after a buffer change at the end of input.
            FieldInfo field = typeof(TokenStream).GetField("bufSize", BindingFlags.NonPublic | BindingFlags.Static);
            string bufSizeStr = field.GetRawConstantValue().ToString();
            int bufSize = int.Parse(bufSizeStr);

            //string oneThousandTwentyFourBlanks = 
            string code = new string('#', bufSize - 1) + ":>";
            MemoryStream ms = new MemoryStream(System.Text.Encoding.ASCII.GetBytes(code));
            TokenStream ts = new TokenStream(ms, true, true);

            IEnumerable<Token> enumerable = ts.GetTokenEnumerable();
            IEnumerator<Token> i = enumerable.GetEnumerator();

            Token t;

            for (int iter = 0; iter < bufSize - 1; iter++)
            {
                Assert.IsTrue(i.MoveNext());
                t = i.Current;
                Assert.AreEqual((int)t.tokenType, (int)TokenType.HASH);
                Assert.AreEqual(iter + 1, t.column);
            }

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.R_SQ_BRACKET);
            Assert.AreEqual(bufSize, t.column);

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.EOF);
            Assert.AreEqual(bufSize + 2, t.column);
        }

        [Test]
        public void TestDigram16()
        {
            // test <% after a buffer change at the end of input.
            FieldInfo field = typeof(TokenStream).GetField("bufSize", BindingFlags.NonPublic | BindingFlags.Static);
            string bufSizeStr = field.GetRawConstantValue().ToString();
            int bufSize = int.Parse(bufSizeStr);

            //string oneThousandTwentyFourBlanks = 
            string code = new string('#', bufSize - 1) + "<%";
            MemoryStream ms = new MemoryStream(System.Text.Encoding.ASCII.GetBytes(code));
            TokenStream ts = new TokenStream(ms, true, true);

            IEnumerable<Token> enumerable = ts.GetTokenEnumerable();
            IEnumerator<Token> i = enumerable.GetEnumerator();

            Token t;

            for (int iter = 0; iter < bufSize - 1; iter++)
            {
                Assert.IsTrue(i.MoveNext());
                t = i.Current;
                Assert.AreEqual((int)t.tokenType, (int)TokenType.HASH);
                Assert.AreEqual(iter + 1, t.column);
            }

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.L_CURLY_BRACE);
            Assert.AreEqual(bufSize, t.column);

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.EOF);
            Assert.AreEqual(bufSize + 2, t.column);
        }

        [Test]
        public void TestDigram17()
        {
            // test %> after a buffer change at the end of input.
            FieldInfo field = typeof(TokenStream).GetField("bufSize", BindingFlags.NonPublic | BindingFlags.Static);
            string bufSizeStr = field.GetRawConstantValue().ToString();
            int bufSize = int.Parse(bufSizeStr);

            //string oneThousandTwentyFourBlanks = 
            string code = new string('#', bufSize - 1) + "%>";
            MemoryStream ms = new MemoryStream(System.Text.Encoding.ASCII.GetBytes(code));
            TokenStream ts = new TokenStream(ms, true, true);

            IEnumerable<Token> enumerable = ts.GetTokenEnumerable();
            IEnumerator<Token> i = enumerable.GetEnumerator();

            Token t;

            for (int iter = 0; iter < bufSize - 1; iter++)
            {
                Assert.IsTrue(i.MoveNext());
                t = i.Current;
                Assert.AreEqual((int)t.tokenType, (int)TokenType.HASH);
                Assert.AreEqual(iter + 1, t.column);
            }

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.R_CURLY_BRACE);
            Assert.AreEqual(bufSize, t.column);

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.EOF);
            Assert.AreEqual(bufSize + 2, t.column);
        }

        [Test]
        public void TestDigram18()
        {
            // test %> after a buffer change at the end of input.
            FieldInfo field = typeof(TokenStream).GetField("bufSize", BindingFlags.NonPublic | BindingFlags.Static);
            string bufSizeStr = field.GetRawConstantValue().ToString();
            int bufSize = int.Parse(bufSizeStr);

            //string oneThousandTwentyFourBlanks = 
            string code = new string('#', bufSize - 1) + "%>";
            MemoryStream ms = new MemoryStream(System.Text.Encoding.ASCII.GetBytes(code));
            TokenStream ts = new TokenStream(ms, true, true);

            IEnumerable<Token> enumerable = ts.GetTokenEnumerable();
            IEnumerator<Token> i = enumerable.GetEnumerator();

            Token t;

            for (int iter = 0; iter < bufSize - 1; iter++)
            {
                Assert.IsTrue(i.MoveNext());
                t = i.Current;
                Assert.AreEqual((int)t.tokenType, (int)TokenType.HASH);
                Assert.AreEqual(iter + 1, t.column);
            }

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.R_CURLY_BRACE);
            Assert.AreEqual(bufSize, t.column);

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.EOF);
            Assert.AreEqual(bufSize + 2, t.column);
        }

        [Test]
        public void TestDigram19()
        {
            // test invalid < digram after a buffer change at the end of input.
            FieldInfo field = typeof(TokenStream).GetField("bufSize", BindingFlags.NonPublic | BindingFlags.Static);
            string bufSizeStr = field.GetRawConstantValue().ToString();
            int bufSize = int.Parse(bufSizeStr);

            //string oneThousandTwentyFourBlanks = 
            string code = new string('#', bufSize - 1) + "<#";
            MemoryStream ms = new MemoryStream(System.Text.Encoding.ASCII.GetBytes(code));
            TokenStream ts = new TokenStream(ms, true, true);

            IEnumerable<Token> enumerable = ts.GetTokenEnumerable();
            IEnumerator<Token> i = enumerable.GetEnumerator();

            Token t;

            for (int iter = 0; iter < bufSize - 1; iter++)
            {
                Assert.IsTrue(i.MoveNext());
                t = i.Current;
                Assert.AreEqual((int)t.tokenType, (int)TokenType.HASH);
                Assert.AreEqual(iter + 1, t.column);
            }

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.LESS_THEN);
            Assert.AreEqual(bufSize, t.column);

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.HASH);
            Assert.AreEqual(bufSize + 1, t.column);

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.EOF);
            Assert.AreEqual(bufSize + 2, t.column);
        }

        [Test]
        public void TestDigram20()
        {
            // test invalid : digram after a buffer change at the end of input.
            FieldInfo field = typeof(TokenStream).GetField("bufSize", BindingFlags.NonPublic | BindingFlags.Static);
            string bufSizeStr = field.GetRawConstantValue().ToString();
            int bufSize = int.Parse(bufSizeStr);

            //string oneThousandTwentyFourBlanks = 
            string code = new string('#', bufSize - 1) + ":#";
            MemoryStream ms = new MemoryStream(System.Text.Encoding.ASCII.GetBytes(code));
            TokenStream ts = new TokenStream(ms, true, true);

            IEnumerable<Token> enumerable = ts.GetTokenEnumerable();
            IEnumerator<Token> i = enumerable.GetEnumerator();

            Token t;

            for (int iter = 0; iter < bufSize - 1; iter++)
            {
                Assert.IsTrue(i.MoveNext());
                t = i.Current;
                Assert.AreEqual((int)t.tokenType, (int)TokenType.HASH);
                Assert.AreEqual(iter + 1, t.column);
            }

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.COLON);
            Assert.AreEqual(bufSize, t.column);

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.HASH);
            Assert.AreEqual(bufSize + 1, t.column);

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.EOF);
            Assert.AreEqual(bufSize + 2, t.column);
        }

        [Test]
        public void TestDigram21()
        {
            // test invalid % digram after a buffer change at the end of input.
            FieldInfo field = typeof(TokenStream).GetField("bufSize", BindingFlags.NonPublic | BindingFlags.Static);
            string bufSizeStr = field.GetRawConstantValue().ToString();
            int bufSize = int.Parse(bufSizeStr);

            //string oneThousandTwentyFourBlanks = 
            string code = new string('#', bufSize - 1) + "%#";
            MemoryStream ms = new MemoryStream(System.Text.Encoding.ASCII.GetBytes(code));
            TokenStream ts = new TokenStream(ms, true, true);

            IEnumerable<Token> enumerable = ts.GetTokenEnumerable();
            IEnumerator<Token> i = enumerable.GetEnumerator();

            Token t;

            for (int iter = 0; iter < bufSize - 1; iter++)
            {
                Assert.IsTrue(i.MoveNext());
                t = i.Current;
                Assert.AreEqual((int)t.tokenType, (int)TokenType.HASH);
                Assert.AreEqual(iter + 1, t.column);
            }

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.PERCENT);
            Assert.AreEqual(bufSize, t.column);

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.HASH);
            Assert.AreEqual(bufSize + 1, t.column);

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.EOF);
            Assert.AreEqual(bufSize + 2, t.column);
        }

        [Test]
        public void TestPunctuation()
        {
            string code = "!^&*()-+={}|~[];>,./#";
            MemoryStream ms = new MemoryStream(System.Text.Encoding.ASCII.GetBytes(code));
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

            Assert.AreEqual((int)t.tokenType, (int)TokenType.EOF);
        }
    }
}
