using Assets.Scripts.CardModule.CardEffects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.CardModule.CardActions
{
    public class AttackTrail : CardAction
    {
        public float Distance_max;      // 可选定的最大距离
        public float Distance_min;      // 可选定的最小距离
        public int TargetNum;           // 可选定的对象数目
        public AttackTrail(float min,float max,int targetNum)
        {
            Distance_min = min;
            Distance_max = max;
            TargetNum = targetNum;
        }
    }
}
