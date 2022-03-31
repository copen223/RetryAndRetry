using System;
using Tools;
using UnityEngine;

namespace Physics
{
    public class PhysicsManager : MonoBehaviour
    {
        public static PhysicsManager Insatance;
        
        [SerializeField] private PhysicsHitTarget targetPrefab;
        [SerializeField] private Transform targetParent;
        private MyFactory<PhysicsHitTarget> hitTargetFactory;


        private void Awake()
        {
            if (Insatance == null)
                Insatance = this;
            else
            {
                Destroy(this);
            }
        }

        private void Start()
        {
            hitTargetFactory = new MyFactory<PhysicsHitTarget>(targetPrefab, targetParent);
        }

        /// <summary>
        /// 设置一个碰撞体,销毁请使用脚本上的ReturnToFactory手动销毁
        /// </summary>
        /// <param name="size"></param>
        /// <param name="worldPosition"></param>
        public PhysicsHitTarget SetHitTargetAt(Vector2 size, Vector3 worldPosition)
        {
            var target = hitTargetFactory.GetTarget();
            target.transform.position = worldPosition;
            target.Init(hitTargetFactory,size);
            return target;
        }
    }
}
