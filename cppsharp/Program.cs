//
//  Program.cs
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
using libcppsharp;
using System.Reflection;
using System.Text;
using System.IO;
using System.Collections.Generic;

namespace cppsharp
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            String code = @"\
i\
n\
t\


//#include <stdio.h>\a
//#include ""string.h""

#define EXIST(A,B)
#define EXI
#define EXI2 A
#define EXI3(A, B) A ## B

#ifdef NOTEXIST
#define TMP
#warning IFNDEF is not properly implemented!
#endif

#ifndef EXIST
#undef TMP
#warning IFNDEF is not properly implemented!
#endif

int main(void)
{
    return 0;
}";
            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(code.ToCharArray(), 0, code.Length));
            libcppsharp.parser.Scanner scanner = new libcppsharp.parser.Scanner(ms, true, true);

            int tok = 3;
            while ((tok = scanner.Scan()) != 3)
            {
                Console.Out.WriteLine(tok);
            }
        }
    }
}
