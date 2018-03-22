//
//  Token.cs
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
    public struct Token
    {
        public TokenType tokenType;
        public String value;
        public Token[] tokens;

        public Token(TokenType tokenType, String value, Token[] tokens)
        {
            this.tokenType = tokenType;
            this.value = value;
            this.tokens = null;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            else if (obj.GetType() != typeof(Token))
            {
                return false;
            }
            else
            {
                Token tok2 = (Token)obj;
                if (tokenType != tok2.tokenType)
                {
                    return false;
                }
                else if (!value.Equals(tok2.value))
                {
                    return false;
                }
                else if (tokens == null && tok2.tokens != null)
                {
                    return false;
                }
                else if (tokens != null && tok2.tokens == null)
                {
                    return false;
                }
                else if (tokens != null)
                {
                    if (tokens.Length != tok2.tokens.Length)
                    {
                        return false;
                    }

                    for (int i = 0; i < tokens.Length; i++)
                    {
                        if (!tokens[i].Equals(tok2.tokens[i]))
                        {
                            return false;
                        }
                    }
                }

                return true;
            }
		}
	}
}
