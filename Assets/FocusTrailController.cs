using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.CardModule;

public class FocusTrailController : MonoBehaviour
{
    //------------------链接-----------------
    LineRenderer lineRenderer;
    PolygonCollider2D collider2d;

    public GameObject Actor;   // 该专注曲线应用的对象
    public Card Card;          // 产生该专注曲线的卡牌

    //------------------数据-----------------
    Vector3[] linePoints;

    //------------------测试集---------------
    public List<Vector3> points_test = new List<Vector3>();

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
        if (Input.GetKeyDown(KeyCode.W))
        {
            Debug.Log("dddd");
            SetPoints(points_test);
        }
    }

    public void SetPoints(List<Vector3> points)
    {
        linePoints = points.ToArray();
        lineRenderer.positionCount = linePoints.Length;
        lineRenderer.SetPositions(linePoints);  
        UpdateColliderByLinePoints();
    }

    /// <summary>
    /// 根据当前的linePoints生成线框碰撞体
    /// </summary>
    private void UpdateColliderByLinePoints()
    {
        transform.position = linePoints[0];

        List<Vector2> linePoints2 = new List<Vector2>();
        for (int i = 0; i < linePoints.Length; i++) linePoints2.Add(linePoints[i]); // 二维化
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

}
