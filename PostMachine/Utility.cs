using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostMachine
{
    class Utility
    {
        public void ClearBuffer(Stream istream)
        {
            char ch;
            do
            {
                ch = Convert.ToChar(istream.ReadByte());  // (char)istream.ReadByte();
            } while ( (istream.CanRead) && (ch != '\n') );
        }
    }
}
