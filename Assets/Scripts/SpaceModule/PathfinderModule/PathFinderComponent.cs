using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;
using Assets.Scripts.SpaceModule.PathfinderModule;
using System;

public class PathFinderComponent : MonoBehaviour
{
    private Tilemap groundTilemap;
    private Tilemap platformTilemap;
    private Tilemap ladderTilemap;
    private Grid grid;
    private int mapMargin = 5;
    public List<Vector3> CurPath;
    public List<Node> CurNodePath;

    private Map map; // 扫描场景获得的地图数据
    private Gragh gragh; // 根据map生成的图
    private Dijkstra dijkstra;

    //--------属性值配置--------//
    private int CostPerUnit_Walk = 4;
    private int CostPerUnit_Climb = 1;
    private int CostPerUnit_JumpV = 1;
    private int CostPerUnit_JumpH = 8;
    private int CostPerUnit_PassE = 24;
    private int CostPerUnit_PassF = 8;


    //--------------------------//

    private void Start()
    {
        groundTilemap = GameObject.Find("GroundTilemap").GetComponent<Tilemap>();
        platformTilemap = GameObject.Find("PlatformTilemap").GetComponent<Tilemap>();
        ladderTilemap = GameObject.Find("LadderTilemap").GetComponent<Tilemap>();
        grid = groundTilemap.layoutGrid;
    }

    /// <summary>
    /// 通过射线检测方式扫描整个场景，以更新map
    /// </summary>
    private void GenerateMapByRaycast()
    {
        map = new Map();
        BoundsInt groundBounds = groundTilemap.cellBounds;
        BoundsInt platformBounds = platformTilemap.cellBounds;
        BoundsInt ladderBounds = ladderTilemap.cellBounds;

        int xMin = Mathf.Min(groundBounds.xMin, platformBounds.xMin, ladderBounds.xMin);
        int xMax = Mathf.Max(groundBounds.xMax, platformBounds.xMax, ladderBounds.xMax);
        int yMin = Mathf.Min(groundBounds.yMin, platformBounds.yMin, ladderBounds.yMin);
        int yMax = Mathf.Max(groundBounds.yMax, platformBounds.yMax, ladderBounds.yMax);

        BoundsInt bounds = new BoundsInt();
        bounds.xMin = xMin- mapMargin; bounds.xMax = xMax+ mapMargin; 
        bounds.yMin = yMin- mapMargin; bounds.yMax = yMax+ mapMargin;
        bounds.zMax = 1;bounds.zMin = 0;

        Grid gridLayout = groundTilemap.layoutGrid;

        // 创建cell，填充type
        foreach(var pos in bounds.allPositionsWithin)
        {
            MapCell cell = new MapCell((pos.x,pos.y));
            var rayPos = gridLayout.GetCellCenterWorld(pos);
            var hit = Physics2D.Raycast(rayPos,Vector2.zero,4f,LayerMask.GetMask("Terrain"));
            var hit_actor = Physics2D.Raycast(rayPos, Vector2.zero, 4f, LayerMask.GetMask("Actor"));    // 对象检测用hit
            // 判定cell类型
            cell.height = -200;
            if (map.map_dic.ContainsKey((pos.x, pos.y - 1)))
                cell.height = map.map_dic[(pos.x, pos.y - 1)].height + 1;
            if (hit.collider == null)
                cell.Type = MapCellType.Empty;
            else if (hit.collider.tag == "Obstacle")
            { cell.Type = MapCellType.Ground; cell.height = -1; }
            else if (hit.collider.tag == "Platform")
            { cell.Type = MapCellType.Platform; cell.height = -1; }
            else if (hit.collider.tag == "Ladder")
                cell.Type = MapCellType.Ladder;

            if(hit_actor.collider != null)
            {
                GameObject gb = hit_actor.transform.parent.gameObject;
                var actor = gb.GetComponent<ActorController>();
                var self = gameObject.GetComponent<ActorController>();
                if (actor.group.type != self.group.type)
                {
                    cell.Type = MapCellType.EnemyActor;
                }
                else if(actor != self)
                    cell.Type = MapCellType.FriendActor;
            }

            map.map_dic[(pos.x, pos.y)] = cell;
            
        }

        // 判断Cell的单位停留类型
        foreach(var mapCell in map.map_dic)
        {
            MapCell cell = mapCell.Value;
            (int, int) pos = mapCell.Key;

            cell.StayState = ObjectStayState.Fall;

            if (cell.Type == MapCellType.EnemyActor || cell.Type == MapCellType.Ground ||cell.Type == MapCellType.FriendActor)
                cell.StayState = ObjectStayState.CantHold;

            if (cell.Type == MapCellType.Empty || cell.Type == MapCellType.Ladder)
            {
                // climb要弱于stand，所以先判断，若可站立，后续代码会覆盖该状态
                if (cell.Type == MapCellType.Ladder)
                    cell.StayState = ObjectStayState.Climb;

                if (map.map_dic.ContainsKey((pos.Item1, pos.Item2 - 1)))
                {
                    // 下方为地面，自身为空、梯，则为stand
                    var cellType = map.map_dic[(pos.Item1, pos.Item2 - 1)].Type;
                    if(cellType == MapCellType.Ground || cellType == MapCellType.Platform)
                    {
                        cell.StayState = ObjectStayState.Stand;
                    }

                    //如果上方4格子内含有Ground，则表明空间不足，不可容纳
                    for (int i = 0; i < 4; i++)
                    {
                        if (map.map_dic.ContainsKey((pos.Item1, pos.Item2 + i + 1)))
                        {
                            if (map.map_dic[(pos.Item1, pos.Item2 + i + 1)].Type == MapCellType.Ground ||
                                map.map_dic[(pos.Item1, pos.Item2 + i + 1)].Type == MapCellType.EnemyActor ||
                                map.map_dic[(pos.Item1, pos.Item2 + i + 1)].Type == MapCellType.FriendActor)
                            {
                                cell.StayState = ObjectStayState.CantHold;
                            }
                        }
                    }

                }
            }
        }
        

    }

