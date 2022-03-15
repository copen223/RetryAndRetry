namespace BufferModule.Buffs
{
    /// <summary>
    /// 改变hit人物的hit属性
    /// </summary>
    public class Buff_AtkChange : Buff
    {
        public int AtkChangeValue;

        /// <summary>
        /// 对target对象启用该buff
        /// </summary>
        /// <param name="target"></param>
        public override void LauchBuff()
        {
            Target.Ability.Attack.AddTheAddValue(AtkChangeValue);
            OnBuffEndEvent += OnFinish;
        }

        private void OnFinish()
        {
            Target.Ability.Attack.AddTheAddValue(-AtkChangeValue);
            OnBuffEndEvent -= OnFinish;
        }

        public void Init(int value,int time)
        {
            DurationTime = time;
            AtkChangeValue = value;
        }
    }
}
