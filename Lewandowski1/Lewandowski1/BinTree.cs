using System;

namespace Lewandowski2
{
    public struct Symbol
    {
        public string Element;
        public int Value;
        public bool RFlag, IFlag, MFlag;
    }
    public class Leaf
    {
        public Leaf Left;
        public Leaf Right;
        public Symbol Element;
    }
    class BinSearchTree
    {
        private Leaf tree;
        /**********************************************************************
         *** FUNCTION:    BinSearchTree                                     ***
         **********************************************************************
         *** DESCRIPTION: Constructor                                       ***
         *** INPUT ARGS:  NONE                                              ***
         *** OUTPUT ARGS: NONE                                              ***
         *** IN/OUT ARGS: NONE                                              ***
         *** RETURN:      NONE                                              ***
         **********************************************************************/
        public BinSearchTree()
        {
            tree = null;
        }
        /**********************************************************************
         *** FUNCTION:    Insert                                            ***
         **********************************************************************
         *** DESCRIPTION: Insert symbol                                     ***
         *** INPUT ARGS:  None                                              ***
         *** OUTPUT ARGS: None                                              ***
         *** IN/OUT ARGS: None                                              ***
         *** RETURN:      void                                              ***
         **********************************************************************/
        public void Insert(Symbol symbol)
        {
            Insert(symbol, ref tree);
        }
        /***********************************************************************
         *** FUNCTION:    View
         ***********************************************************************
         *** DESCRIPTION: View the symbol table.
         *** INPUT ARGS:  None
         *** OUTPUT ARGS: None
         *** IN/OUT ARGS: None
         *** RETURN:      void
         **********************************************************************/
        public void View()
        {
            Console.WriteLine("Symbol".PadRight(10) + "Value".PadRight(10) + "RFlag".PadRight(10) + "IFlag".PadRight(10) + "MFlag".PadRight(10) + "\n");
            PrintTree(tree);
            Console.WriteLine();
        }
        /**********************************************************************
         *** FUNCTION: Search                                               ***
         **********************************************************************
         *** DESCRIPTION: Search for the symbol                             ***
         *** INPUT ARGS: string value                                       ***
         *** OUTPUT ARGS: NONE                                              ***
         *** IN/OUT ARGS: NONE                                              ***
         *** RETURN: Leaf Tree                                              ***
         **********************************************************************/
        public Leaf Search(string value)
        {
            _ = new Leaf();
            Leaf Tree = Search(tree, value);
            return Tree;
        }
        /**********************************************************************
         *** FUNCTION: Search                                               ***
         **********************************************************************
         *** DESCRIPTION: Search the binary search tree                     ***
         *** INPUT ARGS: Leaf root, string value                            ***
         *** OUTPUT ARGS: NONE                                              ***
         *** IN/OUT ARGS: NONE                                              ***
         *** RETURN: Leaf                                                   ***
         **********************************************************************/
        private Leaf Search(Leaf root, string value)
        {
            if (root == null)
            {
                return null;
            }
            else if (string.Compare(root.Element.Element, value) < 0)
            {
                return Search(root.Right, value);
            }
            else if (string.Compare(root.Element.Element, value) > 0)
            {
                return Search(root.Left, value);
            }
            else if (string.Compare(root.Element.Element, value) == 0)
            {
                return root;
            }
            return null;
        }
        /***********************************************************************
         * FUNCTION:    Insert
         * *********************************************************************
         * DESCRIPTION: Insert into binary search tree
         * INPUT ARGS:  Symbol symbol, BSTNode root
         * OUTPUT ARGS: None
         * IN/OUT ARGS: None
         * RETURN:      void
         **********************************************************************/
        private void Insert(Symbol symbol, ref Leaf root)
        {
            if (symbol.Element.Length < 6)
                symbol.Element = symbol.Element.Trim();
            else
                symbol.Element = symbol.Element.Substring(0, 6);

            var newNode = new Leaf
            {
                Left = null,
                Right = null,
                Element = symbol
            };

            if (root == null)
                root = newNode;
            else if (string.Compare(symbol.Element, root.Element.Element) < 0)
                Insert(symbol, ref root.Left);
            else if (string.Compare(symbol.Element, root.Element.Element) > 0)
                Insert(symbol, ref root.Right);
            else if (string.Compare(symbol.Element, root.Element.Element) == 0)
            {
                root.Element.MFlag = true;
                Console.WriteLine("{0} -> Symbol already exists", symbol.Element);
            }
        }

        //private int lines = 0;
        /***********************************************************************
         * FUNCTION:    PrintTree
         * *********************************************************************
         * DESCRIPTION: View the symbol table.
         * INPUT ARGS:  BSTNode myTree
         * OUTPUT ARGS: None
         * IN/OUT ARGS: None
         * RETURN:      void
         **********************************************************************/
        private void PrintTree(Leaf myTree)
        {
            if (myTree != null)
            {
                if (myTree.Left != null)
                    PrintTree(myTree.Left);
                Console.WriteLine("{0,-10}{1,-10}{2,-10}{3,-10}{4,-10}", myTree.Element.Element, myTree.Element.Value, myTree.Element.RFlag ? 1 : 0, myTree.Element.IFlag ? 1 : 0, myTree.Element.MFlag ? 1 : 0);
                if (myTree.Right != null)
                    PrintTree(myTree.Right);

            }
        }
    }
}

