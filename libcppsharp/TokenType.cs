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
        COLON_COLON,
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
        THIN_ARROW,
        PLUS_PLUS,
        MINUS_MINUS,
        DOT_STAR,
        THIN_ARROW_STAR,
        SAUCER,
        LESS_THEN_LESS_THEN,
        GREATER_THEN_GRATER_THEN,
        LESS_THEN_OR_EQUAL_TO,
        GREATER_THEN_OR_EQUAL_TO,
        EQUALS_EQUALS,
        NOT_EQUALS,
        AMPERSTAND_AMPERSTAND,
        PIPE_PIPE,
        ASTERISK_EQUALS,
        FORWARD_SLASH_EQUALS,
        PERCENT_EQUALS,
        PLUS_EQUALS,
        MINUS_EQUALS,
        LESS_THEN_LESS_THEN_EQUALS,
        GREATER_THEN_GREATER_THEN_EQUALS,
        AMPERSTAND_EQUALS,
        PIPE_EQUALS,
        CARROT_EQUALS,
        QUESTION_MARK_PERIOD,
        QUESTION_MARK_L_SQ_BRACKET,
        QUESTION_MARK_QUESTION_MARK,
        HASH_HASH,
        EQUALS_SIGN_GREATER_THEN,
        ELLIPSE,
        SIZEOF,
        DEFINED,
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
