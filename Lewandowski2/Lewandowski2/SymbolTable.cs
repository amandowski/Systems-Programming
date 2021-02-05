using System;
using System.IO;
using System.Linq;


namespace Lewandowski2
{
    class SymbolTable
    {
        private readonly BinSearchTree tree = new BinSearchTree();

       /**********************************************************************
        *** FUNCTION: SymbolCheck                                          ***
        **********************************************************************
        *** DESCRIPTION: check to see if correct symbol                    ***
        *** INPUT ARGS: string line                                        ***
        *** OUTPUT ARGS: NONE                                              ***
        *** IN/OUT ARGS: NONE                                              ***
        *** RETURN: bool                                                   ***
        **********************************************************************/
        private bool SymbolCheck(string line)
        {
            string sString = line;
            int sLength = sString.Length;
            bool error = false;
            
            if (sLength <= 12) //symbol Length cannot exceed length of 12
            {
                if (char.IsLetter(sString[0])) //symbol must start with char
                {
                    foreach (char character in sString.Substring(1))
                    {
                        if (!(char.IsLetterOrDigit(character))) //symbol must be either char or digit
                            error = true;
                    }

                    if (error == true)
                    {
                        Console.WriteLine("|ERROR| Symbol: {0} contains invalid character(s)", CutLength(sString));
                        return false;
                    }
                }
                else
                {
                    Console.WriteLine("|ERROR| Symbol: {0} must start with letter", CutLength(sString));
                    return false;
                }
            }
            else
            {
                Console.WriteLine("|ERROR| Symbol: {0} exceeds max characters", CutLength(sString));
                return false;
            }
            return true;
        }

       /**********************************************************************
        *** FUNCTION: ValueCheck                                           ***
        **********************************************************************
        *** DESCRIPTION: check to see if correct value                     ***
        *** INPUT ARGS: string symbol, string value                        ***
        *** OUTPUT ARGS: NONE                                              ***
        *** IN/OUT ARGS: out int newVal                                    ***
        *** RETURN: bool                                                   ***
        **********************************************************************/
        private bool ValueCheck(string symbol, string value, out int newVal)
        {
            if (value[0] == '+')
            {
                Console.WriteLine("|ERROR| Symbol: {0} Contains invalid value: {1}", CutLength(symbol), value);
                newVal = 0;
                return false;
            }

            if (int.TryParse(value, out newVal))
                return true;
            else
            {
                Console.WriteLine("|ERROR| Symbol: {0} Contains invalid value: {1}", CutLength(symbol), value);
                return false;
            }
        }

       /**********************************************************************
        *** FUNCTION: RFleg                                                ***
        **********************************************************************
        *** DESCRIPTION: check for RFlag as true or false                  ***
        *** INPUT ARGS: string symbol, string Rflag                        ***
        *** OUTPUT ARGS: NONE                                              ***
        *** IN/OUT ARGS: out bool RBool                                    ***
        *** RETURN: bool                                                   ***
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
            else    //if capital letters
            {
                Console.WriteLine("|ERROR| Symbol: {0} RFlag invalid: {1}", CutLength(symbol), flag);
                RBool = false;
                return false;
            }
        }

       /**********************************************************************
        *** FUNCTION: CutLength                                            ***
        **********************************************************************
        *** DESCRIPTION: truncate symbol to 6 chars                        ***
        *** INPUT ARGS: string cut                                         ***
        *** OUTPUT ARGS: NONE                                              ***
        *** IN/OUT ARGS: NONE                                              ***
        *** RETURN: cut                                                    ***
        **********************************************************************/
        public static string CutLength(string cut)
        {
            if (cut.Length > 6)         //string must be shorter th
                return cut.Substring(0, 6);
            else                        //if already shorter than 6
                return cut;
        }

       /**********************************************************************
        *** FUNCTION: SearchFile                                           ***
        **********************************************************************
        *** DESCRIPTION: searches for matches between the two files        ***
        *** INPUT ARGS: string pathSearched                                ***
        *** OUTPUT ARGS: NONE                                              ***
        *** IN/OUT ARGS: NONE                                              ***
        *** RETURN: NONE                                                   ***
        **********************************************************************/
        public void SearchFile(string pathSearched)
        {
            string sym;
            string[] lines = File.ReadAllLines(pathSearched);
            foreach (var line in lines)
            {
                sym = line.Trim();

                if (sym.Length > 6)                 //if it is greater than 6 trim it
                    sym = sym.Substring(0, 6);
                if (tree.Search(sym) != null) //if it is searched are returned not null then its found
                    Console.WriteLine("SUCCESS Symbol: {0} found", sym);
                else if (tree.Search(sym) == null)    //if searched and returned null then not found
                    Console.WriteLine("|ERROR| Symbol: {0} not found", sym);
            }
        }
       /**********************************************************************
        *** FUNCTION: Search                                               ***
        **********************************************************************
        *** DESCRIPTION: search with returning symbol                      ***
        *** INPUT ARGS: NONE                                               ***
        *** OUTPUT ARGS: NONE                                              ***
        *** IN/OUT ARGS: NONE                                              ***
        *** RETURN: NONE                                                   ***
        **********************************************************************/
        public Symbol Search(string symSearch)
        {
            if (symSearch.Length > 6)
                symSearch = symSearch.Substring(0, 6);
            var good = tree.Search(symSearch);

            if (good != null)
                return good.Element;
            else
                throw new NullReferenceException("|ERROR| Symbol not found.");
        }

       /**********************************************************************
        *** FUNCTION: ReadAndSeparateFile                                  ***
        **********************************************************************
        *** DESCRIPTION: reads from SYMBOLS.DAT or provided and inserts    ***
        ***                the symbols into the table                      ***
        *** INPUT ARGS: NONE                                               ***
        *** OUTPUT ARGS: NONE                                              ***
        *** IN/OUT ARGS: NONE                                              ***
        *** RETURN: NONE                                                   ***
        **********************************************************************/
        public void ReadAndSeparateFile()
        {
            char[] separateChar = { ':', ' ' };
            try
            {
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), "..\\..\\SYMBOLS.DAT");
                //string filePath = $@"{Environment.CurrentDirectory}\\..\\..\\..\\SYMBOLS.DAT";
                while (!File.Exists(filePath)) //user Enter File Name - SYMBOLS.DAT not found
                {
                    Console.WriteLine("|ERROR| SYMBOLS.DAT not found");
                    Console.WriteLine("File name: ");
                    var fileName = Console.ReadLine();
                    filePath = Path.Combine(Directory.GetCurrentDirectory(), fileName + (fileName.Contains(".dat") ? "" : ".dat"));
                    Console.Write("");
                }

                if (File.Exists(filePath)) //read file successfully
                {
                    Console.WriteLine("Invalid Symbols:");
                    string[] fileLines = File.ReadAllLines(filePath);
                    foreach (string line in fileLines)   //read each line
                    {
                        string temp = line.Trim();
                        string[] lineSymbol = temp.Split(separateChar);
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
                            this.tree.Insert(sym);
                            //Console.WriteLine("Valid - insert {0} and all attributes into symbol table", CutLength(lineSymbol[0]));
                        }
                    }
                }
                Console.WriteLine("All valid symbols have been inserted into the symbol table via SYMBOLS.DAT");
                Console.WriteLine("\n");
                Console.Write("Press any key to continue");
                Console.ReadKey();
                Console.Clear();
                this.tree.View();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}

