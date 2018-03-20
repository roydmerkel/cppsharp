//
//  Preprocessor.cs
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
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;

namespace libcppsharp
{
    public class Preprocessor
    {
        private struct Define
        {
            public string name;
            public string[] paramList;
            public Token[] tokens;
        }

        bool handleTrigraphs;
        bool handleDigraphs;
        bool whitespacePaste;
        Stream inStream;
        DirectoryInfo[] includePath;
        List<Define> defines;

        public Preprocessor(Stream inStream, bool handleTrigraphs = false, bool handleDigraphs = false, bool whitespacePaste = false)
        {
            List<DirectoryInfo> directories = new List<DirectoryInfo>();

            this.inStream = inStream;
            this.handleDigraphs = handleDigraphs;
            this.handleTrigraphs = handleTrigraphs;
            this.whitespacePaste = whitespacePaste;

            switch ((int)Environment.OSVersion.Platform)
            {
                case (int)PlatformID.MacOSX:
                case (int)PlatformID.Unix:
                case 128:
                    if (Directory.Exists("/Applications")
                    & Directory.Exists("/System")
                    & Directory.Exists("/Users")
                    & Directory.Exists("/Volumes"))
                    {
                        String[] includePathsArr = new string[] { "HEADER_SEARCH_PATHS", "USER_HEADER_SEARCH_PATHS" };

                        foreach (String includePathsEnv in includePathsArr)
                        {
                            String includePaths = Environment.GetEnvironmentVariable(includePathsEnv);

                            if (includePaths != null)
                            {
                                String[] paths = includePaths.Split(new char[] { Path.PathSeparator });

                                foreach (string path in paths)
                                {
                                    DirectoryInfo di = null;

                                    try
                                    {
                                        di = new DirectoryInfo(path);

                                        if (di.Exists)
                                        {
                                            directories.Add(di);
                                        }
                                    }
                                    catch (Exception)
                                    {
                                    }
                                }
                            }
                        }

                        Process p = new Process();
                        p.StartInfo.UseShellExecute = false;
                        p.StartInfo.RedirectStandardOutput = true;
                        p.StartInfo.FileName = "sh";
                        p.StartInfo.Arguments = @"-c ""xcode-select -p""";
                        p.Start();
                        string output = p.StandardOutput.ReadToEnd();
                        p.WaitForExit();

                        if (output != null && output.Length > 0)
                        {
                            output = output.Trim();
                            String arguments = @"-c ""echo | " + output + @"/usr/bin/gcc -c -x c++ -Wp,-v - 2>&1 | grep -v \""^#\"" | grep -v \""End of\"" | grep -v \""^ignoring \"" | sed -e 's/^[ \t]*//g' | sed -e 's/(framework directory)//g' | grep -v '^clang '""";
                            p = new Process();
                            p.StartInfo.UseShellExecute = false;
                            p.StartInfo.RedirectStandardOutput = true;
                            p.StartInfo.FileName = "sh";
                            p.StartInfo.Arguments = arguments;
                            p.Start();
                            output = p.StandardOutput.ReadToEnd();
                            p.WaitForExit();

                            if (output != null && output.Length > 0)
                            {
                                includePathsArr = output.Split(new char[] { '\n' });

                                foreach (String path in includePathsArr)
                                {
                                    if (path != null)
                                    {
                                        DirectoryInfo di = null;

                                        try
                                        {
                                            di = new DirectoryInfo(path);

                                            if (di.Exists)
                                            {
                                                directories.Add(di);
                                            }
                                        }
                                        catch (Exception)
                                        {
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        String[] includePathsArr = new string[] { "CPATH", "C_INCLUDE_PATH", "CPLUS_INCLUDE_PATH", "OBJC_INCLUDE_PATH" };

                        foreach (String includePathsEnv in includePathsArr)
                        {
                            String includePaths = Environment.GetEnvironmentVariable(includePathsEnv);

                            if (includePaths != null)
                            {
                                String[] paths = includePaths.Split(new char[] { Path.PathSeparator });

                                foreach (string path in paths)
                                {
                                    DirectoryInfo di = null;

                                    try
                                    {
                                        di = new DirectoryInfo(path);

                                        if (di.Exists)
                                        {
                                            directories.Add(di);
                                        }
                                    }
                                    catch (Exception)
                                    {
                                    }
                                }
                            }
                        }

                        Process p = new Process();
                        p.StartInfo.UseShellExecute = false;
                        p.StartInfo.RedirectStandardOutput = true;
                        p.StartInfo.FileName = "sh";
                        p.StartInfo.Arguments = @"-c ""echo | cpp -x c++ -Wp,-v - 2>&1 | grep -v \""^#\"" | grep -v \""End of\"" | grep -v \""^ignoring \"" | sed -e 's/^[ \t]*//g'""";
                        p.Start();
                        string output = p.StandardOutput.ReadToEnd();
                        p.WaitForExit();

                        includePathsArr = output.Split(new char[] { '\n' });

                        foreach (String path in includePathsArr)
                        {
                            if (path != null)
                            {
                                DirectoryInfo di = null;

                                try
                                {
                                    di = new DirectoryInfo(path);

                                    if (di.Exists)
                                    {
                                        directories.Add(di);
                                    }
                                }
                                catch (Exception)
                                {
                                }
                            }
                        }

                    }
                    break;
                case (int)PlatformID.Win32NT:
                case (int)PlatformID.Win32S:
                case (int)PlatformID.Win32Windows:
                case (int)PlatformID.WinCE:
                case (int)PlatformID.Xbox:
                    {
                        String includePaths = Environment.GetEnvironmentVariable("INCLUDE");

                        if (includePaths != null)
                        {
                            String[] paths = includePaths.Split(new char[] { Path.PathSeparator });

                            foreach (string path in paths)
                            {
                                DirectoryInfo di = null;

                                try
                                {
                                    di = new DirectoryInfo(path);

                                    if (di.Exists)
                                    {
                                        directories.Add(di);
                                    }
                                }
                                catch (Exception)
                                {
                                }
                            }
                        }
                    }
                    break;
            }

            this.includePath = directories.ToArray();
            this.defines = new List<Define>();
        }

        public bool Preprocess(TextWriter outStream, TextWriter warnStream)
        {
            return Preprocess(outStream, warnStream, this.inStream);
        }

        public bool Preprocess(TextWriter outStream, TextWriter warnStream, Stream inStream)
        {
            bool foundStartHash = false;

            TokenStream tokenStream = tokenStream = new TokenStream(inStream, handleTrigraphs, handleDigraphs, true, true);
            IEnumerable<Token> enumerable = tokenStream.GetTokenEnumerable();
            IEnumerator<Token> enumerator = enumerable.GetEnumerator();
            List<Token> lineToks = new List<Token>();

            while (true)
            {
                foundStartHash = false;
                lineToks.Clear();

                while (enumerator.MoveNext())
                {
                    Token t = enumerator.Current;

                    if (t.tokenType == TokenType.NEWLINE)
                    {
                        lineToks.Add(t);
                        break;
                    }
                    else if (t.tokenType == TokenType.EOF)
                    {
                        break;
                    }
                    else
                    {
                        lineToks.Add(t);
                    }
                }

                foreach (Token t in lineToks)
                {
                    if (t.tokenType == TokenType.WHITESPACE)
                    {
                    }
                    else if (t.tokenType == TokenType.HASH)
                    {
                        foundStartHash = true;
                        break;
                    }
                    else
                    {
                        foundStartHash = false;
                        break;
                    }
                }

                // if not found hash, do replacements:
                Token last;
                last.tokenType = TokenType.UNKNOWN;

                if (!foundStartHash)
                {
                    bool replaced = false;

                    // do replacement loop.
                    foreach (Token t in lineToks)
                    {
                        if (t.tokenType == TokenType.IDENTIFIER)
                        {
                            foreach (Define def in defines)
                            {
                                if (t.value == def.name)
                                {
                                    // TODO: do replacements.
                                    replaced = true;
                                }
                            }
                        }
                        last = t;
                    }
                }
                else
                {
                    bool foundHash = false;
                    bool foundDirective = false;
                    string directive = null;

                    foreach (Token t in lineToks)
                    {
                        if (!foundHash)
                        {
                            if (t.tokenType == TokenType.HASH)
                            {
                                foundHash = true;
                            }
                        }
                        else if (foundHash && !foundDirective)
                        {
                            if (t.tokenType == TokenType.WHITESPACE)
                            {
                                continue;
                            }
                            else if (t.tokenType == TokenType.IDENTIFIER)
                            {
                                foundDirective = true;
                                directive = t.value;

                                switch (directive.ToLower())
                                {
                                    case "include":
                                    case "define":
                                    case "undef":
                                    case "if":
                                    case "elif":
                                    case "else":
                                    case "endif":
                                    case "ifdef":
                                    case "ifndef":
                                    case "line":
                                    case "pragma":
                                    case "error":
                                    case "warning":
                                        break;
                                    default:
                                        throw new InvalidDataException("invalid preprocessor directive:" + directive);
                                }

                                break;
                            }
                            else
                            {
                                throw new InvalidDataException("preprocessor directive not found after newline #...");
                            }
                        }
                    }

                    if (foundDirective)
                    {
                        switch (directive)
                        {
                            case "include":
                                {
                                    string incPath = "";
                                    bool foundInclude = false;
                                    bool foundString = false;
                                    bool foundLessThen = false;
                                    bool foundGreaterThen = false;
                                    bool foundEOL = false;

                                    foundHash = false;
                                    foreach (Token t in lineToks)
                                    {
                                        if (!foundHash)
                                        {
                                            if (t.tokenType == TokenType.HASH)
                                            {
                                                foundHash = true;
                                            }
                                        }
                                        else if (foundHash && !foundInclude)
                                        {
                                            if (t.tokenType == TokenType.IDENTIFIER)
                                            {
                                                foundInclude = true;
                                            }
                                        }
                                        else if (foundInclude && !foundString && !foundLessThen)
                                        {
                                            switch (t.tokenType)
                                            {
                                                case TokenType.WHITESPACE:
                                                    break;
                                                case TokenType.STRING:
                                                    if (!t.value.StartsWith("\"") || !t.value.EndsWith("\""))
                                                    {
                                                        throw new InvalidDataException("Invalid stringtype in #include, basic string literal expected (e.g. \"myfile.h\", or similar.)");
                                                    }

                                                    foundString = true;
                                                    incPath = t.value;
                                                    break;
                                                case TokenType.LESS_THEN:
                                                    foundLessThen = true;
                                                    incPath = "<";
                                                    break;
                                                default:
                                                    throw new InvalidDataException("Unexpected token: " + t.value + " while process #include directive.");
                                            }
                                        }
                                        else if (foundLessThen && !foundGreaterThen)
                                        {
                                            if (t.tokenType == TokenType.GREATER_THEN)
                                            {
                                                incPath = incPath + ">";
                                                foundGreaterThen = true;
                                            }
                                            else if (t.tokenType == TokenType.NEWLINE)
                                            {
                                                throw new InvalidDataException("Unexpected \n while parsing #include.");
                                            }
                                            else
                                            {
                                                incPath = incPath + t.value;
                                            }
                                        }
                                        else if ((foundString || foundGreaterThen) && !foundEOL)
                                        {
                                            if (t.tokenType == TokenType.WHITESPACE)
                                            {
                                            }
                                            else if (t.tokenType == TokenType.NEWLINE)
                                            {
                                                foundEOL = true;
                                            }
                                            else
                                            {
                                                throw new InvalidDataException("Unexpected " + t.value + " while processing #include.");
                                            }
                                        }
                                    }

                                    if (!foundEOL)
                                    {
                                        throw new InvalidDataException("unterminated #include.");
                                    }

                                    string fname = incPath.Substring(1, incPath.Length - 2);
                                    FileInfo incFile = null;
                                    if (incPath.StartsWith("\""))
                                    {
                                        string searchFile = Environment.CurrentDirectory + Path.DirectorySeparatorChar + fname;
                                        if (File.Exists(searchFile))
                                        {
                                            incFile = new FileInfo(searchFile);
                                        }
                                        else
                                        {
                                            foreach (DirectoryInfo dir in includePath)
                                            {
                                                searchFile = dir.FullName + Path.DirectorySeparatorChar + fname;

                                                if (File.Exists(searchFile))
                                                {
                                                    incFile = new FileInfo(searchFile);
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                    else if (incPath.StartsWith("<"))
                                    {
                                        foreach (DirectoryInfo dir in includePath)
                                        {
                                            string searchFile = dir.FullName + Path.DirectorySeparatorChar + fname;

                                            if (File.Exists(searchFile))
                                            {
                                                incFile = new FileInfo(searchFile);
                                                break;
                                            }
                                        }
                                    }

                                    if (incFile != null)
                                    {
                                        bool includeSuccess = Preprocess(outStream, warnStream, incFile.OpenRead());

                                        if (!includeSuccess)
                                        {
                                            return false;
                                        }
                                    }
                                }
                                break;
                            case "error":
                                {
                                    bool foundError = false;
                                    bool foundEOL = false;
                                    string errText = "";

                                    foundHash = false;
                                    foreach (Token t in lineToks)
                                    {
                                        if (!foundHash)
                                        {
                                            if (t.tokenType == TokenType.HASH)
                                            {
                                                foundHash = true;
                                            }
                                        }
                                        else if (foundHash && !foundError)
                                        {
                                            if (t.tokenType == TokenType.IDENTIFIER)
                                            {
                                                foundError = true;
                                            }
                                        }
                                        else if (foundError && !foundEOL)
                                        {
                                            if (t.tokenType == TokenType.WHITESPACE && errText.Length == 0)
                                            {
                                            }
                                            else if (t.tokenType == TokenType.NEWLINE)
                                            {
                                                foundEOL = true;
                                            }
                                            else
                                            {
                                                errText = errText + t.value;
                                            }
                                        }
                                    }

                                    if (!foundEOL)
                                    {
                                        throw new InvalidDataException("unterminated #error.");
                                    }

                                    throw new ApplicationException(errText);
                                }
                            case "warning":
                                {
                                    bool foundError = false;
                                    bool foundEOL = false;
                                    string errText = "";

                                    foundHash = false;
                                    foreach (Token t in lineToks)
                                    {
                                        if (!foundHash)
                                        {
                                            if (t.tokenType == TokenType.HASH)
                                            {
                                                foundHash = true;
                                            }
                                        }
                                        else if (foundHash && !foundError)
                                        {
                                            if (t.tokenType == TokenType.IDENTIFIER)
                                            {
                                                foundError = true;
                                            }
                                        }
                                        else if (foundError && !foundEOL)
                                        {
                                            if (t.tokenType == TokenType.WHITESPACE && errText.Length == 0)
                                            {
                                            }
                                            else if (t.tokenType == TokenType.NEWLINE)
                                            {
                                                foundEOL = true;
                                            }
                                            else
                                            {
                                                errText = errText + t.value;
                                            }
                                        }
                                    }

                                    if (!foundEOL)
                                    {
                                        throw new InvalidDataException("unterminated #error.");
                                    }

                                    warnStream.WriteLine(errText);
                                }
                                break;
                            case "define":
                                {
                                    bool foundInclude = false;
                                    bool foundDefineName = false;
                                    bool hasStartParams = false;
                                    bool hasParams = false;
                                    bool hasWhitespaceAfterParams = false;
                                    bool awaitIdentifier = false;
                                    bool hasComma = false;
                                    bool hasEOL = false;
                                    string defineName = "";
                                    List<string> paramList = new List<string>();
                                    List<Token> definitionList = new List<Token>();
                                    int numPeriods = 0;

                                    foundHash = false;
                                    foreach (Token t in lineToks)
                                    {
                                        if (!foundHash)
                                        {
                                            if (t.tokenType == TokenType.HASH)
                                            {
                                                foundHash = true;
                                            }
                                        }
                                        else if (foundHash && !foundInclude)
                                        {
                                            if (t.tokenType == TokenType.IDENTIFIER)
                                            {
                                                foundInclude = true;
                                            }
                                        }
                                        else if (foundInclude && !foundDefineName)
                                        {
                                            if (t.tokenType == TokenType.IDENTIFIER)
                                            {
                                                foundDefineName = true;
                                                defineName = t.value;
                                            }
                                            else if (t.tokenType == TokenType.WHITESPACE)
                                            {
                                            }
                                            else
                                            {
                                                throw new InvalidDataException("Unexpected " + t.value + " in #define.");
                                            }
                                        }
                                        else if (foundDefineName && !hasStartParams && !hasParams)
                                        {
                                            if (t.tokenType == TokenType.L_PAREN)
                                            {
                                                hasStartParams = true;
                                                awaitIdentifier = true;
                                            }
                                            else if (t.tokenType == TokenType.WHITESPACE)
                                            {
                                                hasParams = true;
                                                hasWhitespaceAfterParams = true;
                                            }
                                            else
                                            {
                                                throw new InvalidDataException("Unexpected " + t.value + " in #define.");
                                            }
                                        }
                                        else if (hasStartParams && !hasParams)
                                        {
                                            switch (t.tokenType)
                                            {
                                                case TokenType.PERIOD:
                                                    if (!awaitIdentifier)
                                                    {
                                                        throw new InvalidDataException("Unexpected identifier in macro parameter list.");
                                                    }
                                                    else
                                                    {
                                                        numPeriods++;

                                                        if (numPeriods == 3)
                                                        {
                                                            paramList.Add("...");
                                                            awaitIdentifier = false;
                                                        }
                                                        else if (numPeriods > 3)
                                                        {
                                                            throw new InvalidDataException("Unexpected period in #define parameter list.");
                                                        }
                                                    }

                                                    break;
                                                case TokenType.COMMA:
                                                    if (awaitIdentifier)
                                                    {
                                                        throw new InvalidDataException("Unexpected comma at beinning of macro parameter list.");
                                                    }
                                                    else
                                                    {
                                                        hasComma = true;
                                                        awaitIdentifier = true;
                                                    }
                                                    break;
                                                case TokenType.IDENTIFIER:
                                                    if (!awaitIdentifier)
                                                    {
                                                        throw new InvalidDataException("Unexpected identifier in macro parameter list.");
                                                    }
                                                    else
                                                    {
                                                        awaitIdentifier = false;
                                                        paramList.Add(t.value);
                                                    }

                                                    break;
                                                case TokenType.R_PAREN:
                                                    if (hasComma && awaitIdentifier)
                                                    {
                                                        throw new InvalidDataException("Expected parameter at end of macro parameter list.");
                                                    }
                                                    else
                                                    {
                                                        hasParams = true;
                                                    }

                                                    break;
                                                case TokenType.WHITESPACE:
                                                    if (numPeriods >= 1 && numPeriods < 3)
                                                    {
                                                        throw new InvalidDataException("Unexpected whitespace in the middle of va_args list.");
                                                    }

                                                    break;
                                                default:
                                                    throw new InvalidDataException("Unexpected " + t.value + " in parameter function macro list.");
                                            }
                                        }
                                        else if (hasParams && !hasWhitespaceAfterParams)
                                        {
                                            if (t.tokenType == TokenType.WHITESPACE)
                                            {
                                                hasWhitespaceAfterParams = true;
                                            }
                                            else
                                            {
                                                throw new InvalidDataException("Unexpected " + t.value + " in #define between name/parameters and value.");
                                            }
                                        }
                                        else if (hasWhitespaceAfterParams && !hasEOL)
                                        {
                                            if (t.tokenType == TokenType.NEWLINE)
                                            {
                                                hasEOL = true;
                                            }
                                            else if (t.tokenType == TokenType.WHITESPACE)
                                            {
                                                Token addTok = t;
                                                addTok.value = t.value.Replace("\n", "");

                                                definitionList.Add(addTok);
                                            }
                                            else
                                            {
                                                definitionList.Add(t);
                                            }
                                        }
                                    }

                                    if (!hasEOL)
                                    {
                                        throw new InvalidDataException("unterminated #define.");
                                    }
                                    else
                                    {
                                        foundHash = false;
                                        int start;
                                        int stop;
                                        bool replaced = true;

                                        while (replaced)
                                        {
                                            replaced = false;
                                            // Handle ## and #
                                            for (int i = 0; i < definitionList.Count; i++)
                                            {
                                                // handle comment paste.
                                                if (whitespacePaste && definitionList[i].tokenType == TokenType.COMMENT)
                                                {
                                                    int j;

                                                    // find start.
                                                    for (j = i - 1; j >= 0; j--)
                                                    {
                                                        if (definitionList[j].tokenType != TokenType.WHITESPACE)
                                                        {
                                                            break;
                                                        }
                                                    }

                                                    if (j < 0)
                                                    {
                                                        continue;
                                                    }

                                                    start = j;

                                                    for (j = i + 1; j < definitionList.Count; j++)
                                                    {
                                                        if (definitionList[j].tokenType != TokenType.WHITESPACE)
                                                        {
                                                            break;
                                                        }
                                                    }

                                                    if (j >= definitionList.Count)
                                                    {
                                                        continue;
                                                    }

                                                    stop = j;

                                                    List<Token> newList = new List<Token>();

                                                    newList.AddRange(definitionList.GetRange(0, start));

                                                    Token newTok;
                                                    newTok.tokenType = TokenType.TOKEN_PASTE;
                                                    newTok.value = "";
                                                    newTok.tokens = new Token[] { definitionList[start], definitionList[stop] };
                                                    newList.Add(newTok);

                                                    newList.AddRange(definitionList.GetRange(stop + 1, definitionList.Count - stop - 1));

                                                    definitionList.Clear();
                                                    definitionList.AddRange(newList);

                                                    replaced = true;
                                                    break;
                                                }
                                                else if (definitionList[i].tokenType == TokenType.HASH)
                                                {
                                                    // handle ##
                                                    if (foundHash)
                                                    {
                                                        int j;

                                                        // find start.
                                                        for (j = i - 2; j >= 0; j--)
                                                        {
                                                            if (definitionList[j].tokenType != TokenType.WHITESPACE)
                                                            {
                                                                break;
                                                            }
                                                        }

                                                        if (j < 0)
                                                        {
                                                            throw new InvalidDataException("dangling pasting operator...");
                                                        }

                                                        start = j;

                                                        for (j = i + 1; j < definitionList.Count; j++)
                                                        {
                                                            if (definitionList[j].tokenType != TokenType.WHITESPACE)
                                                            {
                                                                break;
                                                            }
                                                        }

                                                        if (j >= definitionList.Count)
                                                        {
                                                            throw new InvalidDataException("dangling pasting operator...");
                                                        }

                                                        stop = j;

                                                        List<Token> newList = new List<Token>();

                                                        newList.AddRange(definitionList.GetRange(0, start));

                                                        Token newTok;
                                                        newTok.tokenType = TokenType.TOKEN_PASTE;
                                                        newTok.value = "";
                                                        newTok.tokens = new Token[] { definitionList[start], definitionList[stop] };
                                                        newList.Add(newTok);

                                                        newList.AddRange(definitionList.GetRange(stop + 1, definitionList.Count - stop - 1));

                                                        definitionList.Clear();
                                                        definitionList.AddRange(newList);

                                                        replaced = true;
                                                        foundHash = false;
                                                        break;
                                                    }
                                                    else
                                                    {
                                                        foundHash = true;
                                                    }
                                                }
                                                else
                                                {
                                                    // handle #
                                                    if (foundHash)
                                                    {
                                                        Token newTok;
                                                        newTok.tokenType = TokenType.STRINGIFY;
                                                        newTok.value = "";
                                                        newTok.tokens = new Token[] { definitionList[i] };

                                                        definitionList[i] = newTok;

                                                        definitionList.RemoveAt(i - 1);

                                                        replaced = true;
                                                        foundHash = false;
                                                        break;
                                                    }
                                                }
                                            }
                                        }

                                        Define d = new Define();
                                        d.name = defineName;
                                        d.paramList = paramList.ToArray();
                                        d.tokens = definitionList.ToArray();

                                        foreach (Define testDefn in this.defines)
                                        {
                                            if (testDefn.name.Equals(defineName, StringComparison.CurrentCultureIgnoreCase))
                                            {
                                                bool same = true;

                                                if (testDefn.paramList.Length != d.paramList.Length)
                                                {
                                                    same = false;
                                                }

                                                if (same)
                                                {
                                                    for (int i = 0; i < testDefn.paramList.Length; i++)
                                                    {
                                                        if (!testDefn.paramList[i].Equals(d.paramList[i]))
                                                        {
                                                            same = false;
                                                            break;
                                                        }
                                                    }
                                                }

                                                if (same)
                                                {
                                                    if (testDefn.tokens.Length != d.tokens.Length)
                                                    {
                                                        same = false;
                                                    }
                                                }

                                                if (same)
                                                {
                                                    for (int i = 0; i < testDefn.tokens.Length; i++)
                                                    {
                                                        if (!testDefn.Equals(d))
                                                        {
                                                            same = false;
                                                            break;
                                                        }
                                                    }
                                                }

                                                if (!same)
                                                {
                                                    throw new InvalidDataException("Can't redefine macro " + defineName);
                                                }
                                            }
                                        }

                                        defines.Add(d);
                                    }
                                }
                                break;
                            default:
                                break;
                        }
                    }
                }

                foreach (Token t in lineToks)
                {
                    last = t;

                    if (!foundStartHash)
                    {
                        outStream.Write(t.value);
                    }
                }

                if (last.tokenType == TokenType.EOF || lineToks.Count == 0)
                {
                    break;
                }
            }

            return true;
        }
    }
}
