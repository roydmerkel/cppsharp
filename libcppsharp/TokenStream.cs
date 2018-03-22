//
//  TokenStream.cs
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
using System.Text.RegularExpressions;

namespace libcppsharp
{
    public class TokenStream
    {
        private delegate void HandleRegex(out Token[] tokOut, ref State state, string matchString, Match match, ref int curColumn, ref int curLine);

        private enum State
        {
            DEFAULT,
            CPP_COMMENT,
            C_COMMENT,
            RAW_STRING,
            STRING,
            CHAR,
        }

        private struct TokenMatch
        {
            public Regex regex;
            public HandleRegex handler;

            public TokenMatch(Regex regex, HandleRegex handler)
            {
                this.regex = regex;
                this.handler = handler;
            }
        }

        bool ignoreStrayBackslash;
        bool allowSlashNInString;
        bool treatStringSlashNAsNothing;
        bool digraphs;
        bool trigraphs;
        private StringBuilder curTokVal;
        private string rawStringPrefix;

        private Dictionary<State, TokenMatch[]> tokenRegexs;

        private const string identifierFirstChar = @"(?:[_a-zA-Z\u00A8\u00AA\u00AD\u00AF\u00B2-\u00B5\u00B7-\u00BA\u00BC-\u00BE\u00C0-\u00D6\u00D8-\u00F6\u00F8-\u00FF\u0100-\u02FF\u0370-\u167F\u1681-\u180D\u180F-\u1DBF\u1E00-\u1FFF\u200B-\u200D\u202A-\u202E\u203F-\u2040\u2054\u2060-\u206F\u2070-\u20CF\u2100-\u218F\u2460-\u24FF\u2776-\u2793\u2C00-\u2DFF\u2E80-\u2FFF\u3004-\u3007\u3021-\u302F\u3031-\u303F\u3040-\uD7FF\uF900-\uFD3D\uFD40-\uFDCF\uFDF0-\uFE1F\uFE30-\uFE44\uFE47-\uFFFD]|(?:[\uD800-\uDB7F][\uDC00-\uDFFD])|(?:\\\n))";
        private const string identifierSecondChar = @"(?:[_0-9a-zA-Z\u00A8\u00AA\u00AD\u00AF\u00B2-\u00B5\u00B7-\u00BA\u00BC-\u00BE\u00C0-\u00D6\u00D8-\u00F6\u00F8-\u00FF\u0100-\u02FF\u0370-\u167F\u1681-\u180D\u180F-\u1DBF\u1E00-\u1FFF\u200B-\u200D\u202A-\u202E\u203F-\u2040\u2054\u2060-\u206F\u2070-\u20CF\u2100-\u218F\u2460-\u24FF\u2776-\u2793\u2C00-\u2DFF\u2E80-\u2FFF\u3004-\u3007\u3021-\u302F\u3031-\u303F\u3040-\uD7FF\uF900-\uFD3D\uFD40-\uFDCF\uFDF0-\uFE1F\uFE30-\uFE44\uFE47-\uFFFD\u0300-\u036F\u1DC0-\u1DFF\u20D0-\u20FF\uFE20-\uFE2F]|(?:[\uD800-\uDB7F][\uDC00-\uDFFD])|(?:\\\n))";
        private const string identifierRegex = identifierFirstChar + identifierSecondChar + @"*";
        private const string numberSufix = @"(?:(?:[uUlL]+)|(?:_" + identifierRegex + @"))";
        private const string floatSufix = @"(?:(?:[lLfF]+)|(?:_" + identifierRegex + @"))";
        private const string stringSuffix = @"(?:(?:s)|(?:_" + identifierRegex + @"))";
        private const string digit = @"(?:[0-9]|\\\n)";
        private const string digits = digit + @"*";
        private const string hexDigit = @"(?:[0-9a-fA-F]|\\\n)";
        private const string hexDigits = hexDigit + @"*";
        private const string escNewlines = @"(?:\\\n)*";
        private const string exponent = @"(?:" + escNewlines + @"[eEpP]" + escNewlines + @"[+-]?" + escNewlines + @"[0-9]" + digit + @"*)";
        private const string optexponent = exponent + "?";

