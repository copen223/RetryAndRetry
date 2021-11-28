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

            origin.AddEdge(this);
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

        public List<(float, float)> PassWorldPositions = new List<(float, float)>();
        /// <summary>
        /// 设置经过位点，从origin移动到end位点时，会经过世界坐标中哪些位点
        /// </summary>
        /// <param name="list"></param>
        public void SetPassWorldPositions(List<(float, float)> list)
        {
            PassWorldPositions = list;
        }

        public Node AnotherNode(Node _node)
        {
            return _node == origin ? end : origin;
        }

        public override bool Equals(object obj)
        {
            Edge go = obj as Edge;
            if (origin == go.origin && end == go.end && weight == go.weight)
                return true;
            else
                return false;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
