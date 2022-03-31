using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Tools
{
    /// <summary>
    /// 这是一个生成T的工厂
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MyFactory<T> where T:Component
    {
        public MyFactory(T _prefab,Transform _parent)
        {
            prefab = _prefab;
            parent = _parent;
        }

        private readonly T prefab;
        private readonly Transform parent;
        
        private readonly List<T> waitingTargets = new List<T>();
        private readonly List<T> usingTargets = new List<T>();

        public T GetTarget()
        {
            if (waitingTargets.Count <= 0)
            {
                var target = CreatNewTarget();
                usingTargets.Add(target);
                target.gameObject.SetActive(true);
                return target;
            }
            else
            {
                var target = waitingTargets[0];
                waitingTargets.RemoveAt(0);
                usingTargets.Add(target);
                target.gameObject.SetActive(true);
                return target;
            }
        }

        public void RemoveTarget(T _target)
        {
            usingTargets.Remove(_target);
            waitingTargets.Add(_target);
            _target.gameObject.SetActive(false);
        }

        private T CreatNewTarget()
        {
            var newTarget = Object.Instantiate(prefab, parent);
            return newTarget;
        }

    }
}