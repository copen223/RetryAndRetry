using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.SpaceModule.PathfinderModule
{
    public class Edge
    {
        public Edge(Node _origin, Node _end,int _weight)
        {
            origin = _origin;
            end = _end;
            weight = _weight;
            hasDir = 0;
        }
        public Edge(Node _origin, Node _end, int _weight,bool _hasDir)
        {
            origin = _origin;
            end = _end;
            weight = _weight;
            hasDir = _hasDir?1:0;
        }

        public Node origin; // 起点
        public Node end;    // 终点
        public int weight;  // 权值
        public int hasDir;  // 是否有向

        public Node AnotherNode(Node _node)
        {
            return _node == origin ? end : origin;
        }
    }
}
