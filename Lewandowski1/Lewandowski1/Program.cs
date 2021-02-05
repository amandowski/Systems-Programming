using System;
using System.IO;

namespace Lewandowski2
{
    class Program
    {
        static void Main(string[] args)
        {
            SymbolTable readtxt = new SymbolTable();
            readtxt.ReadFile();
            string searchPath = DecodeArguments(args);
            readtxt.SearchFile(searchPath);
            Console.Read();
        }

        /****************************************************************
         *** FUNCTION:    DecodeArguments                               *               
         ****************************************************************
         *** DESCRIPTION: Setting command line argument if not found    *
         *** INPUT ARGS:  string[] arguments                            *
         *** OUTPUT ARGS: None                                          *
         *** IN/OUT ARGS: None                                          *
         *** RETURN:      string searchPath                             *
         ****************************************************************/
        static string DecodeArguments(string[] arguments)
        {
            string input;
            if (arguments.Length == 1)
            {
                input = arguments[0];
            }
            else
            {
                Console.Write("Enter search file: ");
                input = Console.ReadLine();
                Console.WriteLine();
            }
            string searchPath = Path.Combine(Directory.GetCurrentDirectory(), ("..\\..\\..\\" + input));
            if (File.Exists(searchPath))
            {
                Console.WriteLine("Reading from command line arguments...\n");
                return searchPath;
            }
            else
            {
                string[] args = { };
                Console.WriteLine("Search file given does not exist");
                return DecodeArguments(args);
            }
        }
    }
}




