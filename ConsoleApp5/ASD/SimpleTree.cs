using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ConsoleApp5
{
    public class SimpleTreeNode<T>
    {
        public T NodeValue;
        public SimpleTreeNode<T> Parent;
        public List<SimpleTreeNode<T>> Children;
        public int Depth = 0;

        public SimpleTreeNode(T val, SimpleTreeNode<T> parent)
        {
            NodeValue = val;
            Parent = parent;
            Children = new List<SimpleTreeNode<T>>();
            Depth = 0;
        }
        public SimpleTreeNode(T val)
        {
            NodeValue = val;
            Parent = null;
            Children = new List<SimpleTreeNode<T>>();
            Depth = 0;
        }
    }
    public class SimpleTree<T>
    {
        public static SimpleTreeNode<T> Root;
        public SimpleTree(SimpleTreeNode<T> root)
        {
            Root = root;
        }
        public void AddChild(SimpleTreeNode<T> ParentNode, SimpleTreeNode<T> NewChild)
        {
            if (ParentNode == null || NewChild == null) return;

            if (NodeExist(ParentNode))
            {
                ParentNode.Children.Add(NewChild);
                NewChild.Parent = ParentNode;
            }
        }
        private static bool NodeExist(SimpleTreeNode<T> node)
        {
            if (Equals(Root, node)) return true;
            bool exist = false;
            return NodeExistStart(Root, node, ref exist);
        }
        private static bool NodeExistStart(SimpleTreeNode<T> root, SimpleTreeNode<T> node, ref bool exist)
        {
            foreach (var child in root.Children)
            {
                if (Equals(child, node))
                {
                    exist = true;
                    return exist;
                }
                NodeExistStart(child, node, ref exist);
            }
            return exist;
        }

        public void DeleteNode(SimpleTreeNode<T> NodeToDelete)
        {
            if (NodeToDelete == null) return;

            if (Equals(Root, NodeToDelete))
            {
                Root = null;
                return;
            }
            StartDeleteNode(Root, NodeToDelete);
        }
        private void StartDeleteNode(SimpleTreeNode<T> root, SimpleTreeNode<T> NodeToDelete)
        {
            for (int i = 0; i < root.Children.Count; i++)
            {
                if (Equals(root.Children[i], NodeToDelete))
                {
                    root.Children.RemoveAt(i);
                    return;
                }
                StartDeleteNode(root.Children[i], NodeToDelete);
            }
        }

        public List<SimpleTreeNode<T>> GetAllNodes()
        {
            var result = new List<SimpleTreeNode<T>>() { Root };
            int depth = 1;
            StartGetAllNodes(Root, ref result, ref depth);
            return result;
        }
        private static void StartGetAllNodes(SimpleTreeNode<T> root, ref List<SimpleTreeNode<T>> result, ref int depth)
        {
            foreach (var child in root.Children)
            {
                depth++;

                child.Depth = depth;
                result.Add(child);
                StartGetAllNodes(child, ref result, ref depth);
            }
            depth--;
            return;
        }

        public List<SimpleTreeNode<T>> FindNodesByValue(T val)
        {
            if (Root == null) return new List<SimpleTreeNode<T>>();

            var result = new List<SimpleTreeNode<T>>();

            if (Equals(Root.NodeValue, val))
            {
                result.Add(Root);
            }
            StartFindNodesByValue(Root, val, ref result);

            return result;
        }
        private void StartFindNodesByValue(SimpleTreeNode<T> root, T val, ref List<SimpleTreeNode<T>> result)
        {
            foreach (var child in root.Children)
            {
                if (Equals(child.NodeValue, val))
                {
                    result.Add(child);
                }
                StartFindNodesByValue(child, val, ref result);
            }
            return;
        }

        public void MoveNode(SimpleTreeNode<T> OriginalNode, SimpleTreeNode<T> NewParent)
        {
            if (OriginalNode == null || NewParent == null) return;

            if (!NodeExist(OriginalNode) && !NodeExist(NewParent)) return;

            var childrens = OriginalNode.Parent.Children;
            var buff = OriginalNode;

            for (int i = 0; i < childrens.Count; i++)
            {
                if (Equals(childrens[i], OriginalNode))
                {
                    childrens.RemoveAt(i);
                    NewParent.Children.Add(buff);
                    buff.Parent = NewParent;
                    return;
                }
            }
        }

        public int Count()
        {
            if (Root == null) return 0;

            var count = 1; // ++root
            StartCount(Root, ref count);
            return count;
        }

        public void StartCount(SimpleTreeNode<T> root, ref int count)
        {
            foreach (var child in root.Children)
            {
                count++;
                StartCount(child, ref count);
            }
            return;
        }
        public int LeafCount()
        {
            if (Root == null) return 0;

            var count = 0;
            StartLeafCount(Root, ref count);
            return count;
        }
        private void StartLeafCount(SimpleTreeNode<T> root, ref int count)
        {
            foreach (var child in root.Children)
            {
                StartLeafCount(child, ref count);
            }
            if (root.Children.Count == 0) count++;

            return;
        }

        public List<T> EvenTrees()
        {
            if (Root == null) return new List<T>();

            List<SimpleTreeNode<T>> subTrees = new List<SimpleTreeNode<T>>() { Root };
            List<T> forest = new List<T>();

            EvenTreesSearch(Root, ref subTrees, ref forest);
            return forest;
        }

        private void EvenTreesSearch(SimpleTreeNode<T> root, ref List<SimpleTreeNode<T>> subTrees, ref List<T> forest)
        {
            if (root.Children.Count == 0 && subTrees.Count % 2 == 0)
            {
                if (subTrees[0].Parent != null)
                {
                    forest.Add(subTrees[0].Parent.NodeValue);
                    forest.Add(subTrees[0].NodeValue);
                }
                subTrees.Clear();
            }

            foreach (var child in root.Children)
            {
                subTrees.Add(child);

                EvenTreesSearch(child, ref subTrees, ref forest);
            }

            return;
        }

        public List<SimpleTreeNode<T>> WideAllNodes()
        {
            if (Root == null) return null;

            List<SimpleTreeNode<T>> result = new List<SimpleTreeNode<T>>() { Root };
            List<SimpleTreeNode<T>> currLayer = new List<SimpleTreeNode<T>>();

            foreach (var child in Root.Children) 
            { 
                result.Add(child);
                currLayer.Add(child);
            }

            StartWideAllNodes(ref result, ref currLayer);
            return result;
        }

        private void StartWideAllNodes(ref List<SimpleTreeNode<T>> result, ref List<SimpleTreeNode<T>> currLayer)
        {
            if (result.Count == Count()) return;
            List<SimpleTreeNode<T>> nextLayer = new List<SimpleTreeNode<T>>();

            foreach (SimpleTreeNode<T> node in currLayer)
            {
                if (node == null) continue;

                nextLayer.AddRange(node.Children);
            }
            currLayer = nextLayer;
            result.AddRange(currLayer);

            StartWideAllNodes(ref result, ref currLayer);
        }
    }
}
