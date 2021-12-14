using Assets.Scripts.CardModule.CardEffects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.CardModule.CardActions
{
    [CreateAssetMenu(fileName = "Action", menuName = "MyInfo/使用形式/攻击轨迹_直线")]
    public class AttackTrailLine : AttackTrail
    {
        public override List<Vector2> GetLinePoints(Vector2 basePoint,Vector2 endPoint)
        {
            return new List<Vector2>() { basePoint, endPoint };
        }
    }
}
