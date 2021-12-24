using Assets.Scripts.CardModule.CardEffects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.CardModule.CardActions
{
    [CreateAssetMenu(fileName = "Action", menuName = "MyInfo/使用形式/攻击轨迹_抛物线")]
    public class AttackTrailParabola : AttackTrail
    {
        public float Speed_X;
        private float speed_Y;
        private float g = 10;
        public override List<Vector2> GetLinePoints(Vector2 basePoint, Vector2 endPoint)
        {
            List<Vector2> points = new List<Vector2>();

            int dirX = (endPoint - basePoint).x > 0 ? 1 : -1;

            float dis_x = Mathf.Abs(endPoint.x - basePoint.x);
            float dis_y = endPoint.y - basePoint.y;

            float time = dis_x / Speed_X;

            int pointCount = 200;

            //speed_Y = Speed_X;
            speed_Y = (dis_y + 0.5f * g * time * time) / time;

            for (float timer = 0; timer < time; timer += time / pointCount)
            {
                float point_x = basePoint.x + Speed_X * timer * dirX;
                float point_y = basePoint.y + speed_Y * timer - 0.5f * g * timer * timer;

                Vector2 curPoint = new Vector2(point_x, point_y);

                float distance = Vector2.Distance(curPoint, basePoint);
                if (distance > Distance_max)
                    break;

                points.Add(curPoint);
            }

            return points;
        }


    }
}
