using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace libcppsharp.CPP
{
    internal partial class CPPParser
    {
        public CPPParser() : base(null) { }

        public void Parse(string s)
        {
            byte[] inputBuffer = System.Text.Encoding.Default.GetBytes(s);
            MemoryStream stream = new MemoryStream(inputBuffer);
            this.Scanner = new CPPScanner(stream);
            this.Parse();
        }
    }
}
