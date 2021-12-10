using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.CardModule.CardEffects
{

    [CreateAssetMenu(fileName = "Effect", menuName = "MyInfo/效果/冲刺")]
    /// <summary>
    /// 冲刺效果
    /// </summary>
    public class NormalDash : CardEffect
    {
        public Vector2 dir;    // 冲刺方向
        public  int dis;        // 冲刺距离

        /// <summary>
        /// 成为一个效果的附加效果
        /// </summary>
        /// <param name="_dis"></param>
        /// <param name="effect"></param>
        public NormalDash(int _dis)
        {
            dis = _dis;
        }

        public override CardEffect Clone()
        {
            NormalDash effect = CreateInstance<NormalDash>();
            effect.dis = dis;
            effect.AdditionalEffects_List = CloneAdditionalEffects();
            return effect;
        }

        //public override void DoEffect(ActorController user, List<ActorController> targets)
        //{
        //    dir = user.FaceDir;
        //    var finder = user.GetComponent<PathFinderComponent>();
        //    var path = finder.SearchAndGetPathByEnforcedMove(user.transform.position, dir, dis);
        //    var move = user.GetComponent<ActorMoveComponent>();
        //    move.StartForceMoveByPathList(path);
        //}

        public override void DoEffect(Combat combat)
        {
            var user = isAtking ? combat.Atker : combat.Dfder;

            dir = user.FaceDir;
            var finder = user.GetComponent<PathFinderComponent>();
            var path = finder.SearchAndGetPathByEnforcedMove(user.transform.position, dir, dis, true);
            var move = user.GetComponent<ActorMoveComponent>();
            move.StartForceMoveByPathList(finder.VectorPath2NodePath(path));

            base.DoEffect(combat);  // 附加combat进行
        }
    }
}