    /// <summary>
    /// 根据map创建节点和边
    /// </summary>
    private void GenerateGraghByMap()
    {
        gragh = new Gragh();

        foreach (var mapCell in map.map_dic)
        {
            MapCell cell = mapCell.Value;
            int x = mapCell.Key.Item1;
            int y = mapCell.Key.Item2;
            (int, int) pos = mapCell.Key;

            if(cell.StayState == ObjectStayState.Stand || cell.StayState == ObjectStayState.Climb)
            {
                CreatEdgeByWalkCheck(x, y); // 行走带来的路径
                CreatEdgeByClimbCheck(x, y);
                CreatEdgeByPassActorCheck(x, y);
            }
            if(cell.StayState == ObjectStayState.Stand)
            {
                CreatEdgeByJumpHorizontalCheck(x, y);
                CreatEdgeByJumpVerticalCheck(x, y);
            }

        }
    }

    #region 外部调用
    ///// <summary>
    ///// 进行一次搜索，获得所有路径，存储在图中
    ///// </summary>
    ///// <param name="x"></param>
    ///// <param name="y"></param>
    //public void SearchPathFrom(int x,int y)
    //{
    //    // 读图
    //    GenerateMapByRaycast();
    //    GenerateGraghByMap();

    //    // 寻路 
    //    dijkstra = new Dijkstra(gragh);
    //    Node start = gragh.GetNode((x, y));
    //    dijkstra.SearchShortPathFrom(start);
    //}

    /// <summary>
    /// 进行一次搜索，获得所有路径，存储在图中
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public void SearchPathFrom(Vector2 worldPos)
    {
        // 读图
        GenerateMapByRaycast();
        GenerateGraghByMap();

        // 寻路 
        dijkstra = new Dijkstra(gragh);
        var mapPos = grid.WorldToCell(worldPos);
        int x = mapPos.x;int y = mapPos.y;
        Node start = gragh.GetNode((x, y));
        dijkstra.SearchShortPathFrom(start);
    }

