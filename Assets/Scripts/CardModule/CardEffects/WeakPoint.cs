using UI;
using UnityEngine;

namespace CardModule.CardEffects
{

    [CreateAssetMenu(fileName = "EWeakPoint", menuName = "MyInfo/效果/弱点")]
    public class WeakPoint:CardEffect
    {
        [Header("受伤倍率")]
        /// <summary>
        /// 伤害倍率
        /// </summary>
        public float injuredValue;

        public override void DoEffect(Combat combat)
        {
            var v = combat.GetActorValue(Card.User);
            v.BeHitMutiValue *= injuredValue;

            Vector3 worldPos1 = Card.User.Sprite.transform.position;
            Vector3 worldPos2 = combat.GetOtherActor(Card.User).Sprite.transform.position;
            Vector3 uiWorldPos = (worldPos1 + worldPos2) / 2;
            UIManager.instance.CreatFloatUIAt(uiWorldPos, Vector2.zero, 1f, Color.red, "暴击");

            base.DoEffect(combat);
        }

        public override CardEffect Clone()
        {
            WeakPoint effect = CreateInstance<WeakPoint>();
            effect.Trigger = Trigger;
            effect.CombatPriority = 3;
            effect.AdditionalEffects_List = CloneAdditionalEffects();
            effect.injuredValue = injuredValue;
            return effect;
        }

        public override string GetDescriptionText()
        {
            string text = "受到<color=red>" + injuredValue + "</color>倍伤害"+ GetAdditionalDescriptionText();
            return text;
        }
    }
}
