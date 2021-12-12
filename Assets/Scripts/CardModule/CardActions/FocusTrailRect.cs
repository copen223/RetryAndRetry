using Assets.Scripts.CardModule.CardEffects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.CardModule.CardActions
{
    [CreateAssetMenu(fileName = "Action", menuName = "MyInfo/使用形式/专注轨迹")]
    public class FocusTrailRect : FocusTrail
    {
        public float Distance_X;      // 放置轨迹的x距离
        public float Distance_Y;      // 放置轨迹的y距离
        public FocusTrailRect(float x,float y)
        {
            Distance_X = x;
            Distance_Y = y;
        }

        public override List<Vector3> GetLineOffsetPoints(Vector2 setDir)
        {
            float x = Distance_X * (setDir.x < 0 ? (-1) : 1);
            float y = Distance_Y * (setDir.y < 0 ? (-1) : 1);

            List<Vector3> points = new List<Vector3>();

            // 获得世界坐标
            Vector3 point1_offset = new Vector3(x, 0);
            Vector3 point2_offset = new Vector3(x, y);
            Vector3 point3_offset = new Vector3(0, y);
            points.Add(point1_offset); points.Add(point2_offset); points.Add(point3_offset);

            return points;
        }
    }
}
