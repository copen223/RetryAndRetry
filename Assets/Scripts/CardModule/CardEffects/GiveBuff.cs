using Assets.Scripts.ActorModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.CardModule.CardEffects
{

    [CreateAssetMenu(fileName = "Effect", menuName = "MyInfo/效果/给予buff")]
    public class GiveBuff:CardEffect
    {
        string buff;
        public GiveBuff(EffectTrigger trigger,string buff)
        {
            Trigger = trigger;
            this.buff = buff;
            CombatPriority = 3;
        }

        public override CardEffect Clone()
        {
            GiveBuff effect = ScriptableObject.CreateInstance<GiveBuff>();
            effect.Trigger = Trigger;
            effect.buff = buff;
            effect.AdditionalEffects_List = CloneAdditionalEffects();
            return effect;
        }

        public override void DoEffect(Combat combat)
        {
            var target = isAtking ? combat.Dfder : combat.Atker;
            target.OnBuff(buff);
            base.DoEffect(combat);
        }

        public override string GetDescriptionText()
        {
            return "施加" + buff + GetAdditionalDescriptionText();
        }
    }
}
