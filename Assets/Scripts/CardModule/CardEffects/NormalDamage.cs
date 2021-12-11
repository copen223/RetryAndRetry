﻿using Assets.Scripts.ActorModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.CardModule.CardEffects
{

    [CreateAssetMenu(fileName = "Effect", menuName = "MyInfo/效果/伤害")]
    public class NormalDamage:CardEffect
    {
        [Header("伤害倍率")]
        /// <summary>
        /// 伤害倍率
        /// </summary>
        public float damageValue;

        /// <summary>
        /// Combat加值区
        /// </summary>
        public float DamageAddValue_Combat { get; set; }
        /// <summary>
        /// Combat乘值区
        /// </summary>
        public float DamageMultiValue_Combat { get; set; }

        /// <summary>
        /// 初始化伤害计算公式各项
        /// </summary>
        public void InitDamageValue()
        {
            DamageAddValue_Combat = 0;
            DamageMultiValue_Combat = 1;
        }

        public NormalDamage(float _damageValue, EffectTrigger trigger)
        {
            damageValue = _damageValue;
            Trigger = trigger;
            CombatPriority = 1;
            InitDamageValue();
        }

        public override void DoEffect(Combat combat)
        {
            var damage = damageValue * Card.User.Ability.Attack.FinalValue;

            var dir = isAtking ? combat.Atker.transform.position - combat.Dfder.transform.position : combat.Dfder.transform.position - combat.Atker.transform.position;
            float finalDamage = (damage + DamageAddValue_Combat) * DamageMultiValue_Combat;
            if (finalDamage < 0)
                finalDamage = 0;

            ActorController target = isAtking ? combat.Dfder : combat.Atker;

            combat.DoHit(target, new DamageData(finalDamage, dir, Card.User.Ability.Hit.FinalValue));

            InitDamageValue();

            base.DoEffect(combat);
        }

        public override CardEffect Clone()
        {
            NormalDamage effect = CreateInstance<NormalDamage>();
            effect.damageValue = damageValue;
            effect.Trigger = Trigger;
            effect.CombatPriority = 1;
            effect.InitDamageValue();
            effect.AdditionalEffects_List = CloneAdditionalEffects();
            return effect;
        }

        public override string GetDescriptionText()
        {
            string text = "造成<color=red>" + Card.User.Ability.Attack.FinalValue + "</color>点伤害"+ GetAdditionalDescriptionText();
            return text;
        }
    }
}
