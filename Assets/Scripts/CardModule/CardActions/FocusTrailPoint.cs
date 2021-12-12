using Assets.Scripts.CardModule.CardEffects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.CardModule.CardActions
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
