//
//  TokenType.cs
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
namespace libcppsharp
{
    public enum TokenType
    {
        UNKNOWN = 0,
        WHITESPACE,
        NEWLINE,
        HASH,
        QUESTION_MARK,
        EQUAL_SIGN,
        LESS_THEN,
        GREATER_THEN,
        COLON,
        SEMI_COLON,
        PERCENT,
        COMMA,
        PERIOD,
        TILDE,
        PIPE,
        PLUS_SIGN,
        MINUS_SIGN,
        ASTERISK,
        AMPERSTAND,
        CARROT,
        EXCLAIMATION_MARK,
        GRAVE,
        AT,
        BACK_SLASH,
        FORWARD_SLASH,
        L_CURLY_BRACE,
        R_CURLY_BRACE,
        L_SQ_BRACKET,
        R_SQ_BRACKET,
        L_PAREN,
        R_PAREN,
        IDENTIFIER,
        STRING,
        CHAR,
        NUMBER,
        COMMENT,
        TOKEN_PASTE,
        STRINGIFY,
        EOF
    }
}
