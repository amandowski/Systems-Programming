/**************************************************************************
 *** Name: Amanda Lewandowski                                           ***
 *** Due Date: October 30th, 2019                                       ***
 *** Assignment: 3 Pass 1                                               ***
 *** Class: CSc 354                                                     ***
 *** Instructor: Gamradt                                                ***
 **************************************************************************
 *** Description: Opcode file                                           ***
 **************************************************************************/
using System;
using System.Collections.Generic;

namespace Lewandowski3
{
    public struct Opcode
    {
        public string mnem;
        public string code;
        public int form;
    };
    class OpcodeTable
    {
        private int mnemCount = 0;
        private int formCount = 1;
        private int codeCount = 2;

        /********************************************************************
        *** FUNCTION    : CodeTable                                       ***
        *********************************************************************
        *** DESCRIPTION : getter for table                                ***
        *** INPUT ARGS  : get                                             ***
        *** OUTPUT ARGS : NONE                                            ***
        *** RETURN      : Opcode[] Table                                  ***
        *********************************************************************/
        public Opcode[] OTable 
        { get { return Table; } }

        /********************************************************************
        *** FUNCTION    : Table                                           ***
        *********************************************************************
        *** DESCRIPTION : setter for table                                ***
        *** INPUT ARGS  : get, set                                        ***
        *** OUTPUT ARGS : NONE                                            ***
        *** RETURN      : NONE                                            ***
        *********************************************************************/
        public Opcode[] Table 
        { get; set; }

        /********************************************************************
        *** FUNCTION    : Move                                            ***
        *********************************************************************
        *** DESCRIPTION : parses the opcode                               ***
        *** INPUT ARGS  : string[] lines                                  ***
        *** OUTPUT ARGS : NONE                                            ***
        *** RETURN      : void                                            ***
        *********************************************************************/
        public void Move(string[] lines)
        {
            List<Opcode> codes = new List<Opcode>();
            char[] splitC = { ' ' };
            foreach(var line in lines)  //for each line
            {
                var splitL = line.ToUpper().Split(splitC, StringSplitOptions.RemoveEmptyEntries);   //split on space
                codes.Add(new Opcode() { mnem = splitL[mnemCount], code = splitL[codeCount], form = int.Parse(splitL[formCount]) });
            }
            codes.Sort(new Compares());
            this.Table = codes.ToArray();
        }

        /********************************************************************
        *** FUNCTION    : Search                                          ***
        *********************************************************************
        *** DESCRIPTION : searches for opcode                             ***
        *** INPUT ARGS  : string mnmK, out Opcode code                    ***
        *** OUTPUT ARGS : Opcode code                                     ***
        *** RETURN      : bool                                            ***
        *********************************************************************/
        public bool Search(string mnemK, out Opcode code)
        {
            int found = Array.BinarySearch(Table, new Opcode { mnem = mnemK }, new Compares());
            if(found >= 0)  //found in table
            {
                code = Table[found];
                return true;
            }
            else    //not found return false
            {
                code = new Opcode();
                return false;
            }
        }

        public class Compares : Comparer<Opcode>
        {
            /********************************************************************
            *** FUNCTION    : Compare                                         ***
            *********************************************************************
            *** DESCRIPTION : compares the opcode                             ***
            *** INPUT ARGS  : Opcode x, Opcode y                              ***
            *** OUTPUT ARGS : NONE                                            ***
            *** RETURN      : override                                        ***
            *********************************************************************/
            public override int Compare(Opcode x, Opcode y)
            {   return string.Compare(x.mnem, y.mnem);    }
        }

        /********************************************************************
        *** FUNCTION    : OpcodeTable                                     ***
        *********************************************************************
        *** DESCRIPTION : constructor                                     ***
        *** INPUT ARGS  : string[] codes                                  ***
        *** OUTPUT ARGS : NONE                                            ***
        *** RETURN      : NONE                                            ***
        *********************************************************************/
        public OpcodeTable(string[] codes)
        { Move(codes); }

    }
}
