/*

  C_CPLUSPLUS_CSHARP.Language.analyzer.lex

  Author:
       Roy Merkel <merkel-roy@comcast.net>

  Copyright (c) 2018 Roy Merkel

  This program is free software: you can redistribute it and/or modify
  it under the terms of the GNU General Public License as published by
  the Free Software Foundation, either version 3 of the License, or
  (at your option) any later version.

  This program is distributed in the hope that it will be useful,
  but WITHOUT ANY WARRANTY; without even the implied warranty of
  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
  GNU General Public License for more details.

  You should have received a copy of the GNU General Public License
  along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

%namespace libcppsharp.parser
%scannertype C_CPLUSPLUS_CSHARPScanner
%visibility internal
%tokentype Token

%option stack, classes, minimize, parser, verbose, persistbuffer, noembedbuffers, unicode 

%x C_COMMENT
%x CPP_COMMENT

%{
    StringBuilder curTokVal = new StringBuilder();
%}

IdentUndAZaz    [_a-zA-Z]
Ident0000       [\u00A8\u00AA\u00AD\u00AF\u00B2-\u00B5\u00B7-\u00BA\u00BC-\u00BE\u00C0-\u00D6\u00D8-\u00F6\u00F8-\u00FF\u0100-\u02FF\u0370-\u167F\u1681-\u180D\u180F-\u1DBF\u1E00-\u1FFF]
Ident2000       [\u200B-\u200D\u202A-\u202E\u203F-\u2040\u2054\u2060-\u206F\u2070-\u20CF\u2100-\u218F\u2460-\u24FF\u2776-\u2793\u2C00-\u2DFF\u2E80-\u2FFF]
Ident3000       [\u3004-\u3007\u3021-\u302F\u3031-\u303F\u3040-\uD7FF]
IdentF000       [\uF900-\uFD3D\uFD40-\uFDCF\uFDF0-\uFE1F\uFE30-\uFE44\uFE47-\uFFFD]
//Ident10000      ([\uD800-\uD83E][\uDC00-\uDFFF]|\uD83F[\uDC00-\uDFFD])
Ident10000      [\U00010000-\U0001FFFD]
//Ident20000      ([\uD840-\uD87E][\uDC00-\uDFFF]|\uD87F[\uDC00-\uDFFD])
Ident20000      [\U00020000-\U0002FFFD]
//Ident30000      ([\uD880-\uD8BE][\uDC00-\uDFFF]|\uD8BF[\uDC00-\uDFFD])
Ident30000      [\U00030000-\U0003FFFD]
//Ident40000      ([\uD8C0-\uD8FE][\uDC00-\uDFFF]|\uD8FF[\uDC00-\uDFFD])
Ident40000      [\U00040000-\U0004FFFD]
//Ident50000      ([\uD900-\uD93E][\uDC00-\uDFFF]|\uD93F[\uDC00-\uDFFD])
Ident50000      [\U00050000-\U0005FFFD]
//Ident60000      ([\uD940-\uD97E][\uDC00-\uDFFF]|\uD97F[\uDC00-\uDFFD])
Ident60000      [\U00060000-\U0006FFFD]
//Ident70000      ([\uD980-\uD9BE][\uDC00-\uDFFF]|\uD9BF[\uDC00-\uDFFD])
Ident70000      [\U00070000-\U0007FFFD]
//Ident80000      ([\uD9C0-\uD9FE][\uDC00-\uDFFF]|\uD9FF[\uDC00-\uDFFD])
Ident80000      [\U00080000-\U0008FFFD]
//Ident90000      ([\uDA00-\uDA3E][\uDC00-\uDFFF]|\uDA3F[\uDC00-\uDFFD])
Ident90000      [\U00090000-\U0009FFFD]
//IdentA0000      ([\uDA40-\uDA7E][\uDC00-\uDFFF]|\uDA7F[\uDC00-\uDFFD])
IdentA0000      [\U000A0000-\U000AFFFD]
//IdentB0000      ([\uDA80-\uDABE][\uDC00-\uDFFF]|\uDABF[\uDC00-\uDFFD])
IdentB0000      [\U000B0000-\U000BFFFD]
//IdentC0000      ([\uDAC0-\uDAFE][\uDC00-\uDFFF]|\uDAFF[\uDC00-\uDFFD])
IdentC0000      [\U000C0000-\U000CFFFD]
//IdentD0000      ([\uDB00-\uDB3E][\uDC00-\uDFFF]|\uDB3F[\uDC00-\uDFFD])
IdentD0000      [\U000D0000-\U000DFFFD]
//IdentE0000      ([\uDB40-\uDB7E][\uDC00-\uDFFF]|\uDB7F[\uDC00-\uDFFD])
IdentE0000      [\U000E0000-\U000EFFFD]
IdentCh1        ([$]|{IdentUndAZaz}|{Ident0000}|{Ident2000}|{Ident3000}|{IdentF000}|{Ident10000}|{Ident20000}|{Ident30000}|{Ident40000}|{Ident50000}|{Ident60000}|{Ident70000}|{Ident80000}|{Ident90000}|{IdentA0000}|{IdentB0000}|{IdentC0000}|{IdentD0000}|{IdentE0000})

IdentCh2Unicode ([\u0300-\u036F\u1DC0-\u1DFF\u20D0-\u20FF\uFE20-\uFE2F])
Ident09         ([0-9])
IdentCh2        ({IdentCh1}|{IdentCh2Unicode}|{Ident09})

Identifier      ({IdentCh1}((\\\n)*{IdentCh2})*)

Space           [ \t\v\f]
SlashNL         (\\\n)
Whitespace      ({Space}|{SlashNL})

numberSuffix    (([a-zA-Z]((\\\n)*[a-zA-Z])*)|(_(\\\n)*{Identifier}))
hexNumberSuffix (([g-zG-Z]((\\\n)*[g-zG-Z])*)|(_(\\\n)*{Identifier}))
floatSufix      (([a-zA-Z]((\\\n)*[a-zA-Z])*)|(_(\\\n)*{Identifier}))
stringSuffix    (([a-zA-Z]((\\\n)*[a-zA-Z])*)|(_(\\\n)*{Identifier}))

mandDigSeq      [0-9]((\\\n)*[0-9]*)*
mandDigSepSeq   [0-9]((\\\n)*[0-9']*)*
mandHexSeq      [0-9a-fA-F]((\\\n)*[0-9a-fA-F]*)*
optHexSeq       {mandHexSeq}?

decimalLit      [1-9]((\\\n)*[0-9']*)*
octLit          0((\\\n)*[0-7]*)*
hexLit          0(\\\n)*[xX](\\\n)*[0-9a-fA-F]((\\\n)*[0-9a-fA-F]*)*
binLit          0(\\\n)*[bB](\\\n)*[0-1]((\\\n)*[0-1]*)*

floatExp        [eEpP](\\\n)*[\-+]?(\\\n)*{mandDigSeq}

%%

/* Scanner body */

