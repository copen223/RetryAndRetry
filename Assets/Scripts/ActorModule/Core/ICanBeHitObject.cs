namespace ActorModule.Core
{
    /// <summary>
    /// 可交互对象接口，该对象能被交互
    /// </summary>
    interface ICanBeHitObject
    {
        /// <summary>
        /// 受伤方法
        /// </summary>
        /// <param name="data"></param>
        void OnInjured(DamageData data);
    }
}
