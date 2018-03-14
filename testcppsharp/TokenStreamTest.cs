//
//  TokenStreamTest.cs
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
    public class TokenStreamTest
    {
        public TokenStreamTest()
        {
        }

        [Test]
        public void CanGetToken()
        {
            string code = @"#include <stdio.h>

int main(void)
{
    return 0;
}";
            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(code));
            TokenStream ts = new TokenStream(ms);

            IEnumerable<Token> enumerable = ts.GetTokenEnumerable();
            IEnumerator<Token> i = enumerable.GetEnumerator();
            i.MoveNext();
            Token t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.HASH);
        }

        [Test]
        public void TestByteRefill()
        {
            FieldInfo field = typeof(TokenStream).GetField("bufSize", BindingFlags.NonPublic | BindingFlags.Static);
            string bufSizeStr = field.GetRawConstantValue().ToString();
            int bufSize = int.Parse(bufSizeStr);

            string code = @"#include <stdio.h>

int main(void)
{
    return 0;
}";
            code = "\xFEFF" + new string(' ', bufSize) + code;

            Encoding utf16 = new UnicodeEncoding(false, true);
            MemoryStream ms = new MemoryStream(utf16.GetBytes(code));
            TokenStream ts = new TokenStream(ms);

            IEnumerable<Token> enumerable = ts.GetTokenEnumerable();
            IEnumerator<Token> i = enumerable.GetEnumerator();
            i.MoveNext();
            Token t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.WHITESPACE);

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.HASH);
        }

        [Test]
        public void TestBasicTrigraph()
        {
            // test basic trigraph code.
            string code = @"??=include <stdio.h>

int main(void)
{
    return 0;
}";

            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(code));
            TokenStream ts = new TokenStream(ms, true);

            IEnumerable<Token> enumerable = ts.GetTokenEnumerable();
            IEnumerator<Token> i = enumerable.GetEnumerator();
            i.MoveNext();
            Token t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.HASH);
        }

        [Test]
        public void TestBasicTrigraph2()
        {
            // test basic trigraph code.
            string code = @"??=include <stdio.h>

int main(void)
{
    return 0;
}";

            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(code));
            TokenStream ts = new TokenStream(ms);

            IEnumerable<Token> enumerable = ts.GetTokenEnumerable();
            IEnumerator<Token> i = enumerable.GetEnumerator();
            i.MoveNext();
            Token t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.QUESTION_MARK);

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.QUESTION_MARK);

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.EQUAL_SIGN);
        }

        [Test]
        public void TestBasicTrigraph3()
        {
            // test basic trigraph code.
            string code = @"\??=#include <stdio.h>

int main(void)
{
    return 0;
}";

            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(code));
            TokenStream ts = new TokenStream(ms, true);
            FieldInfo fieldInfo = ts.GetType().GetField("ignoreStrayBackslash", BindingFlags.NonPublic | BindingFlags.Instance);
            fieldInfo.SetValue(ts, true);

            IEnumerable<Token> enumerable = ts.GetTokenEnumerable();
            IEnumerator<Token> i = enumerable.GetEnumerator();
            i.MoveNext();
            Token t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.BACK_SLASH);
            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.QUESTION_MARK);

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.QUESTION_MARK);

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.EQUAL_SIGN);

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.HASH);
        }

        [Test]
        public void TestBasicTrigraph4()
        {
            // test basic trigraph code.
            string code = @"?\?=#include <stdio.h>

int main(void)
{
    return 0;
}";

            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(code));
            TokenStream ts = new TokenStream(ms, true);
            FieldInfo fieldInfo = ts.GetType().GetField("ignoreStrayBackslash", BindingFlags.NonPublic | BindingFlags.Instance);
            fieldInfo.SetValue(ts, true);

            IEnumerable<Token> enumerable = ts.GetTokenEnumerable();
            IEnumerator<Token> i = enumerable.GetEnumerator();
            i.MoveNext();
            Token t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.QUESTION_MARK);

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.BACK_SLASH);

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.QUESTION_MARK);

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.EQUAL_SIGN);

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.HASH);
        }

        [Test]
        public void TestBasicTrigraph5()
        {
            // test basic trigraph code.
            string code = @"\   ??=#include <stdio.h>

int main(void)
{
    return 0;
}";

            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(code));
            TokenStream ts = new TokenStream(ms, true);
            FieldInfo fieldInfo = ts.GetType().GetField("ignoreStrayBackslash", BindingFlags.NonPublic | BindingFlags.Instance);
            fieldInfo.SetValue(ts, true);

            IEnumerable<Token> enumerable = ts.GetTokenEnumerable();
            IEnumerator<Token> i = enumerable.GetEnumerator();
            i.MoveNext();
            Token t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.WHITESPACE);

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.BACK_SLASH);

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.HASH);

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.HASH);
        }

        [Test]
        public void TestBasicTrigraph6()
        {
            // test basic trigraph code.
            string code = @"??/??=??=include <stdio.h>

int main(void)
{
    return 0;
}";

            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(code));
            TokenStream ts = new TokenStream(ms, true);
            FieldInfo fieldInfo = ts.GetType().GetField("ignoreStrayBackslash", BindingFlags.NonPublic | BindingFlags.Instance);
            fieldInfo.SetValue(ts, true);

            IEnumerable<Token> enumerable = ts.GetTokenEnumerable();
            IEnumerator<Token> i = enumerable.GetEnumerator();
            i.MoveNext();
            Token t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.BACK_SLASH);

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.QUESTION_MARK);


            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.QUESTION_MARK);

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.EQUAL_SIGN);

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.HASH);
        }

        [Test]
        public void TestBoundryTrigraph1()
        {
            // testcppsharp 1 question mark before buffer boundry.
            FieldInfo field = typeof(TokenStream).GetField("bufSize", BindingFlags.NonPublic | BindingFlags.Static);
            string bufSizeStr = field.GetRawConstantValue().ToString();
            int bufSize = int.Parse(bufSizeStr);

            string code = @"?=#include <stdio.h>

int main(void)
{
    return 0;
}";
            code = new string(' ', bufSize - 1) + "?" + code;

            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(code));
            TokenStream ts = new TokenStream(ms, true);

            IEnumerable<Token> enumerable = ts.GetTokenEnumerable();
            IEnumerator<Token> i = enumerable.GetEnumerator();
            i.MoveNext();
            Token t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.WHITESPACE);

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.HASH);

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.HASH);
        }

        [Test]
        public void TestBoundryTrigraph2()
        {
            // test two question marks before buffer boundry.
            FieldInfo field = typeof(TokenStream).GetField("bufSize", BindingFlags.NonPublic | BindingFlags.Static);
            string bufSizeStr = field.GetRawConstantValue().ToString();
            int bufSize = int.Parse(bufSizeStr);

            string code = @"=#include <stdio.h>

int main(void)
{
    return 0;
}";
            code = new string(' ', bufSize - 2) + "??" + code;

            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(code));
            TokenStream ts = new TokenStream(ms, true);

            IEnumerable<Token> enumerable = ts.GetTokenEnumerable();
            IEnumerator<Token> i = enumerable.GetEnumerator();
            i.MoveNext();
            Token t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.WHITESPACE);

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.HASH);

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.HASH);
        }

        [Test]
        public void TestBoundryTrigraph3()
        {
            // test triple question mark.
            string code = @"????=#include <stdio.h>

int main(void)
{
    return 0;
}";
            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(code));
            TokenStream ts = new TokenStream(ms, true);

            IEnumerable<Token> enumerable = ts.GetTokenEnumerable();
            IEnumerator<Token> i = enumerable.GetEnumerator();
            i.MoveNext();
            Token t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.QUESTION_MARK);

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.QUESTION_MARK);

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.HASH);

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.HASH);

        }

        [Test]
        public void TestBoundryTrigraph4()
        {
            // test bad trigraph
            string code = @"??#include <stdio.h>

int main(void)
{
    return 0;
}";
            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(code));
            TokenStream ts = new TokenStream(ms, true);

            IEnumerable<Token> enumerable = ts.GetTokenEnumerable();
            IEnumerator<Token> i = enumerable.GetEnumerator();
            i.MoveNext();
            Token t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.QUESTION_MARK);

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.QUESTION_MARK);

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.HASH);
        }

        [Test]
        public void TestBoundryTrigraph5()
        {
            // test bad trigraph at char 2.
            string code = @"?#include <stdio.h>

int main(void)
{
    return 0;
}";
            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(code));
            TokenStream ts = new TokenStream(ms, true);

            IEnumerable<Token> enumerable = ts.GetTokenEnumerable();
            IEnumerator<Token> i = enumerable.GetEnumerator();
            i.MoveNext();
            Token t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.QUESTION_MARK);

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.HASH);
        }

        [Test]
        public void TestBoundryTrigraph6()
        {
            // test bad trigraph at char 3 with one char before the boundry.
            FieldInfo field = typeof(TokenStream).GetField("bufSize", BindingFlags.NonPublic | BindingFlags.Static);
            string bufSizeStr = field.GetRawConstantValue().ToString();
            int bufSize = int.Parse(bufSizeStr);

            string code = @"?##include <stdio.h>

int main(void)
{
    return 0;
}";

            code = new string(' ', bufSize - 1) + "?" + code;
            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(code));
            TokenStream ts = new TokenStream(ms, true);

            IEnumerable<Token> enumerable = ts.GetTokenEnumerable();
            IEnumerator<Token> i = enumerable.GetEnumerator();
            i.MoveNext();
            Token t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.WHITESPACE);

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.QUESTION_MARK);

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.QUESTION_MARK);

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.HASH);

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.HASH);
        }


        [Test]
        public void TestBoundryTrigraph7()
        {
            // test bad trigraph at char 3 with one char before the boundry.
            FieldInfo field = typeof(TokenStream).GetField("bufSize", BindingFlags.NonPublic | BindingFlags.Static);
            string bufSizeStr = field.GetRawConstantValue().ToString();
            int bufSize = int.Parse(bufSizeStr);

            string code = @"##include <stdio.h>

int main(void)
{
    return 0;
}";

            code = new string(' ', bufSize - 1) + "?" + code;
            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(code));
            TokenStream ts = new TokenStream(ms, true);

            IEnumerable<Token> enumerable = ts.GetTokenEnumerable();
            IEnumerator<Token> i = enumerable.GetEnumerator();
            i.MoveNext();
            Token t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.WHITESPACE);

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.QUESTION_MARK);

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.HASH);

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.HASH);
        }

        [Test]
        public void TestBoundryTrigraph8()
        {
            // test bad trigraph at char 3 with one char before the boundry.
            FieldInfo field = typeof(TokenStream).GetField("bufSize", BindingFlags.NonPublic | BindingFlags.Static);
            string bufSizeStr = field.GetRawConstantValue().ToString();
            int bufSize = int.Parse(bufSizeStr);

            string code = @"##include <stdio.h>

int main(void)
{
    return 0;
}";

            code = new string(' ', bufSize - 2) + "??" + code;
            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(code));
            TokenStream ts = new TokenStream(ms, true);

            IEnumerable<Token> enumerable = ts.GetTokenEnumerable();
            IEnumerator<Token> i = enumerable.GetEnumerator();
            i.MoveNext();
            Token t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.WHITESPACE);

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.QUESTION_MARK);

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.QUESTION_MARK);

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.HASH);

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.HASH);
        }


        [Test]
        public void TestBoundryTrigraph9()
        {
            // test bad trigraph at char 3 with one char before the boundry.
            FieldInfo field = typeof(TokenStream).GetField("bufSize", BindingFlags.NonPublic | BindingFlags.Static);
            string bufSizeStr = field.GetRawConstantValue().ToString();
            int bufSize = int.Parse(bufSizeStr);

            string code = @"#include <stdio.h>

int main(void)
{
    return 0;
}";

            code = new string(' ', bufSize - 2) + "?#" + code;
            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(code));
            TokenStream ts = new TokenStream(ms, true);

            IEnumerable<Token> enumerable = ts.GetTokenEnumerable();
            IEnumerator<Token> i = enumerable.GetEnumerator();
            i.MoveNext();
            Token t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.WHITESPACE);

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.QUESTION_MARK);

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.HASH);

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.HASH);
        }

        [Test]
        public void TestBoundryTrigraph10()
        {
            // test trigraph at end of file.
            FieldInfo field = typeof(TokenStream).GetField("bufSize", BindingFlags.NonPublic | BindingFlags.Static);
            string bufSizeStr = field.GetRawConstantValue().ToString();
            int bufSize = int.Parse(bufSizeStr);

            string code = new string(' ', bufSize - 3) + "??=";
            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(code));
            TokenStream ts = new TokenStream(ms, true);

            IEnumerable<Token> enumerable = ts.GetTokenEnumerable();
            IEnumerator<Token> i = enumerable.GetEnumerator();
            i.MoveNext();
            Token t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.WHITESPACE);

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.HASH);

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.EOF);
        }

        [Test]
        public void TestBoundryTrigraph11()
        {
            // test trigraph at end of file, with two chars before buffer end.
            FieldInfo field = typeof(TokenStream).GetField("bufSize", BindingFlags.NonPublic | BindingFlags.Static);
            string bufSizeStr = field.GetRawConstantValue().ToString();
            int bufSize = int.Parse(bufSizeStr);

            string code = @"=";

            code = new string(' ', bufSize - 2) + "??" + code;
            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(code));
            TokenStream ts = new TokenStream(ms, true);

            IEnumerable<Token> enumerable = ts.GetTokenEnumerable();
            IEnumerator<Token> i = enumerable.GetEnumerator();
            i.MoveNext();
            Token t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.WHITESPACE);

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.HASH);

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.EOF);
        }

        [Test]
        public void TestBoundryTrigraph12()
        {
            // test trigraph at end of file, with two chars before buffer end.
            FieldInfo field = typeof(TokenStream).GetField("bufSize", BindingFlags.NonPublic | BindingFlags.Static);
            string bufSizeStr = field.GetRawConstantValue().ToString();
            int bufSize = int.Parse(bufSizeStr);

            string code = @"?=";

            code = new string(' ', bufSize - 1) + "?" + code;
            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(code));
            TokenStream ts = new TokenStream(ms, true);

            IEnumerable<Token> enumerable = ts.GetTokenEnumerable();
            IEnumerator<Token> i = enumerable.GetEnumerator();
            i.MoveNext();
            Token t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.WHITESPACE);

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.HASH);

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.EOF);
        }

        [Test]
        public void TestBoundryTrigraph13()
        {
            // test unfinished trigraph at the end of input, after a buffer.
            FieldInfo field = typeof(TokenStream).GetField("bufSize", BindingFlags.NonPublic | BindingFlags.Static);
            string bufSizeStr = field.GetRawConstantValue().ToString();
            int bufSize = int.Parse(bufSizeStr);

            string code = @"?";

            code = new string(' ', bufSize - 1) + "?" + code;
            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(code));
            TokenStream ts = new TokenStream(ms, true);

            IEnumerable<Token> enumerable = ts.GetTokenEnumerable();
            IEnumerator<Token> i = enumerable.GetEnumerator();
            i.MoveNext();
            Token t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.WHITESPACE);

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.QUESTION_MARK);

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.QUESTION_MARK);

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.EOF);
        }

        [Test]
        public void TestBoundryTrigraph14()
        {
            // test unfinished trigraph at the end of input, after a buffer.
            FieldInfo field = typeof(TokenStream).GetField("bufSize", BindingFlags.NonPublic | BindingFlags.Static);
            string bufSizeStr = field.GetRawConstantValue().ToString();
            int bufSize = int.Parse(bufSizeStr);

            string code;

            code = new string(' ', bufSize - 2) + "??";
            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(code));
            TokenStream ts = new TokenStream(ms, true);

            IEnumerable<Token> enumerable = ts.GetTokenEnumerable();
            IEnumerator<Token> i = enumerable.GetEnumerator();
            i.MoveNext();
            Token t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.WHITESPACE);

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.QUESTION_MARK);

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.QUESTION_MARK);

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.EOF);
        }

        [Test]
        public void TestBoundryTrigraph15()
        {
            // test unfinished trigraph at the end of input, after a buffer.
            FieldInfo field = typeof(TokenStream).GetField("bufSize", BindingFlags.NonPublic | BindingFlags.Static);
            string bufSizeStr = field.GetRawConstantValue().ToString();
            int bufSize = int.Parse(bufSizeStr);

            string code;

            code = new string(' ', bufSize - 1) + "?";
            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(code));
            TokenStream ts = new TokenStream(ms, true);

            IEnumerable<Token> enumerable = ts.GetTokenEnumerable();
            IEnumerator<Token> i = enumerable.GetEnumerator();
            i.MoveNext();
            Token t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.WHITESPACE);

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.QUESTION_MARK);

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.EOF);
        }

        [Test]
        public void TestTrigraphStreamNewlines()
        {
            // test Windows/Mac OSX line endings.
            String code = "s\r\na";
            char ch;

            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(code));
            TrigraphStream ts = new TrigraphStream(ms);

            IEnumerable<char> enumerable = ts.GetCharEnumerable();
            IEnumerator<char> i = enumerable.GetEnumerator();

            i.MoveNext();
            ch = i.Current;

            Assert.AreEqual(ch, 's');

            i.MoveNext();
            ch = i.Current;

            Assert.AreEqual(ch, '\n');

            i.MoveNext();
            ch = i.Current;

            Assert.AreEqual(ch, 'a');
        }

        [Test]
        public void TestTrigraphStreamNewlines2()
        {
            // test linux line endings.
            String code = "s\na";
            char ch;

            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(code));
            TrigraphStream ts = new TrigraphStream(ms);

            IEnumerable<char> enumerable = ts.GetCharEnumerable();
            IEnumerator<char> i = enumerable.GetEnumerator();

            i.MoveNext();
            ch = i.Current;

            Assert.AreEqual(ch, 's');

            i.MoveNext();
            ch = i.Current;

            Assert.AreEqual(ch, '\n');

            i.MoveNext();
            ch = i.Current;

            Assert.AreEqual(ch, 'a');
        }

        [Test]
        public void TestTrigraphStreamNewlines3()
        {
            // test old mac line endings.
            String code = "s\ra";
            char ch;

            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(code));
            TrigraphStream ts = new TrigraphStream(ms);

            IEnumerable<char> enumerable = ts.GetCharEnumerable();
            IEnumerator<char> i = enumerable.GetEnumerator();

            i.MoveNext();
            ch = i.Current;

            Assert.AreEqual(ch, 's');

            i.MoveNext();
            ch = i.Current;

            Assert.AreEqual(ch, '\n');

            i.MoveNext();
            ch = i.Current;

            Assert.AreEqual(ch, 'a');
        }

        [Test]
        public void TestTrigraphStreamNewlines4()
        {
            // test broken line endings (linux file opened on old mac.)
            String code = "s\n\ra";
            char ch;

            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(code));
            TrigraphStream ts = new TrigraphStream(ms);

            IEnumerable<char> enumerable = ts.GetCharEnumerable();
            IEnumerator<char> i = enumerable.GetEnumerator();

            i.MoveNext();
            ch = i.Current;

            Assert.AreEqual(ch, 's');

            i.MoveNext();
            ch = i.Current;

            Assert.AreEqual(ch, '\n');

            i.MoveNext();
            ch = i.Current;

            Assert.AreEqual(ch, 'a');
        }

        [Test]
        public void TestTrigraphStreamNewlines5()
        {
            // test broken line endings (linux file opened on old mac.)
            String code = "s\r\n\r\na";
            char ch;

            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(code));
            TrigraphStream ts = new TrigraphStream(ms);

            IEnumerable<char> enumerable = ts.GetCharEnumerable();
            IEnumerator<char> i = enumerable.GetEnumerator();

            i.MoveNext();
            ch = i.Current;

            Assert.AreEqual(ch, 's');

            i.MoveNext();
            ch = i.Current;

            Assert.AreEqual(ch, '\n');

            i.MoveNext();
            ch = i.Current;

            Assert.AreEqual(ch, '\n');

            i.MoveNext();
            ch = i.Current;

            Assert.AreEqual(ch, 'a');
        }

        [Test]
        public void TestTrigraphStreamNewlines6()
        {
            // test broken line endings (linux file opened on old mac.)
            String code = "s\r\n\n\ra";
            char ch;

            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(code));
            TrigraphStream ts = new TrigraphStream(ms);

            IEnumerable<char> enumerable = ts.GetCharEnumerable();
            IEnumerator<char> i = enumerable.GetEnumerator();

            i.MoveNext();
            ch = i.Current;

            Assert.AreEqual(ch, 's');

            i.MoveNext();
            ch = i.Current;

            Assert.AreEqual(ch, '\n');

            i.MoveNext();
            ch = i.Current;

            Assert.AreEqual(ch, '\n');

            i.MoveNext();
            ch = i.Current;

            Assert.AreEqual(ch, 'a');
        }

        [Test]
        public void TestTrigraphStreamNewlines7()
        {
            // test broken line endings (linux file opened on old mac.)
            String code = "s\r\n\na";
            char ch;

            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(code));
            TrigraphStream ts = new TrigraphStream(ms);

            IEnumerable<char> enumerable = ts.GetCharEnumerable();
            IEnumerator<char> i = enumerable.GetEnumerator();

            i.MoveNext();
            ch = i.Current;

            Assert.AreEqual(ch, 's');

            i.MoveNext();
            ch = i.Current;

            Assert.AreEqual(ch, '\n');

            i.MoveNext();
            ch = i.Current;

            Assert.AreEqual(ch, '\n');

            i.MoveNext();
            ch = i.Current;

            Assert.AreEqual(ch, 'a');
        }

        [Test]
        public void TestTrigraphStreamNewlines8()
        {
            // test broken line endings (linux file opened on old mac.)
            String code = "s\n\r\n\ra";
            char ch;

            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(code));
            TrigraphStream ts = new TrigraphStream(ms);

            IEnumerable<char> enumerable = ts.GetCharEnumerable();
            IEnumerator<char> i = enumerable.GetEnumerator();

            i.MoveNext();
            ch = i.Current;

            Assert.AreEqual(ch, 's');

            i.MoveNext();
            ch = i.Current;

            Assert.AreEqual(ch, '\n');

            i.MoveNext();
            ch = i.Current;

            Assert.AreEqual(ch, '\n');

            i.MoveNext();
            ch = i.Current;

            Assert.AreEqual(ch, 'a');
        }

        [Test]
        public void TestTrigraphStreamNewlines9()
        {
            // test broken line endings (linux file opened on old mac.)
            String code = "s\r\ra";
            char ch;

            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(code));
            TrigraphStream ts = new TrigraphStream(ms);

            IEnumerable<char> enumerable = ts.GetCharEnumerable();
            IEnumerator<char> i = enumerable.GetEnumerator();

            i.MoveNext();
            ch = i.Current;

            Assert.AreEqual(ch, 's');

            i.MoveNext();
            ch = i.Current;

            Assert.AreEqual(ch, '\n');

            i.MoveNext();
            ch = i.Current;

            Assert.AreEqual(ch, '\n');

            i.MoveNext();
            ch = i.Current;

            Assert.AreEqual(ch, 'a');
        }

        [Test]
        public void TestTrigraphStreamNewlines10()
        {
            // test broken line endings (linux file opened on old mac.)
            String code = "s\n\r\r\na";
            char ch;

            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(code));
            TrigraphStream ts = new TrigraphStream(ms);

            IEnumerable<char> enumerable = ts.GetCharEnumerable();
            IEnumerator<char> i = enumerable.GetEnumerator();

            i.MoveNext();
            ch = i.Current;

            Assert.AreEqual(ch, 's');

            i.MoveNext();
            ch = i.Current;

            Assert.AreEqual(ch, '\n');

            i.MoveNext();
            ch = i.Current;

            Assert.AreEqual(ch, '\n');

            i.MoveNext();
            ch = i.Current;

            Assert.AreEqual(ch, 'a');
        }

        [Test]
        public void TestWhitespaceToken()
        {
            string code = @"
       	#include <stdio.h>

int main(void)
{
    return 0;
}";
            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(code));
            TokenStream ts = new TokenStream(ms);

            IEnumerable<Token> enumerable = ts.GetTokenEnumerable();
            IEnumerator<Token> i = enumerable.GetEnumerator();
            i.MoveNext();
            Token t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.NEWLINE);
            Assert.AreEqual(t.value, "");
            Assert.AreEqual(t.column, 1);
            Assert.AreEqual(t.line, 1);

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.WHITESPACE);
            Assert.AreEqual(t.value, "       \t");
            Assert.AreEqual(t.column, 1);
            Assert.AreEqual(t.line, 2);

            i.MoveNext();
            t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.HASH);
        }

        [Test]
        public void TestStrayBackslash()
        {
            string code = @"\#include <stdio.h>

int main(void)
{
    return 0;
}";
            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(code));
            TokenStream ts = new TokenStream(ms);

            IEnumerable<Token> enumerable = ts.GetTokenEnumerable();
            IEnumerator<Token> i = enumerable.GetEnumerator();
            try
            {
                i.MoveNext();
                Assert.Fail();
            }
            catch (InvalidDataException)
            {
                Assert.Pass();
            }
        }

        [Test]
        public void TestStrayBackslash2()
        {
            string code = @"\??=include <stdio.h>

int main(void)
{
    return 0;
}";
            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(code));
            TokenStream ts = new TokenStream(ms);

            IEnumerable<Token> enumerable = ts.GetTokenEnumerable();
            IEnumerator<Token> i = enumerable.GetEnumerator();
            try
            {
                i.MoveNext();
                Assert.Fail();
            }
            catch (InvalidDataException)
            {
                Assert.Pass();
            }
        }

        [Test]
        public void TestStrayBackslash3()
        {
            string code = @"\";
            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(code));
            TokenStream ts = new TokenStream(ms);

            IEnumerable<Token> enumerable = ts.GetTokenEnumerable();
            IEnumerator<Token> i = enumerable.GetEnumerator();
            try
            {
                i.MoveNext();
                Assert.Fail();
            }
            catch (InvalidDataException)
            {
                Assert.Pass();
            }
        }
    }
}