    /// <summary>
    /// 建图的同时，根据checkfunc进行检测，返回满足条件的节点
    /// </summary>
    /// <param name="worldPos"></param>
    /// <param name="_checkFunc"></param>
    /// <returns></returns>
    public List<Node> SearchPathForm(Vector2 worldPos, Predicate<Node> _checkFunc)
    {
        // 读图
        GenerateMapByRaycast();
        GenerateGraghByMap();

        // 寻路 
        dijkstra = new Dijkstra(gragh);
        var mapPos = grid.WorldToCell(worldPos);
        int x = mapPos.x; int y = mapPos.y;
        Node start = gragh.GetNode((x, y));
        return dijkstra.SearchShortPathFromAndGetMoveTargets(start,_checkFunc);
    }

    /// <summary>
    /// 搜索后调用，获得指定路径,确保已调用searchpathfrom
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <returns></returns>
    private Stack<(int, int)> GetPathFromTo((int,int)start,(int,int)end)
    {
        Stack<(int, int)> path = new Stack<(int, int)>();
        Node cur = gragh.GetNode(end);
        if(cur == null)
        {
           // Debug.Log("noNode at" + end);
            return path;
        }

        while(true)
        {
            var pos = (cur.x, cur.y);
            if ((cur.x, cur.y) == start) // 目标节点是开始节点，则完成路径搜索，返回一个带有开始节点的路径
            {
                path.Push(pos);
                return path;
            }
            if (cur.parent == cur)  // 目标节点的parent是自己，代表没有路径，直接返回一个空的path
            {
                
                return path;
            }
            path.Push(pos);
             //Debug.Log("世界" + tilemap.GetCellCenterWorld(new Vector3Int(pos.x, pos.y, 0)) + "Cell" + pos.x +","+ pos.y);
            cur = cur.parent;
        }
    }

    /// <summary>
    /// 搜索后调用，获得指定路径,确保已使用dijkstra，即调用searchpathfrom
    /// 其获得的是一个带有开始节点的路径，长度至少为1
    /// 如果没有路径，则长度为0
    /// </summary>
    /// <param name="startPos_world"></param>
    /// <param name="endPos_world"></param>
    /// <returns></returns>
    public List<Vector3> GetPathFromTo(Vector3 startPos_world, Vector3 endPos_world)
    {
        List<Vector3> path_world = new List<Vector3>();
        Vector3Int startPos_map = grid.WorldToCell(startPos_world);
        Vector3Int endPos_map = grid.WorldToCell(endPos_world);

        //Debug.Log(startPos_map.x);

        Stack<(int, int)> path_stack = GetPathFromTo((startPos_map.x, startPos_map.y), (endPos_map.x, endPos_map.y));
        int count = path_stack.Count;
        for (int i = 0;i < count; i++)
        {
            (int, int) point_map = path_stack.Pop();
            Vector3 point_world = grid.GetCellCenterWorld(new Vector3Int(point_map.Item1, point_map.Item2, 0));
            path_world.Add(point_world);
        }

        CurPath = path_world;
        return path_world;
    }

    /// <summary>
    /// 该方法用于确定离目标点最近的可用点的路径,确保已使用dijkstra，即调用searchpathfrom
    /// </summary>
    /// <param name="startPos_world"></param>
    /// <param name="endPos_world"></param>
    /// <returns></returns>
    public List<Vector3> GetPathFromToNearst(Vector3 startPos_world, Vector3 endPos_world)
    {
        List<Vector3> path_world = new List<Vector3>();
        Vector3Int startPos_map = grid.WorldToCell(startPos_world);
        Vector3Int endPos_map = grid.WorldToCell(endPos_world);

        List<Node> nearstNodes = new List<Node>();

        foreach(var node in gragh.Nodes_dir)
        {
            nearstNodes.Add(node.Value);
        }
        nearstNodes.Sort((n1, n2) =>
        {
            float disN1 = Mathf.Abs(endPos_map.x - n1.x) + Mathf.Abs(endPos_map.y - n1.y) *0.25f;
            float disN2 = Mathf.Abs(endPos_map.x - n2.x) + Mathf.Abs(endPos_map.y - n2.y) *0.25f;
            if (disN1 > disN2)
                return 1;
            else if (disN1 == disN2)
                return 0;
            else
                return -1;
        }
        );


        foreach(var n in nearstNodes)
        {
            var path = GetPathFromTo(startPos_world,grid.GetCellCenterWorld(new Vector3Int(n.x,n.y,0)));
            if (path.Count >= 1)
            {
                CurPath = path;
                return path;
            }
        }

        return null;
    }

