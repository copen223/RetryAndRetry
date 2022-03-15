using ActorModule.Core;
using BufferModule;
using UnityEngine;

namespace CardModule.CardEffects
{

    [CreateAssetMenu(fileName = "Effect", menuName = "MyInfo/效果/给予buff")]
    public class GiveBuff:CardEffect
    {
        [Header("所上的buff，给一个挂有buff组件的Prefab即可")]
        public Buff buff;
        //public GiveBuff(EffectTrigger trigger,string buff)
        //{
        //    Trigger = trigger;
        //    this.buff = buff;
        //    CombatPriority = 3;
        //}
        [Header("是否作用于自己")]
        public bool IsTargetSelf;

        [Header("是否与卡牌绑定")]
        public bool IfLinkToCard;

        public override CardEffect Clone()
        {
            // return Instantiate(this); 事实证明可以直接用这个！！！而不用下面那一大堆
            GiveBuff effect = ScriptableObject.CreateInstance<GiveBuff>();
            effect.Trigger = Trigger;
            effect.buff = buff;
            effect.AdditionalEffects_List = CloneAdditionalEffects();
            effect.IfLinkToCard = IfLinkToCard;
            effect.IsTargetSelf = IsTargetSelf;
            return effect;
        }

        public override void DoEffect(Combat combat)
        {
            ActorController target = null;

            if (IsTargetSelf)
                target = Card.User;
            else
                target = isAtking ? combat.Dfder : combat.Atker;
            
            if(IfLinkToCard)
                target.BuffCon.AddBuff(buff,Card);
            else
                target.BuffCon.AddBuff(buff);



            base.DoEffect(combat);
        }

        public override string GetDescriptionText()
        {
            return "施加" + buff.BuffName + GetAdditionalDescriptionText();
        }
    }
}
