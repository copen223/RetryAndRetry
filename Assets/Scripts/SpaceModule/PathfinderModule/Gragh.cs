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
        public List<Node> Nodes_list = new List<Node>();
        public Gragh(IEnumerable<Edge> _edges,IEnumerable<Node> _nodes)
        {
            Edges_list = _edges as List<Edge>;
            Nodes_list = _nodes as List<Node>;
        }
    }
}