    /// <summary>
    /// GetPathFromTo的Node版本,可以获得更多信息
    /// </summary>
    /// <param name="startPos_world"></param>
    /// <param name="endPos_world"></param>
    /// <returns></returns>
    public List<Node> GetNodePathFromTo(Vector3 startPos_world, Vector3 endPos_world)
    {
        List<Vector3> pathList = GetPathFromTo(startPos_world, endPos_world);
        List<Node> nodePathList = new List<Node>();

        foreach(var pos in pathList)
        {
            var cellPos = grid.WorldToCell(pos);
            Node node = GetNode((cellPos.x, cellPos.y));
            nodePathList.Add(node);
        }

        CurNodePath = nodePathList;
        return nodePathList;
    }






    /// <summary>
    /// 获取当前节点的费用，用于资源消耗
    /// </summary>
    /// <param name="targetPos_world"></param>
    /// <returns></returns>
    public float GetPathCostToNode(Vector3 targetPos_world)
    {
        var targetPos_map = grid.WorldToCell(targetPos_world);
        Node node = gragh.GetNode((targetPos_map.x,targetPos_map.y));
        return (float)node.cost;
    }

    /// <summary>
    /// 获取击退的路径
    /// 包含了GenerateMap等操作，请直接调用
    /// 如果没有路径则返回0
    /// </summary>
    /// <param name="startPos_world"></param>
    /// <param name="dir"></param>
    /// <param name="dis"></param>
    /// <returns></returns>
    public List<Vector3> SearchAndGetPathByEnforcedMove(Vector3 startPos_world,Vector2 dir,int dis,bool ifIgnoreActor)
    {
        if( dir == Vector2.right)
        {
            Debug.Log("");
        }
        GenerateMapByRaycast();

        Vector3Int startPos_map = grid.WorldToCell(startPos_world);
        gragh = new Gragh();
        var targetPos = CreatEdgeByBeatBack(startPos_map.x,startPos_map.y,dir,dis, ifIgnoreActor);

        dijkstra = new Dijkstra(gragh);
        dijkstra.SearchShortPathFrom(GetNode((startPos_map.x, startPos_map.y)));

        return GetPathFromTo(startPos_world, grid.CellToWorld(new Vector3Int(targetPos.Item1, targetPos.Item2, 0)));
    }

    #endregion

    #region 边创建方法
    //--------------边和节点创造方法----------------//
    private void CreatEdgeByWalkCheck(int x,int y)
    {
        //// 当前节点的建立与读取

        Node curNode = GetNode((x, y));
        if (curNode == null) return;

        // 搜算周边节点
        List<(int, int)> linkPos_list = new List<(int, int)> { (-1, 0),(1, 0),(-1, 1),(1 , 1),(-1,-1),( 1 , -1) };
        foreach(var offset_pos in linkPos_list)
        {
            int offset_x = offset_pos.Item1;int offset_y = offset_pos.Item2;
            (int, int) pos = (x + offset_x, y + offset_y);
            if (map.map_dic.ContainsKey(pos))
            {
                MapCell link = map.map_dic[pos];
                if (link.StayState == ObjectStayState.Stand)
                {
                    // 周边节点的建立与读取
                    Node linkNode = GetNode((pos.Item1, pos.Item2));
                    linkNode.ActionToNode = ActorActionToNode.Walk;

                    // 添加edge
                    Edge edge = new Edge(curNode, linkNode, CostPerUnit_Walk);
                    gragh.AddEdge(edge);
                }
            }
        }
    }

