using Assets.Scripts.ActorModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.CardModule.CardEffects
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
