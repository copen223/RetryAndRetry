using ActorModule.Core;
using UnityEngine;

namespace BufferModule.Buffs
{
    /// <summary>
    /// 改变hit人物的hit属性
    /// </summary>
    public class Buff_HpChange : Buff
    {
        public float HpChangeValue;

        /// <summary>
        /// 对target对象启用该buff
        /// </summary>
        /// <param name="target"></param>
        public override void LauchBuff()
        {
            Target.OnHealUp(HpChangeValue);
            OnBuffEndEvent += OnFinish;
        }

        private void OnFinish()
        {
            Target.OnInjured(new DamageData(HpChangeValue, Vector2.zero, 99));
            OnBuffEndEvent -= OnFinish;
        }

        public void Init(int value, int time)
        {
            DurationTime = time;
            HpChangeValue = value;
        }
    }
}
