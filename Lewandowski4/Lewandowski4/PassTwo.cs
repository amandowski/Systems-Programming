/**************************************************************************
 *** Name: Amanda Lewandowski                                           ***
 *** Due Date: November 20th, 2019                                      ***
 *** Assignment: 4 Pass 2                                               ***
 *** Class: CSc 354                                                     ***
 *** Instructor: Gamradt                                                ***
 **************************************************************************
 *** Description: Pass Two file                                         ***
 **************************************************************************/
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Globalization;

namespace Lewandowski4
{
    public enum Register { A = 0, X = 1, L = 2, B = 3, S = 4, T = 5, F = 6, PC = 8, SW = 9 }
    class PassTwo
    {
        /********************************************************************
        *** FUNCTION: ReadLine                                            ***
        *********************************************************************
        *** DESCRIPTION: Splits the function                              ***
        *** INPUT ARGS: string line                                       ***
        *** OUTPUT ARGS: NONE                                             ***
        *** IN/OUT ARGS: NONE                                             ***
        *** RETURN: string ret                                            ***
        *********************************************************************/
        public static string[] ReadLine(string line)
        {
            char[] splitCat = { ' ', '\t' };
            var sLine = line.Split(splitCat, StringSplitOptions.RemoveEmptyEntries);
            string[] ret = new string[5];

            if (sLine.Length == 4)
            {
                ret[4] = sLine[3];
                ret[3] = sLine[2];
                ret[2] = string.Empty;
                ret[1] = sLine[1];
                ret[0] = sLine[0];
            }
            else
                ret = sLine;
            return ret;
        }

        /********************************************************************
        *** FUNCTION: Modifier                                            ***
        *********************************************************************
        *** DESCRIPTION: changes modification code                        ***
        *** INPUT ARGS: Symbol sym, string loc, string name, string size, ***
        ***                 string sign                                   ***
        *** OUTPUT ARGS: NONE                                             ***
        *** IN/OUT ARGS: NONE                                             ***
        *** RETURN: string mRecord.ToString()                             ***
        *********************************************************************/
        public static string Modifier(Symbol sym, string loc, string name, string size, char sign)
        {
            StringBuilder mRecord = new StringBuilder("M");
            mRecord.Append("^" + loc.PadLeft(6, '0') + "^");
            mRecord.Append(size.PadLeft(2, '0'));
            if (!sym.iFlag)
                mRecord.Append("^" + sign + sym.element);
            else
                mRecord.Append("^" + sign + (name.Length > 5 ? name.Remove(5) : name));

            return mRecord.ToString();
        }

        /********************************************************************
        *** FUNCTION ObjectCode                                           ***
        *********************************************************************
        *** DESCRIPTION: assembles the object code                        ***
        *** INPUT ARGS: Opcode opcode, string add, string exp, string LC, ***
        ***                 string BC                                     ***
        *** OUTPUT ARGS: NONE                                             ***
        *** IN/OUT ARGS: NONE                                             ***
        *** RETURN: string objCode                                        ***
        *********************************************************************/
        public static string ObjectCode(Opcode opcode, string add, string exp, string LC, string BC)
        {
            string objCode = string.Empty;
            if (opcode.form == 3 || opcode.form == 4)
            {
                byte[] objInstruct = { byte.Parse(opcode.code[0].ToString(), NumberStyles.HexNumber),
                                          byte.Parse(opcode.code[1].ToString(), NumberStyles.HexNumber), 0 },
                                          objAddress = Expressions.CheckBits(exp);

                string objAdd = "000";

                objInstruct[1] |= objAddress[0];
                objInstruct[2] |= objAddress[1];

                if (opcode.form == 4)
                {
                    objInstruct[2] |= 0b0001;
                    if (int.TryParse(Expressions.CheckSymbol(exp), out int num))
                        objAdd = num.ToString("X").PadLeft(5, '0');
                    else
                        objAdd = add.PadLeft(5, '0');
                }
                else if (opcode.form == 3)
                {
                    if (int.TryParse(Expressions.CheckSymbol(exp), out int num))
                        objAdd = num.ToString("X").PadLeft(3, '0');
                    else
                    {
                        int count = 0,
                            addressInt = int.Parse(add, NumberStyles.HexNumber);

                        if (addressInt >= -2048 && addressInt <= 2047)
                        {
                            objInstruct[2] |= 0b0010;
                            count = int.Parse(LC, NumberStyles.HexNumber);
                        }
                        else if (addressInt >= 0 && addressInt <= 4096)
                        {
                            objInstruct[2] |= 0b0100;
                            count = int.Parse(BC, NumberStyles.HexNumber);
                        }
                        objAdd = (addressInt - count).ToString("X").PadLeft(3, '0');
                    }
                    objAdd = objAdd.Substring(objAdd.Length - 3, 3);
                }
                foreach (var obj in objInstruct)
                    objCode += obj.ToString("X");
                objCode += objAdd;
            }
            else if (opcode.form == 2)
            {
                string[] splitRegisters = exp.Split(',');
                string regNums = string.Empty;
                objCode = opcode.code;
                foreach (var register in splitRegisters)
                    regNums += (int)(Register)Enum.Parse(typeof(Register), register);
                objCode += regNums.PadRight(2, '0');
            }
            else if (opcode.form == 1)
                objCode = opcode.code;

            return objCode;
        }

