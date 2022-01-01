using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.BufferModule.Buffs
{
    /// <summary>
    /// 改变hit人物的hit属性
    /// </summary>
    public class Buff_DdgChange : Buff
    {
        public int DdgChangeValue;

        /// <summary>
        /// 对target对象启用该buff
        /// </summary>
        /// <param name="target"></param>
        public override void LauchBuff()
        {
            Target.Ability.Dodge.AddTheAddValue(DdgChangeValue);
            OnBuffEndEvent += OnFinishCallback;
        }

        private void OnFinishCallback()
        {
            Target.Ability.Dodge.AddTheAddValue(-DdgChangeValue);
            OnBuffEndEvent -= OnFinishCallback;
        }

        public void Init(int value, int time)
        {
            DurationTime = time;
            DdgChangeValue = value;
        }
    }
}
