using System.Collections.Generic;
using UnityEngine;

namespace CardModule.CardActions
{
    //[CreateAssetMenu(fileName = "Action", menuName = "MyInfo/使用形式/攻击轨迹")]
    public abstract class AttackTrail : CardAction
    {
        public float Distance_max;      // 可选定的最大距离
        public float Distance_min;      // 可选定的最小距离
        public int TargetNum;           // 可选定的对象数目

        public abstract List<Vector2> GetLinePoints(Vector2 basePoint, Vector2 endPoint);
    }
}
