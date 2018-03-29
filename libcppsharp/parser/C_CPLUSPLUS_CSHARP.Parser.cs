//
//  C_CPLUSPLUS_CSHARP.Parser.cs
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

namespace libcppsharp.parser
{
    internal partial class C_CPLUSPLUS_CSHARPParser
    {
        private bool trigraphs = false;
        private bool digraphs = false;
        public C_CPLUSPLUS_CSHARPParser(bool trigraphs = false, bool digraphs = false) : base(null) { this.trigraphs = trigraphs; this.digraphs = digraphs; }

        public void Parse(string s)
        {
            byte[] inputBuffer = System.Text.Encoding.Default.GetBytes(s);
            MemoryStream str = new MemoryStream(inputBuffer);
            TrigraphStream stream = new TrigraphStream(str, trigraphs, digraphs);
            this.Scanner = new C_CPLUSPLUS_CSHARPScanner(stream, trigraphs, digraphs);
            this.Parse();
        }

        public void Parse(Stream s)
        {
            TrigraphStream stream = new TrigraphStream(s, trigraphs, digraphs);
            this.Scanner = new C_CPLUSPLUS_CSHARPScanner(stream, trigraphs, digraphs);
            this.Parse();
        }
    }
}
