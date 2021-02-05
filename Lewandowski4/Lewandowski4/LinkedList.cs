/**************************************************************************
 *** Name: Amanda Lewandowski                                           ***
 *** Due Date: November 20th, 2019                                      ***
 *** Assignment: 4 Pass 2                                               ***
 *** Class: CSc 354                                                     ***
 *** Instructor: Gamradt                                                ***
 **************************************************************************
 *** Description: Linked List file                                      ***
 **************************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;

namespace Lewandowski4
{
    class ListSymbol<Lest>
    {
        public Lest literal;
        public ListSymbol<Lest> next;
    }
    public class LinkedList<Lest> : IEnumerable<Lest>
    {
        public delegate string ToStringHandler();
        private ListSymbol<Lest> head;
        /********************************************************************
        *** FUNCTION LinkedList                                           ***
        *********************************************************************
        *** DESCRIPTION: default Constructor                              ***
        *** INPUT ARGS: NONE                                              ***
        *** OUTPUT ARGS: NONE                                             ***
        *** IN/OUT ARGS: NONE                                             ***
        *** RETURN: NONE                                                  ***
        *********************************************************************/
        public LinkedList()
        { head = null; }

        /********************************************************************
        *** FUNCTION AppendNode                                           ***
        *********************************************************************
        *** DESCRIPTION: appends literal to the literal table             ***
        *** INPUT ARGS: NONE                                              ***
        *** OUTPUT ARGS: NONE                                             ***
        *** IN/OUT ARGS: NONE                                             ***
        *** RETURN: void                                                  ***
        ********************************************************************/
        public void Append(Lest element)
        {
            ListSymbol<Lest> newNode = new ListSymbol<Lest>()
            {
                literal = element,
                next = null
            };
            ListSymbol<Lest> ptr;
            if (head == null)
                head = newNode;
            else
            {
                ptr = head;
                while (ptr.next != null)
                {
                    if (!ptr.literal.Equals(newNode.literal))
                        ptr = ptr.next;
                    else
                        break;
                }
                if (!ptr.literal.Equals(newNode.literal))
                    ptr.next = newNode;
            }
        }

        /***********************************************************************
        *** FUNCTION FindNode(T searchNode, out T foundNode)                 ***
        ************************************************************************
        *** DESCRIPTION: This function finds all the literals in the literal ***
        *** table                                                            ***
        *** INPUT ARGS: searchNode                                           ***
        *** OUTPUT ARGS: foundNode                                           ***
        *** IN/OUT ARGS: NONE                                                ***
        *** RETURN: bool                                                     ***
        ***********************************************************************/
        public bool FindNode(Lest searchNode, out Lest foundNode)
        {
            ListSymbol<Lest> ptr = head;
            while (ptr != null)
            {
                if (searchNode.Equals(ptr.literal))
                {
                    foundNode = ptr.literal;
                    return true;
                }
                ptr = ptr.next;
            }
            foundNode = default;
            return false;
        }

        /***********************************************************************
        *** FUNCTION ReplaceNode(T newNode, T previousNode)
        ************************************************************************
        *** DESCRIPTION : This function finds the literals that are dumped in **
        *** at the end of intermediate file and assigns the address to it    ***
        *** table                                                            ***
        *** INPUT ARGS: NONE                                                 ***
        *** OUTPUT ARGS: NONE                                                ***
        *** IN/OUT ARGS: NONE                                                ***
        *** RETURN: VOID                                                     ***
        ************************************************************************/
        public void Replace(Lest newNode, Lest previousNode)
        {
            ListSymbol<Lest> ptr = head;
            while (ptr != null)
            {
                if (previousNode.Equals(ptr.literal))
                {
                    ptr.literal = newNode;
                    break;
                }
                ptr = ptr.next;
            }
        }
        /********************************************************************
        *** FUNCTION View                                                 ***
        *********************************************************************
        *** DESCRIPTION: Function to view literal table                  ***
        *** INPUT ARGS: NONE                                              ***
        *** OUTPUT ARGS: NONE                                             ***
        *** IN/OUT ARGS: NONE                                             ***
        *** RETURN: VOID                                                  ***
        ********************************************************************/
        public void View()
        {
            Console.WriteLine();
            Console.WriteLine("Literal Table");
            Console.WriteLine("{0,-12} {1,-18} {2,-10} {3,-10}", "Name", "Value", "Length", "Address");
            ListSymbol<Lest> ptr = head;
            if (ptr == null)
                Console.WriteLine("No Literals");
            while (ptr != null)
            {
                Console.WriteLine(ptr.literal.ToString());
                ptr = ptr.next;
            }
        }

        /********************************************************************
        *** FUNCTION GetEnumerator()                                    ***
        *********************************************************************
        *** DESCRIPTION: Function to use literal table in a foreach loop  ***
        *** INPUT ARGS: NONE                                              ***
        *** OUTPUT ARGS: NONE                                             ***
        *** IN/OUT ARGS: NONE                                             ***
        *** RETURN: NONE                                                  ***
        *********************************************************************/
        public IEnumerator<Lest> GetEnumerator()
        {
            ListSymbol<Lest> ptr = head;
            while (ptr != null)
            {
                yield return ptr.literal;
                ptr = ptr.next;
            }
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}