        public static Symbol[] Symbols(PassOne passOne, string expression, out char sign)
        {
            sign = '+';
            List<Symbol> symbolsList = new List<Symbol>();
            foreach (var symString in Expressions.SymbolCheck(expression))
            {
                if (passOne.SymbolSearch(symString, out Symbol symbol))
                    symbolsList.Add(symbol);
            }
            if (expression.Contains("-"))
                sign = '-';
            return symbolsList.ToArray();
        }

        public static void StartTwo(string inFile, PassOne passOne)
        {
            //could not get the txt and obj file in the right place for some reason
            string list = inFile.Remove(inFile.IndexOf('.')) + ".txt";
            string listPath = Path.Combine(Path.Combine(Directory.GetCurrentDirectory(), list));
            StreamWriter objFile = new StreamWriter(listPath);
            string objFileName = inFile.Remove(inFile.IndexOf('.')) + ".obj";
            string[] Lines = File.ReadAllLines(Path.Combine(Directory.GetCurrentDirectory(), inFile));
            objFile.WriteLine("Pass Two");
            objFile.WriteLine("{0,-10} {1,-10} {2,-10} {3,-10} {5,-10}", "Line", "LC", "Label", "Operation", "Operand", "Object Code");
            int lineNum = 0;
            bool checkFormatOne = false;
            string[] inLine = ReadLine(Lines[lineNum++]);
            string LC = "0";
            string BC = "0";
            string objAdd = "0";
            string objCode;
            StringBuilder objRecord = new StringBuilder();
            List<string> txtRecord;
            List<string> modRecord = new List<string>();

            if (inLine[3] == "START")
            {
                objRecord.AppendLine(string.Format("H^{0}^{1}^{2}", inLine[2], passOne.StartAdd.PadLeft(6, '0'), passOne.pLength.PadLeft(6, '0')));

                Console.WriteLine("{0,-10} {1,-10} {2,-10} {3,-10} {4,-10}", inLine[0], inLine[1], inLine[2], inLine[3], inLine[4]);
                objFile.WriteLine("{0,-10} {1,-10} {2,-10} {3,-10} {4,-10}", inLine[0], inLine[1], inLine[2], inLine[3], inLine[4]);

                inLine = ReadLine(Lines[lineNum++]);
            }

            txtRecord = new List<string>(new string[2] { "T^" + passOne.StartAdd.PadLeft(6, '0'), passOne.StartAdd });

            while (inLine[3] != "END")
            {
                objCode = string.Empty;
                LC = ReadLine(Lines[lineNum])?[1];
                if (passOne.SearchTable(inLine[3], out Opcode opcode))
                {
                    if (inLine[4] != string.Empty && passOne.SymbolSearch(inLine[4], out Symbol found))
                    {
                        objAdd = found.value.ToString("X");
                        if (opcode.form == 4)
                        {
                            if (inLine[4].Contains("+") || inLine[4].Contains("-"))
                            {
                                Symbol[] symbols = Symbols(passOne, inLine[4], out char sign);
                                char[] signs = { '+', sign };

                                int i = 0;
                                foreach (var symbol in symbols)
                                    modRecord.Add(Modifier(symbol, (int.Parse(inLine[1], NumberStyles.HexNumber) + 1).ToString("X"), passOne.pName, "5", signs[i++]));
                            }
                            else if (passOne.SymbolSearch(inLine[4], out Symbol symbol))
                                modRecord.Add(Modifier(symbol, (int.Parse(inLine[1], NumberStyles.HexNumber) + 1).ToString("X"), passOne.pName, "5", '+'));
                        }
                    }
                    else if (inLine[4].Contains("="))
                    {
                        foreach (var literal in passOne.LiteralTable)
                        {
                            if (inLine[4] == literal.name)
                                objAdd = literal.address;
                        }
                    }
                    else
                        objAdd = "0";
                    objCode = ObjectCode(opcode, objAdd, inLine[4], LC, BC);

                    Console.WriteLine("{0,-10} {1,-10} {2,-10} {3,-10} {4,-10} {5, -10}", inLine[0], inLine[1], inLine[2], inLine[3], inLine[4], objCode);
                    objFile.WriteLine("{0,-10} {1,-10} {2,-10} {3,-10} {4,-10} {5, -10}", inLine[0], inLine[1], inLine[2], inLine[3], inLine[4], objCode);
                }
                else if (inLine[3] == "BYTE")
                {
                    objCode = Expressions.ParseConstant(inLine[4]).value;
                    Console.WriteLine("{0,-10} {1,-10} {2,-10} {3,-10} {4,-10} {5, -10}", inLine[0], inLine[1], inLine[2], inLine[3], inLine[4], objCode);
                    objFile.WriteLine("{0,-10} {1,-10} {2,-10} {3,-10} {4,-10} {5, -10}", inLine[0], inLine[1], inLine[2], inLine[3], inLine[4], objCode);
                }
                else if (inLine[3] == "WORD")
                {
                    try
                    {
                        if (inLine[4].Contains("+") || inLine[4].Contains("-"))
                        {
                            Symbol[] symbols = Symbols(passOne, inLine[4], out char sign);
                            char[] signs = { '+', sign };
                            int i = 0;
                            foreach (var symbol in symbols)
                                modRecord.Add(Modifier(symbol, inLine[1], passOne.pName, "6", signs[i++]));
                            Expressions.CheckExpression(sign, inLine[4].Split('+', '-'), symbols, out int value, out bool rFlag);
                            objCode = value.ToString("X").PadLeft(6, '0');
                        }
                        else if (passOne.SymbolSearch(inLine[4], out Symbol symbol))
                        {
                            modRecord.Add(Modifier(symbol, inLine[1], passOne.pName, "6", '+'));
                            objCode = symbol.value.ToString("X").PadLeft(6, '0');
                        }
                        else
                            objCode = int.Parse(inLine[4]).ToString("X").PadLeft(6, '0');

                        Console.WriteLine("{0,-10} {1,-10} {2,-10} {3,-10} {4,-10} {5, -10}", inLine[0], inLine[1], inLine[2], inLine[3], inLine[4], objCode);
                        objFile.WriteLine("{0,-10} {1,-10} {2,-10} {3,-10} {4,-10} {5, -10}", inLine[0], inLine[1], inLine[2], inLine[3], inLine[4], objCode);

                    }
                    catch
                    { Console.WriteLine("ummm"); }
                }
                else if (inLine[3] == "BASE")
                {
                    if (passOne.SymbolSearch(inLine[4], out Symbol symbol))
                        BC = symbol.value.ToString("X");
                    Console.WriteLine("{0,-10} {1,-10} {2,-10} {3,-10} {4,-10}", inLine[0], inLine[1], inLine[2], inLine[3], inLine[4]);
                    objFile.WriteLine("{0,-10} {1,-10} {2,-10} {3,-10} {4,-10}", inLine[0], inLine[1], inLine[2], inLine[3], inLine[4]);
                }
                else if (inLine[3] == "EXTREF")
                {
                    objRecord.Append("R");
                    foreach (var refEntry in inLine[4].Split(','))
                    {
                        passOne.InsertSymbol(refEntry, "0", false, false, false);
                        objRecord.Append(refEntry);
                    }
                    objRecord.AppendLine();
                    Console.WriteLine("{0,-10} {1,-10} {2,-10} {3,-10} {4,-10}", inLine[0], inLine[1], inLine[2], inLine[3], inLine[4]);
                }
                else if (inLine[3] == "EXTDEF")
                {
                    objRecord.Append("D");
                    foreach (var defEntry in inLine[4].Split(','))
                    {
                        if (passOne.SymbolSearch(defEntry, out Symbol symbol))
                            objRecord.Append(defEntry + symbol.value.ToString("X").PadLeft(6, '0'));
                    }
                    objRecord.AppendLine();
                    Console.WriteLine("{0,-10} {1,-10} {2,-10} {3,-10} {4,-10}", inLine[0], inLine[1], inLine[2], inLine[3], inLine[4]);
                }
                else if (inLine[3] == "RESW" || inLine[3] == "RESB" || inLine[3] == "EQU")
                {
                    Console.WriteLine("{0,-10} {1,-10} {2,-10} {3,-10} {4,-10}", inLine[0], inLine[1], inLine[2], inLine[3], inLine[4]);
                    objFile.WriteLine("{0,-10} {1,-10} {2,-10} {3,-10} {4,-10}", inLine[0], inLine[1], inLine[2], inLine[3], inLine[4]);
                }
                else if (inLine[3] != "RESW" && inLine[3] != "RESB")
                {
                    if (!checkFormatOne)
                    {
                        inLine[2] = inLine[3];
                        inLine[3] = inLine[4];
                        inLine[4] = string.Empty;
                        checkFormatOne = true;
                    }
                    else
                        inLine = ReadLine(Lines[lineNum++]);
                    continue;
                }
                if (objCode != string.Empty && txtRecord.Count < 12)
                    txtRecord.Add(objCode);
                else if (txtRecord.Count > 2)
                {
                    txtRecord[1] = (int.Parse(inLine[1], NumberStyles.HexNumber) - int.Parse(txtRecord[1], NumberStyles.HexNumber)).ToString("X").PadLeft(2, '0');
                    txtRecord.ForEach(x => objRecord.Append("^" + x));
                    objRecord.AppendLine();

                    txtRecord = new List<string>(new string[2] { "T^" + inLine[1].PadLeft(6, '0'), inLine[1] });

                    if (objCode != string.Empty) 
                        txtRecord.Add(objCode);
                }
                checkFormatOne = false;
                inLine = ReadLine(Lines[lineNum++]);
            }
            Console.WriteLine("{0,-10} {1,-10} {2,-10} {3,-10} {4,-10}", inLine[0], inLine[1], inLine[2], inLine[3], inLine[4]);
            objFile.WriteLine("{0,-10} {1,-10} {2,-10} {3,-10} {4,-10}", inLine[0], inLine[1], inLine[2], inLine[3], inLine[4]);

            if (txtRecord.Count > 2)
            {
                try
                {
                    txtRecord[1] = (int.Parse(inLine[1], NumberStyles.HexNumber) - int.Parse(txtRecord[1]))
                            .ToString("X").PadLeft(2, '0');
                }
                catch
                { }
                txtRecord.ForEach(x => objRecord.Append("^" + x));
                objRecord.AppendLine();

                txtRecord = new List<string>(new string[2] { "T^" + inLine[1].PadLeft(6, '0'), inLine[1] });
            }

            foreach (var literal in passOne.LiteralTable)
            {
                inLine = ReadLine(Lines[lineNum++]);
                objCode = literal.value;

                Console.WriteLine("{0,-10} {1,-10} {2,-10} {3,-10} {4,-10} {5,-10}", inLine[0], inLine[1], inLine[2], inLine[3], inLine[4], objCode);
                objFile.WriteLine("{0,-10} {1,-10} {2,-10} {3,-10} {4,-10} {5,-10}", inLine[0], inLine[1], inLine[2], inLine[3], inLine[4], objCode);

                if (txtRecord.Count < 12)
                    txtRecord.Add(objCode);
                else if (txtRecord.Count > 2)
                {
                    txtRecord[1] = (int.Parse(inLine[1], NumberStyles.HexNumber) - int.Parse(txtRecord[1], NumberStyles.HexNumber))
                            .ToString("X").PadLeft(2, '0');
                    txtRecord.ForEach(x => objRecord.Append("^" + x));
                    objRecord.AppendLine();

                    txtRecord = new List<string>(new string[2] { "T^" + inLine[1].PadLeft(6, '0'), inLine[1] });

                    if (objCode != string.Empty) txtRecord.Add(objCode);
                }
            }

            if (txtRecord.Count > 2)
            {
                try
                {
                    txtRecord[1] = (int.Parse(inLine[1], NumberStyles.HexNumber) - int.Parse(txtRecord[1]))
                            .ToString("X").PadLeft(2, '0');
                }
                catch
                { }
                txtRecord.ForEach(x => objRecord.Append("^" + x));
                objRecord.AppendLine();
            }
            foreach (var moRecord in modRecord)
                objRecord.AppendLine(moRecord);

            objRecord.AppendLine("E^" + passOne.StartAdd.PadLeft(6, '0'));

            File.WriteAllText(Path.Combine(Directory.GetCurrentDirectory(), objFileName), objRecord.ToString());
            Console.WriteLine("Program Length: {0}", passOne.pLength);
            Console.WriteLine();

            Console.WriteLine("Object Records");
            Console.WriteLine(objRecord.ToString());

            objFile.WriteLine("Program Length: {0}", passOne.pLength);
            objFile.WriteLine("__________________________________________________________________");
            objFile.WriteLine("                          Symbol Table                                  ");
            objFile.WriteLine("{0,-10} {1,-10} {2,-15} {3,-10} {4,-10}", "Label", "Value", "RFlag", "IFlag", "MFlag");
            objFile.WriteLine("__________________________________________________________________");
            foreach (var symbol in passOne.SymbolTable)
                objFile.WriteLine(symbol);

            objFile.WriteLine("_________________________________________________________________");
            objFile.WriteLine("                         Literal Table                           ");
            objFile.WriteLine("{0,-12} {1,-18} {2,-10} {3,-10}", "Name", "Value", "Length", "Address");
            objFile.WriteLine("_________________________________________________________________");
            foreach (var literal in passOne.LiteralTable)
                objFile.WriteLine(literal);
            objFile.Close();
        }
    }
}
