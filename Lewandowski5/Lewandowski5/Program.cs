/**************************************************************************
 *** Name: Amanda Lewandowski                                           ***
 *** Due Date: December 11, 2019                                        ***
 *** Assignment: 5 Linking Loader                                       ***
 *** Class: CSc 354                                                     ***
 *** Instructor: Gamradt                                                ***
 **************************************************************************
 *** Description: Program file                                          ***
 **************************************************************************/
using System;
using System.Collections.Generic;
using System.IO;

namespace Lewandowski5
{
    public class Program
    {
        public static Dictionary<int, string> memMap = new Dictionary<int, string>();
        public static int exAdd = 0;
        public static int length = 0;
        public static int startMemory = int.Parse("02170", System.Globalization.NumberStyles.HexNumber);
        public static int contSectAddress = startMemory;
        public static void Main(string[] args)
        {
            string file = "memory.txt";
            string memPath = Path.Combine(Directory.GetCurrentDirectory(), ($@"{Environment.CurrentDirectory}\\..\\..\\" + file));

            StreamWriter objFile = new StreamWriter(memPath);
            List<string[]> prog = new List<string[]>();
            foreach (var vari in args)
            {   prog.Add(File.ReadAllLines(Path.Combine(Directory.GetCurrentDirectory(), ($@"{Environment.CurrentDirectory}\\..\\..\\" + vari))));  }

            var symTable = LinkingLoader.PassOne(prog.ToArray());
            Console.WriteLine("{0,-6} {1,-6} {2,-9} {3,-8} {4,-8} {5,-8}", "CSECT", "SYMBOL", "LDADDR", "CSADDR", "ADDR", "LENGTH");
            foreach (var contsect in symTable.ContSectList)
            {
                Console.Write("{0,-8} {1,-8} {2,-8}", contsect.Key, string.Empty, string.Empty);
                foreach (var value in contsect.Value)
                {
                    Console.Write("{0,-6} {1,-6}", value.ToString("X"), string.Empty);
                    Console.Write("{0,-6}", "      ");
                }
                Console.Write("{0,-6}", " ");
                Console.WriteLine();
            }
            foreach (var symbol in symTable.SymList)
            {
                Console.Write("{0,-7} {1,-7}", string.Empty, symbol.Key);
                foreach (var value in symbol.Value)
                {   Console.Write("{0,-10} {1,-8}", value.ToString("X").PadLeft(6, '0'), string.Empty); }
                Console.WriteLine();
            }
            _ = (int)Math.Ceiling((double)length / 16);

            Console.WriteLine("\nMemory Layout : ");
            Console.WriteLine("      0  1  2  3  4  5  6  7  8  9  A  B  C  D  E  F");
            objFile.WriteLine("\nMemory Layout : ");
            objFile.WriteLine("      0  1  2  3  4  5  6  7  8  9  A  B  C  D  E  F");

            var memoryMap = LinkingLoader.PassTwo(prog.ToArray(), symTable);
            int colCount = 0;
            foreach (var item in memoryMap)
            {
                if (colCount == 0 || colCount > 16)
                {
                    colCount++;
                    Console.Write(item.Key.ToString("X") + " ");
                    Console.Write(item.Value + " ");
                    objFile.Write(item.Key.ToString("X") + " ");
                    objFile.Write(item.Value + " ");
                }
                else if (colCount > 0 && colCount < 16)
                {
                    Console.Write(item.Value + " ");
                    objFile.Write(item.Value + " ");
                    colCount++;
                    if (colCount == 16)
                    {
                        Console.WriteLine("");
                        objFile.WriteLine("");
                        colCount = 0;
                    }
                }
            }
            Console.WriteLine("\n");
            Console.WriteLine("Execution begins at Address: {0}", Program.startMemory.ToString("X"));
            Console.WriteLine("Execution Address from Record: {0}", Program.exAdd.ToString("X"));
            objFile.WriteLine("\n");
            objFile.WriteLine("Execution begins at Address: {0}", Program.startMemory.ToString("X"));
            //objFile.WriteLine("Execution Address from Record: {0}", Program.executionAddress.ToString("X"));
            objFile.Close();
            Console.WriteLine("\nEnter any key to exit: ");
            Console.Read();
        }
    }
}
