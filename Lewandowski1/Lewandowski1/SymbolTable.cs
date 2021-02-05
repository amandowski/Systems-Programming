using System;
using System.IO;
using System.Linq;


namespace Lewandowski2
{
    class SymbolTable
    {
        private readonly BinSearchTree searchTree = new BinSearchTree();

        /***********************************************************************
         * FUNCTION:    ValidateSymbol
         * *********************************************************************
         * DESCRIPTION: Checking to see if the symbol is correct
         * INPUT ARGS:  string line
         * OUTPUT ARGS: None
         * IN/OUT ARGS: None
         * RETURN:      bool
         **********************************************************************/
        private bool SymbolCheck(string line)
        {
            string sString = line;
            int sLength = sString.Length;
            bool error = false;
            // Symbol Length cannot exceed length of 12
            if (sLength <= 12)
            {
                // Symbol must start with char
                if (char.IsLetter(sString[0]))
                {
                    foreach (char character in sString.Substring(1))
                    {
                        // Symbol must be either char or digit
                        if (!(char.IsLetterOrDigit(character)))
                        {
                            error = true;
                        }
                    }
                    // Don't want to repeat the same error for reoccuring invalid characters
                    if (error == true)
                    {
                        Console.WriteLine("{0} -> ERROR: Symbol contains invalid character(s)", CutLength(sString));
                        return false;
                    }
                }
                else
                {
                    Console.WriteLine("{0} -> ERROR: Symbol must start with letter", CutLength(sString));
                    return false;
                }
            }
            else
            {
                Console.WriteLine("{0} -> ERROR: Symbol exceeds max characters", CutLength(sString));
                return false;
            }
            return true;
        }
        /***********************************************************************
         * FUNCTION:    ValueCheck
         * *********************************************************************
         * DESCRIPTION: Checking to see if the value is correct
         * INPUT ARGS:  string symbol, string value
         * OUTPUT ARGS: None
         * IN/OUT ARGS: out int newValue
         * RETURN:      bool
         **********************************************************************/
        private bool ValueCheck(string symbol, string value, out int newValue)
        {
            if (value[0] == '+')
            {
                Console.WriteLine("{0} -> ERROR: Contains invalid value: {1}", CutLength(symbol), value);
                newValue = 0;
                return false;
            }
            if (int.TryParse(value, out newValue))
                return true;
            else
            {
                Console.WriteLine("{0} -> ERROR: Contains invalid value: {1}", CutLength(symbol), value);
                return false;
            }
        }
        /***********************************************************************
         * FUNCTION:    RFleg
         * *********************************************************************
         * DESCRIPTION: Checking to see if the RFlag is correct
         * INPUT ARGS:  string symbol, string Rflag
         * OUTPUT ARGS: None
         * IN/OUT ARGS: out bool RBool
         * RETURN:      bool
         **********************************************************************/
        private bool RFleg(string symbol, string RFlag, out bool RBool)
        {
            string flag = RFlag;
            if (flag == "true" || flag == "1")
            {
                RBool = true;
                return true;
            }
            else if (flag == "false" || flag == "0")
            {
                RBool = false;
                return true;
            }
            // Case for anything besides the values above including capital letters.
            else
            {
                Console.WriteLine("{0} -> ERROR: RFlag invalid: {1}", CutLength(symbol), flag);
                RBool = false;
                return false;
            }
        }
        /***********************************************************************
         * FUNCTION:    CutLength
         * *********************************************************************
         * DESCRIPTION: Truncate symbol to a given size
         * INPUT ARGS:  string symbol
         * OUTPUT ARGS: None
         * IN/OUT ARGS: None
         * RETURN:      symbol
         **********************************************************************/
        public static string CutLength(string symbol)
        {
            if (symbol.Length > 6)
                return symbol.Substring(0, 6);
            else
                return symbol;
        }
        /***********************************************************************
         * FUNCTION:    SearchFile
         * *********************************************************************
         * DESCRIPTION: Finds matches between the command line argument file and
         *              SYMBOLS.DAT file or ReadFile file then prints matches
         * INPUT ARGS:  string searchPath
         * OUTPUT ARGS: None
         * IN/OUT ARGS: None
         * RETURN:      void
         **********************************************************************/
        public void SearchFile(string searchPath)
        {
            string sym;
            string[] lines = File.ReadAllLines(searchPath);
            foreach (var line in lines)
            {
                sym = line.Trim();
                if (sym.Length > 6)
                    sym = sym.Substring(0, 6);
                if (searchTree.Search(sym) != null)
                    Console.WriteLine("{0} -> Success: Symbol found", sym);
                else if (searchTree.Search(sym) == null)
                    Console.WriteLine("{0} -> ERROR: Symbol not found", sym);
            }
        }
        /***********************************************************************
         * FUNCTION:    ReadFile
         * *********************************************************************
         * DESCRIPTION: Reads from the SYMBOLS.DAT file or user provided one and
         *              inserts valid symbols into the symbol table.
         * INPUT ARGS:  None
         * OUTPUT ARGS: None
         * IN/OUT ARGS: None
         * RETURN:      void
         **********************************************************************/
        public void ReadFile()
        {
            char[] splitCharacters = { ':', ' ' };
            try
            {
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), "\\..\\..\\..\\SYMBOLS.DAT");
                // User Enter File Name - SYMBOLS.DAT not found
                while (!File.Exists(filePath))
                {
                    Console.WriteLine("ERROR: SYMBOLS.DAT not found");
                    Console.WriteLine("File name: ");
                    var fileName = Console.ReadLine();
                    filePath = Path.Combine(Directory.GetCurrentDirectory(), fileName + (fileName.Contains(".dat") ? "" : ".dat"));
                    Console.Write("");
                }
                // Read File - Complete Validations
                if (File.Exists(filePath))
                {
                    Console.WriteLine("Error Log: \n");
                    string[] datlines = File.ReadAllLines(filePath);
                    // Read each line
                    foreach (string line in datlines)
                    {
                        string temp = line.Trim();
                        string[] lineSymbol = temp.Split(splitCharacters);
                        lineSymbol = lineSymbol.Where(x => !string.IsNullOrEmpty(x)).ToArray();

                        if (SymbolCheck(lineSymbol[0]) && RFleg(lineSymbol[0], lineSymbol[1], out bool RBool) && ValueCheck(lineSymbol[0], lineSymbol[2], out int value))
                        {
                            var sym = new Symbol()
                            {
                                Element = lineSymbol[0],
                                RFlag = RBool,
                                Value = value,
                                IFlag = true,
                                MFlag = false
                            };
                            this.searchTree.Insert(sym);
                            Console.WriteLine("Valid - insert {0} and all attributes into symbol table", CutLength(lineSymbol[0]));
                        }
                    }
                }
                Console.WriteLine("\n");
                Console.Write("Press any key to continue");
                Console.ReadKey();
                Console.Clear();
                this.searchTree.View();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}

