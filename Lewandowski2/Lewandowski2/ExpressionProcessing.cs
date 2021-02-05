using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Lewandowski2
{
    public struct Literal
    {
        public string name;
        public string value;
        public int length;
        public string address;
    };

    class ExpressionProcessing
    {
        private SymbolTable searchExp;
        int count = 0;

        LinkedList<Literal> literalTable = new LinkedList<Literal>();
        List<string> log = new List<string>();

        /**********************************************************************
         *** FUNCTION: ExpressionProcessing                                 ***
         **********************************************************************
         *** DESCRIPTION: constructor                                       ***
         *** INPUT ARGS: SymbolTable exp                                    ***
         *** OUTPUT ARGS: NONE                                              ***
         *** IN/OUT ARGS: NONE                                              ***
         *** RETURN: NONE                                                   ***
         **********************************************************************/
        public ExpressionProcessing(SymbolTable exp)
        { searchExp = exp; }

        /**********************************************************************
         *** FUNCTION: RFlagCheck                                           ***
         **********************************************************************
         *** DESCRIPTION: change the rFlag accordingly                      ***
         *** INPUT ARGS: char type, bool left, bool right                   ***
         *** OUTPUT ARGS: NONE                                              ***
         *** IN/OUT ARGS:                                                   ***
         *** RETURN: NONE                                                   ***
         **********************************************************************/
        private bool RFlagCheck(char type, bool left, bool right)
        {
            if (type == '+')    //checking for addition
            {
                if (!left && !right)    //if they are both false
                    return false;
                else if (!left && right)    //if the left is false
                    return true;
                else if (left && !right)    //if the right is false
                    return true;
                else if (left && right) //if they are both true
                    Console.WriteLine("|ERROR| You cannot have RELATIVE + RELATIVE");
            }
            else if (type == '-')   //checking for subtraction
            {
                if (!left && !right)    //if they are both false
                    return false;
                else if (!left && right)    //if the left is false
                    Console.WriteLine("|ERROR| You cannot have ABSOLUTE - RELATIVE");
                else if (left && !right)    //if the right is false
                    return true;
                else if (left && right) //if they are both true
                    return false;
            }
            return false;   //otherwise return false
        }

        /**********************************************************************
         *** FUNCTION: CheckNumExpression                                   ***
         **********************************************************************
         *** DESCRIPTION: check if it is the number in expression           ***
         *** INPUT ARGS: char type, string[] vals, Symbols[] syms           ***
         *** OUTPUT ARGS: NONE                                              ***
         *** IN/OUT ARGS: out bool RFleg, out int value                     ***
         *** RETURN: NONE                                                   ***
         **********************************************************************/
        private void CheckNumExpression(char type, string[] vals, Symbol[] syms, out bool RFleg, out int value)
        {
            bool one = char.IsLetter(vals[0][0]);   
            bool two = char.IsLetter(vals[1][0]);

            if (type == '+')    //checking for addition
            {
                if (one && two)     //if they are both letters
                {
                    value = syms[0].Value + syms[1].Value;  //add both values together
                    RFleg = RFlagCheck(type, syms[0].RFlag, syms[1].RFlag); //change the rflag accordingly
                }
                else if (one && !two) //if the second is not a letter
                {
                    value = syms[0].Value + int.Parse(vals[1]); //add the first value and the second num
                    RFleg = RFlagCheck(type, false, syms[0].RFlag); //change the rflag accordingly
                }
                else if (!one && two)   //if the first is not a letter          
                {
                    value = int.Parse(vals[0]) + syms[0].Value; //add the first num and the second value
                    RFleg = RFlagCheck(type, false, syms[0].RFlag); //change the rflag accordingly
                }
                else    //if both of them are not letters
                {
                    value = int.Parse(vals[0]) + int.Parse(vals[1]);    //add the two nums
                    RFleg = RFlagCheck(type, false, false); //change the rflag accordingly
                }
            }
            else if (type == '-')   //checking for subtraction
            {
                if (one && two) //if they are both letters
                {
                    value = syms[0].Value - syms[1].Value;  //subtract the first value from the second
                    RFleg = RFlagCheck(type, syms[0].RFlag, syms[1].RFlag); //change the rflag accordingly
                }
                else if (one && !two)   //if the second is not a letter
                {
                    value = syms[0].Value - int.Parse(vals[1]); //subtract the second num from the first value
                    RFleg = RFlagCheck(type, syms[0].RFlag, false); //change the rflag accordingly
                }
                else if (!one && two)   //if the first is not a letter
                {
                    value = int.Parse(vals[0]) - syms[0].Value; //subtract the second value from the first num
                    RFleg = RFlagCheck(type, false, syms[0].RFlag); //change the rflag accordingly
                }
                else    //if both of them are not letters
                {
                    value = int.Parse(vals[0]) - int.Parse(vals[1]);    //subtract the second num from the first num
                    RFleg = RFlagCheck(type, false, false); //change the rflag accordingly
                }
            }
            else    //if its not addition or subtraction default 
            {
                value = 0;
                RFleg = false;
            }
        }

        /**********************************************************************
         *** FUNCTION: ExpressionCheck                                      ***
         **********************************************************************
         *** DESCRIPTION: check if it is a expression                       ***
         *** INPUT ARGS: string exp                                         ***
         *** OUTPUT ARGS: NONE                                              ***
         *** IN/OUT ARGS: NONE                                              ***
         *** RETURN: NONE                                                   ***
         **********************************************************************/
        public void ExpressionCheck(string exp)
        {
            string exp1 = exp;
            _ = new Symbol();
            bool relocate = false;
            int value = 0;

            BitsCheck(exp, out bool NBit, out bool IBit, out bool XBit);

            try
            {
                if (exp[0] == '@' || exp[0] == '#')     //check for either @ or # because they are the same points
                {
                    exp = exp.Substring(1, exp.Length - 1); //shorten the substring
                    if (exp.Contains(",X") || exp.Contains(",x"))  
                        Console.WriteLine("|ERROR| Cannot have indexing with either immediate addressing or indirect addressing.");
                }

                if (exp.Contains("+") || exp.Contains("-")) //check for either + or -
                {
                    bool add = exp.Contains("+");   //bool for add  
                    string[] numValues;
                    List<Symbol> numSymbols = new List<Symbol>();   //List to hold symbols

                    numValues = add ? exp.Split('+') : exp.Split('-');

                    if (exp[0] != '-')  //if it is -
                    {
                        if (numValues[1].Contains(",X") || numValues[1].Contains(",x")) //if it has ,X  
                            numValues[1] = numValues[1].Substring(0, numValues[1].Length - 2);  //take off the ,X

                        foreach (var character in numValues)
                        {
                            if (char.IsLetter(character[0]))    //if it is letter
                                numSymbols.Add(searchExp.Search(character));    //add to list
                        }

                        CheckNumExpression(add ? '+' : '-', numValues, numSymbols.ToArray(), out relocate, out value);  //check + or -
                    }
                    else
                        Int32.TryParse(exp, out value);
                }
                else
                {
                    if (char.IsLetter(exp[0]))  //if it is a letter
                    {
                        Symbol searchSym;
                        if (exp.Contains(",X") || exp.Contains(",x"))   //if it has ,X
                        {
                            exp = exp.Remove(exp.IndexOf(',')); //remove the ,
                            searchSym = searchExp.Search(exp);  //search for the expression
                            relocate = searchSym.RFlag; //define the relocatable
                            value = searchSym.Value;    //define the value
                        }
                        else
                        {
                            searchSym = searchExp.Search(exp);  //search for the expression
                            relocate = searchSym.RFlag; //define the relocatable
                            value = searchSym.Value;    //define the value
                        }
                    }
                    else if (char.IsDigit(exp[0]))  //if it is a digit
                    {
                        value = int.Parse(exp); //define the value
                        relocate = false;   //dfine the reloacable
                    }
                    else    //throw error
                        Console.WriteLine("|ERROR| It is in incorrect format.");
                }
                //add to the log 
                log.Add(string.Format("{0,-10} {1,-10} {2,-15} {3,-10} {4,-10} {5,-10}", exp1, value, relocate ? "RELATIVE" : "ABSOLUTE", NBit ? 1 : 0, IBit ? 1 : 0, XBit ? 1 : 0));
            }
            catch (NullReferenceException)
            {
                Console.WriteLine("|ERROR| Symbol not found."); //throw error
            }
        }

        /**********************************************************************
         *** FUNCTION: BitsCheck                                            ***
         **********************************************************************
         *** DESCRIPTION: checks whether the bits are true or false         ***
         *** INPUT ARGS: string exp                                         ***
         *** OUTPUT ARGS: NONE                                              ***
         *** IN/OUT ARGS: out bool NBit, out bool IBit, out bool XBit       ***
         *** RETURN: NONE                                                   ***
         **********************************************************************/
        private void BitsCheck(string exp, out bool NBit, out bool IBit, out bool XBit)
        {
            if (exp.Contains(",X") || exp.Contains(",x"))   //if it contains ,X
                XBit = true;    //xbit is true
            else    //if it doest contain ,X
            {
                XBit = false;   //xbit is false
                if (exp[0] == '@')      //if it has @
                {
                    IBit = false;   //ibit is false
                    NBit = true;    //nbit is true
                    return;
                }
                else if (exp[0] == '#') //if it has #
                {
                    IBit = true;    //ibit is true
                    NBit = false;   //nbit is false
                    return;
                }
            }

            if (char.IsDigit(exp[0]))   //if it is a digit
            {
                IBit = true;    //ibit is true
                NBit = false;   //nbit is false
                return;
            }
            NBit = true;    //otherwise nbit and ibit is true
            IBit = true;
        }

        /**********************************************************************
         *** FUNCTION: LiteralCheck                                         ***
         **********************************************************************
         *** DESCRIPTION: check if it is a literal and puts it in the list  ***
         *** INPUT ARGS: string exp                                         ***
         *** OUTPUT ARGS: NONE                                              ***
         *** IN/OUT ARGS: out int val                                       ***
         *** RETURN: NONE                                                   ***
         **********************************************************************/
        private void LiteralCheck(string exp)
        {
            var lit = new Literal();
            string val = string.Empty;

            if (exp[0] == '=') //if it is a literal '='
            {
                lit.name = exp;
                try
                {
                    if (exp[2] != '\'') //if it doesnt ave a single quote
                        Console.WriteLine("|ERROR| There must be a single quote at the beginning of the literal.");
                    for (int counter = 3; exp[counter] != '\''; counter++) //start after '
                        val += exp[counter];
                }
                catch (IndexOutOfRangeException)
                {
                    Console.WriteLine("|ERROR| There must be a single quote at the end of the literal.");
                }

                if (exp[1] == 'X' || exp[1] == 'x')     //if it ih hexadecimal, has x
                {
                    foreach (var num in val)   
                    {
                        try
                        {
                            if (int.TryParse(num.ToString(), System.Globalization.NumberStyles.HexNumber, null, out int hex)) //make it hex
                                lit.value += num; //add to value
                            else
                                throw new FormatException();
                        }
                        catch (FormatException)
                        {
                            Console.WriteLine("|ERROR| It is not a Hexadecimal digit.");
                        }
                    }
                    if (lit.value.Length % 2 != 0)
                        Console.WriteLine("|ERROR| There is an odd number of X bits.");
                    lit.length = lit.value.Length / 2;

                    if (!literalTable.Contains(lit)) //if the literaltable does not already have it
                    {
                        count++;    
                        lit.address = count.ToString("x"); 
                        lit.value = lit.value.ToUpper();
                        literalTable.AddLast(lit);
                    }
                }
                else if (exp[1] == 'C' || exp[1] == 'c')    //if it is a character
                {
                    foreach (var num in val)
                    {
                        lit.value += ((int)num).ToString("X");
                        lit.length++;
                    }
                    if (!literalTable.Contains(lit))    //if the literal table doesnt already contain it
                    {
                        count++;
                        lit.address = count.ToString("X");
                        literalTable.AddLast(lit);
                    }
                }
                else
                    Console.WriteLine("|ERROR| The literal must begin with either =X' or =C'.");
            }
        }

        /**********************************************************************
         *** FUNCTION: ReadAndProcess                                       ***
         **********************************************************************
         *** DESCRIPTION: reads from provided and processes them            ***
         *** INPUT ARGS: NONE                                               ***
         *** OUTPUT ARGS: NONE                                              ***
         *** IN/OUT ARGS: NONE                                              ***
         *** RETURN: NONE                                                   ***
         **********************************************************************/
        public void Processing(string expPath)
        {
            try
            {
                if (File.Exists(expPath))  
                {
                    string[] fileLines = File.ReadAllLines(expPath);
                    foreach (string line in fileLines)   //read each line
                    {
                        string line1 = line.Replace(" ", string.Empty); //replace ' ' with nothing
                        try
                        {
                            if (line1[0] == '=')    //if it starts with an =
                                LiteralCheck(line1);    //run literalCheck
                            else if (line1[0] == '@' || line1[0] == '#' || char.IsLetterOrDigit(line1[0]) || line1[0] == '-')   //if starts with a @ or #
                                ExpressionCheck(line1); //run expressionCheck
                        }
                        catch (FormatException exit)    //if the format is incorrect
                        {
                            log.Add(string.Format("|ERROR| Format {0} is {1}", line, exit.Message));
                        }
                        catch (NullReferenceException exit) //if it has a nullReferenceError
                        {
                            log.Add(string.Format("|ERROR| Null reference error {0} is {1}", line, exit.Message));
                        }
                        catch (InvalidOperationException exit)  //if there is an invalid operation
                        {
                            log.Add(string.Format("|ERROR| Invalid operation error {0} is {1}", line, exit.Message));
                        }
                    }

                    Console.WriteLine("Press any key to view the Expression Table.");
                    Console.ReadKey();
                    Console.Clear();
                    ViewExpTable();
                    Console.WriteLine("");
                    Console.WriteLine("Press any key to view the Literal Table.");
                    Console.ReadKey();
                    Console.Clear();
                    ViewLiteral();
                    Console.WriteLine("");
                    Console.WriteLine("Press any key to exit.");
                    Console.ReadKey();
                    Console.Clear();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

       /**********************************************************************
        *** FUNCTION: ViewExpTable                                         ***
        **********************************************************************
        *** DESCRIPTION: view the expression table                         ***
        *** INPUT ARGS: NONE                                               ***
        *** OUTPUT ARGS: NONE                                              ***
        *** IN/OUT ARGS: NONE                                              ***
        *** RETURN: NONE                                                   ***
        **********************************************************************/
        public void ViewExpTable()
        {
            Console.WriteLine("Expression Table");
            Console.WriteLine("{0,-10} {1,-10} {2,-15} {3,-10} {4,-10} {5,-10}", "Expression", "Value", "Relocatable", "N-Bit", "I-Bit", "X-Bit");
            foreach (var line in log)
                Console.WriteLine(line);
        }

       /**********************************************************************
        *** FUNCTION: ViewLiteral                                          ***
        **********************************************************************
        *** DESCRIPTION: view the literal table                            ***
        *** INPUT ARGS: NONE                                               ***
        *** OUTPUT ARGS: NONE                                              ***
        *** IN/OUT ARGS: NONE                                              ***
        *** RETURN: NONE                                                   ***
        **********************************************************************/
        public void ViewLiteral()
        {
            Console.WriteLine("Literal Table");
            Console.WriteLine("{0,-10} {1,-10} {2,-10} {3,-10}", "Name", "Value", "Length", "Address");
            foreach (Literal line in literalTable)
                Console.WriteLine("{0,-10} {1,-10} {2,-10} {3,-10}", line.name, line.value, line.length, line.address);
        }
    }
}