s(\\\n)*i(\\\n)*z(\\\n)*e(\\\n)*o(\\\n)*f                                                               { Console.WriteLine("token: {0}", yytext); return (int)Token.SIZEOF; }
d(\\\n)*e(\\\n)*f(\\\n)*i(\\\n)*n(\\\n)*e(\\\n)*d                                                       { Console.WriteLine("token: {0}", yytext); return (int)Token.DEFINED; }
{Identifier}                                                                                            { Console.WriteLine("token: {0}", yytext); GetIdentifier(); return (int)Token.IDENTIFIER; }

{Whitespace}+	                                                                                        /* skip */
\\{Space}*\n                                                                                            /* skip */

\\{Space}*([^\n]|<<EOF>>)                                                                               { throw new InvalidDataException("Unexpected \\"); }

\n                                                                                                      { Console.WriteLine("token: {0}", yytext); return (int)Token.NEWLINE; } 

:(\\\n)*:                                                                                               { Console.WriteLine("token: {0}", yytext); return (int)Token.COLON_COLON; }
:                                                                                                       { Console.WriteLine("token: {0}", yytext); return (int)Token.COLON; }
\.(\\\n)*\.(\\\n)*\.                                                                                    { Console.WriteLine("token: {0}", yytext); return (int)Token.ELLIPSE; }
\.(\\\n)*\*                                                                                             { Console.WriteLine("token: {0}", yytext); return (int)Token.DOT_STAR; }
\.                                                                                                      { Console.WriteLine("token: {0}", yytext); return (int)Token.PERIOD; }
-(\\\n)*>(\\\n)*\*                                                                                      { Console.WriteLine("token: {0}", yytext); return (int)Token.THIN_ARROW_STAR; }
-(\\\n)*>                                                                                               { Console.WriteLine("token: {0}", yytext); return (int)Token.THIN_ARROW; }
-(\\\n)*-                                                                                               { Console.WriteLine("token: {0}", yytext); return (int)Token.MINUS_MINUS; }
-(\\\n)*=                                                                                               { Console.WriteLine("token: {0}", yytext); return (int)Token.MINUS_EQUALS; }
-                                                                                                       { Console.WriteLine("token: {0}", yytext); return (int)Token.MINUS_SIGN; }
\+(\\\n)*\+                                                                                             { Console.WriteLine("token: {0}", yytext); return (int)Token.PLUS_PLUS; }
\+(\\\n)*=                                                                                              { Console.WriteLine("token: {0}", yytext); return (int)Token.PLUS_EQUALS; }
\+                                                                                                      { Console.WriteLine("token: {0}", yytext); return (int)Token.PLUS_SIGN; }
[<](\\\n)*[<](\\\n)*=                                                                                   { Console.WriteLine("token: {0}", yytext); return (int)Token.LESS_THEN_LESS_THEN_EQUALS; }
[<](\\\n)*[<]                                                                                           { Console.WriteLine("token: {0}", yytext); return (int)Token.LESS_THEN_LESS_THEN; }
[<](\\\n)*=(\\\n)*[>]                                                                                   { Console.WriteLine("token: {0}", yytext); return (int)Token.SAUCER; }
[<](\\\n)*=                                                                                             { Console.WriteLine("token: {0}", yytext); return (int)Token.LESS_THEN_OR_EQUAL_TO; }
[<]                                                                                                     { Console.WriteLine("token: {0}", yytext); return (int)Token.LESS_THEN; }
[>](\\\n)*[>](\\\n)*=                                                                                   { Console.WriteLine("token: {0}", yytext); return (int)Token.GREATER_THEN_GREATER_THEN_EQUALS; }
[>](\\\n)*[>]                                                                                           { Console.WriteLine("token: {0}", yytext); return (int)Token.GREATER_THEN_GRATER_THEN; }
[>](\\\n)*=                                                                                             { Console.WriteLine("token: {0}", yytext); return (int)Token.GREATER_THEN_OR_EQUAL_TO; }
[>]                                                                                                     { Console.WriteLine("token: {0}", yytext); return (int)Token.GREATER_THEN; }
=(\\\n)*=                                                                                               { Console.WriteLine("token: {0}", yytext); return (int)Token.EQUALS_EQUALS; }
=(\\\n)*>                                                                                               { Console.WriteLine("token: {0}", yytext); return (int)Token.EQUALS_SIGN_GREATER_THEN; }
=                                                                                                       { Console.WriteLine("token: {0}", yytext); return (int)Token.EQUAL_SIGN; }
\|(\\\n)*\|                                                                                             { Console.WriteLine("token: {0}", yytext); return (int)Token.PIPE_PIPE; }
\|(\\\n)*=                                                                                              { Console.WriteLine("token: {0}", yytext); return (int)Token.PIPE_EQUALS; }
\|                                                                                                      { Console.WriteLine("token: {0}", yytext); return (int)Token.PIPE; }
\?(\\\n)*\.                                                                                             { Console.WriteLine("token: {0}", yytext); return (int)Token.QUESTION_MARK_PERIOD; }
\?(\\\n)*\[                                                                                             { Console.WriteLine("token: {0}", yytext); return (int)Token.QUESTION_MARK_L_SQ_BRACKET; }
\?(\\\n)*\?                                                                                             { Console.WriteLine("token: {0}", yytext); return (int)Token.QUESTION_MARK_QUESTION_MARK; }
\?                                                                                                      { Console.WriteLine("token: {0}", yytext); return (int)Token.QUESTION_MARK; }
!(\\\n)*=                                                                                               { Console.WriteLine("token: {0}", yytext); return (int)Token.NOT_EQUALS; }
!                                                                                                       { Console.WriteLine("token: {0}", yytext); return (int)Token.EXCLAIMATION_MARK; }
&(\\\n)*=                                                                                               { Console.WriteLine("token: {0}", yytext); return (int)Token.AMPERSTAND_EQUALS; }
&(\\\n)*&                                                                                               { Console.WriteLine("token: {0}", yytext); return (int)Token.AMPERSTAND_AMPERSTAND; }
&                                                                                                       { Console.WriteLine("token: {0}", yytext); return (int)Token.AMPERSTAND; }
\*(\\\n)*=                                                                                              { Console.WriteLine("token: {0}", yytext); return (int)Token.ASTERISK_EQUALS; }
\*                                                                                                      { Console.WriteLine("token: {0}", yytext); return (int)Token.ASTERISK; }
/(\\\n)*=                                                                                               { Console.WriteLine("token: {0}", yytext); return (int)Token.FORWARD_SLASH_EQUALS; }
/                                                                                                       { Console.WriteLine("token: {0}", yytext); return (int)Token.FORWARD_SLASH; }
\^(\\\n)*=                                                                                              { Console.WriteLine("token: {0}", yytext); return (int)Token.CARROT_EQUALS; }
\^                                                                                                      { Console.WriteLine("token: {0}", yytext); return (int)Token.CARROT; }
%(\\\n)*=                                                                                               { Console.WriteLine("token: {0}", yytext); return (int)Token.PERCENT_EQUALS; }
%                                                                                                       { Console.WriteLine("token: {0}", yytext); return (int)Token.PERCENT; }
#(\\\n)*#                                                                                               { Console.WriteLine("token: {0}", yytext); return (int)Token.HASH_HASH; }
#                                                                                                       { Console.WriteLine("token: {0}", yytext); return (int)Token.HASH; }
\[                                                                                                      { Console.WriteLine("token: {0}", yytext); return (int)Token.L_SQ_BRACKET; }
\]                                                                                                      { Console.WriteLine("token: {0}", yytext); return (int)Token.R_SQ_BRACKET; }
\(                                                                                                      { Console.WriteLine("token: {0}", yytext); return (int)Token.L_PAREN; }
\)                                                                                                      { Console.WriteLine("token: {0}", yytext); return (int)Token.R_PAREN; }
\{                                                                                                      { Console.WriteLine("token: {0}", yytext); return (int)Token.L_CURLY_BRACE; }
\}                                                                                                      { Console.WriteLine("token: {0}", yytext); return (int)Token.R_CURLY_BRACE; }
~                                                                                                       { Console.WriteLine("token: {0}", yytext); return (int)Token.TILDE; }
,                                                                                                       { Console.WriteLine("token: {0}", yytext); return (int)Token.COMMA; }
;                                                                                                       { Console.WriteLine("token: {0}", yytext); return (int)Token.SEMI_COLON; }
@                                                                                                       { Console.WriteLine("token: {0}", yytext); return (int)Token.AT; }
`                                                                                                       { Console.WriteLine("token: {0}", yytext); return (int)Token.GRAVE; }

{decimalLit}((\\\n)*{numberSuffix})?                                                                    { Console.WriteLine("token: {0}", yytext); GetNumber(); return (int)Token.NUMBER; }
{octLit}((\\\n)*{numberSuffix})?                                                                        { Console.WriteLine("token: {0}", yytext); GetNumber(); return (int)Token.NUMBER; }
{hexLit}((\\\n)*{hexNumberSuffix})?                                                                     { Console.WriteLine("token: {0}", yytext); GetNumber(); return (int)Token.NUMBER; }
{binLit}((\\\n)*{numberSuffix})?                                                                        { Console.WriteLine("token: {0}", yytext); GetNumber(); return (int)Token.NUMBER; }

{mandDigSepSeq}(\\\n)*{floatExp}((\\\n)*{floatSufix})?                                                  { Console.WriteLine("token: {0}", yytext); GetNumber(); return (int)Token.NUMBER; }
{mandDigSepSeq}(\\\n)*\.((\\\n)*{floatExp})?((\\\n)*{floatSufix})?                                      { Console.WriteLine("token: {0}", yytext); GetNumber(); return (int)Token.NUMBER; }
({mandDigSepSeq}(\\\n)*)?\.(\\\n)*{mandDigSepSeq}((\\\n)*{floatExp})?((\\\n)*{floatSufix})?             { Console.WriteLine("token: {0}", yytext); GetNumber(); return (int)Token.NUMBER; }

0(\\\n)*[xX](\\\n)*{mandHexSeq}(\\\n)*{floatExp}((\\\n)*{floatSufix})?                                  { Console.WriteLine("token: {0}", yytext); GetNumber(); return (int)Token.NUMBER; }
0(\\\n)*[xX](\\\n)*{mandHexSeq}(\\\n)*\.(\\\n)*{floatExp}((\\\n)*{floatSufix})?                         { Console.WriteLine("token: {0}", yytext); GetNumber(); return (int)Token.NUMBER; }
0(\\\n)*[xX](\\\n)*({mandHexSeq}(\\\n)*)?\.(\\\n)*{mandHexSeq}(\\\n)*{floatExp}((\\\n)*{floatSufix})?   { Console.WriteLine("token: {0}", yytext); GetNumber(); return (int)Token.NUMBER; }

/(\\\n)*/                                                                                               { BEGIN(CPP_COMMENT); curTokVal.Clear(); curTokVal.Append("//"); }
/(\\\n)*\*                                                                                              { BEGIN(C_COMMENT); curTokVal.Clear(); curTokVal.Append("/*"); }

<CPP_COMMENT>[^\\\n]+                                                                                   { curTokVal.Append(yytext); }
<CPP_COMMENT>\\{Space}*\\n                                                                              { curTokVal.Append(yytext); }
<CPP_COMMENT>\\{Space}*[^\\n]                                                                           { curTokVal.Append(yytext); }
<CPP_COMMENT>\\{Space}*<<EOF>>                                                                          { throw new InvalidDataException("unterminated //"); }
<CPP_COMMENT>\n                                                                                         { curTokVal.Append(yytext); Console.WriteLine("token: {0}", curTokVal.ToString()); GetComment(); BEGIN(INITIAL); return (int)Token.COMMENT; }

<C_COMMENT>[^*/]+                                                                                       { curTokVal.Append(yytext); }
<C_COMMENT>\*[^/]                                                                                       { curTokVal.Append(yytext); }
<C_COMMENT>\*[/]                                                                                        { curTokVal.Append(yytext); Console.WriteLine("token: {0}", curTokVal.ToString()); GetComment(); BEGIN(INITIAL); return (int)Token.COMMENT; }
<C_COMMENT>/                                                                                            { curTokVal.Append(yytext); }
<C_COMMENT><<EOF>>                                                                                      { throw new InvalidDataException("unterminated /*"); }

%%