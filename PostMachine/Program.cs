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
        private List<string> program_; // stores programs (instructions)
        private Queue<char> tape_;     // the tape input strings
        private char internalState_;           // post machine's state 

        private enum States
        {
            Start = 'S',
            Halt = 'H'
        };
        public PostMachine()
        {
            Console.WriteLine("PostMachine initialized.");
            program_ = new List<string>();
            tape_ = new Queue<char>();
        }

        /// <summary>
        /// Load the input for the post machine from a file (.pp - "post program") into List<string> program_.
        /// The file represents instructions (one per line) 
        /// </summary>
        /// <param name="batch"></param>
        /// <returns> bool </returns>
        public bool Load(bool batch = false)
        {
            StreamReader fstreamReader; 
            
            // Open input file 
            do
            {
                Console.Write(" Name of instruction file: ");
                string filename = Console.ReadLine();
                if (!OpenText(out fstreamReader, filename))
                    Console.WriteLine("** Cannot open file'" + filename + "'. Try again");
            } while (fstreamReader == StreamReader.Null);
            
            // if (batch) Console.WriteLine(filename);

            program_.Clear();
            string s;
            
            while ( (s = fstreamReader.ReadLine()) != null ) 
            {
                int pos = s.IndexOf('*', 0); // returns -1 if value is not found
                if (pos < 0) pos = s.Length; 
                if (pos > 0)// (pos < s.Length - 1 && pos > 0) //  not a comment (> 0) 
                {
                    StringBuilder temp = new StringBuilder(" ", pos); 
                    for (int j = 0; j < pos; ++j)
                        temp[j] = s[j];
                    Console.WriteLine(s);
                    program_.Add(temp.ToString());
                }
                else if (pos == 0 && s.IndexOf('\n', 0) == 1)  program_.Add(s);
                else if (pos == s.Length)  program_.Add(s);
            }
            fstreamReader.Dispose();
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="batch"></param>
        public void Run(bool batch = false)
        {
            
            bool finished = false, halt = false, crash = false, maxitersReached = false, match = false;
            internalState_ = 'S';

            while (true)
            {
                tape_.Clear();
                Console.Write("  Input string (* to end): ");
                // if(its == 0)
                char input = (char) Console.Read(); // Console.ReadKey().KeyChar;
                // if(batch) put
                if (input == '*') // end of program OR if at the start of a line, a comment.
                {   
                    // Console.WriteLine("{0}", Console.OpenStandardInput());
                    // Utility.ClearBuffer(Console.OpenStandardInput());  //Console.OpenStandardInput());
                    // if(batch) Console.Write('\n');
                }
                while (input != '\n')
                {
                    tape_.Enqueue(input);
                    input = (char) Console.Read(); // Console.ReadKey().KeyChar;
                    // if(batch) Console.Write('\n');
                }
                // if(batch) Console.Write('\n');
                tape_.Enqueue('#'); // # separates input strings

                // Run post machine
                long iteration = 0;
                do
                {
                    match = false;
                    foreach (var instruction in program_)
                    {
                        char currentState = instruction.ElementAt(0);
                        char ch = instruction.ElementAt(1);
                        char newState = instruction.ElementAt(2);
                        if (currentState.Equals(internalState_) && tape_.Count != 0 && ch == tape_.First())
                        {
                            Console.WriteLine("program_ first char of instruction matches first tape_ element: {0}", tape_.First());
                            //if (tape_.Count != 0)
                            //{
                                //char tapeNext = tape_.First();
                                //if (instruction.ElementAt(1) == tape_.First())//tapeNext)
                                //{
                                    match = true;
                                    tape_.Dequeue();
                                    internalState_ = instruction.ElementAt(2); // new state
                                    for (int j = 3; j < instruction.Length; ++j) // 
                                    {
                                        tape_.Enqueue(instruction.ElementAt(j));
                                    }
                                //}
                            //}
                        }
                    } // foreach

                    internalState_ = 'H';
                    ++iteration;
                    halt = (internalState_ == 'H');
                    crash = !match;
                    maxitersReached = (iteration == maxIterations);
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
                    Console.WriteLine("Last state: " + internalState_);
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
