using System.Collections.Generic;
using UnityEngine;

namespace Test
{
    public class Test_DrawRayAndLine : MonoBehaviour
    {
        public LineRenderer lineRenderer;
        public AnimationCurve RayCurve;
        public float Min_distance;
        public float Max_distance;
        public int PointNum;
        public float angle;

        public Vector3 MouseWorldPos;

        public List<Vector3> points;


        private void Start()
        {
            lineRenderer = gameObject.GetComponent<LineRenderer>();
        }

        private void Update()
        {
            var mouseScreenPos = Input.mousePosition;
            MouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);     // 鼠标位置获取

            points = GetRayPoints(MouseWorldPos);
            DrawLine(points);


        }


        public List<Vector3> GetRayPoints(Vector3 mouseWorldPos)
        {
            Vector3 dirVector = mouseWorldPos - transform.position;         // 方向向量
            dirVector = new Vector3(dirVector.x, dirVector.y, 0);

            Vector3 form = dirVector.x > 0 ? Vector3.right : Vector3.left;
            angle = Vector3.Angle(form, dirVector);          // 计算夹角
            if (dirVector.y < transform.position.y) angle *= (-1);
            Quaternion qua = Quaternion.Euler(0, 0, angle);  // 旋转用四元数

            float distance = dirVector.magnitude;                           // 计算距离
            if (distance < Min_distance) distance = Min_distance;
            else if (distance > Max_distance) distance = Max_distance;

            List<Vector3> rayPoints = new List<Vector3>();

            //----------------求射线点集---------------//
            for (float x = 0; x < distance; x+=distance/PointNum)
            {
                float y = RayCurve.Evaluate(x / distance);  // 插值求y
                Vector3 point = new Vector3(x, y, 0);
                point = qua * point;                        // 旋转得到正确位置

                rayPoints.Add(point);
            }

            return rayPoints;
        }

        public void DrawLine(List<Vector3> points)
        {
            lineRenderer.SetPositions(points.ToArray());
            lineRenderer.positionCount = points.Count;
        }
    }
}
