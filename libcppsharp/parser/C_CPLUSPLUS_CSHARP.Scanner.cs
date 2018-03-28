//
//  C_CPLUSPLUS_CSHARP.Scanner.cs
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
using System.Text;
using System.IO;

namespace libcppsharp.parser
{
    internal partial class C_CPLUSPLUS_CSHARPScanner
    {
#if !NOFILES
        internal C_CPLUSPLUS_CSHARPScanner(Stream file, bool handleTrigraphs, bool handleDigraphs = false, bool allowSlashNInString = true, bool treatStringSlashNAsNothing = true) : this(file)
        {
            this.handleTrigraphs = handleTrigraphs;
            this.handleDigraphs = handleDigraphs;
            this.allowSlashNInString = allowSlashNInString;
            this.treatStringSlashNAsNothing = treatStringSlashNAsNothing;
        }

        public C_CPLUSPLUS_CSHARPScanner(Stream file, string codepage, bool handleTrigraphs, bool handleDigraphs = false, bool allowSlashNInString = true, bool treatStringSlashNAsNothing = true) : this(file, codepage)
        {
            this.handleTrigraphs = handleTrigraphs;
            this.handleDigraphs = handleDigraphs;
            this.allowSlashNInString = allowSlashNInString;
            this.treatStringSlashNAsNothing = treatStringSlashNAsNothing;
        }
#endif // !NOFILES

        internal C_CPLUSPLUS_CSHARPScanner(bool handleTrigraphs, bool handleDigraphs = false, bool allowSlashNInString = true, bool treatStringSlashNAsNothing = true) : this()
        {
            this.handleTrigraphs = handleTrigraphs;
            this.handleDigraphs = handleDigraphs;
            this.allowSlashNInString = allowSlashNInString;
            this.treatStringSlashNAsNothing = treatStringSlashNAsNothing;
        }

        void GetNumber()
        {
            yylval.s = yytext;
            yylval.n = int.Parse(yytext);
        }

        void GetIdentifier()
        {
            yylval.s = yytext.Replace("\\\n", "");
        }

        void GetComment()
        {
            yylval.s = curTokVal.ToString();
        }

		public override void yyerror(string format, params object[] args)
		{
			base.yyerror(format, args);
			Console.WriteLine(format, args);
			Console.WriteLine();
		}
    }
}
