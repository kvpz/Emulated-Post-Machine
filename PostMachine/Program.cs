/*
 * http://logica.ugent.be/liesbeth/postsmachine.pdf
 * http://www.win.tue.nl/~wstomv/misc/PostMachineUspensky.pdf
 */
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
        private static int maxIterations = 10000;
        private List<string> program_; // stores programs
        private Queue<char> tape_;     // the tape
        private char state_;           // post machine's state 

        public PostMachine()
        {
            Console.WriteLine("PostMachine initialized.");
            program_ = new List<string>();
            tape_ = new Queue<char>();
        }

        /// <summary>
        /// Load the input for the post machine into List<string> program_
        /// </summary>
        /// <param name="batch"></param>
        /// <returns></returns>
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
            
            // if (batch) Console.WriteLine(filename);

            program_.Clear();
            string s;
            int count = 0;
            while ( (s = in1.ReadLine()) != null ) 
            {
                int pos = s.IndexOf('*', 0); // returns -1 if value is not found
                if (pos == -1) pos = s.Length; 
                if (pos < s.Length - 1 && pos > 0)
                {
                    StringBuilder temp = new StringBuilder(' ', pos);
                    for (int j = 0; j < pos; ++j)
                        temp[j] = s[j];
                    Console.WriteLine(s);
                    program_.Add(temp.ToString());
                }
                else if (pos == 0 && s.IndexOf('\n', 0) == 1)  program_.Add(s);
                else if (pos == s.Length)  program_.Add(s);
                
            }
            in1.Dispose();
            return true;
        }

        public void Run(bool batch = false)
        {
            char ch;
            bool finished, halt, crash, maxitersReached, match;
            long its = 0;

            while (true)
            {
                tape_.Clear();
                Console.Write("  Input string (* to end): ");
                // if(its == 0)
                ch = (char) Console.Read(); // Console.ReadKey().KeyChar;
                // if(batch) put
                if (ch == '*')
                {   
                    // Console.WriteLine("{0}", Console.OpenStandardInput());
                    // Utility.ClearBuffer(Console.OpenStandardInput());  //Console.OpenStandardInput());
                    // if(batch) Console.Write('\n');
                }
                while (ch != '\n')
                {
                    tape_.Enqueue(ch);
                    ch = (char) Console.Read(); // Console.ReadKey().KeyChar;
                    // if(batch) Console.Write('\n');
                }
                // if(batch) Console.Write('\n');
                tape_.Enqueue('#');

                // Run post machine
                state_ = 'S';
                finished = halt = crash = maxitersReached = false;
                its = 0;
                do
                {
                    match = false;
                    foreach (var i in program_)
                    {
                        string s = i;
                        if (s.ElementAt(0).Equals(state_))
                        {
                            if (tape_.Count != 0)
                            {
                                char tapeNext = tape_.First();
                                if (s.ElementAt(1) == tapeNext)
                                {
                                    match = true;
                                    tape_.Dequeue();
                                    state_ = s.ElementAt(2);
                                    for (int j = 3; j < s.Length; ++j)
                                    {
                                        tape_.Enqueue(s.ElementAt(j));
                                    }
                                }
                            }
                        }
                    } // foreach

                    ++its;
                    halt = (state_ == 'H');
                    crash = !match;
                    maxitersReached = (its == maxIterations);
                    finished = halt || crash || maxitersReached;

                } while (!finished);

                if (halt)
                {
                    Console.WriteLine("String accepted.");
                    Console.WriteLine("\n\n");
                    Console.WriteLine("String at halt: ");
                    foreach (var t in tape_)
                        Console.Write("{0} ", t);

                }
                else if (crash)
                {
                    Console.WriteLine("String rejected.");
                    Console.WriteLine("Last state: " + state_);
                    Console.WriteLine("Tape contents at rejection: ");
                    foreach (var t in tape_)
                        Console.Write("{0} ", t);
                }
                else if (maxitersReached)
                {
                    Console.WriteLine("Machine stopped after " + maxIterations + " iterations.");
                    Console.WriteLine("Tape contents when stopped");
                }
                else
                {
                    Console.WriteLine("PostMachine ERROR termination");
                }
            } // while (true)
        } // Run

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
            catch (ArgumentException e)
            {
                Console.WriteLine("ArgumentException: An argument is required");
            }

            return false;
        } 
    }
}
