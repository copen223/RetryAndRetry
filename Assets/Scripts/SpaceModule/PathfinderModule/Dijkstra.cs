using System;
using System.Collections.Generic;

namespace SpaceModule.PathfinderModule
{
    public class Dijkstra
    {
        private Gragh gragh;   
        public Dijkstra(Gragh _gragh)
        {
            gragh = _gragh;
        }

        private List<Node> open_list = new List<Node>();    // 探测到的节点
        private List<Node> close_list = new List<Node>();   // 已确定最短路径的节点

        private List<Node> moveTargets_list = new List<Node>();     // 按设定方法确定的可移动节点列表 一般由AI使用


        /// <summary>
        /// 从开始节点开始寻找前往所有节点的最短路径
        /// </summary>
        /// <param name="start"></param>
        public void SearchShortPathFrom(Node _start)
        {
            // 初始化
            open_list.Clear(); close_list.Clear();

            // 加入初始节点
            open_list.Add(_start); _start.cost = 0;
            Node cur = _start;

            while (open_list.Count > 0)
            {
                // openlist 根据费用升序排序，获得费用最小节点，说明该节点路径已被确定，加入close列表，后续的搜索不改变其cost和parent值
                open_list.Sort((x, y) => x.cost.CompareTo(y.cost));
                // compareTo方法在调用者大时返回1否则返回0 和 -1；Sort传入的委托参数x，y，返回1代表把x放右边。所以用x.compare函数作为委托的输出时，就是x大放右边,升序排序。
                cur = open_list[0];

                // 标记该节点，并检索相邻节点，更新他们的cost
                open_list.Remove(cur); close_list.Add(cur);
                UpdateLinkedNodes(cur);

            }
        }

        /// <summary>
        /// 在搜索路径，遍历Node的同时会使用checkFunc对节点进行检测，如果节点满足检查条件则加入列表并返回
        /// </summary>
        /// <param name="_start"></param>
        /// <param name="_checkFunc"></param>
        /// <returns></returns>
        public List<Node> SearchShortPathFromAndGetMoveTargets(Node _start, Predicate<Node> _checkFunc)
        {
            // 初始化
            open_list.Clear(); close_list.Clear(); moveTargets_list.Clear();

            // 加入初始节点
            open_list.Add(_start); _start.cost = 0;
            Node cur = _start;

            while (open_list.Count > 0)
            {
                // openlist 根据费用升序排序，获得费用最小节点，说明该节点路径已被确定，加入close列表，后续的搜索不改变其cost和parent值
                open_list.Sort((x, y) => x.cost.CompareTo(y.cost));
                // compareTo方法在调用者大时返回1否则返回0 和 -1；Sort传入的委托参数x，y，返回1代表把x放右边。所以用x.compare函数作为委托的输出时，就是x大放右边,升序排序。
                cur = open_list[0];

                if (_checkFunc(cur))
                    moveTargets_list.Add(cur);

                // 标记该节点，并检索相邻节点，更新他们的cost
                open_list.Remove(cur); close_list.Add(cur);
                UpdateLinkedNodes(cur);
            }

            moveTargets_list.Sort((n1, n2) =>
            {
                if (n1.cost > n2.cost)
                    return 1;
                else if (n1.cost < n2.cost)
                    return 0;
                else
                    return -1;
            });

            return moveTargets_list;
        }

        private void UpdateLinkedNodes(Node _cur)
        {
            foreach(var edge in _cur.Edges)
            {
                Node link = edge.AnotherNode(_cur);

                if (close_list.Contains(link))
                    continue;

                // 更新费用
                UpdateCost(link, _cur);
                if(!open_list.Contains(link)) open_list.Add(link);
            }
        }

        private void UpdateCost(Node link,Node last)
        {
            Edge edge = last.EdgeToNode(link);
            // 松弛函数
            if(link.cost > edge.weight + last.cost)
            {
                link.cost = edge.weight + last.cost;
                // 费用降低，说明该路径目前费用最小，更新parent
                link.parent = last;
                // 应用中间经过位置
                link.SetPrePassWorldPositions(edge.PassWorldPositions);
            }
        }

    }
}
