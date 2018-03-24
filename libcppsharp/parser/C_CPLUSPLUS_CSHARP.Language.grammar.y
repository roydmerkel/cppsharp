/*

  C_CPLUSPLUS_CSHARP.Language.grammar.y

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
%partial
%parsertype C_CPLUSPLUS_CSHARPParser
%visibility internal
%tokentype Token

%union { 
			public int n; 
			public string s; 
	   }

%start main

%token WHITESPACE
%token NEWLINE
%token COLON_COLON
%token HASH
%token QUESTION_MARK
%token EQUAL_SIGN
%token LESS_THEN
%token GREATER_THEN
%token COLON
%token SEMI_COLON
%token PERCENT
%token COMMA
%token PERIOD
%token TILDE
%token PIPE
%token PLUS_SIGN
%token MINUS_SIGN
%token ASTERISK
%token AMPERSTAND
%token CARROT
%token EXCLAIMATION_MARK
%token GRAVE
%token AT
%token BACK_SLASH
%token FORWARD_SLASH
%token L_CURLY_BRACE
%token R_CURLY_BRACE
%token L_SQ_BRACKET
%token R_SQ_BRACKET
%token L_PAREN
%token R_PAREN
%token THIN_ARROW
%token PLUS_PLUS
%token MINUS_MINUS
%token DOT_STAR
%token THIN_ARROW_STAR
%token SAUCER
%token LESS_THEN_LESS_THEN
%token GREATER_THEN_GRATER_THEN
%token LESS_THEN_OR_EQUAL_TO
%token GREATER_THEN_OR_EQUAL_TO
%token EQUALS_EQUALS
%token NOT_EQUALS
%token AMPERSTAND_AMPERSTAND
%token PIPE_PIPE
%token ASTERISK_EQUALS
%token FORWARD_SLASH_EQUALS
%token PERCENT_EQUALS
%token PLUS_EQUALS
%token MINUS_EQUALS
%token LESS_THEN_LESS_THEN_EQUALS
%token GREATER_THEN_GREATER_THEN_EQUALS
%token AMPERSTAND_EQUALS
%token PIPE_EQUALS
%token CARROT_EQUALS
%token QUESTION_MARK_PERIOD
%token QUESTION_MARK_L_SQ_BRACKET
%token QUESTION_MARK_QUESTION_MARK
%token HASH_HASH
%token EQUALS_SIGN_GREATER_THEN
%token ELLIPSE
%token SIZEOF
%token DEFINED
%token IDENTIFIER
%token STRING
%token CHAR
%token NUMBER
%token COMMENT
%token TOKEN_PASTE
%token STRINGIFY
%token EOF

%{
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

%}

%%

main   : number
       ;

number : 
       | NUMBER							{ Console.WriteLine("Rule -> number: {0}", $1.n); }
       ;

%%