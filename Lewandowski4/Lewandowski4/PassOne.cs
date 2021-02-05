/**************************************************************************
 *** Name: Amanda Lewandowski                                           ***
 *** Due Date: November 20th, 2019                                      ***
 *** Assignment: 4 Pass 2                                               ***
 *** Class: CSc 354                                                     ***
 *** Instructor: Gamradt                                                ***
 **************************************************************************
 *** Description: Pass One file                                         ***
 **************************************************************************/
using System;
using System.Collections.Generic;
using System.IO;

namespace Lewandowski4
{
    public struct Opcode
    {
        public string mnem;
        public string code;
        public int form;
    }
    public class OpcodeCompare : Comparer<Opcode>
    {
        public override int Compare(Opcode x, Opcode y)
        {   return string.Compare(x.mnem, y.mnem);  }
    }

    public struct Literal
    {
        public string name;
        public string value;
        public int length;
        public string address;

        public override string ToString()
        {
            return string.Format("{0,-12} {1,-18} {2,-10} {3,-10}", name, value, length, address);
        }
    }
    public struct Symbol
    {
        public string element;
        public int value;
        public bool rFlag;
        public bool iFlag;
        public bool mFlag;

        public override string ToString()
        {   return string.Format("{0, -10} {1, -10} {2, -15} {3, -10} {4, -10}", element, value.ToString("X"), rFlag, iFlag, mFlag);    }
    }

    public class SymbolCompare : IComparer<Symbol>
    {
        public int Compare(Symbol x, Symbol y)
        {   return string.Compare(x.element, y.element);    }
    }

    class PassOne
    {
        private int mnemIndex = 0, codeIndex = 1, formIndex = 2;
        private LinkedList<Literal> literalTable;
        public string pLength { get; private set; }
        public string StartAdd { get; private set; }
        public string pName { get; private set; }
        public Opcode[] Opcodes { get; private set; }
        public BinSearchTree<Symbol> SymbolTable { get; }
        public LinkedList<Literal> LiteralTable => literalTable;
        
        /********************************************************************
        *** FUNCTION SearchTable                                          ***
        *********************************************************************
        *** DESCRIPTION: searches the table for the opcode                ***
        *** INPUT ARGS: string mnemonic                                   ***
        *** OUTPUT ARGS: Opcode opcode                                    ***
        *** IN/OUT ARGS: NONE                                             ***
        *** RETURN: bool                                                  ***
        *********************************************************************/
        public bool SearchTable(string mnemonic, out Opcode opcode)
        {
            int foundNum;
            if (mnemonic[0] == '+')
            {
                mnemonic = mnemonic.Substring(1, mnemonic.Length - 1);
                foundNum = Array.BinarySearch(Opcodes, new Opcode { mnem = mnemonic }, new OpcodeCompare());
                if (foundNum >= 0)
                {
                    opcode = Opcodes[foundNum];
                    opcode.form = 4;
                    return true;
                }
                else
                {
                    opcode = new Opcode();
                    return false;
                }
            }
            else
            {
                foundNum = Array.BinarySearch(Opcodes, new Opcode { mnem = mnemonic }, new OpcodeCompare());
                if (foundNum >= 0)
                {
                    opcode = Opcodes[foundNum];
                    return true;
                }
                else
                {
                    opcode = new Opcode();
                    return false;
                }
            }
        }

        /*********************************************************************
        *** FUNCTION CheckOpcode                                           ***
        **********************************************************************
        *** DESCRIPTION: checks for opcode for pass one                    ***
        *** INPUT ARGS: string [] lines                                    ***
        *** OUTPUT ARGS: NONE                                              ***
        *** IN/OUT ARGS: NONE                                              ***
        *** RETURN: void                                                   ***
        **********************************************************************/
        private void CheckOpcode(string[] lines)
        {
            List<Opcode> opcodes = new List<Opcode>();
            char[] StrSeparator = { ' ', '\t' };

            foreach (var line in lines)
            {
                int i = 0;
                var SplitOneLine = line.ToUpper().Split(StrSeparator, StringSplitOptions.RemoveEmptyEntries);
                for (i = 0; i < SplitOneLine.Length; i++)
                {
                    if (i == 0)
                        mnemIndex = i;
                    if (i == 1)
                        formIndex = i;
                    if (i == 2)
                        codeIndex = i;
                }
                opcodes.Add
                    (new Opcode()
                    {
                        mnem = SplitOneLine[mnemIndex],
                        form = int.Parse(SplitOneLine[formIndex]),
                        code = SplitOneLine[codeIndex]
                    });
            }
            opcodes.Sort(new OpcodeCompare());
            this.Opcodes = opcodes.ToArray();
        }

