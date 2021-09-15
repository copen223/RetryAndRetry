using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.SpaceModule.PathfinderModule
{
    public class Gragh
    {
        public List<Edge> Edges_list = new List<Edge>();
        public readonly Dictionary<(int, int), Node> Nodes_dir = new Dictionary<(int, int), Node>();
        public Gragh()
        {

        }

        /// <summary>
        /// 获取gragh中的node，如果不存在该位置的node，则return null
        /// </summary>
        /// <returns></returns>
        public Node GetNode((int,int) intPos)
        {
            if (Nodes_dir.ContainsKey((intPos.Item1, intPos.Item2)))
                return Nodes_dir[(intPos.Item1, intPos.Item2)];
            else return null;
                
        }
        /// <summary>
        /// 获取gragh中的node，如果不存在，则用设定node进行填充
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public Node SetOrGetNode(Node node)
        {
            var intPos = (node.x, node.y);
            if (Nodes_dir.ContainsKey((intPos.Item1, intPos.Item2)))
                return Nodes_dir[(intPos.Item1, intPos.Item2)];
            else
            {
                Nodes_dir[intPos] = node;
                return node;
            }
        }

        public void AddEdge(Edge edge)
        {
            if(!Edges_list.Contains(edge))
                Edges_list.Add(edge);
        }
    }
}
