using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostMachine
{
    class Program
    {
        static void Main(string[] args)
        {
            bool batch = false;
            if (args.Length > 1) batch = true;
            PostMachine m = new PostMachine();
            bool loaded = false;
            char response = 'x';

            do
            {
                do loaded = m.Load(batch); while (!loaded);
                m.Run(batch);
                Console.WriteLine("\n** Load another program?");
                response = (char)Console.Read();
                if (batch) Console.WriteLine(response);
            } while (response == 'y' || response == 'Y');
        }
    }

    class PostMachine
    {
        private List<string> program_; // stores programs
        private Queue<char> tape_;     // the tape
        private char state_;           // stores the internal state 
        public PostMachine()
        {
            Console.WriteLine("PostMachine started");
            program_ = new List<string>();
            tape_ = new Queue<char>();
        }

        public bool Load(bool batch = false)
        {
            StreamReader in1; 
            string filename;
            do
            {
                Console.Write(" Name of instruction file: ");
                filename = Console.ReadLine();
                if (!OpenText(out in1, filename))
                    Console.WriteLine("** Cannot open file'" + filename + "'. Try again");
            } while (in1 == StreamReader.Null);

            if (batch) Console.WriteLine(filename);

            program_.Clear();
            string s;
            int count = 0;
            while ( (s = in1.ReadLine()) != null ) 
            {
                Console.WriteLine("#{0}", count++);
                int pos = s.IndexOf('*', 0);
                if (pos < s.Length - 1 && pos > 0)
                {
                    StringBuilder temp = new StringBuilder(' ', pos);
                    for (int j = 0; j < pos; ++j)
                        temp[j] = s[j];
                    
                    program_.Add(temp.ToString());
                }
                else if (pos == 0 && s.IndexOf('\n', 0) == 1)
                {
                    program_.Add(s);
                }
                else if (pos == s.Length) 
                {
                    program_.Add(s);
                }
            }

            in1.Dispose();
            return batch;
        }

        public void Run(bool batch = false)
        {
            
        }

        private static bool OpenText(out StreamReader inStreamReader, string filename)
        {
            inStreamReader = StreamReader.Null;
            try
            {
                inStreamReader = File.OpenText(filename);
                return true;
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine("File \"" + e.FileName + "\" not found.");
            }
            catch (UnauthorizedAccessException e)
            {
                Console.WriteLine("Unauthorized Access. ");
            }

            return false;
        } 
    }
}
