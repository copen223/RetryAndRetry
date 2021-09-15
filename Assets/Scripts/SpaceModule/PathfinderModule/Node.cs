using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.SpaceModule.PathfinderModule
{
    public class Node
    {
        public Node(int _x,int _y)
        {
            x = _x;
            y = _y;
            cost = 9999f;
            parent = this;
        }

        public int x;
        public int y;
        public float cost;  // 寻路算法中的费用，代表起点到该节点的距离费用

        public Node parent;

        public List<Edge> Edges = new List<Edge>();
        public void AddEdge(Edge edge)
        {
            // UnityEngine.Debug.Log(x + "," + y);
            if (Edges.Contains(edge))
                return;
            else
                Edges.Add(edge);
        }

        public Edge EdgeToNode(Node target)
        {
            foreach(Edge edge in Edges)
            {
                if (edge.AnotherNode(this) == target)
                    return edge;
            }
            return null;
        }

        public override bool Equals(object obj)
        {
            return ((obj as Node).x == x) && ((obj as Node).y == y);
        }
        public override int GetHashCode()
        {
            return x * 10000 + y;
        }

        public float Distance((int,int) pos)
        {
            return Math.Abs(pos.Item1 - x) + Math.Abs(pos.Item2 - y);
        }
    }
}
