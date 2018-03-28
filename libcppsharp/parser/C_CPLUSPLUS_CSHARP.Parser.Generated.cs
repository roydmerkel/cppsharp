// This code was generated by the Gardens Point Parser Generator
// Copyright (c) Wayne Kelly, John Gough, QUT 2005-2014
// (see accompanying GPPGcopyright.rtf)

// GPPG version 1.5.2
// Machine:  ubuntu
// DateTime: 3/27/2018 8:38:13 PM
// UserName: rmerkel
// Input file <parser/C_CPLUSPLUS_CSHARP.Language.grammar.y - 3/23/2018 11:26:44 AM>

// options: no-lines gplex

using System;
using System.Collections.Generic;
using System.CodeDom.Compiler;
using System.Globalization;
using System.Text;
using QUT.Gppg;

namespace libcppsharp.parser
{
internal enum Token {error=2,EOF=3,WHITESPACE=4,NEWLINE=5,COLON_COLON=6,
    HASH=7,QUESTION_MARK=8,EQUAL_SIGN=9,LESS_THEN=10,GREATER_THEN=11,COLON=12,
    SEMI_COLON=13,PERCENT=14,COMMA=15,PERIOD=16,TILDE=17,PIPE=18,
    PLUS_SIGN=19,MINUS_SIGN=20,ASTERISK=21,AMPERSTAND=22,CARROT=23,EXCLAIMATION_MARK=24,
    GRAVE=25,AT=26,BACK_SLASH=27,FORWARD_SLASH=28,L_CURLY_BRACE=29,R_CURLY_BRACE=30,
    L_SQ_BRACKET=31,R_SQ_BRACKET=32,L_PAREN=33,R_PAREN=34,THIN_ARROW=35,PLUS_PLUS=36,
    MINUS_MINUS=37,DOT_STAR=38,THIN_ARROW_STAR=39,SAUCER=40,LESS_THEN_LESS_THEN=41,GREATER_THEN_GRATER_THEN=42,
    LESS_THEN_OR_EQUAL_TO=43,GREATER_THEN_OR_EQUAL_TO=44,EQUALS_EQUALS=45,NOT_EQUALS=46,AMPERSTAND_AMPERSTAND=47,PIPE_PIPE=48,
    ASTERISK_EQUALS=49,FORWARD_SLASH_EQUALS=50,PERCENT_EQUALS=51,PLUS_EQUALS=52,MINUS_EQUALS=53,LESS_THEN_LESS_THEN_EQUALS=54,
    GREATER_THEN_GREATER_THEN_EQUALS=55,AMPERSTAND_EQUALS=56,PIPE_EQUALS=57,CARROT_EQUALS=58,QUESTION_MARK_PERIOD=59,QUESTION_MARK_L_SQ_BRACKET=60,
    QUESTION_MARK_QUESTION_MARK=61,HASH_HASH=62,EQUALS_SIGN_GREATER_THEN=63,ELLIPSE=64,SIZEOF=65,DEFINED=66,
    IDENTIFIER=67,STRING=68,CHAR=69,NUMBER=70,COMMENT=71,TOKEN_PASTE=72,
    STRINGIFY=73};

internal partial struct ValueType
{ 
			public int n; 
			public string s; 
	   }
// Abstract base class for GPLEX scanners
[GeneratedCodeAttribute( "Gardens Point Parser Generator", "1.5.2")]
internal abstract class ScanBase : AbstractScanner<ValueType,LexLocation> {
  private LexLocation __yylloc = new LexLocation();
  public override LexLocation yylloc { get { return __yylloc; } set { __yylloc = value; } }
  protected virtual bool yywrap() { return true; }
}

// Utility class for encapsulating token information
[GeneratedCodeAttribute( "Gardens Point Parser Generator", "1.5.2")]
internal class ScanObj {
  public int token;
  public ValueType yylval;
  public LexLocation yylloc;
  public ScanObj( int t, ValueType val, LexLocation loc ) {
    this.token = t; this.yylval = val; this.yylloc = loc;
  }
}

[GeneratedCodeAttribute( "Gardens Point Parser Generator", "1.5.2")]
internal partial class C_CPLUSPLUS_CSHARPParser: ShiftReduceParser<ValueType, LexLocation>
{
  // Verbatim content from parser/C_CPLUSPLUS_CSHARP.Language.grammar.y - 3/23/2018 11:26:44 AM
//
//  C_CPLUSPLUS_CSHARP.Parser.Generated.cs
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

  // End verbatim content from parser/C_CPLUSPLUS_CSHARP.Language.grammar.y - 3/23/2018 11:26:44 AM

#pragma warning disable 649
  private static Dictionary<int, string> aliases;
#pragma warning restore 649
  private static Rule[] rules = new Rule[5];
  private static State[] states = new State[5];
  private static string[] nonTerms = new string[] {
      "main", "$accept", "number", };

  static C_CPLUSPLUS_CSHARPParser() {
    states[0] = new State(new int[]{70,4,3,-3},new int[]{-1,1,-3,3});
    states[1] = new State(new int[]{3,2});
    states[2] = new State(-1);
    states[3] = new State(-2);
    states[4] = new State(-4);

    for (int sNo = 0; sNo < states.Length; sNo++) states[sNo].number = sNo;

    rules[1] = new Rule(-2, new int[]{-1,3});
    rules[2] = new Rule(-1, new int[]{-3});
    rules[3] = new Rule(-3, new int[]{});
    rules[4] = new Rule(-3, new int[]{70});
  }

  protected override void Initialize() {
    this.InitSpecialTokens((int)Token.error, (int)Token.EOF);
    this.InitStates(states);
    this.InitRules(rules);
    this.InitNonTerminals(nonTerms);
  }

  protected override void DoAction(int action)
  {
#pragma warning disable 162, 1522
    switch (action)
    {
      case 4: // number -> NUMBER
{ Console.WriteLine("Rule -> number: {0}", ValueStack[ValueStack.Depth-1].n); }
        break;
    }
#pragma warning restore 162, 1522
  }

  protected override string TerminalToString(int terminal)
  {
    if (aliases != null && aliases.ContainsKey(terminal))
        return aliases[terminal];
    else if (((Token)terminal).ToString() != terminal.ToString(CultureInfo.InvariantCulture))
        return ((Token)terminal).ToString();
    else
        return CharToString((char)terminal);
  }

}
}
