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
            string code = @"
#include <stdio.h>

int main(void)
{
    return 0;
}";
            MemoryStream ms = new MemoryStream(System.Text.Encoding.ASCII.GetBytes(code));
            TokenStream ts = new TokenStream(ms);

            IEnumerable<Token> enumerable = ts.GetNextToken();
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

            //string oneThousandTwentyFourBlanks = 
            string code = @"#include <stdio.h>

int main(void)
{
    return 0;
}";
            code = "\xFEFF" + new string(' ', bufSize) + code;

            Encoding utf16 = new UnicodeEncoding(false, true);
            MemoryStream ms = new MemoryStream(utf16.GetBytes(code));
            TokenStream ts = new TokenStream(ms);

            IEnumerable<Token> enumerable = ts.GetNextToken();
            IEnumerator<Token> i = enumerable.GetEnumerator();
            i.MoveNext();
            Token t = i.Current;

            Assert.AreEqual((int)t.tokenType, (int)TokenType.HASH);
        }

    }
}
