using UnityEngine;

namespace CardModule.CardEffects
{

    [CreateAssetMenu(fileName = "消耗", menuName = "MyInfo/效果/消耗")]
    public class Consume : CardEffect
    {

        public override void DoEffect(Combat combat)
        {
            Card.ifDisapear = true;
            base.DoEffect(combat);
        }

        public override CardEffect Clone()
        {
            Consume effect = CreateInstance<Consume>();
            effect.Trigger = EffectTrigger.OnDiscard;
            effect.CombatPriority = 0;
            effect.AdditionalEffects_List = CloneAdditionalEffects();
            return effect;
        }

        public override string GetDescriptionText()
        {
            string text = "<color=green>消耗</color>"+ GetAdditionalDescriptionText();
            return text;
        }
    }
}