        public TokenStream(bool handleTrigraphs = false, bool handleDigraphs = false, bool allowSlashNInString = true, bool treatStringSlashNAsNothing = true)
        {
            ignoreStrayBackslash = false;
            this.allowSlashNInString = allowSlashNInString;
            this.treatStringSlashNAsNothing = treatStringSlashNAsNothing;

            digraphs = handleDigraphs;
            trigraphs = handleTrigraphs;
            curTokVal = new StringBuilder();

            List<TokenMatch> defaultStateMatches = new List<TokenMatch>()
            {
                new TokenMatch(new Regex(@"[ \t\v\f]+", RegexOptions.Compiled), delegate(out Token[] tokOut, ref State state, string matchString, Match match, ref int curColumn, ref int curLine) { tokOut = new Token[] { new Token(TokenType.WHITESPACE, matchString, null) }; curColumn += matchString.Length; } ),
                new TokenMatch(new Regex(@"\\[ \t\v\f]+(?!\n)", RegexOptions.Compiled), delegate(out Token[] tokOut, ref State state, string matchString, Match match, ref int curColumn, ref int curLine) { if(!ignoreStrayBackslash) { throw new InvalidDataException("Unexpected \"" + matchString + "\" at column " + (curColumn + 1) + " line " + (curLine + 1)); } tokOut = new Token[] { new Token(TokenType.WHITESPACE, matchString.Substring(1), null), new Token(TokenType.BACK_SLASH, "\\", null) }; curColumn += matchString.Length; } ),
                new TokenMatch(new Regex(@"\\[ \t\v\f]+\n", RegexOptions.Compiled), delegate(out Token[] tokOut, ref State state, string matchString, Match match, ref int curColumn, ref int curLine) { tokOut = new Token[] { new Token(TokenType.WHITESPACE, matchString, null) }; curColumn += matchString.Length; } ),
                new TokenMatch(new Regex(@":(?:\\\n)*:", RegexOptions.Compiled), delegate(out Token[] tokOut, ref State state, string matchString, Match match, ref int curColumn, ref int curLine) { tokOut = new Token[] { new Token(TokenType.COLON_COLON, "::", null) }; curColumn += matchString.Length; } ),
                new TokenMatch(new Regex(@"\.", RegexOptions.Compiled), delegate(out Token[] tokOut, ref State state, string matchString, Match match, ref int curColumn, ref int curLine) { tokOut = new Token[] { new Token(TokenType.PERIOD, ".", null) }; curColumn += matchString.Length; }),
                new TokenMatch(new Regex(@"-(?:\\\n)*>", RegexOptions.Compiled), delegate(out Token[] tokOut, ref State state, string matchString, Match match, ref int curColumn, ref int curLine) { tokOut = new Token[] { new Token(TokenType.THIN_ARROW, "->", null) }; curColumn += matchString.Length; }),
                new TokenMatch(new Regex(@"\[", RegexOptions.Compiled), delegate(out Token[] tokOut, ref State state, string matchString, Match match, ref int curColumn, ref int curLine) { tokOut = new Token[] { new Token(TokenType.L_SQ_BRACKET, "[", null) }; curColumn += matchString.Length; }),
                new TokenMatch(new Regex(@"\]", RegexOptions.Compiled), delegate(out Token[] tokOut, ref State state, string matchString, Match match, ref int curColumn, ref int curLine) { tokOut = new Token[] { new Token(TokenType.R_SQ_BRACKET, "]", null) }; curColumn += matchString.Length; }),
                new TokenMatch(new Regex(@"\(", RegexOptions.Compiled), delegate(out Token[] tokOut, ref State state, string matchString, Match match, ref int curColumn, ref int curLine) { tokOut = new Token[] { new Token(TokenType.L_PAREN, "(", null) }; curColumn += matchString.Length; }),
                new TokenMatch(new Regex(@"\)", RegexOptions.Compiled), delegate(out Token[] tokOut, ref State state, string matchString, Match match, ref int curColumn, ref int curLine) { tokOut = new Token[] { new Token(TokenType.R_PAREN, ")", null) }; curColumn += matchString.Length; }),
                new TokenMatch(new Regex(@"\{", RegexOptions.Compiled), delegate(out Token[] tokOut, ref State state, string matchString, Match match, ref int curColumn, ref int curLine) { tokOut = new Token[] { new Token(TokenType.L_CURLY_BRACE, "{", null) }; curColumn += matchString.Length; }),
                new TokenMatch(new Regex(@"\}", RegexOptions.Compiled), delegate(out Token[] tokOut, ref State state, string matchString, Match match, ref int curColumn, ref int curLine) { tokOut = new Token[] { new Token(TokenType.R_CURLY_BRACE, "}", null) }; curColumn += matchString.Length; }),
                new TokenMatch(new Regex(@"\+(?:\\\n)*\+", RegexOptions.Compiled), delegate(out Token[] tokOut, ref State state, string matchString, Match match, ref int curColumn, ref int curLine) { tokOut = new Token[] { new Token(TokenType.PLUS_PLUS, "++", null) }; curColumn += matchString.Length; }),
                new TokenMatch(new Regex(@"-(?:\\\n)*-", RegexOptions.Compiled), delegate(out Token[] tokOut, ref State state, string matchString, Match match, ref int curColumn, ref int curLine) { tokOut = new Token[] { new Token(TokenType.MINUS_MINUS, "--", null) }; curColumn += matchString.Length; }),
                new TokenMatch(new Regex(@"~", RegexOptions.Compiled), delegate(out Token[] tokOut, ref State state, string matchString, Match match, ref int curColumn, ref int curLine) { tokOut = new Token[] { new Token(TokenType.TILDE, "~", null) }; curColumn += matchString.Length; }),
                new TokenMatch(new Regex(@"!", RegexOptions.Compiled), delegate(out Token[] tokOut, ref State state, string matchString, Match match, ref int curColumn, ref int curLine) { tokOut = new Token[] { new Token(TokenType.EXCLAIMATION_MARK, "!", null) }; curColumn += matchString.Length; }),
                new TokenMatch(new Regex(@"&", RegexOptions.Compiled), delegate(out Token[] tokOut, ref State state, string matchString, Match match, ref int curColumn, ref int curLine) { tokOut = new Token[] { new Token(TokenType.AMPERSTAND, "&", null) }; curColumn += matchString.Length; }),
                new TokenMatch(new Regex(@"\.(?:\\\n)*\*", RegexOptions.Compiled), delegate(out Token[] tokOut, ref State state, string matchString, Match match, ref int curColumn, ref int curLine) { tokOut = new Token[] { new Token(TokenType.DOT_STAR, ".*", null) }; curColumn += matchString.Length; }),
                new TokenMatch(new Regex(@"-(?:\\\n)*>(?:\\\n)*\*", RegexOptions.Compiled), delegate(out Token[] tokOut, ref State state, string matchString, Match match, ref int curColumn, ref int curLine) { tokOut = new Token[] { new Token(TokenType.THIN_ARROW_STAR, "->*", null) }; curColumn += matchString.Length; }),
                new TokenMatch(new Regex(@"\*", RegexOptions.Compiled), delegate(out Token[] tokOut, ref State state, string matchString, Match match, ref int curColumn, ref int curLine) { tokOut = new Token[] { new Token(TokenType.ASTERISK, "*", null) }; curColumn += matchString.Length; }),
                new TokenMatch(new Regex(@"/", RegexOptions.Compiled), delegate(out Token[] tokOut, ref State state, string matchString, Match match, ref int curColumn, ref int curLine) { tokOut = new Token[] { new Token(TokenType.FORWARD_SLASH, "/", null) }; curColumn += matchString.Length; }),
                new TokenMatch(new Regex(@"%", RegexOptions.Compiled), delegate(out Token[] tokOut, ref State state, string matchString, Match match, ref int curColumn, ref int curLine) { tokOut = new Token[] { new Token(TokenType.PERCENT, "%", null) }; curColumn += matchString.Length; }),
                new TokenMatch(new Regex(@"-", RegexOptions.Compiled), delegate(out Token[] tokOut, ref State state, string matchString, Match match, ref int curColumn, ref int curLine) { tokOut = new Token[] { new Token(TokenType.MINUS_SIGN, "-", null) }; curColumn += matchString.Length; }),
                new TokenMatch(new Regex(@"\+", RegexOptions.Compiled), delegate(out Token[] tokOut, ref State state, string matchString, Match match, ref int curColumn, ref int curLine) { tokOut = new Token[] { new Token(TokenType.PLUS_SIGN, "+", null) }; curColumn += matchString.Length; }),
                new TokenMatch(new Regex(@"<(?:\\\n)*=(?:\\\n)*>", RegexOptions.Compiled), delegate(out Token[] tokOut, ref State state, string matchString, Match match, ref int curColumn, ref int curLine) { tokOut = new Token[] { new Token(TokenType.SAUCER, "<=>", null) }; curColumn += matchString.Length; }),
                new TokenMatch(new Regex(@"<(?:\\\n)*<", RegexOptions.Compiled), delegate(out Token[] tokOut, ref State state, string matchString, Match match, ref int curColumn, ref int curLine) { tokOut = new Token[] { new Token(TokenType.LESS_THEN_LESS_THEN, "<<", null) }; curColumn += matchString.Length; }),
                new TokenMatch(new Regex(@">(?:\\\n)*>", RegexOptions.Compiled), delegate(out Token[] tokOut, ref State state, string matchString, Match match, ref int curColumn, ref int curLine) { tokOut = new Token[] { new Token(TokenType.GREATER_THEN_GRATER_THEN, ">>", null) }; curColumn += matchString.Length; }),
                new TokenMatch(new Regex(@"<", RegexOptions.Compiled), delegate(out Token[] tokOut, ref State state, string matchString, Match match, ref int curColumn, ref int curLine) { tokOut = new Token[] { new Token(TokenType.LESS_THEN, "<", null) }; curColumn += matchString.Length; }),
                new TokenMatch(new Regex(@">", RegexOptions.Compiled), delegate(out Token[] tokOut, ref State state, string matchString, Match match, ref int curColumn, ref int curLine) { tokOut = new Token[] { new Token(TokenType.GREATER_THEN, ">", null) }; curColumn += matchString.Length; }),
                new TokenMatch(new Regex(@"<(?:\\\n)*=", RegexOptions.Compiled), delegate(out Token[] tokOut, ref State state, string matchString, Match match, ref int curColumn, ref int curLine) { tokOut = new Token[] { new Token(TokenType.LESS_THEN_OR_EQUAL_TO, "<=", null) }; curColumn += matchString.Length; }),
                new TokenMatch(new Regex(@">(?:\\\n)*=", RegexOptions.Compiled), delegate(out Token[] tokOut, ref State state, string matchString, Match match, ref int curColumn, ref int curLine) { tokOut = new Token[] { new Token(TokenType.GREATER_THEN_OR_EQUAL_TO, ">=", null) }; curColumn += matchString.Length; }),
                new TokenMatch(new Regex(@"=(?:\\\n)*=", RegexOptions.Compiled), delegate(out Token[] tokOut, ref State state, string matchString, Match match, ref int curColumn, ref int curLine) { tokOut = new Token[] { new Token(TokenType.EQUALS_EQUALS, "==", null) }; curColumn += matchString.Length; }),
                new TokenMatch(new Regex(@"!(?:\\\n)*=", RegexOptions.Compiled), delegate(out Token[] tokOut, ref State state, string matchString, Match match, ref int curColumn, ref int curLine) { tokOut = new Token[] { new Token(TokenType.NOT_EQUALS, "!=", null) }; curColumn += matchString.Length; }),
                new TokenMatch(new Regex(@"\^", RegexOptions.Compiled), delegate(out Token[] tokOut, ref State state, string matchString, Match match, ref int curColumn, ref int curLine) { tokOut = new Token[] { new Token(TokenType.CARROT, "^", null) }; curColumn += matchString.Length; }),
                new TokenMatch(new Regex(@"\|", RegexOptions.Compiled), delegate(out Token[] tokOut, ref State state, string matchString, Match match, ref int curColumn, ref int curLine) { tokOut = new Token[] { new Token(TokenType.PIPE, "|", null) }; curColumn += matchString.Length; }),
                new TokenMatch(new Regex(@"&(?:\\\n)*&", RegexOptions.Compiled), delegate(out Token[] tokOut, ref State state, string matchString, Match match, ref int curColumn, ref int curLine) { tokOut = new Token[] { new Token(TokenType.AMPERSTAND_AMPERSTAND, "&&", null) }; curColumn += matchString.Length; }),
                new TokenMatch(new Regex(@"\|(?:\\\n)*\|", RegexOptions.Compiled), delegate(out Token[] tokOut, ref State state, string matchString, Match match, ref int curColumn, ref int curLine) { tokOut = new Token[] { new Token(TokenType.PIPE_PIPE, "||", null) }; curColumn += matchString.Length; }),
                new TokenMatch(new Regex(@"\?", RegexOptions.Compiled), delegate(out Token[] tokOut, ref State state, string matchString, Match match, ref int curColumn, ref int curLine) { tokOut = new Token[] { new Token(TokenType.QUESTION_MARK, "?", null) }; curColumn += matchString.Length; }),
                new TokenMatch(new Regex(@":", RegexOptions.Compiled), delegate(out Token[] tokOut, ref State state, string matchString, Match match, ref int curColumn, ref int curLine) { tokOut = new Token[] { new Token(TokenType.COLON, ":", null) }; curColumn += matchString.Length; }),
                new TokenMatch(new Regex(@"=", RegexOptions.Compiled), delegate(out Token[] tokOut, ref State state, string matchString, Match match, ref int curColumn, ref int curLine) { tokOut = new Token[] { new Token(TokenType.EQUAL_SIGN, "=", null) }; curColumn += matchString.Length; }),
                new TokenMatch(new Regex(@"\*(?:\\\n)*=", RegexOptions.Compiled), delegate(out Token[] tokOut, ref State state, string matchString, Match match, ref int curColumn, ref int curLine) { tokOut = new Token[] { new Token(TokenType.ASTERISK_EQUALS, "*=", null) }; curColumn += matchString.Length; }),
                new TokenMatch(new Regex(@"/(?:\\\n)*=", RegexOptions.Compiled), delegate(out Token[] tokOut, ref State state, string matchString, Match match, ref int curColumn, ref int curLine) { tokOut = new Token[] { new Token(TokenType.FORWARD_SLASH_EQUALS, "/=", null) }; curColumn += matchString.Length; }),
                new TokenMatch(new Regex(@"%(?:\\\n)*=", RegexOptions.Compiled), delegate(out Token[] tokOut, ref State state, string matchString, Match match, ref int curColumn, ref int curLine) { tokOut = new Token[] { new Token(TokenType.PERCENT_EQUALS, "%=", null) }; curColumn += matchString.Length; }),
                new TokenMatch(new Regex(@"\+(?:\\\n)*=", RegexOptions.Compiled), delegate(out Token[] tokOut, ref State state, string matchString, Match match, ref int curColumn, ref int curLine) { tokOut = new Token[] { new Token(TokenType.PLUS_EQUALS, "+=", null) }; curColumn += matchString.Length; }),
                new TokenMatch(new Regex(@"-(?:\\\n)*=", RegexOptions.Compiled), delegate(out Token[] tokOut, ref State state, string matchString, Match match, ref int curColumn, ref int curLine) { tokOut = new Token[] { new Token(TokenType.MINUS_EQUALS, "-=", null) }; curColumn += matchString.Length; }),
                new TokenMatch(new Regex(@"<(?:\\\n)*<(?:\\\n)*=", RegexOptions.Compiled), delegate(out Token[] tokOut, ref State state, string matchString, Match match, ref int curColumn, ref int curLine) { tokOut = new Token[] { new Token(TokenType.LESS_THEN_LESS_THEN_EQUALS, "<<=", null) }; curColumn += matchString.Length; }),
                new TokenMatch(new Regex(@">(?:\\\n)*>(?:\\\n)*=", RegexOptions.Compiled), delegate(out Token[] tokOut, ref State state, string matchString, Match match, ref int curColumn, ref int curLine) { tokOut = new Token[] { new Token(TokenType.GREATER_THEN_GREATER_THEN_EQUALS, ">>=", null) }; curColumn += matchString.Length; }),
                new TokenMatch(new Regex(@"&(?:\\\n)*=", RegexOptions.Compiled), delegate(out Token[] tokOut, ref State state, string matchString, Match match, ref int curColumn, ref int curLine) { tokOut = new Token[] { new Token(TokenType.AMPERSTAND_EQUALS, "&=", null) }; curColumn += matchString.Length; }),
                new TokenMatch(new Regex(@"\|(?:\\\n)*=", RegexOptions.Compiled), delegate(out Token[] tokOut, ref State state, string matchString, Match match, ref int curColumn, ref int curLine) { tokOut = new Token[] { new Token(TokenType.PIPE_EQUALS, "|=", null) }; curColumn += matchString.Length; }),
                new TokenMatch(new Regex(@"\^(?:\\\n)*=", RegexOptions.Compiled), delegate(out Token[] tokOut, ref State state, string matchString, Match match, ref int curColumn, ref int curLine) { tokOut = new Token[] { new Token(TokenType.CARROT_EQUALS, "^=", null) }; curColumn += matchString.Length; }),
                new TokenMatch(new Regex(@",", RegexOptions.Compiled), delegate(out Token[] tokOut, ref State state, string matchString, Match match, ref int curColumn, ref int curLine) { tokOut = new Token[] { new Token(TokenType.COMMA, ",", null) }; curColumn += matchString.Length; }),
                new TokenMatch(new Regex(@"#", RegexOptions.Compiled), delegate(out Token[] tokOut, ref State state, string matchString, Match match, ref int curColumn, ref int curLine) { tokOut = new Token[] { new Token(TokenType.HASH, "#", null) }; curColumn += matchString.Length; }),
                new TokenMatch(new Regex(@";", RegexOptions.Compiled), delegate(out Token[] tokOut, ref State state, string matchString, Match match, ref int curColumn, ref int curLine) { tokOut = new Token[] { new Token(TokenType.SEMI_COLON, ";", null) }; curColumn += matchString.Length; }),
                new TokenMatch(new Regex(@"@", RegexOptions.Compiled), delegate(out Token[] tokOut, ref State state, string matchString, Match match, ref int curColumn, ref int curLine) { tokOut = new Token[] { new Token(TokenType.AT, "@", null) }; curColumn += matchString.Length; }),
                new TokenMatch(new Regex(@"`", RegexOptions.Compiled), delegate(out Token[] tokOut, ref State state, string matchString, Match match, ref int curColumn, ref int curLine) { tokOut = new Token[] { new Token(TokenType.GRAVE, "`", null) }; curColumn += matchString.Length; }),
                new TokenMatch(new Regex(@"#(?:\\\n)*#", RegexOptions.Compiled), delegate(out Token[] tokOut, ref State state, string matchString, Match match, ref int curColumn, ref int curLine) { tokOut = new Token[] { new Token(TokenType.HASH_HASH, "##", null) }; curColumn += matchString.Length; }),
                new TokenMatch(new Regex(@"\?(?:\\\n)*\.", RegexOptions.Compiled), delegate(out Token[] tokOut, ref State state, string matchString, Match match, ref int curColumn, ref int curLine) { tokOut = new Token[] { new Token(TokenType.QUESTION_MARK_PERIOD, "?.", null) }; curColumn += matchString.Length; }),
                new TokenMatch(new Regex(@"\?(?:\\\n)*\[", RegexOptions.Compiled), delegate(out Token[] tokOut, ref State state, string matchString, Match match, ref int curColumn, ref int curLine) { tokOut = new Token[] { new Token(TokenType.QUESTION_MARK_L_SQ_BRACKET, "?[", null) }; curColumn += matchString.Length; }),
                new TokenMatch(new Regex(@"\?(?:\\\n)*\?", RegexOptions.Compiled), delegate(out Token[] tokOut, ref State state, string matchString, Match match, ref int curColumn, ref int curLine) { tokOut = new Token[] { new Token(TokenType.QUESTION_MARK_QUESTION_MARK, "??", null) }; curColumn += matchString.Length; }),
                new TokenMatch(new Regex(@"=(?:\\\n)*>", RegexOptions.Compiled), delegate(out Token[] tokOut, ref State state, string matchString, Match match, ref int curColumn, ref int curLine) { tokOut = new Token[] { new Token(TokenType.EQUALS_SIGN_GREATER_THEN, "=>", null) }; curColumn += matchString.Length; }),
                new TokenMatch(new Regex(@"\\(?!\n)", RegexOptions.Compiled), delegate(out Token[] tokOut, ref State state, string matchString, Match match, ref int curColumn, ref int curLine) { if(!ignoreStrayBackslash) { throw new InvalidDataException("Unexpected \"" + matchString + "\" at column " + (curColumn + 1) + " line " + (curLine + 1)); } tokOut = new Token[] { new Token(TokenType.BACK_SLASH, matchString, null) }; curColumn += matchString.Length; }),
                new TokenMatch(new Regex(@"\\\n", RegexOptions.Compiled), delegate(out Token[] tokOut, ref State state, string matchString, Match match, ref int curColumn, ref int curLine) { tokOut = null; curColumn = 0; curLine++; }),
                new TokenMatch(new Regex(@"\n", RegexOptions.Compiled), delegate(out Token[] tokOut, ref State state, string matchString, Match match, ref int curColumn, ref int curLine) { tokOut = new Token[] { new Token(TokenType.NEWLINE, "\n", null)}; curColumn = 0; curLine++; }),

                // identifiers
                new TokenMatch(new Regex(identifierRegex, RegexOptions.Compiled), delegate(out Token[] tokOut, ref State state, string matchString, Match match, ref int curColumn, ref int curLine) { tokOut = new Token[] { new Token(TokenType.IDENTIFIER, matchString.Replace("\\\n", ""), null) }; curColumn += matchString.Length; }),

                // numbers.
                new TokenMatch(new Regex(@"[1-9](?:[0-9']|\\\n)*" + numberSufix + @"?", RegexOptions.Compiled), delegate(out Token[] tokOut, ref State state, string matchString, Match match, ref int curColumn, ref int curLine) { tokOut = new Token[] { new Token(TokenType.NUMBER, matchString.Replace("\\\n", ""), null) }; curColumn += matchString.Length; }),
                new TokenMatch(new Regex(@"0(?:[0-7]|\\\n)*" + numberSufix + @"?", RegexOptions.Compiled), delegate(out Token[] tokOut, ref State state, string matchString, Match match, ref int curColumn, ref int curLine) { tokOut = new Token[] { new Token(TokenType.NUMBER, matchString.Replace("\\\n", ""), null) }; curColumn += matchString.Length; }),
                new TokenMatch(new Regex(@"0[xX](?:[0-9a-fA-F]|\\\n)+" + numberSufix + @"?", RegexOptions.Compiled), delegate(out Token[] tokOut, ref State state, string matchString, Match match, ref int curColumn, ref int curLine) { tokOut = new Token[] { new Token(TokenType.NUMBER, matchString.Replace("\\\n", ""), null) }; curColumn += matchString.Length; }),
                new TokenMatch(new Regex(@"0[bB](?:[0-1]|\\\n)+" + numberSufix + @"?", RegexOptions.Compiled), delegate(out Token[] tokOut, ref State state, string matchString, Match match, ref int curColumn, ref int curLine) { tokOut = new Token[] { new Token(TokenType.NUMBER, matchString.Replace("\\\n", ""), null) }; curColumn += matchString.Length; }),

                new TokenMatch(new Regex(@"[0-9]" + digits + exponent + floatSufix + @"?", RegexOptions.Compiled), delegate(out Token[] tokOut, ref State state, string matchString, Match match, ref int curColumn, ref int curLine) { tokOut = new Token[] { new Token(TokenType.NUMBER, matchString.Replace("\\\n", ""), null) }; curColumn += matchString.Length; }),
                new TokenMatch(new Regex(@"[0-9]" + digits + @"\." + digits + optexponent + floatSufix + @"?", RegexOptions.Compiled), delegate(out Token[] tokOut, ref State state, string matchString, Match match, ref int curColumn, ref int curLine) { tokOut = new Token[] { new Token(TokenType.NUMBER, matchString.Replace("\\\n", ""), null) }; curColumn += matchString.Length; }),
                new TokenMatch(new Regex(digits + @"\." + escNewlines + @"[0-9]" + digits + optexponent + floatSufix + @"?", RegexOptions.Compiled), delegate(out Token[] tokOut, ref State state, string matchString, Match match, ref int curColumn, ref int curLine) { tokOut = new Token[] { new Token(TokenType.NUMBER, matchString.Replace("\\\n", ""), null) }; curColumn += matchString.Length; }),
                new TokenMatch(new Regex(@"0" + escNewlines + @"[xX]" + escNewlines + @"[0-9a-fA-F]" + hexDigits + @"(?:" + escNewlines + @"\." + hexDigits + @")?" + exponent + floatSufix + @"?", RegexOptions.Compiled), delegate(out Token[] tokOut, ref State state, string matchString, Match match, ref int curColumn, ref int curLine) { tokOut = new Token[] { new Token(TokenType.NUMBER, matchString.Replace("\\\n", ""), null) }; curColumn += matchString.Length; }),
                new TokenMatch(new Regex(@"0" + escNewlines + @"[xX]" + hexDigits + @"\." + escNewlines + @"[0-9a-fA-F]" + hexDigits + exponent + floatSufix + @"?", RegexOptions.Compiled), delegate(out Token[] tokOut, ref State state, string matchString, Match match, ref int curColumn, ref int curLine) { tokOut = new Token[] { new Token(TokenType.NUMBER, matchString.Replace("\\\n", ""), null) }; curColumn += matchString.Length; }),
                    
                // comments.
                new TokenMatch(new Regex(@"/(?:\\\n)*/", RegexOptions.Compiled), delegate(out Token[] tokOut, ref State state, string matchString, Match match, ref int curColumn, ref int curLine) { tokOut = null; state = State.CPP_COMMENT; curTokVal.Clear(); curTokVal.Append("//"); curColumn += matchString.Length; }),
                new TokenMatch(new Regex(@"/(?:\\\n)*\*", RegexOptions.Compiled), delegate(out Token[] tokOut, ref State state, string matchString, Match match, ref int curColumn, ref int curLine) { tokOut = null; state = State.C_COMMENT; curTokVal.Clear(); curTokVal.Append("/*"); curColumn += matchString.Length; }),
            
                // strings.
                new TokenMatch(new Regex(@"(?:(u(?:\\\n)*8?)|U|L)?(?:\\\n)*R(?:\\\n)*""(?<prefix>([^ \t\v\f\\\n\)""]|(\\\n))*)\(", RegexOptions.Compiled),
                               delegate(out Token[] tokOut, ref State state, string matchString, Match match, ref int curColumn, ref int curLine)
                               {
                                    rawStringPrefix = match.Groups["prefix"].Value.Replace("\\\n", "");
                                    tokOut = null; state = State.RAW_STRING;
                                    curTokVal.Clear();
                                    curTokVal.Append(matchString);
                                    curColumn += matchString.Length;

                                    TokenMatch[] matches;
                                    tokenRegexs.TryGetValue(State.RAW_STRING, out matches);

                                    matches[2].regex = new Regex(@"\)(?<!" + Regex.Escape(rawStringPrefix) + @"""" + stringSuffix + @"?)", RegexOptions.Compiled);
                                    matches[3].regex = new Regex(@"\)" + Regex.Escape(rawStringPrefix) + @"""" + stringSuffix + @"?", RegexOptions.Compiled);
                               }),
                new TokenMatch(new Regex(@"(?:(u(?:\\\n)*8?)|U|L)?(?:\\\n)*""", RegexOptions.Compiled), delegate(out Token[] tokOut, ref State state, string matchString, Match match, ref int curColumn, ref int curLine) { tokOut = null; curTokVal.Clear(); curTokVal.Append(matchString); state = State.STRING; } ),
                new TokenMatch(new Regex(@"(?:(u(?:\\\n)*8?)|U|L)?(?:\\\n)*'", RegexOptions.Compiled), delegate(out Token[] tokOut, ref State state, string matchString, Match match, ref int curColumn, ref int curLine) { tokOut = null; curTokVal.Clear(); curTokVal.Append(matchString); state = State.CHAR; } ),

                // chars.
            };

            if (digraphs)
            {
                // digrams
                defaultStateMatches.AddRange(new List<TokenMatch>() {
                    new TokenMatch(new Regex(@"<(?:\\\n)*:", RegexOptions.Compiled), delegate (out Token[] tokOut, ref State state, string matchString, Match match, ref int curColumn, ref int curLine) { tokOut = new Token[] { new Token(TokenType.L_SQ_BRACKET, "[", null) }; curColumn += matchString.Length; }),
                    new TokenMatch(new Regex(@":(?:\\\n)*>", RegexOptions.Compiled), delegate (out Token[] tokOut, ref State state, string matchString, Match match, ref int curColumn, ref int curLine) { tokOut = new Token[] { new Token(TokenType.R_SQ_BRACKET, "]", null) }; curColumn += matchString.Length; }),
                    new TokenMatch(new Regex(@"<(?:\\\n)*%", RegexOptions.Compiled), delegate (out Token[] tokOut, ref State state, string matchString, Match match, ref int curColumn, ref int curLine) { tokOut = new Token[] { new Token(TokenType.L_CURLY_BRACE, "{", null) }; curColumn += matchString.Length; }),
                    new TokenMatch(new Regex(@"%(?:\\\n)*>", RegexOptions.Compiled), delegate (out Token[] tokOut, ref State state, string matchString, Match match, ref int curColumn, ref int curLine) { tokOut = new Token[] { new Token(TokenType.R_CURLY_BRACE, "}", null) }; curColumn += matchString.Length; }),
                    new TokenMatch(new Regex(@"%(?:\\\n)*:", RegexOptions.Compiled), delegate (out Token[] tokOut, ref State state, string matchString, Match match, ref int curColumn, ref int curLine) { tokOut = new Token[] { new Token(TokenType.HASH, "#", null) }; curColumn += matchString.Length; }),
                    new TokenMatch(new Regex(@"%(?:\\\n)*:#", RegexOptions.Compiled), delegate (out Token[] tokOut, ref State state, string matchString, Match match, ref int curColumn, ref int curLine) { tokOut = new Token[] { new Token(TokenType.HASH_HASH, "#", null) }; curColumn += matchString.Length; }),
                    new TokenMatch(new Regex(@"#%(?:\\\n)*:", RegexOptions.Compiled), delegate (out Token[] tokOut, ref State state, string matchString, Match match, ref int curColumn, ref int curLine) { tokOut = new Token[] { new Token(TokenType.HASH_HASH, "#", null) }; curColumn += matchString.Length; }),
                    new TokenMatch(new Regex(@"%(?:\\\n)*:(?:\\\n)*%(?:\\\n)*:", RegexOptions.Compiled), delegate (out Token[] tokOut, ref State state, string matchString, Match match, ref int curColumn, ref int curLine) { tokOut = new Token[] { new Token(TokenType.HASH_HASH, "##", null) }; curColumn += matchString.Length; }),
                    new TokenMatch(new Regex(@"c(?:\\\n)*o(?:\\\n)*m(?:\\\n)*p(?:\\\n)*l", RegexOptions.Compiled | RegexOptions.IgnoreCase), delegate (out Token[] tokOut, ref State state, string matchString, Match match, ref int curColumn, ref int curLine) { tokOut = new Token[] { new Token(TokenType.HASH_HASH, "~", null) }; curColumn += matchString.Length; }),
                    new TokenMatch(new Regex(@"n(?:\\\n)*o(?:\\\n)*t", RegexOptions.Compiled | RegexOptions.IgnoreCase), delegate (out Token[] tokOut, ref State state, string matchString, Match match, ref int curColumn, ref int curLine) { tokOut = new Token[] { new Token(TokenType.HASH_HASH, "!", null) }; curColumn += matchString.Length; }),
                    new TokenMatch(new Regex(@"b(?:\\\n)*i(?:\\\n)*t(?:\\\n)*a(?:\\\n)*n(?:\\\n)*d", RegexOptions.Compiled | RegexOptions.IgnoreCase), delegate (out Token[] tokOut, ref State state, string matchString, Match match, ref int curColumn, ref int curLine) { tokOut = new Token[] { new Token(TokenType.HASH_HASH, "&", null) }; curColumn += matchString.Length; }),
                    new TokenMatch(new Regex(@"b(?:\\\n)*i(?:\\\n)*t(?:\\\n)*o(?:\\\n)*r", RegexOptions.Compiled | RegexOptions.IgnoreCase), delegate (out Token[] tokOut, ref State state, string matchString, Match match, ref int curColumn, ref int curLine) { tokOut = new Token[] { new Token(TokenType.HASH_HASH, "|", null) }; curColumn += matchString.Length; }),
                    new TokenMatch(new Regex(@"a(?:\\\n)*n(?:\\\n)*d", RegexOptions.Compiled | RegexOptions.IgnoreCase), delegate (out Token[] tokOut, ref State state, string matchString, Match match, ref int curColumn, ref int curLine) { tokOut = new Token[] { new Token(TokenType.HASH_HASH, "&&", null) }; curColumn += matchString.Length; }),
                    new TokenMatch(new Regex(@"o(?:\\\n)*r", RegexOptions.Compiled | RegexOptions.IgnoreCase), delegate (out Token[] tokOut, ref State state, string matchString, Match match, ref int curColumn, ref int curLine) { tokOut = new Token[] { new Token(TokenType.HASH_HASH, "||", null) }; curColumn += matchString.Length; }),
                    new TokenMatch(new Regex(@"x(?:\\\n)*o(?:\\\n)*r", RegexOptions.Compiled | RegexOptions.IgnoreCase), delegate (out Token[] tokOut, ref State state, string matchString, Match match, ref int curColumn, ref int curLine) { tokOut = new Token[] { new Token(TokenType.HASH_HASH, "^", null) }; curColumn += matchString.Length; }),
                    new TokenMatch(new Regex(@"a(?:\\\n)*n(?:\\\n)*d(?:\\\n)*_(?:\\\n)*e(?:\\\n)*q", RegexOptions.Compiled | RegexOptions.IgnoreCase), delegate (out Token[] tokOut, ref State state, string matchString, Match match, ref int curColumn, ref int curLine) { tokOut = new Token[] { new Token(TokenType.HASH_HASH, "&=", null) }; curColumn += matchString.Length; }),
                    new TokenMatch(new Regex(@"o(?:\\\n)*r(?:\\\n)*_(?:\\\n)*e(?:\\\n)*q", RegexOptions.Compiled | RegexOptions.IgnoreCase), delegate (out Token[] tokOut, ref State state, string matchString, Match match, ref int curColumn, ref int curLine) { tokOut = new Token[] { new Token(TokenType.HASH_HASH, "|=", null) }; curColumn += matchString.Length; }),
                    new TokenMatch(new Regex(@"x(?:\\\n)*o(?:\\\n)*r(?:\\\n)*_(?:\\\n)*e(?:\\\n)*q", RegexOptions.Compiled | RegexOptions.IgnoreCase), delegate (out Token[] tokOut, ref State state, string matchString, Match match, ref int curColumn, ref int curLine) { tokOut = new Token[] { new Token(TokenType.HASH_HASH, "^=", null) }; curColumn += matchString.Length; }),
                    new TokenMatch(new Regex(@"n(?:\\\n)*o(?:\\\n)*t(?:\\\n)*_(?:\\\n)*e(?:\\\n)*q", RegexOptions.Compiled | RegexOptions.IgnoreCase), delegate (out Token[] tokOut, ref State state, string matchString, Match match, ref int curColumn, ref int curLine) { tokOut = new Token[] { new Token(TokenType.HASH_HASH, "!=", null) }; curColumn += matchString.Length; }),
                });
            }

            tokenRegexs = new Dictionary<State, TokenMatch[]>()
            {
                {State.DEFAULT, defaultStateMatches.ToArray() },
                { State.CPP_COMMENT, new TokenMatch[] {
                        new TokenMatch(new Regex(@"[^\\\n]+|(?:\\(?!\n))", RegexOptions.Compiled), delegate(out Token[] tokOut, ref State state, string matchString, Match match, ref int curColumn, ref int curLine) { tokOut = null; curTokVal.Append(matchString); curColumn += matchString.Length; } ),
                        new TokenMatch(new Regex(@"\\\n", RegexOptions.Compiled), delegate(out Token[] tokOut, ref State state, string matchString, Match match, ref int curColumn, ref int curLine) { tokOut = null; curTokVal.Append("\\\n"); curColumn = 0; curLine++; } ),
                        new TokenMatch(new Regex(@"(?<!\\)\n", RegexOptions.Compiled), delegate(out Token[] tokOut, ref State state, string matchString, Match match, ref int curColumn, ref int curLine) { curTokVal.Append(matchString); tokOut = new Token[] { new Token(TokenType.COMMENT, curTokVal.ToString(), null) }; state = State.DEFAULT; curColumn = 0; curLine++; } ),
                    }
                },
                { State.C_COMMENT, new TokenMatch[] {
                        new TokenMatch(new Regex(@"[^*]+", RegexOptions.Compiled), delegate(out Token[] tokOut, ref State state, string matchString, Match match, ref int curColumn, ref int curLine) { tokOut = null; curTokVal.Append(matchString); curColumn += matchString.Length; } ),
                        new TokenMatch(new Regex(@"\*(?!/)", RegexOptions.Compiled), delegate(out Token[] tokOut, ref State state, string matchString, Match match, ref int curColumn, ref int curLine) { tokOut = null; curTokVal.Append(matchString); curColumn = 0; curLine+= matchString.Length; } ),
                        new TokenMatch(new Regex(@"\*/", RegexOptions.Compiled), delegate(out Token[] tokOut, ref State state, string matchString, Match match, ref int curColumn, ref int curLine) { curTokVal.Append(matchString); tokOut = new Token[] { new Token(TokenType.COMMENT, curTokVal.ToString(), null) }; state = State.DEFAULT; curColumn = 0; curLine++; } ),
                    }
                },
                { State.RAW_STRING, new TokenMatch[] {
                        new TokenMatch(new Regex(@"[^)\n]+", RegexOptions.Compiled), delegate(out Token[] tokOut, ref State state, string matchString, Match match, ref int curColumn, ref int curLine) { curTokVal.Append(matchString); tokOut = null; curColumn += matchString.Length; } ),
                        new TokenMatch(new Regex(@"\n", RegexOptions.Compiled), delegate(out Token[] tokOut, ref State state, string matchString, Match match, ref int curColumn, ref int curLine) { curTokVal.Append(matchString); tokOut = null; curColumn = 0; curLine++; } ),
                        new TokenMatch(new Regex(@"\)(?<!" + "" + @""")", RegexOptions.Compiled), delegate(out Token[] tokOut, ref State state, string matchString, Match match, ref int curColumn, ref int curLine) { curTokVal.Append(matchString); tokOut = null; curColumn = 0; curLine++; } ),
                        new TokenMatch(new Regex(@"\)" + "" + @"""", RegexOptions.Compiled), delegate(out Token[] tokOut, ref State state, string matchString, Match match, ref int curColumn, ref int curLine) { curTokVal.Append(matchString); tokOut = new Token[] { new Token(TokenType.STRING, curTokVal.ToString().Replace("\\\n", ""), null) }; curColumn = 0; curLine++; state = State.DEFAULT; } ),
                    }
                },
                { State.STRING, new TokenMatch[] {
                        new TokenMatch(new Regex(@"[^\\\n""]+", RegexOptions.Compiled), delegate(out Token[] tokOut, ref State state, string matchString, Match match, ref int curColumn, ref int curLine) { curTokVal.Append(matchString); tokOut = null; curColumn += matchString.Length; } ),
                        new TokenMatch(new Regex(@"\\\n", RegexOptions.Compiled), delegate(out Token[] tokOut, ref State state, string matchString, Match match, ref int curColumn, ref int curLine) { if(!allowSlashNInString) { throw new InvalidDataException("Unexpected \"" + matchString + "\" at column " + (curColumn + 1) + " line " + (curLine + 1)); } if(!treatStringSlashNAsNothing) { curTokVal.Append(matchString); } tokOut = null; curColumn = 0; curLine++; } ),
                        new TokenMatch(new Regex(@"(?<!\\)\n", RegexOptions.Compiled), delegate(out Token[] tokOut, ref State state, string matchString, Match match, ref int curColumn, ref int curLine) { throw new InvalidDataException("Unexpected string at column " + (curColumn + 1) + " line " + (curLine + 1)); } ),
                        new TokenMatch(new Regex(@"\\""", RegexOptions.Compiled), delegate(out Token[] tokOut, ref State state, string matchString, Match match, ref int curColumn, ref int curLine) { tokOut = null; curTokVal.Append(matchString); curColumn += matchString.Length; } ),
                        new TokenMatch(new Regex(@"\\(?![""\n])", RegexOptions.Compiled), delegate(out Token[] tokOut, ref State state, string matchString, Match match, ref int curColumn, ref int curLine) { tokOut = null; curTokVal.Append(matchString); curColumn += matchString.Length; } ),
                        new TokenMatch(new Regex(@"(?!<\\)""" + stringSuffix + @"?", RegexOptions.Compiled), delegate(out Token[] tokOut, ref State state, string matchString, Match match, ref int curColumn, ref int curLine) { curTokVal.Append(matchString); tokOut = new Token[] { new Token(TokenType.STRING, curTokVal.ToString(), null) }; curColumn += matchString.Length; state = State.DEFAULT; } ),
                    }
                },
                { State.CHAR, new TokenMatch[] {
                        new TokenMatch(new Regex(@"[^\\\n']+", RegexOptions.Compiled), delegate(out Token[] tokOut, ref State state, string matchString, Match match, ref int curColumn, ref int curLine) { curTokVal.Append(matchString); tokOut = null; curColumn += matchString.Length; } ),
                        new TokenMatch(new Regex(@"\\\n", RegexOptions.Compiled), delegate(out Token[] tokOut, ref State state, string matchString, Match match, ref int curColumn, ref int curLine) { if(!allowSlashNInString) { throw new InvalidDataException("Unexpected \"" + matchString + "\" at column " + (curColumn + 1) + " line " + (curLine + 1)); } if(!treatStringSlashNAsNothing) { curTokVal.Append(matchString); } tokOut = null; curColumn = 0; curLine++; } ),
                        new TokenMatch(new Regex(@"(?<!\\)\n", RegexOptions.Compiled), delegate(out Token[] tokOut, ref State state, string matchString, Match match, ref int curColumn, ref int curLine) { throw new InvalidDataException("Unexpected string at column " + (curColumn + 1) + " line " + (curLine + 1)); } ),
                        new TokenMatch(new Regex(@"\\'", RegexOptions.Compiled), delegate(out Token[] tokOut, ref State state, string matchString, Match match, ref int curColumn, ref int curLine) { tokOut = null; curTokVal.Append(matchString); curColumn += matchString.Length; } ),
                        new TokenMatch(new Regex(@"\\(?!['\n])", RegexOptions.Compiled), delegate(out Token[] tokOut, ref State state, string matchString, Match match, ref int curColumn, ref int curLine) { tokOut = null; curTokVal.Append(matchString); curColumn += matchString.Length; } ),
                        new TokenMatch(new Regex(@"(?!<\\)'", RegexOptions.Compiled), delegate(out Token[] tokOut, ref State state, string matchString, Match match, ref int curColumn, ref int curLine) { curTokVal.Append(matchString); tokOut = new Token[] { new Token(TokenType.CHAR, curTokVal.ToString(), null) }; curColumn += matchString.Length; state = State.DEFAULT; } ),
                    }
                },
            };
        }

        public bool IgnoreStrayBackslash
        {
            get
            {
                return ignoreStrayBackslash;
            }

            set
            {
                this.ignoreStrayBackslash = value;
            }
        }

        public IEnumerable<Token> GetTokenEnumerable(Stream inStream)
        {
            State state = State.DEFAULT;
            TrigraphStream charStream = new TrigraphStream(inStream, trigraphs, digraphs);
            IEnumerable<char> charEnumerable = charStream.GetCharEnumerable();
            IEnumerator<char> charEnumerator = charEnumerable.GetEnumerator();
            StringBuilder fileContentsBuilder = new StringBuilder();
            String fileContents = null;
            int linePtr = 0;
            int curColumn = 0;
            int curLine = 0;

            while (charEnumerator.MoveNext())
            {
                fileContentsBuilder.Append(charEnumerator.Current);
            }

            fileContents = fileContentsBuilder.ToString();

            while (linePtr < fileContents.Length)
            {
                TokenMatch[] matches;

                if (!tokenRegexs.TryGetValue(state, out matches))
                {
                    throw new NotImplementedException("No code exists to handle state: " + state.ToString());
                }

                int longestMatch = int.MinValue;
                TokenMatch? maxTokenMatch = null;
                Match maxMatch = null;

                foreach (TokenMatch tokMatch in matches)
                {
                    Match m = tokMatch.regex.Match(fileContents, linePtr);

                    if (m.Success && m.Index == linePtr && m.Length > longestMatch)
                    {
                        longestMatch = m.Length;
                        maxTokenMatch = tokMatch;
                        maxMatch = m;
                    }
                }

                if (maxTokenMatch == null || !maxTokenMatch.HasValue)
                {
                    throw new InvalidDataException("Unexpected \"" + fileContents[linePtr] + "\" at column " + (curColumn + 1) + " line " + (curLine + 1));
                }
                else
                {
                    Token[] nextTokens = null;

                    maxTokenMatch.Value.handler(out nextTokens, ref state, maxMatch.Value, maxMatch, ref curColumn, ref curLine);

                    if (nextTokens != null)
                    {
                        foreach (Token nextTok in nextTokens)
                        {
                            yield return nextTok;
                        }
                    }

                    linePtr += maxMatch.Length;
                }
            }

            if (state != State.DEFAULT)
            {
                throw new InvalidDataException("Unterminated token.");
            }

            Token eof;
            eof.tokenType = TokenType.EOF;
            eof.value = "";
            eof.tokens = null;

            yield return eof;
        }
    }
}
