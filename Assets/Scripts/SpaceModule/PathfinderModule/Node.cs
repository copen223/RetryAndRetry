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
        }

        public int x;
        public int y;
        public float cost;  // 寻路算法中的费用，代表起点到该节点的距离费用

        public Node parent;

        public List<Edge> Edges = new List<Edge>();
        public void AddEdge(Edge edge)
        {
            if (Edges.Contains(edge))
                return;
            Edges.Add(edge);
        }

        public Edge EdgeWithNode(Node target)
        {
            foreach(Edge edge in Edges)
            {
                if (edge.AnotherNode(this) == target)
                    return edge;
            }
            return null;
        }
    }
}