        /********************************************************************
        *** FUNCTION PassOne                                              ***
        *********************************************************************
        *** DESCRIPTION: default constructor                              ***
        *** INPUT ARGS: string [] codes                                   ***
        *** OUTPUT ARGS: NONE                                             ***
        *** IN/OUT ARGS: NONE                                             ***
        *** RETURN: NONE                                                  ***
        ********************************************************************/
        public PassOne(string[] codes)
        {
            SymbolTable = new BinSearchTree<Symbol>(new SymbolCompare());
            literalTable = new LinkedList<Literal>();
            CheckOpcode(codes);
        }

        /******************************************************************* 
        *** FUNCTION: ToFile                                             ***
        ********************************************************************  
        *** DESCRIPTION: writes source file lines to intermediate file   ***
        *** INPUT ARGS: string LC, string[] line, string filename        ***
        *** OUTPUT ARGS: NONE                                            ***
        *** IN/OUT ARGS: int lineNumber                                  ***
        *** RETURN: void                                                 ***
        ********************************************************************/
        private static void ToFile(ref int lineNumber, string LC, string[] line, string fileName)
        {
            using (StreamWriter writer = File.AppendText(Path.Combine(Directory.GetCurrentDirectory(), fileName)))
            {   writer.WriteLine("{0, -10} {1, -10} {2, -10} {3, -10} {4, -10}", (++lineNumber).ToString().PadLeft(2, '0'), LC.PadLeft(5, '0'), line[0], line[1], line[2]); }
        }

        /******************************************************************* 
        *** FUNCTION: ReadLine                                           ***
        ********************************************************************  
        *** DESCRIPTION: This function reads all lines and splits it.    ***
        *** INPUT ARGS: string line                                      ***
        *** OUTPUT ARGS: bool comment                                    ***
        *** IN/OUT ARGS: NONE                                            ***
        *** RETURN: string[] returnLine                                  ***
        ********************************************************************/
        private static string[] ReadLine(string line, out bool comment)
        {
            comment = false;
            char[] splitCatergories = { ' ', '\t' };
            var splitLine = line.Split(splitCatergories, StringSplitOptions.RemoveEmptyEntries);
            string[] returnLine = new string[3];
            if (splitLine[0][0] == '.')
            {
                comment = true;
                return returnLine;
            }
            if (splitLine.Length >= 3)
            {
                if (splitLine[2][0] == '.')
                {
                    returnLine[0] = string.Empty;
                    returnLine[1] = splitLine[0];
                    returnLine[2] = splitLine[1];
                }
                else
                {
                    returnLine[0] = splitLine[0];
                    returnLine[1] = splitLine[1];
                    returnLine[2] = splitLine[2];
                }
            }
            else if (splitLine.Length == 2)
            {
                returnLine[0] = string.Empty;
                returnLine[1] = splitLine[0];
                returnLine[2] = splitLine[1];
            }
            return returnLine;
        }

        /******************************************************************** 
        *** FUNCTION: LCCount                                             ***
        *********************************************************************  
        *** DESCRIPTION: increments the location counter                  ***
        *** INPUT ARGS: string LC, int increment                          ***
        *** OUTPUT ARGS: NONE                                             ***
        *** IN/OUT ARGS: NONE                                             ***
        *** RETURN: string                                                ***
        *********************************************************************/
        private static string LCCount(string LC, int increment)
        {   return (int.Parse(LC, System.Globalization.NumberStyles.HexNumber) + increment).ToString("X");  }

       /********************************************************************* 
       *** FUNCTION: LitToFile                                            ***
       **********************************************************************  
       *** DESCRIPTION: puts literal in end of intermediate file          ***
       *** INPUT ARGS: string filename                                    ***
       *** OUTPUT ARGS: NONE                                              ***
       *** IN/OUT ARGS: LinkedList literal, int lineNum, string LC        ***
       *** RETURN: int size                                               ***
       **********************************************************************/
        private static int LitToFile(ref LinkedList<Literal> literal, ref int lineNum, ref string LC, string fileName)
        {
            int size = 0;
            foreach (var lit in literal)
            {
                string[] litLine = { "*", lit.name, string.Empty };
                ToFile(ref lineNum, LC, litLine, fileName);
                Literal litAddress = lit;
                litAddress.address = LC;
                literal.Replace(litAddress, lit);
                LC = LCCount(LC, lit.length);
                size += lit.length;
            }
            return size;
        }

