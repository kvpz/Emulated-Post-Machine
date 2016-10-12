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
    public class Program
    {
        public static void Main(string[] args)
        {
            bool batch = false;
            if (args.Length > 1) batch = true;
            PostMachine m = new PostMachine();
            char response;

            do
            {
                bool loaded = false;
                do loaded = m.Load(batch); while (!loaded);
                m.Run(batch);
                Console.WriteLine("\n** Load another program?");
                response = (char) Console.Read();
                Console.ReadLine();
                if (batch) Console.WriteLine(response);
            } while (response == 'y' || response == 'Y');
        }
    }

    class PostMachine
    {
        private static int maxIterations = 10000;
        private readonly List<string> program_; // stores programs (instructions)
        private Queue<char> tape_; // the tape input strings
        private char internalState_; // post machine's state 

        public PostMachine()
        {
            program_ = new List<string>();
            tape_ = new Queue<char>();
            internalState_ = 'S';
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
                Console.Write(" Name of post machine instruction file: ");
                string filename = Console.ReadLine();
                if (!OpenText(out fstreamReader, filename))
                    Console.WriteLine("** Cannot open file'" + filename + "'. Try again");
            } while (fstreamReader == StreamReader.Null);

            // if (batch) Console.WriteLine(filename);

            program_.Clear();
            string s;

            // Read instructions into List<string> program_ until '*'(which is not added to List)
            while ((s = fstreamReader.ReadLine()) != null)
            {
                if (s.IndexOf('*', 0) < 0 && s.Length > 0) //pos > 0) // if not a comment (> 0) 
                {
                    program_.Add(s);
                }
            }
            fstreamReader.Dispose();
            return true;
        }

        /// <summary>
        /// Run user inputs against the post program.
        /// </summary>
        /// <param name="batch"></param>
        public void Run(bool batch = false)
        {
            while (true) // until first '*' appears
            {
                internalState_ = 'S';
                bool finished = false, halt = false, crash = false, maxitersReached = false;
                tape_.Clear();
                Console.Write("  Input string (* to end): ");
                // if(its == 0)
                // if(batch) put
                //TextReader istream = Console.In;
                var input = Console.ReadLine().ToCharArray();
                try
                {   
                    if (input.ElementAt(0) == '*')
                    {
                        // ClearBuffer
                        // if(batch) Console.Write('\n');
                        return;
                    }
                }
                catch 
                {
                    continue;
                }
                
                tape_ = new Queue<char>(input);
  
                // if(batch) Console.Write('\n');
                tape_.Enqueue('#'); // marks end of input string

                // Run post machine
                long iteration = 0;
                do
                {
                    bool match = false;
                    foreach (var instruction in program_)
                    {
                        char currentState = instruction.ElementAt(0);
                        char ch = instruction.ElementAt(1);
                        char newState = instruction.ElementAt(2);
                        // if first instruction char equals internal state && second char equals the front of tape
                        if (currentState.Equals(internalState_) && tape_.Count != 0 && ch == tape_.First())
                        {
                            match = true;
                            tape_.Dequeue();
                            internalState_ = newState; // new state
                            for (int j = 3; j < instruction.Length; ++j) // word portion of instruction to be appended to tape
                            {
                                tape_.Enqueue(instruction[j]);
                            }
                        }
                    } // foreach

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
                    Console.Write("String(tape) at halt: ");
                    foreach (var t in tape_)
                        Console.Write("{0}", t);

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
