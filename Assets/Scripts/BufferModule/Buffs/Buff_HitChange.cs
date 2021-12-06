using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.BufferModule.Buffs
{
    /// <summary>
    /// 改变hit人物的hit属性
    /// </summary>
    public class Buff_HitChange : Buff
    {
        public int HitChangeValue;

        /// <summary>
        /// 对target对象启用该buff
        /// </summary>
        /// <param name="target"></param>
        public override void LauchBuff()
        {
            Target.Ability.Hit.AddTheAddValue(HitChangeValue);
            OnBuffEndEvent += OnFinish;
        }

        private void OnFinish()
        {
            Target.Ability.Hit.AddTheAddValue(-HitChangeValue);
            OnBuffEndEvent -= OnFinish;
        }

        public void Init(int value, int time)
        {
            DurationTime = time;
            HitChangeValue = value;
        }
    }
}