    /// <summary>
    /// 创建攀爬路径
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    private void CreatEdgeByClimbCheck(int x, int y)
    {
        // 当前节点的建立与读取
        Node curNode = GetNode((x, y));
        if (curNode == null) return;

        // 搜算上下节点
        List<(int, int)> linkPos_list = new List<(int, int)> {(0, 1), (0, -1)};
        foreach (var offset_pos in linkPos_list)
        {
            int offset_x = offset_pos.Item1; int offset_y = offset_pos.Item2;
            (int, int) pos = (x + offset_x, y + offset_y);
            if (map.map_dic.ContainsKey(pos))
            {
                MapCell link = map.map_dic[pos];
                if ((link.StayState == ObjectStayState.Climb || link.StayState == ObjectStayState.Stand )&& link.StayState!= ObjectStayState.CantHold)
                {
                    Node linkNode = GetNode((pos.Item1, pos.Item2));
                    linkNode.ActionToNode = ActorActionToNode.Climb;

                    // 添加edge
                    Edge edge = new Edge(curNode, linkNode, CostPerUnit_Climb);
                    gragh.AddEdge(edge);
                }
            }
        }
        // 搜索跳跃节点
        List<(int, int)> jumpPos_list = new List<(int, int)> { (0, 2), (0, -2) };
        foreach (var offset_pos in jumpPos_list)
        {
            int offset_x = offset_pos.Item1; int offset_y = offset_pos.Item2;
            (int, int) pos = (x + offset_x, y + offset_y);

            (int, int) link_pos = (x + offset_x/2, y + offset_y/2);
            if (map.map_dic.ContainsKey(pos) && map.map_dic.ContainsKey(link_pos))
            {
                MapCell link = map.map_dic[link_pos], jump = map.map_dic[pos];

                // 如果和目标点隔了一层平台，则可以跳过

                if (link.Type == MapCellType.Platform && 
                    (jump.StayState == ObjectStayState.Stand || jump.StayState == ObjectStayState.Climb))
                {
                    // 跳跃节点的建立与读取
                    Node linkNode = GetNode((pos.Item1, pos.Item2));
                    linkNode.ActionToNode = ActorActionToNode.Climb;

                    // 添加edge
                    Edge edge = new Edge(curNode, linkNode, CostPerUnit_Climb);
                    gragh.AddEdge(edge);
                }
            }
        }

    }

