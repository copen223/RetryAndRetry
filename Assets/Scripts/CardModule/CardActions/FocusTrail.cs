using System.Collections.Generic;
using ActorModule.Core;
using UnityEngine;

namespace CardModule.CardActions
{
    public abstract class FocusTrail : CardAction
    {
        /// <summary>
        /// 根据设置方向，获得形成专注轨迹的点集合
        /// </summary>
        /// <param name="setDir"></param>
        /// <returns></returns>
        public abstract List<Vector3> GetLineOffsetPoints(Vector2 setDir);

        public bool CanCutOff;

        public ActorController Actor;
    }
}