        /********************************************************************
        *** FUNCTION InsertSymbol                                         ***
        *********************************************************************
        *** DESCRIPTION: insert the symbol                                ***
        *** INPUT ARGS: string name, string value, bool rFlag,            ***
        ***                 bool iFlag, bool mFlag                        ***
        *** OUTPUT ARGS: NONE                                             ***
        *** IN/OUT ARGS: NONE                                             ***
        *** RETURN: void                                                  ***
        ********************************************************************/
        public void InsertSymbol(string name, string value, bool rFlag, bool iFlag, bool mFlag)
        {
            SymbolTable.InsertNode(new Symbol { element = name.Length > 5 ? name.Remove(5) : name,
                                                value = int.Parse(value, System.Globalization.NumberStyles.HexNumber),
                                                rFlag = rFlag,
                                                iFlag = iFlag,
                                                mFlag = mFlag });
        }

        /********************************************************************
        *** FUNCTION SymbolSearch                                         ***
        *********************************************************************
        *** DESCRIPTION: Search symbol and return it                      ***
        *** INPUT ARGS: string name, out Symbol symbol                    ***
        *** OUTPUT ARGS: NONE                                             ***
        *** IN/OUT ARGS: NONE                                             ***
        *** RETURN: bool                                                  ***
        *********************************************************************/
        public bool SymbolSearch(string name, out Symbol symbol)
        {
            string parsedSymbol = Expressions.CheckSymbol(name);
            return SymbolTable.SearchNode(new Symbol { element = parsedSymbol.Length > 5 ? parsedSymbol.Remove(5) : parsedSymbol }, out symbol);
        }

        /********************************************************************* 
        *** FUNCTION : DefineSymbol(ref int lineNumber, string LC, string[] line, string fileName)
        **********************************************************************  
        *** DESCRIPTION : define the symbol
        *** INPUT ARGS : linenumber, lc, line, filename                    ***
        *** OUTPUT ARGS : -                                                ***
        *** IN/OUT ARGS : -                                                ***
        *** RETURN :                                                       ***  
        **********************************************************************/
        private void DefineSymbol(ref int lineNumber, string LC, string[] line, string fileName)
        {
            bool rFlag = true;
            int intValue;
            string[] splitLine;
            List<Symbol> symbols = new List<Symbol>();

            if (int.TryParse(line[2], out intValue))
            {
                rFlag = false;
            }
            else if (line[2] == "*")
            {
                rFlag = true;
                intValue = int.Parse(LC, System.Globalization.NumberStyles.HexNumber);
            }
            else if (line[2].Contains("+"))
            {
                splitLine = line[2].Split('+');
                foreach (var symbol in splitLine)
                {
                    if (SymbolSearch(symbol, out Symbol found))
                    {
                        symbols.Add(found);
                    }
                }

                Expressions.CheckExpression('+', splitLine, symbols.ToArray(), out intValue, out rFlag);
            }
            else if (line[2].Contains("-"))
            {
                splitLine = line[2].Split('-');
                foreach (var symbol in splitLine)
                {
                    if (SymbolSearch(symbol, out Symbol found))
                    {
                        symbols.Add(found);
                    }
                }

                Expressions.CheckExpression('-', splitLine, symbols.ToArray(), out intValue, out rFlag);
            }
            ToFile(ref lineNumber, intValue.ToString("X"), line, fileName);
            InsertSymbol(line[0], intValue.ToString("X"), rFlag, true, false);
        }

