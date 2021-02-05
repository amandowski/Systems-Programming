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
        *** RETURN: NONE                                                   ***
        **********************************************************************/
        public BinSearchTree()
        {   tree = null;    }

       /**********************************************************************
        *** FUNCTION:    Insert                                            ***
        **********************************************************************
        *** DESCRIPTION: Insert symbol by calling private function         ***
        *** INPUT ARGS:  NONE                                              ***
        *** OUTPUT ARGS: NONE                                              ***
        *** IN/OUT ARGS: NONE                                              ***
        *** RETURN: NONE                                                   ***
        **********************************************************************/
        public void Insert(Symbol symbol)
        {   Insert(symbol, ref tree);   }

       /**********************************************************************
        *** FUNCTION: View                                                 ***
        **********************************************************************
        *** DESCRIPTION: View the symbol table by calling private function ***
        *** INPUT ARGS: NONE                                               ***
        *** OUTPUT ARGS: NONE                                              ***
        *** IN/OUT ARGS: NONE                                              ***
        *** RETURN: NONE                                                   ***
        **********************************************************************/
        public void View()
        {
            //display the tree
            Console.WriteLine("Symbol".PadRight(10) + "Value".PadRight(10) + "RFlag".PadRight(10) + "IFlag".PadRight(10) + "MFlag".PadRight(10) + "\n");
            PrintTree(tree);    //call the private function
            Console.WriteLine();
        }

       /**********************************************************************
        *** FUNCTION: Search                                               ***
        **********************************************************************
        *** DESCRIPTION: Search for the symbol by calling private function ***
        *** INPUT ARGS: string value                                       ***
        *** OUTPUT ARGS: NONE                                              ***
        *** IN/OUT ARGS: NONE                                              ***
        *** RETURN: Leaf Tree                                              ***
        **********************************************************************/
        public Leaf Search(string value)
        {
            _ = new Leaf();     //declare variable
            Leaf Tree = Search(tree, value);    //call private function
            return Tree;    //return the leaf value
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
            if (root == null)   //if there is nothing in the tree then return nothing
               return null;
            else if (string.Compare(root.Element.Element, value) < 0)   //if it is greater than then go to right
                return Search(root.Right, value);
            else if (string.Compare(root.Element.Element, value) > 0)   //if it is less than then go to left
                return Search(root.Left, value);
            else if (string.Compare(root.Element.Element, value) == 0)  //if it is equal return that
                return root;

            return null; //if nothing then return nothing
        }

       /**********************************************************************
        *** FUNCTION: Insert                                               ***
        **********************************************************************
        *** DESCRIPTION: Insert into the bin search tree                   ***
        *** INPUT ARGS: Symbol symbol Leaf root                            ***
        *** OUTPUT ARGS: NONE                                              ***
        *** IN/OUT ARGS: NONE                                              ***
        *** RETURN: NONE                                                   ***
        **********************************************************************/
        private void Insert(Symbol symbol, ref Leaf root)
        {
            if (symbol.Element.Length < 6)      //if its shorter than 6 trim 
                symbol.Element = symbol.Element.Trim();
            else    //else make it 6 characters long
                symbol.Element = symbol.Element.Substring(0, 6);

            var newNode = new Leaf  //declare newNode from leaf
            {
                Left = null,
                Right = null,
                Element = symbol
            };

            if (root == null)   //if there is nothing in it make it the first
                root = newNode;
            else if (string.Compare(symbol.Element, root.Element.Element) < 0)  //if it is less than then go left
                Insert(symbol, ref root.Left);
            else if (string.Compare(symbol.Element, root.Element.Element) > 0)  //if it is greater than then go right
                Insert(symbol, ref root.Right);
            else if (string.Compare(symbol.Element, root.Element.Element) == 0) //if it is already there throw error
            {
                root.Element.MFlag = true;
                Console.WriteLine("|ERROR| Symbol: {0}  already exists", symbol.Element);
            }
        }

        //private int lines = 0;
       /**********************************************************************
        *** FUNCTION: Insert                                               ***
        **********************************************************************
        *** DESCRIPTION: recursive view the symbol table                   ***
        *** INPUT ARGS: Leaf myTree                                        ***
        *** OUTPUT ARGS: NONE                                              ***
        *** IN/OUT ARGS: NONE                                              ***
        *** RETURN: NONE                                                   ***
        **********************************************************************/
        private void PrintTree(Leaf myTree)
        {
            if (myTree != null) //if there is something in the tree
            {
                if (myTree.Left != null)
                    PrintTree(myTree.Left); //printtree left
                Console.WriteLine("{0,-10}{1,-10}{2,-10}{3,-10}{4,-10}", myTree.Element.Element, myTree.Element.Value, myTree.Element.RFlag ? 1 : 0, myTree.Element.IFlag ? 1 : 0, myTree.Element.MFlag ? 1 : 0);
                if (myTree.Right != null)   //printtree right
                    PrintTree(myTree.Right);

            }
        }
    }
}

