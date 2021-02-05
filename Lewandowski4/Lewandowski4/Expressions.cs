/**************************************************************************
 *** Name: Amanda Lewandowski                                           ***
 *** Due Date: November 20th, 2019                                      ***
 *** Assignment: 4 Pass 2                                               ***
 *** Class: CSc 354                                                     ***
 *** Instructor: Gamradt                                                ***
 **************************************************************************
 *** Description: Expression file                                       ***
 **************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lewandowski4
{
    class Expressions
    {
        /**************************************************************************
        *** FUNCTION: ParseConstant                                             ***                              
        ***************************************************************************
        *** DESCRIPTION: goes through the constant                              ***
        *** INPUT ARGS: string line                                             *** 
        *** OUTPUT ARGS: NONE                                                   ***
        *** IN/OUT ARGS: NONE                                                   ***
        *** RETURN: static literal                                              ***
        ***************************************************************************/
        public static Literal ParseConstant(string line)
        {
            var literal = new Literal();
            List<char> parseVals = new List<char>();
            if (line[1] == '\'')
            {
                try
                {
                    for (int i = 2; line[i] != '\''; i++)
                        parseVals.Add(line[i]);
                }
                catch (IndexOutOfRangeException)
                { Console.WriteLine("||ERROR|| No end quote found."); }
            }
            else
                Console.WriteLine("||ERROR|| No beginning quote found.");

            if (char.ToUpper(line[0]) == 'C')
            {
                parseVals.ForEach(value => literal.value += ((int)value).ToString("X"));
                literal.length = parseVals.Count;
                literal.address = "0";
                literal.name = line;
            }
            else if (char.ToUpper(line[0]) == 'X')
            {
                parseVals.ForEach(value => { literal.value += int.TryParse(value.ToString(), System.Globalization.NumberStyles.HexNumber, null, out int i) ?
                                             value : throw new FormatException("||ERROR|| Invalid Hexadecimal Value."); });
                if (parseVals.Count % 2 == 1)
                    Console.WriteLine("||ERROR|| Invalid Hexadecimal Length.");
                literal.length = parseVals.Count / 2;
                literal.address = "0";
                literal.name = line;
            }
            else
                Console.WriteLine("||ERROR|| Literal type not found.");

            return literal;
        }

        /**************************************************************************
        *** FUNCTION: CheckValue                                                ***
        ***************************************************************************
        *** DESCRIPTION: checks to see if value is a number                     ***
        *** INPUT ARGS: string value                                            *** 
        *** OUTPUT ARGS: int num                                                ***
        *** IN/OUT ARGS: NONE                                                   ***
        *** RETURN: bool                                                        ***
        ***************************************************************************/
        public static bool CheckValue(string value, out int num)
        {
            if (int.TryParse(value, out num))
                return true;
            return false;
        }

        /**************************************************************************
        *** FUNCTION: CheckRFlag                                                ***
        ***************************************************************************
        *** DESCRIPTION: checks if the rflag is valid                           ***
        *** INPUT ARGS: string rflag                                            *** 
        *** OUTPUT ARGS: bool check                                             ***
        *** IN/OUT ARGS: NONE                                                   ***
        *** RETURN: bool                                                        ***
        ***************************************************************************/
        public static bool CheckRFlag(string rflag, out bool check)
        {
            string flag = rflag.ToUpper();
            if (flag == "TRUE" || flag == "1" || flag == "T")
            {
                check = true;
                return true;
            }
            else if (flag == "FALSE" || flag == "0" || flag == "F")
            {
                check = false;
                return false;
            }
            else
            {
                Console.WriteLine("||ERROR|| {0} RFlag is invalid.", flag);
                check = false;
                return false;
            }
        }

        /**************************************************************************
        *** FUNCTION: CheckName                                                 ***
        ***************************************************************************
        *** DESCRIPTION: checks if the symbol is valid                          ***
        *** INPUT ARGS: string symbol                                           *** 
        *** OUTPUT ARGS: NONE                                                   ***
        *** IN/OUT ARGS: NONE                                                   ***
        *** RETURN: bool                                                        ***
        ***************************************************************************/
        public static bool CheckName(string symbol)
        {
            if (symbol.Length <= 10 && char.IsLetter(symbol[0]))
            {
                symbol = symbol.Substring(1, symbol.Length - 1);
                foreach (var symChar in symbol)
                {
                    if (!char.IsLetterOrDigit(symChar) && !symChar.Equals('_'))
                        return false;
                }
            }
            else
                return false;

            return true;
        }

        /**************************************************************************
        *** FUNCTION: CheckFlag                                                 ***
        ***************************************************************************
        *** DESCRIPTION: checks the rflag for the expression                    ***
        *** INPUT ARGS: char op, bool left, bool right                          *** 
        *** OUTPUT ARGS: NONE                                                   ***
        *** IN/OUT ARGS: NONE                                                   ***
        *** RETURN: bool ret                                                    ***
        ***************************************************************************/
        public static bool CheckFlag(char op, bool left, bool right)
        {
            int ret = -1;
            if (op == '+')
                ret = (left ? 1 : 0) + (right ? 1 : 0);
            else if (op == '-')
                ret = (left ? 1 : 0) - (right ? 1 : 0);

            if (ret > 1)
                Console.WriteLine("||ERROR|| RELATIVE + RELATIVE");
            else if (ret < 0)
                Console.WriteLine("||ERROR|| ABSOLUTE - RELATIVE");

            return ret == 1 ? true : false;
        }

        /**************************************************************************
        *** FUNCTION: CheckExpression                                           ***
        ***************************************************************************
        *** DESCRIPTION: checks to see if value is a number                     ***
        *** INPUT ARGS: string value                                            *** 
        *** OUTPUT ARGS: int num                                                ***
        *** IN/OUT ARGS: NONE                                                   ***
        *** RETURN: bool                                                        ***
        ***************************************************************************/
        public static void CheckExpression(char op, string[] values, Symbol[] symbols, out int value, out bool rflagged)
        {
            bool right = char.IsLetter(values[0][0]);
            bool left = char.IsLetter(values[1][0]);
            if (op == '+')
            {
                if (right && left)
                {
                    value = symbols[0].value + symbols[1].value;
                    rflagged = CheckFlag(op, symbols[0].rFlag, symbols[1].rFlag);
                }
                else if (right && !left)
                {
                    value = symbols[0].value + int.Parse(values[1]);
                    rflagged = CheckFlag(op, symbols[0].rFlag, false);
                }
                else if (!right && left)
                {
                    value = int.Parse(values[0]) + symbols[0].value;
                    rflagged = CheckFlag(op, false, symbols[0].rFlag);
                }
                else
                {
                    value = int.Parse(values[0]) + int.Parse(values[1]);
                    rflagged = CheckFlag(op, false, false);
                }
            }
            else if (op == '-')
            {
                if (right && left)
                {
                    value = symbols[0].value - symbols[1].value;
                    rflagged = CheckFlag(op, symbols[0].rFlag, symbols[1].rFlag);
                }
                else if (right && !left)
                {
                    value = symbols[0].value - int.Parse(values[1]);
                    rflagged = CheckFlag(op, symbols[0].rFlag, false);
                }
                else if (!right && left)
                {
                    value = int.Parse(values[0]) - symbols[0].value;
                    rflagged = CheckFlag(op, false, symbols[0].rFlag);
                }
                else
                {
                    value = int.Parse(values[0]) - int.Parse(values[1]);
                    rflagged = CheckFlag(op, false, false);
                }
            }
            else
            {
                value = 0;
                rflagged = false;
            }

        
    }

        /**************************************************************************
        *** FUNCTION: CheckBits                                                 ***
        ***************************************************************************
        *** DESCRIPTION: checks the bits N, I, X                                ***
        *** INPUT ARGS: string epx                                              *** 
        *** OUTPUT ARGS: NONE                                                   ***
        *** IN/OUT ARGS: NONE                                                   ***
        *** RETURN: byte[] obj                                                  ***
        ***************************************************************************/
        public static byte[] CheckBits(string exp)
        {
            byte[] obj = { 3, 0 };
            exp = exp.ToUpper().Trim(); ;

            if (exp.Contains(",X"))
                obj[1] |= 0b1000;
            else
            {
                if (exp[0] == '@')
                    obj[0] &= 0b0010;

                if (exp[0] == '#' || char.IsDigit(exp[0]))
                    obj[0] &= 0b0001;
            }
            return obj;
        }

        /**************************************************************************
        *** FUNCTION: CheckSymbol                                               ***
        ***************************************************************************
        *** DESCRIPTION: checks for ,X or @ or #                                ***
        *** INPUT ARGS: string exp                                              *** 
        *** OUTPUT ARGS: int num                                                ***
        *** IN/OUT ARGS: NONE                                                   ***
        *** RETURN: string exp                                                  ***
        ***************************************************************************/
        public static string CheckSymbol(string exp)
        {
            if (exp.Contains(",X"))
                exp = exp.Remove(exp.IndexOf(",X"));

            if (exp[0] == '@' || exp[0] == '#')
                exp = exp.Substring(1, exp.Length - 1);

            return exp;
        }

        /**************************************************************************
        *** FUNCTION: SymbolCheck                                               ***
        ***************************************************************************
        *** DESCRIPTION: checks for equations and changes                       ***
        *** INPUT ARGS: string exp                                              *** 
        *** OUTPUT ARGS: NONE                                                   ***
        *** IN/OUT ARGS: NONE                                                   ***
        *** RETURN: string[] symbols.ToArray();                                 ***
        ***************************************************************************/
        public static string[] SymbolCheck(string exp)
        {
            var symbols = new List<string>();
            if (exp[0] == '@' || exp[0] == '#')
                exp = exp.Substring(1, exp.Length - 1);
            else if (exp.Contains(",X"))
                exp = exp.Remove(exp.IndexOf(','));

            if (exp.Contains('+'))
                symbols = new List<string>(exp.Split('+'));
            else if (exp.Contains('-'))
                symbols = new List<string>(exp.Split('-'));
            else
                symbols.Add(exp);

            return symbols.ToArray();
        }
    }
}
