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
    // 专注轨迹物体与sprite物体的相对位移，用于确定画线位置而非物体位置，物体位置在生成后由Transform父子关系进行确定
    Vector3[] lineOffsetPoints = new Vector3[0];

    [SerializeField]
    private Color LineSettingColor;
    [SerializeField]
    private Color LineColor;

    //------------------标志参量---------------
    public bool IfOccupied;
    public bool IfShow;

    private void Awake()
    {
        lineRenderer = gameObject.GetComponent<LineRenderer>();
        collider2d = gameObject.GetComponent<PolygonCollider2D>();

        gameObject.SetActive(true);

        IfShow = true;
    }

    private void Start()
    {
    }

    private void Update()
    {
        if (!IfShow)
        {
            lineRenderer.positionCount = 0;
            return;
        }
        if(lineOffsetPoints.Length > 1)
        {
            UpdateLineViewByPos();
        }
    }

    /// <summary>
    /// 切换线的颜色
    /// </summary>
    /// <param name="isSetting"></param>
    public void SetColor(bool isSetting)
    {
        //Color color = isSetting? LineColor:seli

        //Gradient gradient = new Gradient();
        //gradient.SetKeys(new GradientColorKey[] {Color.white,lineRenderer }, new GradientAlphaKey[] { });
        //lineRenderer.colorGradient = new Gradient()
        //if (isSetting)
        //    colorKeys[1] = new GradientColorKey(LineColor,0.5f);
        //else
        //    colorKeys[1] = new GradientColorKey(LineColor, 0.5f);
    }

    /// <summary>
    /// 在确认设置专注轨迹时，存储offset变量，以确定移动后的线位置。offset变量是相对sprite的位置变量
    /// </summary>
    /// <param name="pos"></param>
    public void SetOffsetPoints(params Vector3[] pos)
    {
        lineOffsetPoints = pos;
    }
    /// <summary>
    /// 通过预设的位置偏移量、actor对象的scale，来更新线的显示。除使用卡牌设定位置外，轨迹线显示都由该方法实现
    /// </summary>
    private void UpdateLineViewByPos()
    {
        var pos = Actor.transform.Find("Sprite").transform.position;
        Vector3[] linePoints_arry = new Vector3[lineOffsetPoints.Length];
        for(int i =0;i<linePoints_arry.Length;i++)
        {
            //Debug.LogError(transform.position + "player" + Actor.transform.position + " sprite" + Actor.transform.Find("Sprite").transform.position);
            //linePoints_arry[i] = (transform.position + lineOffsetPoints[i]);
            var pos1 = pos; var pos2 = lineOffsetPoints[i]; var scale_x = Actor.transform.localScale.x;
            linePoints_arry[i] = new Vector3((pos1.x + pos2.x * scale_x), pos1.y + pos2.y, pos1.z + pos2.z);
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
                //Debug.Log(collision.gameObject);
                //Debug.Log(collision.GetComponent<FocusTrailController>().Actor);
                //Debug.Log(Seter);
                if (collision.GetComponent<FocusTrailController>().Actor == Seter)
                {
                    //Debug.Log("yes");
                    IfOccupied = true;
                    return;
                }
            }
        }
        IfOccupied = false;
    }
}
