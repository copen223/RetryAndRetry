using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.ActorModule
{

    public class DamageData
    {
        /// <summary>
        /// 伤害来源a
        /// </summary>
        public DamageSource source = DamageSource.Normal;
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
}