    /// <summary>
    /// 创建垂直跳跃的路径
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    private void CreatEdgeByJumpVerticalCheck(int x, int y)
    {
        // 当前节点的建立与读取
        Node curNode = GetNode((x, y));
        if (curNode == null) return;

        // 搜算上下节点
        List<(int, int)> jumpPos_list = new List<(int, int)> { (0, 1), (0, -1) };
        foreach (var offset_pos in jumpPos_list)
        {
            for (int i = 1; i < 5; i++)
            {
                bool canJump = true;
                int offset_x = offset_pos.Item1; int offset_y = offset_pos.Item2;
                (int, int) pos = (x + offset_x * i, y + offset_y * i);
                int adjVal = offset_y == (-1) ? 0 : 1;  // 调整值，向下跳不消耗
                if (map.map_dic.ContainsKey(pos))
                {
                    MapCell jump = map.map_dic[pos];
                    if (jump.StayState == ObjectStayState.Stand)
                    {
                        for(int j = 1;j < i;j++)
                        {
                            (int, int) middlePos = (x + offset_x * j, y + offset_y * j);
                            if (map.map_dic.ContainsKey(middlePos))
                            {
                                MapCell middle = map.map_dic[middlePos];
                                // 移动路径中含有障碍或者本来就是可攀爬点则不创建跳跃点
                                if(middle.Type == MapCellType.Ground || middle.StayState == ObjectStayState.Climb)
                                {
                                    canJump = false;
                                    break;
                                }
                            }
                        }
                        if (canJump)
                        {
                            // 目标节点的建立与读取
                            Node targetNode = GetNode((pos.Item1, pos.Item2));
                            targetNode.ActionToNode = ActorActionToNode.JumpV;

                            // 添加edge
                            Edge edge = new Edge(curNode, targetNode, CostPerUnit_JumpV * i * adjVal);
                            gragh.AddEdge(edge);
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// 创建水平跳跃的路径
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    private void CreatEdgeByJumpHorizontalCheck(int x,int y)
    {
        // 当前节点的建立与读取
        Node curNode = GetNode((x, y));
        if (curNode == null) return;

        // 搜算左右节点
        List<(int, int)> jumpPos_list = new List<(int, int)> { (1, 0), (-1, 0) };
        foreach (var offset_pos in jumpPos_list)
        {
            int offset_x = offset_pos.Item1; int offset_y = offset_pos.Item2;
            (int, int) pos = (x + offset_x * 2, y + offset_y * 2);

            if (map.map_dic.ContainsKey(pos))
            {
                MapCell target = map.map_dic[pos];
                if (target.StayState == ObjectStayState.Stand)
                {
                    bool canJump = true;
                    (int, int) middlePos = (x + offset_x, y + offset_y);
                    if (map.map_dic.ContainsKey(middlePos))
                    {
                        MapCell middle = map.map_dic[middlePos];
                        // 轨迹中含有障碍或者不可容纳本来就是可行走路径，则不创建edge
                        if(middle.Type == MapCellType.Ground || middle.StayState == ObjectStayState.Stand 
                            || middle.StayState== ObjectStayState.CantHold)
                        {
                            canJump = false;
                            continue;
                        }
                    }
                    if (canJump)
                    {
                        Node targetNode = GetNode((pos.Item1, pos.Item2));
                        targetNode.ActionToNode = ActorActionToNode.JumpH;

                        // 添加edge
                        Edge edge = new Edge(curNode, targetNode, CostPerUnit_JumpH);
                        gragh.AddEdge(edge);
                    }
                }
            }
        }
    }

    /// <summary>
    /// 创建通过敌人的路径，消耗更大
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    private void CreatEdgeByPassActorCheck(int x,int y)
    {
        Node curNode = GetNode((x, y));
        if (curNode == null) return;

        // 搜算左右节点
        List<(int, int)> jumpPos_list = new List<(int, int)> { (1, 0), (-1, 0) };
        foreach (var offset_pos in jumpPos_list)
        {
            int passCost = 0;

            int offset_x = offset_pos.Item1; int offset_y = offset_pos.Item2;
            (int, int) pos = (x + offset_x * 2, y + offset_y * 2);

            if (map.map_dic.ContainsKey(pos))
            {
                MapCell target = map.map_dic[pos];
                if (target.StayState == ObjectStayState.Stand)
                {
                    bool canPass = false;
                    (int, int) middlePos = (x + offset_x, y + offset_y);
                    if (map.map_dic.ContainsKey(middlePos))
                    {
                        MapCell middle = map.map_dic[middlePos];
                        // 隔着一个敌人单位，可以pass
                        if (middle.Type == MapCellType.EnemyActor)
                        {
                            canPass = true;
                            passCost = CostPerUnit_PassE;
                        }
                        else if(middle.Type == MapCellType.FriendActor)
                        {
                            canPass = true;
                            passCost = CostPerUnit_PassF;
                        }
                    }
                    if (canPass)
                    {
                        Node targetNode = GetNode((pos.Item1, pos.Item2));
                        targetNode.ActionToNode = ActorActionToNode.JumpH;

                        // 添加edge
                        Edge edge = new Edge(curNode, targetNode, passCost);
                        gragh.AddEdge(edge);
                    }
                }
            }
        }
    }
    #endregion

    #region 额外边创建方法
    /// <summary>
    /// 由击退来创建路径
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="方向"></param>
    /// <param name="距离"></param>
    private (int, int) CreatEdgeByBeatBack(int x, int y, Vector2 dir, int dis, bool ifIgnoreActor)
    {
        Stack<(int, int)> targetPos_stack = new Stack<(int, int)>();
        targetPos_stack.Push((x, y));   // 初始节点可作为最终击退至位置

        // 当前节点的建立与读取
        var curNode = GetNode((x, y));
        if (curNode == null) return (x, y);

        // 求终点位置
        var nDir = dir.normalized;
        (int, int) targetPos = (x, y);
        (int, int) searchPos = (x, y);
        (int, int) endPos = (x, y); // 最终结束点

        for (int i = 1; i < dis + 1; i++)
        { 
            // 现在检测的位置是(tx,ty）
            int tx = searchPos.Item1 + Mathf.RoundToInt(nDir.x), ty = searchPos.Item2 + Mathf.RoundToInt(nDir.y);
            
            var curCell = map.map_dic[(tx, ty)];
            if(ifIgnoreActor)
            {
                if (curCell.Type == MapCellType.EnemyActor || curCell.Type == MapCellType.FriendActor)  // 如果中间块是敌方单位，则先跳过这个块
                {
                    searchPos = (tx, ty);   // 可穿过节点 作为下次搜索的基点，但不可作为targetPos
                    continue;
                }
            }

            // 如果该节点不可容纳对象，则选取其上方的节点
            if (curCell.StayState == ObjectStayState.CantHold)
            {
                ty = ty + 1;
                var curCell2 = map.map_dic[(tx, ty)];
                if (curCell2.StayState == ObjectStayState.CantHold)
                    break;  // 上方的节点也不可通过，通路全部被堵，结束搜索
            }

            // 使用现在搜索的位置进行searchPos 和 targetPos的更新
            searchPos = (tx, ty);
            targetPos = (tx, ty);
            targetPos_stack.Push((tx, ty));
        }

        #region 建立下落边，并修正击退位置
        bool ifMoveToRight = (targetPos.Item1 - x) > 0 ? true : false;
        (int, int) fallPos;
        while (true)
        {
            targetPos = targetPos_stack.Pop();

            if (map.map_dic[targetPos].StayState == ObjectStayState.Fall)
            {
                // fallPos = CreatEdgeByFall(targetPos.Item1, targetPos.Item2);
                fallPos = CreatEdgeByFall(targetPos.Item1, targetPos.Item2, ifMoveToRight); // 添加下落路径

                if (fallPos == targetPos) // 如果无法下落
                {
                    continue;
                }
                else
                {
                    endPos = fallPos;
                    break;  // 否则targetPos就是最终击退位置
                }
            }
            else if (map.map_dic[targetPos].StayState == ObjectStayState.Stand || targetPos == (x, y))
            {
                endPos = targetPos;
                break;
            }
        }
        #endregion

        // 建立击退edge
        var beatBackNode = GetNode(targetPos);
        if (beatBackNode == curNode) return (x, y);   // 如果没有移动，则不创建edge

        Edge edge = new Edge(curNode, beatBackNode, 0);   // 否则创建击退edge
        gragh.AddEdge(edge);

        return (endPos.Item1, endPos.Item2);
    }


    /// <summary>
    /// 从(x,y)开始创建下落路径
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    private (int,int) CreatEdgeByFall(int x, int y)
    {
        // 当前节点的建立与读取
        var curNode = GetNode((x, y));
        if (curNode == null) return (x, y);

        // 下落到达节点的建立与读取
        var cell = map.map_dic[(x, y)];
        var targetCell = map.map_dic[(x, y - cell.height)];

        // 下落处理1 位置被占用 取消下落
        if (targetCell.StayState == ObjectStayState.CantHold)
            return (x, y);

        var targetNode = GetNode((x, y - cell.height));
        targetNode.ActionToNode = ActorActionToNode.Fall;
        targetNode.FallCount = cell.height;

        if (targetNode == curNode) return (x, y);

        Edge edge = new Edge(curNode, targetNode, 0);
        gragh.AddEdge(edge);

        return (x, y - cell.height);
    }
    /// <summary>
    /// 从(x,y)开始创建下落路径，并用ifToRight记录前置移动方向用于决定碰撞
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="ifToRight"></param>
    /// <returns></returns>
    private (int, int) CreatEdgeByFall(int x, int y,bool ifToRight)
    {
        // 当前节点的建立与读取
        var curNode = GetNode((x, y));
        if (curNode == null) return (x, y);

        // 下落到达节点的建立与读取
        var cell = map.map_dic[(x, y)];
        var targetCell = map.map_dic[(x, y - cell.height)];

        // 下落到达Node的建立
        var targetNode = GetNode((x, y - cell.height));
        targetNode.ActionToNode = ActorActionToNode.Fall;
        targetNode.FallCount = cell.height;

        //下落处理1 位置被占用 取消下落
        //if (targetCell.StayState == ObjectStayState.CantHold)
        //    return (x, y);

        // 下落处理2 位置被占用 检测对方是否能被弹开
        if (targetCell.StayState == ObjectStayState.CantHold)
        {
            if (targetCell.Type == MapCellType.FriendActor || targetCell.Type == MapCellType.EnemyActor)
            {
                Vector2 targetWorldPos = grid.GetCellCenterWorld(new Vector3Int(targetCell.IntPos.Item1, targetCell.IntPos.Item2, 0));
                var hit = Physics2D.Raycast(targetWorldPos, Vector2.zero, 4f, LayerMask.GetMask("Actor"));
                PathFinderComponent targetPathFinder = hit.collider.transform.parent.GetComponent<PathFinderComponent>();
                
                var targetBeHitLeftPath = targetPathFinder.SearchAndGetPathByEnforcedMove(hit.collider.transform.parent.position, Vector2.left, 1, true);
                var targetBeHitRightPath = targetPathFinder.SearchAndGetPathByEnforcedMove(hit.collider.transform.parent.position, Vector2.right, 1, true);

                Vector2 dir = Vector2.zero;

                List<Vector3> targetBeHitPath = new List<Vector3>();
                if (ifToRight) { targetBeHitPath = targetBeHitRightPath; dir = Vector2.right; }
                else { targetBeHitPath = targetBeHitLeftPath; dir = Vector2.left; }

                if (targetBeHitPath.Count <= 1) // 原地不动含有一个开始节点，长度为1，没有路径长度为0所以用1作为判据
                {
                    targetBeHitPath = ifToRight ? targetBeHitLeftPath : targetBeHitRightPath;
                    dir = ifToRight ? Vector2.left : Vector2.right;
                }

                if (targetBeHitPath.Count <= 1) return (x, y);  // 两个方向都不能弹开，取消下落

                // 添加碰撞信息
                BeatBackInfomation beatBackInfomation = new BeatBackInfomation(hit.transform.parent.gameObject, dir, 1);

                targetNode.BeatBackInfomation = beatBackInfomation;
            }
        }

        if (targetNode == curNode) return (x, y);

        Edge edge = new Edge(curNode, targetNode, 0);
        gragh.AddEdge(edge);

        return (x, y - cell.height);
    }

    void Onddd() { }
    #endregion

    #region 工具方法

    /// <summary>
    /// 安全的获取节点方法，如果没有则会进行创建。如果无法创建则会返回null
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    private Node GetNode((int,int) pos)
    {
        // 当前节点的建立与读取
        int x = pos.Item1, y = pos.Item2;
        if (!map.map_dic.ContainsKey((x, y)))
            return null;
        if (gragh.GetNode((x, y)) == null)
        {
            var worldPos = grid.GetCellCenterWorld(new Vector3Int(x, y, 0));
            Node ne = new Node(x, y, worldPos.x, worldPos.y);
            gragh.SetOrGetNode(ne);
        }

        Node curNode = gragh.GetNode((x, y));
        return curNode;
    }

    /// <summary>
    /// 使用gird.GetCellCenter将节点坐标转成世界坐标
    /// </summary>
    /// <param name="cellPos"></param>
    /// <returns></returns>
    public Vector3 CellToWorld((int,int)cellPos)
    {
        return grid.GetCellCenterWorld(new Vector3Int(cellPos.Item1, cellPos.Item2, 0));
    }

    public List<Node> VectorPath2NodePath(List<Vector3> path)
    {
        List<Node> nodes = new List<Node>();

        foreach (var pos in path)
        {
            var cellPos = grid.WorldToCell(pos);
            nodes.Add(GetNode((cellPos.x, cellPos.y)));
        }

        return nodes;
    }
    

    #endregion
}
