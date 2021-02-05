/**************************************************************************
 *** Name: Amanda Lewandowski                                           ***
 *** Due Date: November 20, 2019                                        ***
 *** Assignment: 4 Pass 2                                               ***
 *** Class: CSc 354                                                     ***
 *** Instructor: Gamradt                                                ***
 **************************************************************************
 *** Description: BinSearchTree file                                    ***
 **************************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;

namespace Lewandowski4
{
    public class Leaf<Tree>
    {
        public Leaf<Tree> Left;
        public Leaf<Tree> Right;
        public Tree Element;
    }
    public class BinSearchTree<Tree> : IEnumerable<Tree>
    {
        private Leaf<Tree> tree;
        private IComparer<Tree> comparer;

        /********************************************************************
        *** FUNCTION BinSearchTree                                        ***
        *********************************************************************
        *** DESCRIPTION: default constructor                              ***
        *** INPUT ARGS: NONE                                              ***
        *** OUTPUT ARGS: NONE                                             ***
        *** IN/OUT ARGS: NONE                                             ***
        *** RETURN: NONE                                                  ***
        *********************************************************************/
        public BinSearchTree(IComparer<Tree> comparer)
        {
            tree = null;
            this.comparer = comparer;
        }
        /********************************************************************
        *** FUNCTION: InsertNode                                          ***
        *********************************************************************
        *** DESCRIPTION: public inserts symbol                            ***
        *** INPUT ARGS: Tree node                                         ***
        *** OUTPUT ARGS: NONE                                             ***
        *** IN/OUT ARGS: NONE                                             ***
        *** RETURN: void                                                  ***
        *********************************************************************/
        public void InsertNode(Tree node)
        {
            Insert(node, ref tree);
        }

        /******************************************************************** 
         *** FUNCTION: Insert                                             ***
         ********************************************************************  
         *** DESCRIPTION: private inserts symbol                          ***
         *** INPUT ARGS: Tree node, Leaf<Tree> tree                       ***
         *** OUTPUT ARGS: NONE                                            ***
         *** IN/OUT ARGS: NONE                                            ***
         *** RETURN: void                                                 ***
         ********************************************************************/
        private void Insert(Tree node, ref Leaf<Tree> tree)
        {
            if (tree == null)
            {
                tree = new Leaf<Tree>
                {
                    Element = node,
                    Left = null,
                    Right = null
                };
            }
            else if (comparer.Compare(node, tree.Element) < 0)
                Insert(node, ref tree.Left);
            else if (comparer.Compare(node, tree.Element) > 0)
                Insert(node, ref tree.Right);
        }

        /********************************************************************
         *** FUNCTION: SearchNode                                         ***
         ********************************************************************
         *** DESCRIPTION: public to search for symbol                     ***
         *** INPUT ARGS: Tree search                                      ***
         *** OUTPUT ARGS: Tree found                                      ***
         *** IN/OUT ARGS: NONE                                            ***
         *** RETURN: bool                                                 ***
         ********************************************************************/
        public bool SearchNode(Tree search, out Tree found)
        {
            var symbol = Search(search, tree);

            if (symbol != null)
            {
                found = symbol.Element;
                return true;
            }
            else
            {
                found = default;
                return false;
            }
        }

        /********************************************************************
        *** FUNCTION: SearchNode                                          ***
        *********************************************************************
        *** DESCRIPTION: searches for symbol                              ***
        *** INPUT ARGS: Tree symbol, Leaf<Tree> tree                      ***
        *** OUTPUT ARGS: NONE                                             ***
        *** IN/OUT ARGS: NONE                                             ***
        *** RETURN: Leaf<Tree> tree                                       *** 
        *********************************************************************/
        private Leaf<Tree> Search(Tree symbol, Leaf<Tree> tree)
        {
            if (tree == null)
                return null;
            else if (comparer.Compare(symbol, tree.Element) < 0)
                return Search(symbol, tree.Left);
            else if (comparer.Compare(symbol, tree.Element) > 0)
                return Search(symbol, tree.Right);
            else
                return tree;
        }

        /********************************************************************
        *** FUNCTION: Display                                             ***
        *********************************************************************
        *** DESCRIPTION: public display to display the tree               ***
        *** INPUT ARGS: NONE                                              ***
        *** OUTPUT ARGS: NONE                                             ***
        *** RETURN: void                                                  ***
        *********************************************************************/
        public void Display()
        {
            View(tree);
        }

        /********************************************************************
        *** FUNCTION : View                                               ***
        *********************************************************************
        *** DESCRIPTION: private view of tree                             ***
        *** INPUT ARGS: Leaf<Tree> tree                                   ***
        *** OUTPUT ARGS: NONE                                             ***
        *** IN/OUT ARGS: NONE                                             ***
        *** RETURN: void                                                  ***
        *********************************************************************/
        private void View(Leaf<Tree> tree)
        {
            if (tree != null)
            {
                if (tree.Left != null)
                    View(tree.Left);
                Console.WriteLine(tree.Element.ToString());

                if (tree.Right != null)
                    View(tree.Right);
            }
        }

        /********************************************************************
        *** FUNCTION: GetEnumerator                                       ***
        *********************************************************************
        *** DESCRIPTION: get nodes used in for each                       ***
        *** INPUT ARGS: NONE                                              ***
        *** OUTPUT ARGS: NONE                                             ***
        *** IN/OUT ARGS: NONE                                             ***
        *** RETURN: void                                                  ***
        *********************************************************************/
        public IEnumerator<Tree> GetEnumerator()
        {
            var nodes = new List<Tree>();

            AllNodes(ref nodes, tree);

            return nodes.GetEnumerator();
        }

        /********************************************************************
        *** FUNCTION: IEnumerable.GetEnumerator                           ***
        *********************************************************************
        *** DESCRIPTION: IEnumerator for IEnumerable                      ***
        *** INPUT ARGS: NONE                                              ***
        *** OUTPUT ARGS: NONE                                             ***
        *** IN/OUT ARGS: NONE                                             ***
        *** RETURN: NONE                                                  ***
        *********************************************************************/
        IEnumerator IEnumerable.GetEnumerator()
        {   return GetEnumerator();  }

        /********************************************************************
        *** FUNCTION: AllNodes                                            *** 
        *********************************************************************
        *** DESCRIPTION: gets nodes so it is used in for each             ***
        *** INPUT ARGS: List<Tree> nodes, Leaf<Tree> tree                 ***
        *** OUTPUT ARGS: NONE                                             ***
        *** IN/OUT ARGS: NONE                                             ***
        *** RETURN: void                                                  ***
        *********************************************************************/
        private void AllNodes(ref List<Tree> nodes, Leaf<Tree> tree)
        {
            if (tree != null)
            {
                if (tree.Left != null)
                    AllNodes(ref nodes, tree.Left);
                nodes.Add(tree.Element);
                if (tree.Right != null)
                    AllNodes(ref nodes, tree.Right);
            }
        }
    }
}
