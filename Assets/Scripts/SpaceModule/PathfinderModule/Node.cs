using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.SpaceModule.PathfinderModule
{
    public class BeatBackInfomation
    {
        public UnityEngine.GameObject target;
        public UnityEngine.Vector2 dir;
        public int dis;
        public BeatBackInfomation(UnityEngine.GameObject _target, UnityEngine.Vector2 _dir,int _dis)
        {
            target = _target; dir = _dir; dis = _dis;
        }
    }



    /// <summary>
    /// 人物通过什么方式到达该节点
    /// </summary>
    public enum ActorActionToNode
    {
        None,
        Walk,
        Climb,
        JumpV,
        JumpH,
        Fall
    }

    public class Node
    {
        public Node(int _x,int _y)
        {
            x = _x;
            y = _y;
            cost = 9999f;
            parent = this;
        }

        public Node(int _x,int _y,float _worldX,float _worldY)
        {
            x = _x; y = _y;
            worldX = _worldX; worldY = _worldY;
            cost = 9999f;parent = this;
        }

        /// <summary>
        /// 建图时修改此项以储存人物在节点上移动的信息，确定动画等事件
        /// </summary>
        public ActorActionToNode ActionToNode = ActorActionToNode.None;
        /// <summary>
        /// 一般在寻路时处理事件时使用
        /// </summary>
        public event Action ArriveThisNodeEvent;
        /// <summary>
        /// 由ActorMoveComponent调用
        /// </summary>
        public void InvokeAriiveThisNodeEvent() { ArriveThisNodeEvent?.Invoke(); }

        /// <summary>
        /// 达到改节点时引发的碰撞信息
        /// </summary>
        public BeatBackInfomation BeatBackInfomation;

        /// <summary>
        /// 下落到该节点，下落了多少格
        /// </summary>
        public int FallCount = 0;


        readonly public float worldX;
        readonly public float worldY;

        readonly public int x;
        readonly public int y;
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
