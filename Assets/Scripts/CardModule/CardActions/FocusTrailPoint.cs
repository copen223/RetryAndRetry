using System;
using System.Collections.Generic;
using UnityEngine;

namespace CardModule.CardActions
{
    [CreateAssetMenu(fileName = "Action", menuName = "MyInfo/使用形式/专注轨迹_点")]
    public class FocusTrailPoint : FocusTrail
    {

        public override List<Vector3> GetLineOffsetPoints(Vector2 setDir)
        {
            throw new NotImplementedException();
        }
    }
}
