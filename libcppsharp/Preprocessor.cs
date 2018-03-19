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
        TokenStream tokenStream;
        DirectoryInfo[] includePath;

        public Preprocessor(Stream inStream, bool handleTrigraphs = false, bool handleDigraphs = false)
        {
            List<DirectoryInfo> directories = new List<DirectoryInfo>();

            tokenStream = new TokenStream(inStream, handleTrigraphs, handleDigraphs, true, true);

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
                            Console.Out.WriteLine(arguments);
                            p = new Process();
                            p.StartInfo.UseShellExecute = false;
                            p.StartInfo.RedirectStandardOutput = true;
                            p.StartInfo.FileName = "sh";
                            p.StartInfo.Arguments = arguments;
                            p.Start();
                            output = p.StandardOutput.ReadToEnd();
                            p.WaitForExit();

                            System.Console.Out.WriteLine(output);

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
        }
    }
}
