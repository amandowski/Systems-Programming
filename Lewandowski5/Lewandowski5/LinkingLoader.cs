/**************************************************************************
 *** Name: Amanda Lewandowski                                           ***
 *** Due Date: December 11, 2019                                        ***
 *** Assignment: 5 Linking Loader                                       ***
 *** Class: CSc 354                                                     ***
 *** Instructor: Gamradt                                                ***
 **************************************************************************
 *** Description: LinkingLoader file                                    ***
 **************************************************************************/
using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace Lewandowski5
{
    public static class LinkingLoader
    {
        /*******************************************************************
        *** FUNCTION PassOne                                             ***
        ********************************************************************
        *** DESCRIPTION: Displays the symbol, contsect, and memory map   ***
        *** INPUT ARGS:  string[][] program                              ***
        *** OUTPUT ARGS: symboltable                                     ***
        *** IN/OUT ARGS: NONE                                            ***
        *** RETURN: int ExSymTab                                         ***
        ********************************************************************/
        public static ExSymbolTable PassOne(string[][] program)
        {
            int ContSectLength = 0;
            var ExSymTab = new ExSymbolTable();

            foreach (var prog in program)
            {
                foreach (var instruct in prog)
                {
                    if (instruct[0] == 'H')
                    {
                        string ContSectLabel = instruct.Substring(1, 6);
                        ContSectLabel = ContSectLabel.Replace(" ", "");
                        int ContSectSAddress = Program.contSectAddress;
                        try
                        {   ContSectLength = int.Parse(instruct.Substring(13, 6), NumberStyles.HexNumber);  }
                        catch (FormatException)
                        {   Console.WriteLine("||ERROR|| Control Section Length - " + instruct.Substring(13, 6));  }
                        ExSymTab.ContSectList.Add(ContSectLabel, new int[] { ContSectSAddress, ContSectLength });
                    }
                    else if (instruct[0] == 'D')
                    {
                        int DCount = instruct.Substring(1).Length / 10;
                        for (int i = 0; i < DCount; i++)
                        {
                            string SymLabel = instruct.Substring(i * 11 + 1 + i, 6).Replace(@" ", "");
                            string FirstOffset = instruct.Substring(i * 11 + 7 + i, 6);
                            var SymOffset = 0;
                            try
                            {   SymOffset = int.Parse(FirstOffset, NumberStyles.HexNumber);  }
                            catch (FormatException)
                            {   Console.WriteLine("||ERROR|| Symbol Offset - " + FirstOffset); }
                            var SymAddress = Program.contSectAddress + SymOffset;
                            ExSymTab.SymList.Add(SymLabel, new int[] { SymAddress, SymOffset });
                        }
                    }
                    else if (instruct[0] == 'R')
                    {
                        int RCount = instruct.Substring(1).Length / 6 + 1;
                        for (int i = 0; i < RCount; i++)
                        {
                            string SymlLabel;
                            if (instruct.Substring(i * 6 + 1).Length >= 6)
                                SymlLabel = instruct.Substring(i * 6 + 1, 6).Replace(@" ", "");
                            else
                                SymlLabel = instruct.Substring(i * 6 + 1).Replace(@" ", "");

                            var SymOffset = 0; //temporary offset until the modification record
                            var SymAddress = Program.contSectAddress + SymOffset;
                            ExSymTab.SymList.Add(SymlLabel, new int[] { SymAddress, SymOffset });
                        }
                    }
                    else if (instruct[0] == 'E')
                    {
                        if (instruct.Length > 1)
                        {
                            string exAddress = instruct.Substring(1, 6);
                            int temp = 0;
                            try
                            {   temp = int.Parse(exAddress, NumberStyles.HexNumber);    }
                            catch (FormatException)
                            {   Console.WriteLine("||ERROR|| Execution address - " + exAddress);   }
                            Program.exAdd = Program.startMemory + temp;
                        }
                        Program.contSectAddress += ContSectLength;
                        Program.length += ContSectLength;
                    }
                }
            }
            return ExSymTab;
        }

        /*******************************************************************
        *** FUNCTION PassTwo                                             ***
        ********************************************************************
        *** DESCRIPTION: Pass 2 of the linkingloader                     ***
        *** INPUT ARGS: string[][] program, ExSymbolTable ExSymTab       ***
        *** OUTPUT ARGS: NONE                                            ***
        *** IN/OUT ARGS: NONE                                            ***
        *** RETURN: int memMap                                           ***
        ********************************************************************/
        public static Dictionary<int, string> PassTwo(string[][] program, ExSymbolTable ExSymTab)
        {
            int ContSectAddress = Program.startMemory;
            int ContSectLength = 0;
            string ContSectLabel = string.Empty;
            Dictionary<int, string> memMap = new Dictionary<int, string>();

            foreach (var prog in program)
            {
                foreach (var instruct in prog)
                {
                    if (instruct[0] == 'H')
                    {
                        ContSectLabel = instruct.Substring(1, 6);
                        ContSectLabel = ContSectLabel.Replace(@" ", "");
                        try
                        {   ContSectLength = int.Parse(instruct.Substring(13, 6), NumberStyles.HexNumber); }
                        catch (FormatException)
                        {   Console.WriteLine("||ERROR|| Control Section Length - " + instruct.Substring(13, 6));   }

                        for (int i = 0; i < ContSectLength; i++)
                        {   memMap[i + ContSectAddress] = "UU";   }
                    }
                    else if (instruct[0] == 'T')
                    {
                        int TAdd = 0;
                        int TLength = 0;
                        try
                        {   TAdd = int.Parse(instruct.Substring(1, 6), NumberStyles.HexNumber);  }
                        catch (FormatException)
                        {   Console.WriteLine("||ERROR|| Text Record Address - " + instruct.Substring(1, 6));    }

                        try
                        {   TLength = int.Parse(instruct.Substring(7, 2), NumberStyles.HexNumber);   }
                        catch (FormatException)
                        {   Console.WriteLine("|ERROR|| Text Record Length - " + instruct.Substring(7, 2));    }
                        string TRec = instruct.Substring(9, instruct.Length - 9);
                        int memAdd = ContSectAddress + TAdd;
                        for (int i = 0; i < TLength; i++)
                        {
                            memMap[memAdd] = TRec.Substring(i * 2, 2);
                            memAdd += 1;
                        }
                    }
                    else if (instruct[0] == 'M')
                    {
                        int MAdd = 0, length = 0, value = 0;
                        try
                        {   MAdd = int.Parse(instruct.Substring(1, 6), NumberStyles.HexNumber); }
                        catch (FormatException)
                        {   Console.WriteLine("||ERROR|| Modification Record Address - " + instruct.Substring(1, 6));  }
                        int memAdd = ContSectAddress + MAdd;
                        string Nib = memMap[memAdd] + memMap[memAdd + 1] + memMap[memAdd + 2];

                        if (ExSymTab.ContSectList.ContainsKey(ContSectLabel))
                        {   int[] tempAdd = ExSymTab.ContSectList[ContSectLabel];    }
                        try
                        {   length = int.Parse(instruct.Substring(7, 2), NumberStyles.HexNumber);   }
                        catch (FormatException)
                        {   Console.WriteLine("||ERROR|| Modification Record Length - " + instruct.Substring(7, 2));   }

                        if (length == 5)
                            Nib = Nib.Substring(1);

                        try
                        { value = int.Parse(Nib, NumberStyles.HexNumber); }
                        catch (FormatException)
                        { Console.WriteLine("||ERROR|| Modification Record Value - " + Nib); }

                        string SymLabel = instruct.Substring(10);
                        SymLabel = SymLabel.Replace(@" ", "");
                        int SymAddress;
                        if (!ExSymTab.ContSectList.ContainsKey(SymLabel))
                            SymAddress = ExSymTab.SymList[SymLabel][0];
                        else
                            SymAddress = ExSymTab.ContSectList[SymLabel][0];

                        char sign = instruct[9];
                        if (sign == '+')
                            value += SymAddress;
                        else if (sign == '-')
                            value -= SymAddress;

                        Nib = value.ToString("X");
                        if (length == 5)
                        {
                            string temp = "";
                            temp += memMap[memAdd][0].ToString();
                            Nib = AddLeft(Nib, 5);
                            Nib = temp + Nib;
                        }
                        else
                            Nib = AddLeft(Nib, 6);
                        memMap[memAdd] = Nib.Substring(0, 2);
                        memMap[memAdd + 1] = Nib.Substring(2, 2);
                        memMap[memAdd + 2] = Nib.Substring(4, 2);
                    }
                }
                ContSectAddress += ContSectLength;
            }
            return memMap;
        }

        /*******************************************************************
        *** FUNCTION AddLeft                                             ***
        ********************************************************************
        *** DESCRIPTION: Adds to the string with 0 to the left           ***
        *** INPUT ARGS: string str, int i                                ***
        *** OUTPUT ARGS: NONE                                            ***
        *** IN/OUT ARGS: NONE                                            ***
        *** RETURN: string sb.ToString                                   ***
        ********************************************************************/
        static string AddLeft(string str, int num)
        {
            int count = num - str.Length;
            if (count <= 0)
                return str;
            else
            {
                StringBuilder stringB = new StringBuilder();
                for (int j = 0; j < count; j++)
                { stringB.Append('0'); }
                stringB.Append(str);
                return stringB.ToString();
            }
        }
    }
}
