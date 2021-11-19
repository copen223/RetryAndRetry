using Assets.Scripts.CardModule.CardEffects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.CardModule.CardActions
{
    public class FocusTrail : CardAction
    {
        public float Distance_X;      // 放置轨迹的x距离
        public float Distance_Y;      // 放置轨迹的y距离
        public FocusTrail(float x,float y)
        {
            Distance_X = x;
            Distance_Y = y;
        }
    }
}
