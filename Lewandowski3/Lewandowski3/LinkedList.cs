/**************************************************************************
 *** Name: Amanda Lewandowski                                           ***
 *** Due Date: October 30th, 2019                                       ***
 *** Assignment: 3 Pass 1                                               ***
 *** Class: CSc 354                                                     ***
 *** Instructor: Gamradt                                                ***
 **************************************************************************
 *** Description: Linked List file                                      ***
 **************************************************************************/
using System;
using System.Collections.Generic;

namespace Lewandowski3
{
    public struct Literal
    {
        public string name;
        public string value;
        public int length;
        public string address;
    }

    class Listsymbol
    {
        public Literal literal;
        public Listsymbol next;
    }

    class LinkedList
    {
        private Listsymbol head;

        /*********************************************************************
         *** FUNCTION    : LiteralList                                     ***
         *********************************************************************
         *** DESCRIPTION : Constructor                                     ***
         *** INPUT ARGS  : NONE                                            ***
         *** OUTPUT ARGS : NONE                                            ***
         *** RETURN      : NONE                                            ***
         *********************************************************************/
        public LinkedList()
        {   head = null;    }

        /********************************************************************
        *** FUNCTION    : Add                                             ***
        *********************************************************************
        *** DESCRIPTION : add nodes to linked list                        ***
        *** INPUT ARGS  : Literal lit                                     ***
        *** OUTPUT ARGS : NONE                                            ***
        *** RETURN      : void                                            ***
        *********************************************************************/
        public void Add(Literal lit)
        {
            Listsymbol newsymbol = new Listsymbol() { literal = lit, next = null };
            Listsymbol ptr;
            if (head == null)
                head = newsymbol;
            else
            {
                ptr = head;
                while (ptr.next != null)
                    ptr = ptr.next;
                ptr.next = newsymbol;
            }
        }
        /*********************************************************************
         *** FUNCTION    : All                                             ***
         *********************************************************************
         *** DESCRIPTION : find all                                        ***
         *** INPUT ARGS  : NONE                                            ***
         *** OUTPUT ARGS : NONE                                            ***
         *** RETURN      : Literal[]                                       ***
         ********************************************************************/
        public Literal[] All()
        {
            List<Literal> symbols = new List<Literal>();
            Listsymbol ptr = head;
            while (ptr != null)
            {
                symbols.Add(ptr.literal);
                ptr = ptr.next;
            }
            return symbols.ToArray();
        }

        /*********************************************************************
         *** FUNCTION    : Replace                                         ***
         *********************************************************************
         *** DESCRIPTION : Search and Replace the symbol                   ***
         *** INPUT ARGS  : Literals                                        ***
         *** OUTPUT ARGS : None                                            ***
         *** RETURN      : Void                                            ***
         ********************************************************************/
        public void Replace(Literal newSym, Literal prev)
        {
            Listsymbol ptr = head;
            while (ptr != null)
            {
                if (prev.name == ptr.literal.name)
                    ptr.literal = newSym;
                ptr = ptr.next;
            }
        }

        /********************************************************************
        *** FUNCTION    : Display                                         ***
        *********************************************************************
        *** DESCRIPTION : Display the literal table                       ***
        *** INPUT ARGS  : NONE                                            ***
        *** OUTPUT ARGS : NONE                                            ***
        *** RETURN      : void                                            ***
        *********************************************************************/
        public void Display()
        {
            Console.WriteLine("                    LITERAL TABLE");
            Console.WriteLine("{0,-12} {1,-18} {2,-10} {3,-10}", "NAME", "VALUE", "LENGTH", "ADDRESS");
            Console.WriteLine("-------------------------------------------------------");
            Listsymbol ptr = head;
            while (ptr != null)
            {
                Console.WriteLine("{0,-12} {1,-18} {2,-10} {3,-10}", ptr.literal.name, ptr.literal.value, ptr.literal.length, ptr.literal.address);
                ptr = ptr.next;
            }
        }
    }
}
