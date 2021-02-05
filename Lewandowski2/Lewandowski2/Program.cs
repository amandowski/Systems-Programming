using System;
using System.IO;

namespace Lewandowski2
{
    class Program
    {
        static void Main(string[] args)
        {
            SymbolTable readText = new SymbolTable();   //declare readText for the search in search table
            readText.ReadAndSeparateFile();     //read in the file and separate the file
            Console.WriteLine("Press any key to enter search file.");   
            Console.ReadLine();     //any key
            string searchPath = ReadInput(args);  //declare searchPath for the directory path
            readText.SearchFile(searchPath);    //compares the searchPath with readText
            Console.WriteLine("");      //new line
            ExpressionProcessing readText1 = new ExpressionProcessing(readText);    //declare readText1 for expression processing
            Console.WriteLine("Press any key to enter expression file.");
            Console.ReadLine();     //any key
            string expressionPath = ReadInput(args);  //declare expressionPath for the directory path
            readText1.Processing(expressionPath);   //processes the expressionPath
        }

       /****************************************************************
        *** FUNCTION: ReadInput                                      ***             
        ****************************************************************
        *** DESCRIPTION: Reads inputed file from command line        ***
        *** INPUT ARGS: string[] argu                                ***
        *** OUTPUT ARGS: NONE                                        ***
        *** IN/OUT ARGS: NONE                                        ***
        *** RETURN: string searchPath                                ***
        ****************************************************************/
        static string ReadInput(string[] argu)
        {
            string input;
            if (argu.Length == 1)
                input = argu[0];
            else
            {
                Console.Write("Enter file name: ");
                input = Console.ReadLine();
                Console.WriteLine();
            }
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), ("..\\..\\" + input));  //get the path for the input
            if (File.Exists(filePath))
                return filePath;
            else
            {
                string[] args = { };
                Console.WriteLine("That file does not exist.");
                return ReadInput(args);
            }
        }
    }
}

