using System.Collections.Generic;
using UnityEngine;

namespace CardModule.CardActions
{
    [CreateAssetMenu(fileName = "Action", menuName = "MyInfo/使用形式/专注轨迹_圆")]
    public class FocusTrailCicle : FocusTrail
    {
        public float radius;
        public float angle;

        public override List<Vector3> GetLineOffsetPoints(Vector2 setDir)
        {
            List<Vector3> offsetPoints = new List<Vector3>();

            Vector3 referenceDir = setDir;    // 基准方向点

            float startAngle = -angle/2, endAngle = angle/2; int rayCount = 10;
            for (float a = startAngle; a <= endAngle; a += (endAngle - startAngle) / rayCount)
            {
                Quaternion rotation = Quaternion.Euler(0, 0,  a);
                Vector3 dir3D = (Vector3)referenceDir;
                Vector3 thisDir = rotation * dir3D;
                var thisDirPoint = thisDir.normalized * radius;

                offsetPoints.Add(thisDirPoint);
            }

            return offsetPoints;
        }
    }
}
