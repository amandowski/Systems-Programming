using System;
/**************************************************************************
 *** Name: Amanda Lewandowski                                           ***
 *** Due Date: December 11, 2019                                        ***
 *** Assignment: 5 Linking Loader                                       ***
 *** Class: CSc 354                                                     ***
 *** Instructor: Gamradt                                                ***
 **************************************************************************
 *** Description: ExSymbolTable file                                    ***
 **************************************************************************/
using System.Collections.Generic;

namespace Lewandowski5
{
    public class ExSymbolTable
    {
        public Dictionary<string, int[]> ContSectList = new Dictionary<string, int[]>();
        public Dictionary<string, int[]> SymList = new Dictionary<string, int[]>();

       /*******************************************************************
       *** FUNCTION Constructor                                         ***
       ********************************************************************
       *** DESCRIPTION: creates dictionary that stores SymList and      ***
       ***                  ContSectList                                ***
       *** INPUT ARGS: NONE                                             ***
       *** OUTPUT ARGS: NONE                                            ***
       *** IN/OUT ARGS: NONE                                            ***
       *** RETURN: NONE                                                 ***
       ********************************************************************/
        public ExSymbolTable()
        {
            ContSectList = new Dictionary<string, int[]>();
            SymList = new Dictionary<string, int[]>();
        }
    }
}
