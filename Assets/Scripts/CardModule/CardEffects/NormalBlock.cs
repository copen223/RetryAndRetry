using UnityEngine;

namespace CardModule.CardEffects
{

    [CreateAssetMenu(fileName = "Effect", menuName = "MyInfo/效果/格挡")]
    public class NormalBlock: CardEffect
    {
        public float reduceDamage;
        public NormalBlock(float reduce)
        {
            CombatPriority = 2;
            reduceDamage = reduce;
        }

        public void ClearInfluenceCallBack()
        {
            Card.User.Ability.Defense.AddTheAddValue(UnityEngine.Mathf.RoundToInt(- reduceDamage));
            Card.User.ifBlock = false;
            combat.CombatEndEvent -= ClearInfluenceCallBack;
        }

        Combat combat;

        public override void DoEffect(Combat combat)
        {
            this.combat = combat;
            Card.User.Ability.Defense.AddTheAddValue(UnityEngine.Mathf.RoundToInt(reduceDamage));
            Card.User.ifBlock = true;
            //for(int i =0;i < combat.CombatEffects.Count;i++)
            //{
            //    var effect = combat.CombatEffects[i];

            //    if (effect is NormalDamage)
            //    {
            //        if (effect.isAtking && !isAtking)
            //        {
            //            // 攻击方的攻击效果 防守方的该效果
            //            NormalDamage damage = effect as NormalDamage;
            //            damage.DamageAddValue_Combat -= reduceDamage;
            //        }
            //        else if (!effect.isAtking && isAtking)
            //        {
            //        }
            //    }
            //}
            combat.CombatEndEvent += ClearInfluenceCallBack;
            base.DoEffect(combat);  //  附加effect进行
        }

        public override CardEffect Clone()
        {
            NormalBlock effect = CreateInstance<NormalBlock>();
            effect.reduceDamage = reduceDamage;
            effect.CombatPriority = 2;
            effect.AdditionalEffects_List = CloneAdditionalEffects();
            return effect;
        }

        public override string GetDescriptionText()
        {
            return "减少<color=blue>" + reduceDamage + "</color>点伤害" + GetAdditionalDescriptionText();
        }
    }
}
