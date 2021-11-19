using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.CardModule.CardEffects
{
    /// <summary>
    /// 冲刺效果
    /// </summary>
    public class NormalDash : CardEffect
    {
        Vector2 dir;    // 冲刺方向
        int dis;        // 冲刺距离

        /// <summary>
        /// 成为一个效果的附加效果
        /// </summary>
        /// <param name="_dis"></param>
        /// <param name="effect"></param>
        public NormalDash(int _dis)
        {
            dis = _dis;
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
            move.StartForceMoveByPathList(path);

            base.DoEffect(combat);  // 附加combat进行
        }
    }
}
