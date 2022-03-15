using System.Collections.Generic;
using UnityEngine;

namespace CardModule.CardActions
{
    [CreateAssetMenu(fileName = "Action", menuName = "MyInfo/使用形式/攻击轨迹_直线")]
    public class AttackTrailLine : AttackTrail
    {
        public override List<Vector2> GetLinePoints(Vector2 basePoint,Vector2 endPoint)
        {
            Vector2 finalEndPoint = new Vector2(endPoint.x,endPoint.y);

            float distance = Vector2.Distance(basePoint, endPoint);
            Vector2 direction = (endPoint - basePoint).normalized;

            //if(distance < Distance_min)
            //{
            //    finalEndPoint = basePoint + direction * Distance_min;
            //}
            if(distance > Distance_max)
            {
                finalEndPoint = basePoint + direction * Distance_max;
            }

            return new List<Vector2>() { basePoint, finalEndPoint };
        }
    }
}
