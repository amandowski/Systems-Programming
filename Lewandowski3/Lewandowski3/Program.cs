/**************************************************************************
 *** Name: Amanda Lewandowski                                           ***
 *** Due Date: October 30th, 2019                                       ***
 *** Assignment: 3 Pass 1                                               ***
 *** Class: CSc 354                                                     ***
 *** Instructor: Gamradt                                                ***
 **************************************************************************
 *** Description: Program file                                          ***
 **************************************************************************/
using System;
using System.IO;

namespace Lewandowski3
{
    class Program
    {
        static void Main(string[] args)
        {
            string fileName;
            if (args.Length == 0)
            {
                Console.Write("Please enter the name for the SIC file: ");
                fileName = Console.ReadLine();
            }
            else
                fileName = args[0];
            while (!File.Exists(Path.Combine(Directory.GetCurrentDirectory(), ($@"{Environment.CurrentDirectory}\\..\\..\\" + fileName))))
            {
                Console.Write("||Error|| SICXE file path does not exist. Please enter a valid file name: \n");
                fileName = Console.ReadLine();
            }
            Console.Clear();
            OpcodeTable opcodes = new OpcodeTable(File.ReadAllLines(Path.Combine(Directory.GetCurrentDirectory(), ("..\\..\\" + "OPCODES.DAT"))));
            PassOne readFile = new PassOne();
            //string searchPath = ReadInput(args);
            readFile.ProcessFile(fileName, opcodes);
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();

            /*SymbolTable readText = new SymbolTable();   //declare readText for the search in search table
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
            readText1.Processing(expressionPath);   //processes the expressionPath*/
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
                Console.Write("Enter SICXE file name: ");
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

