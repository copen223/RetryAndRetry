using UnityEngine;

namespace ActorModule.Core
{

    public class DamageData
    {
        /// <summary>
        /// 伤害来源a
        /// </summary>
        public DamageSource source = DamageSource.Normal;
        /// <summary>
        /// 伤害元素类型
        /// </summary>
        public DamageElement element = DamageElement.None;
        /// <summary>
        /// 伤害数值
        /// </summary>
        public float damage;
        /// <summary>
        /// 伤害的方向
        /// </summary>
        public Vector2 dir;
        /// <summary>
        /// 伤害的命中值
        /// </summary>
        public int hit;
        public DamageData(float _damage, Vector2 _dir,int hit)
        {
            damage = _damage;
            dir = _dir;
            this.hit = hit;
        }

        public DamageData(float _damage, Vector2 _dir, DamageSource _source)
        {
            source = _source;
            damage = _damage;
            dir = _dir;
        }

    }
    /// <summary>
    /// 伤害来源
    /// </summary>
    public enum DamageSource
    {
        Normal = 0,
        Actor = 1,
        EnvirSurface = 2,
        EnvirObject = 4
    }

    /// <summary>
    /// 伤害元素
    /// </summary>
    public enum DamageElement
    {
        None,
        Fire
    }
}
