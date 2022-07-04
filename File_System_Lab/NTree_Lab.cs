using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace File_System_Lab
{
    delegate void TreeVisitor(F_Lab nodeData);

    class NTree_Lab
    {
        public F_Lab Data { get; set; }
        public LinkedList<NTree_Lab> Children { get; }

        public NTree_Lab(F_Lab data)
        {
            this.Data = data;
            Children = new LinkedList<NTree_Lab>();
        }

        public NTree_Lab(NTree_Lab nTree)
        {
            this.Data = new F_Lab(nTree.Data);
            this.Children = nTree.Children;
        }

        public void AddChild(F_Lab data)
        {
            Children.AddFirst(new NTree_Lab(data));
        }

        public void AddChildTree(NTree_Lab data)
        {
            Children.AddFirst(data);
        }

        public void RemoveChild(int i)
        {
            NTree_Lab temp = GetChild(i);
            Children.Remove(temp);
        }

        public NTree_Lab GetChild(int i)
        {
            foreach (NTree_Lab n in Children)
                if (--i == 0)
                    return n;
            return null;
        }

        public void Traverse(NTree_Lab node, TreeVisitor visitor)
        {
            visitor(node.Data);
            foreach (NTree_Lab kid in node.Children)
                Traverse(kid, visitor);
        }

        public NTree_Lab FindChild(String name, NTree_Lab nTree)
        {
            if (name == "\\" || nTree.Data.Name == name.Remove(0, 1) || nTree.Data.Name == name)
                return nTree;

            else
            {
                String[] path = name.Split("\\\\".ToCharArray());

                if (path.Length <= 1) return null;

                for (int i = 0; i < nTree.Children.Count; i++)
                    if (nTree.GetChild(i + 1).Data.Name == path[1])
                        return FindChild(name.Remove(0, path[0].Length + 1), nTree.GetChild(i + 1));

                return null;
            }
        }
    }
}
