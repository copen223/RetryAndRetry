using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.CardModule;
using Assets.Scripts.ActorModule;

namespace Assets.Scripts.BufferModule.Buffs
{
    /// <summary>
    /// 附带伤害
    /// </summary>
    public class Buff_AdditionalDamage : Buff
    {
        [Header("该buff的触发条件检测函数")]
        public BuffTriggerCheck buffeTriggerCheck;

        public DamageElement TheDamageElement;
        public float DamageValue;

        public override void CheckAndTouchOffBuff(BuffTouchOffEventArgs eventArgs)
        {
            if (!buffeTriggerCheck.CheckIfCanTouchOff(eventArgs))
                return;

            DamageData damageData = new DamageData(DamageValue, Vector2.zero, 99);
            damageData.element = TheDamageElement;

            ActorController target = eventArgs.combat.Atker == controller.Actor ? eventArgs.combat.Dfder : eventArgs.combat.Atker;

            eventArgs.combat.DoHit(target, damageData);

        }
    }
}
