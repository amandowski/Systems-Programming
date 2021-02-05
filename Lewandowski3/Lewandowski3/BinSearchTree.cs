using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lewandowski3
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
        public Leaf tree;

        /********************************************************************
        *** FUNCTION    : BinSearchTree()                                 ***
        *********************************************************************
        *** DESCRIPTION : Constructor                                     ***
        *** INPUT ARGS  : NONE                                            ***
        *** OUTPUT ARGS : NONE                                            ***
        *** RETURN      : NONE                                            ***
        *********************************************************************/
        public BinSearchTree()
        { tree = null; }

        /********************************************************************
        *** FUNCTION    : Insert                                          ***
        *********************************************************************
        *** DESCRIPTION : Insert the symbol into the tree                 ***
        *** INPUT ARGS  : Symbol sym                                      ***
        *** OUTPUT ARGS : NONE                                            ***
        *** RETURN      : void                                            ***
        *********************************************************************/
        public void Insert(Symbol sym)
        {
            sym.Element = sym.Element.Length > 6 ? sym.Element.Remove(6) : sym.Element;
            sym.Element = sym.Element.ToUpper();
            if (sym.Element.Contains(':'))
                sym.Element = sym.Element.Replace(":", string.Empty);
            Insert(sym, ref tree);
        }

        /********************************************************************
        *** FUNCTION    : Insert                                          ***
        *********************************************************************
        *** DESCRIPTION : private insert symbol to leaf                   ***
        *** INPUT ARGS  : Symbol sym, ref Leaf tree                       ***
        *** OUTPUT ARGS : NONE                                            ***
        *** RETURN      : void                                            ***
        *********************************************************************/
        private void Insert(Symbol sym, ref Leaf tree)
        {
            if (tree == null)
            {
                tree = new Leaf()
                {
                    Element = sym,
                    Left = null,
                    Right = null
                };
            }
            else if (string.Compare(sym.Element, tree.Element.Element) < 0)
            {
                Insert(sym, ref tree.Left);
                tree.Element.MFlag = false;
            }
            else if (string.Compare(sym.Element, tree.Element.Element) > 0)
            {
                Insert(sym, ref tree.Right);
                tree.Element.MFlag = false;
            }
            else
                tree.Element.MFlag = true;
        }

        /********************************************************************
        *** FUNCTION    : Search                                          ***
        *********************************************************************
        *** DESCRIPTION : public search using symbol                      ***
        *** INPUT ARGS  : Symbol sym                                      ***
        *** OUTPUT ARGS : NONE                                            ***
        *** RETURN      : Symbol                                          ***
        *********************************************************************/
        public Symbol Search(Symbol sym)
        {
            sym.Element = sym.Element.Length > 6 ? sym.Element.Remove(6) : sym.Element;
            return Search(sym, tree).Element;
        }

        /********************************************************************
        *** FUNCTION    : Search                                          ***
        *********************************************************************
        *** DESCRIPTION : public search using string                      ***
        *** INPUT ARGS  : string sym                                      ***
        *** OUTPUT ARGS : NONE                                            ***
        *** RETURN      : Symbol                                          ***
        *********************************************************************/
        public Symbol Search(string sym)
        {
            var returnSymbol = Search(new Symbol() { Element = sym = sym.Length > 6 ? sym.Remove(6) : sym }, tree);
            if (returnSymbol == null)
                throw new KeyNotFoundException("||ERROR||" + sym + ": Symbol not found");
            else
                return returnSymbol.Element;
        }

        /********************************************************************
        *** FUNCTION    : Search                                          ***
        *********************************************************************
        *** DESCRIPTION : private symbol search func                      ***
        *** INPUT ARGS  : Symbol sym, Leaf tree                           ***
        *** OUTPUT ARGS : NONE                                            ***
        *** RETURN      : Leaf                                            ***
        *********************************************************************/
        private Leaf Search(Symbol sym, Leaf tree)
        {
            if (tree == null)
                return null;
            else if (string.Compare(sym.Element, tree.Element.Element) < 0)
                return Search(sym, tree.Left);
            else if (string.Compare(sym.Element, tree.Element.Element) > 0)
                return Search(sym, tree.Right);
            else
                return tree;
        }

        /********************************************************************
        *** FUNCTION    : Display                                         ***
        *********************************************************************
        *** DESCRIPTION : public call to display                          ***
        *** INPUT ARGS  : NONE                                            ***
        *** OUTPUT ARGS : NONE                                            ***
        *** RETURN      : void                                            ***
        *********************************************************************/
        public void Display()
        { View(tree); }

        /********************************************************************
        *** FUNCTION    : View                                            ***
        *********************************************************************
        *** DESCRIPTION : private display function                        ***
        *** INPUT ARGS  : Leaf tree                                       ***
        *** OUTPUT ARGS : NONE                                            ***
        *** RETURN      : void                                            ***
        *********************************************************************/
        private void View(Leaf tree)
        {
            if (tree != null)
            {
                tree.Element.IFlag = true;
                _ = tree.Element;
                if (tree.Left != null)
                    View(tree.Left);
                Console.WriteLine("{0, -10} {1, -10} {2, -10} {3, -10} {4, -10}", tree.Element.Element, tree.Element.Value.ToString("X"), ChangeFlag(tree.Element.RFlag), ChangeFlag(tree.Element.IFlag), ChangeFlag(tree.Element.MFlag));
                if (tree.Right != null)
                    View(tree.Right);
            }
        }

        /********************************************************************
        *** FUNCTION    : ChangeFlag                                      ***
        *********************************************************************
        *** DESCRIPTION : change the flag value                           ***
        *** INPUT ARGS  : bool flag                                       ***
        *** OUTPUT ARGS : NONE                                            ***
        *** RETURN      : int val                                         ***
        *********************************************************************/
        private int ChangeFlag(bool flag)
        {
            int val;
            if (flag == true)
                val = 1;
            else
                val = 0;
            return val;
        }
    }
}
