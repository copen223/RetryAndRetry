using ActorModule;
using SpaceModule.PathfinderModule;
using UnityEngine;

namespace CardModule.CardEffects
{

    [CreateAssetMenu(fileName = "Effect", menuName = "MyInfo/效果/击退")]
    /// <summary>
    /// 冲刺效果
    /// </summary>
    public class NormalBeatBack : CardEffect
    {
        Vector2 dir;    // 击退方向
        public int dis;        // 冲刺距离

        /// <summary>
        /// 成为一个效果的附加效果
        /// </summary>
        /// <param name="_dis"></param>
        public NormalBeatBack(int _dis)
        {
            dis = _dis;
        }

        public override CardEffect Clone()
        {
            NormalBeatBack effect = CreateInstance<NormalBeatBack>();
            effect.dis = dis;
            effect.AdditionalEffects_List = CloneAdditionalEffects();
            effect.Trigger = Trigger;
            return effect;
        }

        public override void DoEffect(Combat combat)
        {
            var target = isAtking ? combat.Dfder : combat.Atker;
            var user = isAtking ? combat.Atker : combat.Dfder;

            if (target.gameObject.tag == "Background")
                return;

            dir = user.FaceDir;
            var finder = target.GetComponent<PathFinderComponent>();
            var path = finder.SearchAndGetPathByEnforcedMove(target.transform.position, dir, dis, true);
            var move = target.GetComponent<ActorMoveComponent>();
            move.StartForceMoveByPathList(finder.VectorPath2NodePath(path));

            base.DoEffect(combat);  // 附加combat进行
        }

        public override string GetDescriptionText()
        {
            return "击退目标<color=green>" + dis + "</color>格" + GetAdditionalDescriptionText();
        }
    }
}