        /********************************************************************
        *** FUNCTION StartOne                                             ***
        *********************************************************************
        *** DESCRIPTION: Start Pass One                                   ***
        *** INPUT ARGS: string file                                       ***
        *** OUTPUT ARGS: NONE                                             ***
        *** IN/OUT ARGS: NONE                                             ***
        *** RETURN: NONE                                                  ***
        *********************************************************************/
        public void StartOne(string file)
        {
            string[] program = File.ReadAllLines(Path.Combine(Directory.GetCurrentDirectory(), ($@"{Environment.CurrentDirectory}\\..\\..\\" + file)));
            var fileNam = file.Remove(file.IndexOf('.')) + ".tmp";
            string aList = file.Remove(file.IndexOf('.')) + ".tmp";
            //for some reason could get the tmp file in the right place but not the obj or txt file there too
            string fileName = Path.Combine(Path.Combine(Directory.GetCurrentDirectory(), /*($@"{Environment.CurrentDirectory}\\..\\..\\" + */aList));
            //StreamWriter objFile = new StreamWriter(aListPath);
            bool formatOne = false;
            int lineNum = 0;
            string[] lines = ReadLine(program[lineNum], out bool comment);
            string LC;
            File.Create(fileName).Close();
            while (comment)
            {
                lineNum++;
                lines = ReadLine(program[lineNum], out comment);
            }
            if (lines[1] == "START")
            {
                StartAdd = lines[2];
                LC = StartAdd;
                pName = lines[0];
                InsertSymbol(lines[0], LC, true, true, false);
                ToFile(ref lineNum, LC, lines, fileName);
                lines = ReadLine(program[lineNum], out comment);
            }
            else
            {
                StartAdd = "00000";
                LC = StartAdd;
                pName = string.Empty;
            }
            while (lines[1] != "END")
            {
                int count = 0;
                if (!comment)
                {
                    if (lines[0] != string.Empty && lines[1] != "EQU")
                    {
                        if (!SymbolSearch(lines[0], out Symbol found))
                        {
                            if (char.IsLetter(lines[0][0]))
                                InsertSymbol(lines[0], LC, true, true, false);
                            else
                                Console.WriteLine("||ERROR|| Invalid Symbol");
                        }
                    }
                    if (SearchTable(lines[1], out Opcode opcode))
                    {
                        _ = lines[1];
                        if (opcode.form == 4)
                            count = 4;
                        else
                            count = opcode.form;
                    }
                    else if (lines[1] == "BASE" || lines[1] == "EXTREF" || lines[1] == "EXTDEF")
                        lines = ReadLine(program[lineNum], out comment);
                    else if (lines[1] == "WORD")
                        count = 3;
                    else if (lines[1] == "RESW")
                        count = int.Parse(lines[2]) * 3;
                    else if (lines[1] == "RESB")
                        count = int.Parse(lines[2]);
                    else if (lines[1] == "BYTE")
                        count = Expressions.ParseConstant(lines[2]).length;
                    else if (lines[1] == "EQU")
                    {
                        DefineSymbol(ref lineNum, LC, lines, fileName);
                        lines = ReadLine(program[lineNum], out comment);
                        continue;
                    }
                    else
                    {
                        if (!formatOne)
                        {
                            lines[0] = lines[1];
                            lines[1] = lines[2];
                            lines[2] = string.Empty;
                        }
                        else
                        {
                            lines = ReadLine(program[lineNum], out comment);
                            formatOne = false;
                        }
                        continue;
                    }
                    if (lines[2].Length != 0 ? lines[2][0] == '=' : false)
                    {
                        var constant = Expressions.ParseConstant(lines[2].Substring(1, lines[2].Length - 1));
                        constant.name = "=" + constant.name;
                        literalTable.Append(constant);
                    }
                }
                else
                {
                    lineNum++;
                    lines = ReadLine(program[lineNum], out comment);
                    continue;
                }
                ToFile(ref lineNum, LC, lines, fileName);
                LC = LCCount(LC, count);
                lines = ReadLine(program[lineNum], out comment);
            }
            ToFile(ref lineNum, LC, lines, fileName);
            pLength = ((int.Parse(LC, System.Globalization.NumberStyles.HexNumber) -
                                int.Parse(StartAdd, System.Globalization.NumberStyles.HexNumber)) +
                                LitToFile(ref literalTable, ref lineNum, ref LC, fileName)).ToString("X");
            PassTwo.StartTwo(fileName, this);
            Console.WriteLine("Intermediate File");
            Console.WriteLine("{0,-10} {1,-10} {2,-10} {3,-10} {4,-10}", "Line No", "LC", "Label", "Operation", "Operand");
            foreach (var intermediateLine in File.ReadAllLines(Path.Combine(Directory.GetCurrentDirectory(), fileName)))
            {
                Console.WriteLine(intermediateLine);
            }
            Console.WriteLine();
            Console.WriteLine("Symbol Table");
            Console.WriteLine("{0,-10} {1,-10} {2,-15} {3,-10} {4,-10}", "Label", "Value", "RFlag", "IFlag", "MFlag");
            SymbolTable.Display();
            literalTable.View();
        }
        /********************************************************************
        *** FUNCTION    : FlagCheck                                       ***
        *********************************************************************
        *** DESCRIPTION : check rFlag                                     ***
        *** INPUT ARGS  : char op, bool right, bool left                  ***
        *** OUTPUT ARGS : NONE                                            ***
        *** RETURN      : bool                                            ***
        *********************************************************************/
        /*private static bool FlagCheck(char op, bool right, bool left)
        {
            if (op == '+')
            {
                if (right && left)  //relative + relative
                    throw new InvalidOperationException("||ERROR|| Cannot add Relative and Relative.");
                else if (!right && left)    //absolute + relative
                    return true;
                else if (right && !left)    //relative + absolute
                    return true;
                else //!right && !left  absolue + absolute
                    return false;
            }
            else if (op == '-')
            {
                if (right && left)  //relative - relative
                    return false;
                else if (!right && left)    //absolute - relative
                    return true;
                else if (right && !left)    //relative - absolute
                    throw new InvalidOperationException("||ERROR|| Cannot subtract Relative from Absolute.");
                else //!right && !left  absolute - absolute
                    return true;
            }
            return false;
        }

        /********************************************************************
        *** FUNCTION    : CheckExpression                                 ***
        *********************************************************************
        *** DESCRIPTION : checks expression                               ***
        *** INPUT ARGS  : char op, string[] exSym, Symbol[] sym           ***
        *** OUTPUT ARGS : out int value, out bool rflag                   ***
        *** RETURN      : void                                            ***
        *********************************************************************/
        /*private static void CheckExpression(char op, string[] exSym, Symbol[] sym, out int value, out bool rflag)
        {
            value = 0;
            rflag = false;

            bool right = int.TryParse(exSym[1], out int rightVal);  //declare
            bool left = int.TryParse(exSym[0], out int leftVal);    //declare

            if (op == '+')
            {
                if (right && left)  //num and num
                {
                    value = leftVal + rightVal;
                    rflag = FlagCheck('+', false, false);
                }
                else if (!right && left)    //value and num
                {
                    value = leftVal + sym[0].Value;
                    rflag = FlagCheck('+', false, sym[0].RFlag);
                }
                else if (right && !left)    //num and value
                {
                    value = sym[0].Value + rightVal;
                    rflag = FlagCheck('+', sym[0].RFlag, false);
                }
                else if (!right && !left)   //value and value   
                {
                    value = sym[0].Value + sym[1].Value;
                    rflag = FlagCheck('+', sym[0].RFlag, sym[1].RFlag);
                }
            }
            else if (op == '-') //if has -
            {
                if (right && left)  //num and num
                {
                    value = leftVal - rightVal;
                    rflag = FlagCheck('-', false, false);
                }
                else if (!right && left)    //value and num
                {
                    value = leftVal - sym[0].Value;
                    rflag = FlagCheck('-', false, sym[0].RFlag);
                }
                else if (right && !left)    //num and value
                {
                    value = sym[0].Value - rightVal;
                    rflag = FlagCheck('-', sym[0].RFlag, false);
                }
                else if (!right && !left)   //value and value
                {
                    value = sym[0].Value - sym[1].Value;
                    rflag = FlagCheck('-', sym[0].RFlag, sym[1].RFlag);
                }
            }
        }

        /********************************************************************
        *** FUNCTION    : CheckSymbol                                     ***
        *********************************************************************
        *** DESCRIPTION : check symbols                                   ***
        *** INPUT ARGS  : ref int lineNumber, string LC, string[] line,   *** 
        ***                 BinSearchTree symbolTable, string fileName    ***
        *** OUTPUT ARGS : None                                            ***
        *** RETURN      : Symbol                                          ***
        ********************************************************************/
        /* private static Symbol CheckSymbol(ref int lineNumber, string LC, string[] line, BinSearchTree symbolTable, string fileName)
         {
             List<Symbol> symbols = new List<Symbol>();
             bool rFlag = true;
             string[] sLine;

             if (int.TryParse(line[2], out int val))
                 rFlag = false;
             else if (line[2] == "*")
             {
                 rFlag = true;
                 val = int.Parse(LC, System.Globalization.NumberStyles.HexNumber);
             }
             else if (line[2].Contains("+")) //if has +
             {
                 sLine = line[2].Split('+'); //split at +
                 foreach (var symbol in sLine)
                     symbols.Add(symbolTable.Search(symbol));

                 CheckExpression('+', sLine, symbols.ToArray(), out val, out rFlag);
             }
             else if (line[2].Contains("-"))     //if has -
             {
                 sLine = line[2].Split('-'); //split at -
                 foreach (var symbol in sLine)
                     symbols.Add(symbolTable.Search(symbol));

                 CheckExpression('-', sLine, symbols.ToArray(), out val, out rFlag);
             }
             ToFile(ref lineNumber, val.ToString("X"), line, fileName);
             return new Symbol
             {
                 Element = line[0],      //new symbol
                 Value = val,
                 RFlag = rFlag,
                 IFlag = true,
                 MFlag = false
             };
         }

         /*********************************************************************
          *** FUNCTION    : ToFile                                          ***
          *********************************************************************
          *** DESCRIPTION : writes info to temp file                        ***
          *** INPUT ARGS  : ref int num, string LC, string[] line,          ***
          ***                 string file                                   ***
          *** OUTPUT ARGS : NONE                                            ***
          *** RETURN      : void                                            ***
          *********************************************************************/
        /*private static void ToFile(ref int num, string LC, string[] line, string file)
        {
            using (StreamWriter put = File.AppendText(Path.Combine(Directory.GetCurrentDirectory(), file)))
            { put.WriteLine("{0, -5} {1, -7} {2, -8} {3, -10} {4, -10}", (++num).ToString().PadLeft(2, '0'), LC.PadLeft(5, '0'), line[0], line[1], line[2]); }
        }

        /*********************************************************************
         *** FUNCTION    : ToScreen                                        ***
         *********************************************************************
         *** DESCRIPTION : writes to screen                                ***
         *** INPUT ARGS  : string file                                     ***
         *** OUTPUT ARGS : NONE                                            ***
         *** RETURN      : void                                            ***
         *********************************************************************/
        /*private static void ToScreen(string file)
        {
            //string line;
            if (!File.Exists(Path.Combine(Directory.GetCurrentDirectory(), ($@"{Environment.CurrentDirectory}" + file))))
            {
                StreamReader put = new StreamReader(Path.Combine(Directory.GetCurrentDirectory(), (file)));
                Console.WriteLine("{0,-5} {1,-7} {2,-8} {3,-10} {4,-10}", "LINE", "LC", "LABEL", "OPERATION", "OPERAND");
                Console.WriteLine("-------------------------------------------------");
                string line = put.ReadLine(); //streamReader readLine
                while (line != null)
                {
                    Console.WriteLine(line);    //print to screen
                    line = put.ReadLine();
                }
                put.Close();
            }
        }

        /********************************************************************
        *** FUNCTION    : ProcessLine                                     ***
        *********************************************************************
        *** DESCRIPTION : To read in the line and split into sections     ***
        *** INPUT ARGS  : string and bool                                 ***
        *** OUTPUT ARGS : bool                                            ***
        *** RETURN      : String[]                                        ***
        *********************************************************************/
        /*private static string[] ProcessLine(string line, out bool check)
        {
            check = false;
            char[] splitEx = { ' ', '\t' };
            var sLine = line.Split(splitEx, StringSplitOptions.RemoveEmptyEntries);
            string[] rLine = new string[4];

            if (sLine[0].Contains(";"))
            {
                check = true;
                rLine[1] = string.Empty;
                rLine[2] = string.Empty;
                rLine[3] = string.Empty;

                for (int i = 0; i < sLine.Length; i++)
                    rLine[0] = rLine[0] + " " + sLine[i];
            }
            if (sLine.Length == 2)  //read line and move
            {
                if (!check)
                {
                    rLine[0] = string.Empty;
                    rLine[1] = sLine[0];
                    rLine[2] = sLine[1];
                    rLine[3] = string.Empty;
                }
                else
                {
                    rLine[0] = sLine[0] + sLine[1];
                    rLine[1] = string.Empty;
                    rLine[2] = string.Empty;
                    rLine[3] = string.Empty;
                }
            }
            else if (sLine.Length == 3)
            {
                rLine[0] = sLine[0];
                rLine[1] = sLine[1];
                rLine[2] = sLine[2];
                rLine[3] = string.Empty;
            }
            else
            {
                rLine[0] = sLine[0];
                rLine[1] = sLine[1];
                rLine[2] = sLine[2];
                for (int i = 3; i < sLine.Length; i++)
                    rLine[3] = rLine[3] + " " + sLine[i];
            }

            return rLine;
        }

        /********************************************************************
        *** FUNCTION    : LCCount                                         ***
        *********************************************************************
        *** DESCRIPTION : increment LC                                    ***
        *** INPUT ARGS  : string LC, int increment                        ***
        *** OUTPUT ARGS : NONE                                            ***
        *** RETURN      : string                                          ***
        *********************************************************************/
        /*private static string LCCount(string LC, int add)
        { return (int.Parse(LC, System.Globalization.NumberStyles.HexNumber) + add).ToString("X"); }

        /********************************************************************
        *** FUNCTION    : CutLiteral                                      ***
        *********************************************************************
        *** DESCRIPTION : cuts lieral at *                                ***
        *** INPUT ARGS  : ref LinkedList literal, ref int num,            ***
        ***                 ref string LC, string file                    ***
        *** OUTPUT ARGS : NONE                                            ***
        *** RETURN      : int size                                        ***
        *********************************************************************/
        /* private static int CutLiteral(ref LinkedList literal, ref int num, ref string LC, string file)
         {
             int size = 0;
             foreach (var lit in literal.All())  //for each literal
             {
                 string[] line = { "*", lit.name, string.Empty };
                 ToFile(ref num, LC, line, file);    //put in file

                 Literal litAdd = lit;
                 litAdd.address = LC;

                 literal.Replace(litAdd, lit); //change literal
                 LC = LCCount(LC, lit.length);   //increment LC

                 size += lit.length;
             }
             return size;
         }

         /********************************************************************
         *** FUNCTION    : InsertLiteral                                   ***
         *********************************************************************
         *** DESCRIPTION : To insert the literal from the line             ***
         *** INPUT ARGS  : string                                          ***
         *** OUTPUT ARGS : None                                            ***
         *** RETURN      : Literal                                         ***
         *********************************************************************/
        /*private static Literal InsertL(string line)
        {
            var lit = new Literal();
            List<char> val = new List<char>();
            if (line[2] == '\'')    //begin '
            {
                try
                {
                    for (int i = 3; line[i] != '\''; i++)   //for each line
                        val.Add(line[i]);
                }
                catch (IndexOutOfRangeException)
                {
                    Console.WriteLine("||ERROR|| No end quote. " + line);
                }
            }
            else
                Console.WriteLine("||ERROR|| No beginning quote. " + line);

            if (char.ToUpper(line[1]) == 'C')   //literal char
            {
                val.ForEach(value => lit.value += ((int)value).ToString("X"));

                lit.length = val.Count;
                lit.address = "0";
                lit.name = line;
            }
            else if (char.ToUpper(line[1]) == 'X')  //literal hex
            {
                val.ForEach(value => {
                    lit.value += int.TryParse(value.ToString(), System.Globalization.NumberStyles.HexNumber, null, out int i) ?
                           value : throw new FormatException("||ERROR|| Incorrect hexadecimal value");
                });

                if (val.Count % 2 == 1)
                    Console.WriteLine("||ERROR|| Incorrect hexadecimal length.");   //error

                lit.length = val.Count / 2; //divide by two for hex
                lit.address = "0";
                lit.name = line;
            }
            else
                Console.WriteLine("||ERROR|| Incorrect Literal Form.");
            return lit;
        }

        /********************************************************************
        *** FUNCTION    : ProcessFile                                     ***
        *********************************************************************
        *** DESCRIPTION : run pass one                                    ***
        *** INPUT ARGS  : string inFile, OpcodeTable opcodeTable          ***
        *** OUTPUT ARGS : No Output Args                                  ***
        *** RETURN      : Void                                            ***
        *********************************************************************/
        /*public void ProcessFile(string inFile, OpcodeTable oTable)
        {
            BinSearchTree sTable = new BinSearchTree();
            LinkedList lTable = new LinkedList();
            //read file
            string[] program = File.ReadAllLines(Path.Combine(Directory.GetCurrentDirectory(), ($@"{Environment.CurrentDirectory}\\..\\..\\" + inFile)));
            var file = inFile.Remove(inFile.IndexOf('.')) + ".tmp";
            int num = 0;
            string sAdd;
            string LC;
            string pLength;
            string[] line = ProcessLine(program[num], out bool check);
            File.Create(file).Close();

            while (check)   //while there is lines
            {
                num++;
                line = ProcessLine(program[num], out check);
            }

            if (line[1] == "START") //for start
            {
                sAdd = line[2];
                LC = sAdd;
                sTable.Insert(new Symbol()
                {
                    Element = line[0], //insert new symbol
                    Value = int.Parse(LC, System.Globalization.NumberStyles.HexNumber),
                    RFlag = true
                });
                ToFile(ref num, LC, line, file);    //put in file the symbol
                line = ProcessLine(program[num], out check);
            }
            else
            {
                sAdd = "00000"; //starting address 00000
                LC = sAdd;
            }

            while (line[1] != "END")    //for end
            {
                int count = 0;
                if (line[1] != null && line[2] != null)
                {
                    if (line[0].Contains(";"))
                        check = true;

                    if (!check)
                    {
                        if (line[0].Contains(";"))  //check for comment
                            Console.WriteLine("Comment: " + line[0]);
                        if (line[0] != string.Empty && line[1] != "EQU")
                        {
                            try
                            { sTable.Search(line[2]); }
                            catch (KeyNotFoundException)
                            {
                                string symbol = line[0];
                                if (char.IsLetter(symbol[0]))
                                {
                                    sTable.Insert(new Symbol()
                                    {
                                        Element = line[0] + ':',
                                        Value = int.Parse(LC, System.Globalization.NumberStyles.HexNumber),
                                        RFlag = true
                                    });
                                }
                                else
                                {
                                    if (line[0].Contains(";"))  //check for comment
                                        Console.WriteLine("Comment: " + line[0]);
                                    else
                                        Console.WriteLine(string.Format("||ERROR|| Symbol {0} has an invalid character(s).", symbol));
                                }
                            }
                        }
                        if (oTable.Search(line[1], out Opcode opcode))
                            count = opcode.form;
                        else if (line[1] == "WORD") //for word
                            count = 3;
                        else if (line[1] == "BYTE") //for byte
                            count = InsertL("=" + line[2]).length;
                        else if (line[1] == "RESW") //for resw 
                            count = int.Parse(line[2]) * 3;
                        else if (line[1] == "RESB") //for resb
                            count = int.Parse(line[2]);
                        else if (line[2] == "FLOAT" || line[2] == "NORM" || line[2] == "FIX" || line[2] == "HIO" || line[2] == "TIO" || line[2] == "SIO")
                        {
                            count = 1;
                            string[] temp = line;
                            line[0] = temp[1];
                            line[1] = temp[2];
                            line[2] = " ";
                        }
                        else if (line[1] == "EQU")  //for equ
                        {
                            sTable.Insert(CheckSymbol(ref num, LC, line, sTable, file));
                            line = ProcessLine(program[num], out check);
                            continue;
                        }
                        else //if not any of those
                        {
                            Console.WriteLine(string.Format("||ERROR|| Opcode {0} not found.", line[2]));
                            num++;
                            line = ProcessLine(program[num], out check);
                            continue;
                        }

                        if (line[2][0] == '=')
                            lTable.Add(InsertL(line[2]));
                    }

                    ToFile(ref num, LC, line, file);
                    LC = LCCount(LC, count);
                    line = ProcessLine(program[num], out check);
                }
                else
                {
                    num++;
                    line = ProcessLine(program[num], out check);
                }
            }

            ToFile(ref num, LC, line, file);
            ToScreen(file); //display sic
            pLength = ((int.Parse(LC, System.Globalization.NumberStyles.HexNumber) - int.Parse(sAdd, System.Globalization.NumberStyles.HexNumber)) + CutLiteral(ref lTable, ref num, ref LC, file)).ToString("X");
            Console.WriteLine("_______________________________________________________");
            Console.WriteLine("\nProgram Length = " + pLength);
            Console.WriteLine("_______________________________________________________");
            Console.WriteLine("");
            lTable.Display();
            Console.WriteLine("_______________________________________________________");
            Console.WriteLine("\n                    SYMBOL TABLE");
            Console.WriteLine("{0,-10} {1,-10} {2,-10} {3,-10} {4,-10}", "LABEL", "VALUE", "RFLAG", "IFLAG", "MFLAG");
            Console.WriteLine("-------------------------------------------------------");
            sTable.Display();
        }*/
    }
}


