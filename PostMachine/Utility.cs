using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;

namespace PostMachine
{
    class Utility
    {
        public static void ClearBuffer(Stream istream)//ref TextReader istream)
        {
            //StreamReader sr = new StreamReader(istream);
            //int length = sr.ReadLine().Length;
            Console.WriteLine("In CleanBuffer().");
            char ch;
            do
            {
                ch = Convert.ToChar(istream.ReadByte());  // (char)istream.ReadByte();
                Console.WriteLine(ch);
            } while ( (istream.CanRead) && (ch != '\n') );
        }
    }
}
