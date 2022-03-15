namespace BufferModule.Buffs
{
    /// <summary>
    /// 改变hit人物的hit属性
    /// </summary>
    public class Buff_DfsChange : Buff
    {
        public int DfsChangeValue;

        /// <summary>
        /// 对target对象启用该buff
        /// </summary>
        /// <param name="target"></param>
        public override void LauchBuff()
        {
            Target.Ability.Defense.AddTheAddValue(DfsChangeValue);
            OnBuffEndEvent += OnFinish;
        }

        private void OnFinish()
        {
            Target.Ability.Defense.AddTheAddValue(-DfsChangeValue);
            OnBuffEndEvent -= OnFinish;
        }

        public void Init(int value, int time)
        {
            DurationTime = time;
            DfsChangeValue = value;
        }
    }
}
