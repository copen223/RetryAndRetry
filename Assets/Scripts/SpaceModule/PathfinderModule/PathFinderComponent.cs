﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;
using Assets.Scripts.SpaceModule.PathfinderModule;

public class PathFinderComponent : MonoBehaviour
{
    private Tilemap groundTilemap;
    private Tilemap platformTilemap;
    private Tilemap ladderTilemap;
    private Grid grid;
    private int mapMargin = 5;
    public List<Vector3> CurPath;

    private Map map; // 扫描场景获得的地图数据
    private Gragh gragh; // 根据map生成的图
    private Dijkstra dijkstra;

    //--------属性值配置--------//
    private int CostPerUnit_Walk = 4;
    private int CostPerUnit_Climb = 1;
    private int CostPerUnit_JumpV = 1;
    private int CostPerUnit_JumpH = 3;


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
            // 判定cell类型
            if (hit.collider == null)
                cell.Type = MapCellType.Empty;
            else if (hit.collider.tag == "Obstacle")
                cell.Type = MapCellType.Ground;
            else if (hit.collider.tag == "Platform")
                cell.Type = MapCellType.Platform;
            else if (hit.collider.tag == "Ladder")
                cell.Type = MapCellType.Ladder;

            map.map_dic[(pos.x, pos.y)] = cell;
            
        }

        // 判断Cell的单位停留类型
        foreach(var mapCell in map.map_dic)
        {
            MapCell cell = mapCell.Value;
            (int, int) pos = mapCell.Key;

            cell.StayState = ObjectStayState.Fall;

            if (cell.Type == MapCellType.Empty || cell.Type == MapCellType.Ladder)
            {
                // climb要弱于stand，所以先判断，若可站立，后续代码会覆盖该状态
                if (cell.Type == MapCellType.Ladder)
                    cell.StayState = ObjectStayState.Climb;

                if (map.map_dic.ContainsKey((pos.Item1, pos.Item2 - 1)))
                {
                    // 下方为地面，自身为空，则为stand
                    var cellType = map.map_dic[(pos.Item1, pos.Item2 - 1)].Type;
                    if(cellType == MapCellType.Ground || cellType == MapCellType.Platform)
                    {
                        cell.StayState = ObjectStayState.Stand;
                        //如果上方4格子内含有Ground，则表明空间不足，不可站立
                        for (int i = 0; i < 4; i++)
                        {
                            if (map.map_dic[(pos.Item1, pos.Item2 + i + 1)].Type == MapCellType.Ground)
                            {
                                cell.StayState = ObjectStayState.CantStand;
                            }
                        }
                    }
                    else if(cellType == MapCellType.Empty)
                    {
                        //如果上方4格子内含有Ground，则表明空间不足，不可容纳
                        for (int i = 0; i < 4; i++)
                        {
                            if (map.map_dic.ContainsKey((pos.Item1, pos.Item2 + i + 1)))
                            {
                                if (map.map_dic[(pos.Item1, pos.Item2 + i + 1)].Type == MapCellType.Ground)
                                {
                                    cell.StayState = ObjectStayState.CantHold;
                                }
                            }
                        }
                    }
                }
            }
            //else if(cell.Type == MapCellType.Ground)
            //{
            //    cell.StayState = ObjectStayState.CantHold;
            //}
            //if (pos.Item1 == 5 && pos.Item2 == 1) Debug.LogError(cell.Type+" " + cell.StayState);
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
            }
            if(cell.StayState == ObjectStayState.Stand)
            {
                CreatEdgeByJumpHorizontalCheck(x, y);
                CreatEdgeByJumpVerticalCheck(x, y);
            }

        }
    }
    #region 外部调用
    /// <summary>
    /// 进行一次搜索，获得所有路径，存储在图中
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public void SearchPathFrom(int x,int y)
    {
        // 读图
        GenerateMapByRaycast();
        GenerateGraghByMap();

        // 寻路 
        dijkstra = new Dijkstra(gragh);
        Node start = gragh.GetNode((x, y));
        dijkstra.SearchShortPathFrom(start);
    }
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
    /// 搜索后调用，获得指定路径,确保已调用searchpathfrom
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <returns></returns>
    public Stack<(int, int)> GetPathFromTo((int,int)start,(int,int)end)
    {
        Stack<(int, int)> path = new Stack<(int, int)>();
        Node cur = gragh.GetNode(end);
        if(cur == null)
        {
           // Debug.Log("noNode at" + end);
            return path;
        }
        else
        {
           // Debug.Log("Node at" + end);
        }

        while(true)
        {
            var pos = (cur.x, cur.y);
            if ((cur.x, cur.y) == start)
            {
                path.Push(pos);
               // Debug.Log("世界" + tilemap.GetCellCenterWorld(new Vector3Int(pos.x, pos.y, 0)) + "Cell" + pos.x + "," + pos.y);
                return path;
            }
            if (cur.parent == cur)
            {
                //Debug.Log("世界" + tilemap.GetCellCenterWorld(new Vector3Int(pos.x, pos.y, 0)) + "Cell" + pos.x + "," + pos.y);
                return path;
            }
            path.Push(pos);
             //Debug.Log("世界" + tilemap.GetCellCenterWorld(new Vector3Int(pos.x, pos.y, 0)) + "Cell" + pos.x +","+ pos.y);
            cur = cur.parent;
        }
    }
    /// <summary>
    /// 搜索后调用，获得指定路径,确保已调用searchpathfrom
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
    /// 该方法用于确定离目标点最近的可用点的路径
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

    #endregion

    #region 边创建方法
    //--------------边和节点创造方法----------------//
    private void CreatEdgeByWalkCheck(int x,int y)
    {
        // 当前节点的建立与读取
        if (!map.map_dic.ContainsKey((x, y)))
            return;
        if(gragh.GetNode((x,y)) == null)
        {
            Node ne = new Node(x, y);
            gragh.SetOrGetNode(ne);
        }

        Node curNode = gragh.GetNode((x, y));

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
                    if (gragh.GetNode(pos) == null)
                    {
                        Node ne = new Node(pos.Item1, pos.Item2);
                        gragh.SetOrGetNode(ne);
                    }
                    Node linkNode = gragh.GetNode((pos.Item1, pos.Item2));

                    // 添加edge
                    Edge edge = new Edge(curNode, linkNode, CostPerUnit_Walk);
                    gragh.AddEdge(edge);
                }
            }
        }
    }

    private void CreatEdgeByClimbCheck(int x, int y)
    {
        // 当前节点的建立与读取
        if (!map.map_dic.ContainsKey((x, y)))
            return;
        if (gragh.GetNode((x, y)) == null)
        {
            Node ne = new Node(x, y);
            gragh.SetOrGetNode(ne);
        }

        Node curNode = gragh.GetNode((x, y));

        // 搜算上下节点
        List<(int, int)> linkPos_list = new List<(int, int)> {(0, 1), (0, -1)};
        foreach (var offset_pos in linkPos_list)
        {
            int offset_x = offset_pos.Item1; int offset_y = offset_pos.Item2;
            (int, int) pos = (x + offset_x, y + offset_y);
            if (map.map_dic.ContainsKey(pos))
            {
                MapCell link = map.map_dic[pos];
                if (link.StayState == ObjectStayState.Climb || link.StayState == ObjectStayState.Stand)
                {
                    // 周边节点的建立与读取
                    if (gragh.GetNode(pos) == null)
                    {
                        Node ne = new Node(pos.Item1, pos.Item2);
                        gragh.SetOrGetNode(ne);
                    }
                    Node linkNode = gragh.GetNode((pos.Item1, pos.Item2));

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
            if(pos.Item1 == (-5) && pos.Item2 == 3)
            {
                Debug.LogError("3");
            }



            (int, int) link_pos = (x + offset_x/2, y + offset_y/2);
            if (map.map_dic.ContainsKey(pos) && map.map_dic.ContainsKey(link_pos))
            {
                MapCell link = map.map_dic[link_pos], jump = map.map_dic[pos];

                // 如果和目标点隔了一层平台，则可以跳过

                if (link.Type == MapCellType.Platform && 
                    (jump.StayState == ObjectStayState.Stand || jump.StayState == ObjectStayState.Climb))
                {
                    // 跳跃节点的建立与读取
                    if (gragh.GetNode(pos) == null)
                    {
                        Node ne = new Node(pos.Item1, pos.Item2);
                        gragh.SetOrGetNode(ne);
                    }
                    Node linkNode = gragh.GetNode((pos.Item1, pos.Item2));

                    // 添加edge
                    Edge edge = new Edge(curNode, linkNode, CostPerUnit_Climb);
                    gragh.AddEdge(edge);
                }
            }
        }

    }

    private void CreatEdgeByJumpVerticalCheck(int x, int y)
    {
        // 当前节点的建立与读取
        if (!map.map_dic.ContainsKey((x, y)))
            return;
        if (gragh.GetNode((x, y)) == null)
        {
            Node ne = new Node(x, y);
            gragh.SetOrGetNode(ne);
        }

        Node curNode = gragh.GetNode((x, y));

        if(x == -9 && y == -10)
        {
          //  Debug.LogError("d");
        }

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
                            if (gragh.GetNode(pos) == null)
                            {
                                Node ne = new Node(pos.Item1, pos.Item2);
                                gragh.SetOrGetNode(ne);
                            }
                            Node targetNode = gragh.GetNode((pos.Item1, pos.Item2));

                            // 添加edge
                            Edge edge = new Edge(curNode, targetNode, CostPerUnit_JumpV * i * adjVal);
                            gragh.AddEdge(edge);
                        }
                    }
                }
            }
        }
    }

    private void CreatEdgeByJumpHorizontalCheck(int x,int y)
    {
        // 当前节点的建立与读取
        if (!map.map_dic.ContainsKey((x, y)))
            return;
        if (gragh.GetNode((x, y)) == null)
        {
            Node ne = new Node(x, y);
            gragh.SetOrGetNode(ne);
        }

        Node curNode = gragh.GetNode((x, y));

        if (x == -11 && y == -6)
        {
            Debug.Log("1");
        }
        //  -13    -9

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
                        // 周边节点的建立与读取
                        if (gragh.GetNode(pos) == null)
                        {
                            Node ne = new Node(pos.Item1, pos.Item2);
                            gragh.SetOrGetNode(ne);
                        }
                        Node targetNode = gragh.GetNode((pos.Item1, pos.Item2));

                        // 添加edge
                        Edge edge = new Edge(curNode, targetNode, CostPerUnit_JumpH);
                        gragh.AddEdge(edge);
                    }
                }
            }
        }
    }
    #endregion
}
