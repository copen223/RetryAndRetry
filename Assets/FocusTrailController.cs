using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.CardModule;

public class FocusTrailController : MonoBehaviour
{
    //------------------链接-----------------
    LineRenderer lineRenderer;
    PolygonCollider2D collider2d;

    public GameObject Seter;   // 正设置该轨迹的对象
    public GameObject Actor;   // 该专注曲线应用的对象
    public Card Card;          // 产生该专注曲线的卡牌

    //------------------数据-----------------
    Vector3[] linePoints;
    Vector3[] lineOffsetPoints = new Vector3[0];

    //------------------标志参量---------------
    public bool IfOccupied;

    private void Awake()
    {
        lineRenderer = gameObject.GetComponent<LineRenderer>();
        collider2d = gameObject.GetComponent<PolygonCollider2D>();
    }

    private void Start()
    {
    }

    private void Update()
    {
        if(lineOffsetPoints.Length > 1)
        {
            UpdateLineViewByPos();
        }
    }

    /// <summary>
    /// 在确认设置专注轨迹时，存储offset变量，以确定移动后的线位置
    /// </summary>
    /// <param name="pos"></param>
    public void SetOffsetPoints(params Vector3[] pos)
    {
        lineOffsetPoints = pos;
    }
    /// <summary>
    /// 通过预设的位置偏移量来更新线的显示
    /// </summary>
    public void UpdateLineViewByPos()
    {
        var pos = (Vector2) transform.position;
        Vector3[] linePoints_arry = new Vector3[lineOffsetPoints.Length];
        for(int i =0;i<linePoints_arry.Length;i++)
        {
            //Debug.LogError(transform.position + "player" + Actor.transform.position + " sprite" + Actor.transform.Find("Sprite").transform.position);
            linePoints_arry[i] = transform.position + lineOffsetPoints[i];
        }
        lineRenderer.positionCount = linePoints_arry.Length;
        lineRenderer.SetPositions(linePoints_arry);
    }

    public void SetPoints(List<Vector3> points)
    {
        //if(lineRenderer == null)
        //{
        //    lineRenderer = gameObject.GetComponent<LineRenderer>();
        //    collider2d = gameObject.GetComponent<PolygonCollider2D>();
        //}
        linePoints = points.ToArray();
        lineRenderer.positionCount = linePoints.Length;
        lineRenderer.SetPositions(linePoints);  
        UpdateColliderByLinePoints();
        CheckIfOccupied();
    }

    /// <summary>
    /// 根据当前的linePoints生成线框碰撞体
    /// </summary>
    private void UpdateColliderByLinePoints()
    {
        List<Vector2> linePoints2 = new List<Vector2>();
        transform.position = Seter.transform.Find("Sprite").transform.position;
        foreach(var point in linePoints)
        {
            linePoints2.Add(point - transform.position);
        }

        //List<Vector2> linePoints2 = new List<Vector2>();
        //for (int i = 0; i < linePoints.Length; i++) linePoints2.Add(linePoints[i]); // 二维化
        List<Vector2> colliderPoints1 = new List<Vector2>(); // 一侧线框的点集
        List<Vector2> colliderPoints2 = new List<Vector2>(); // 另一侧线框的点集

        float width = lineRenderer.startWidth/2;

        for(int i =0;i<linePoints2.Count;i++)
        {
            Vector2 lastPoint = i == 0?linePoints2[i]:linePoints2[i-1];
            Vector2 nextPoint = (i == (linePoints2.Count-1))?linePoints2[i]: linePoints2[i + 1];
            Vector2 curPoint = linePoints2[i];
            // 求两个方向的向量
            Vector2 lastDir = lastPoint - curPoint;
            Vector2 nextDir = curPoint - nextPoint;
            // 求两方向向量的法线
            Vector2 normalLastDir = Vector3.Cross(lastDir,Vector3.forward);
            Vector2 normalNextDir = Vector3.Cross(nextDir, Vector3.forward);
            // 求两法线的平分线
            Vector2 pointDir1 = (normalLastDir.normalized + normalNextDir.normalized).normalized;
            Vector2 pointDir2 = -1*(normalLastDir.normalized + normalNextDir.normalized).normalized;
            // 求线框两点
            Vector2 point1 = curPoint + pointDir1 * width;
            Vector2 point2 = curPoint + pointDir2 * width;
            colliderPoints1.Add(point1);
            colliderPoints2.Add(point2);
        }
        // 获取最终碰撞体点集
        colliderPoints2.Reverse();
        List<Vector2> colliderPoints = new List<Vector2>();
        colliderPoints.AddRange(colliderPoints1);
        colliderPoints.AddRange(colliderPoints2);

        // 设置组件参数 over
        collider2d.points = colliderPoints.ToArray();
    }

    private void CheckIfOccupied()
    {
        List<Collider2D> results = new List<Collider2D>();
        var filter = new ContactFilter2D();
     
        collider2d.OverlapCollider(filter, results);

        foreach (var collision in results)
        {
            if (collision.gameObject != gameObject && collision.tag == "FocusTrail")
            {
                Debug.Log(collision.gameObject);
                Debug.Log(collision.GetComponent<FocusTrailController>().Actor);
                Debug.Log(Seter);
                if (collision.GetComponent<FocusTrailController>().Actor == Seter)
                {
                    Debug.Log("yes");
                    IfOccupied = true;
                    return;
                }
            }
        }
        IfOccupied = false;
    }
}
