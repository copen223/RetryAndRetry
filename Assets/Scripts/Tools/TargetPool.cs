using System.Collections.Generic;
using UnityEngine;

namespace Tools
{
    public class TargetPool
    {
        public List<GameObject> list = new List<GameObject>();
        private GameObject prefab;

        public TargetPool(GameObject _prefab)
        {
            prefab = _prefab;
        }

        /// <summary>
        /// 获取对象池中可用的一个对象
        /// </summary>
        /// <returns></returns>
        public GameObject GetTarget(Transform parent)
        {
            foreach(var bg in list)
            {
                if(!bg.activeInHierarchy)
                {
                    bg.transform.SetParent(parent);
                    return bg;
                }
            }
            var gb = CreatNewTarget();
            gb.transform.SetParent(parent);
            return gb;
        }

        public GameObject GetTarget()
        {
            foreach (var bg in list)
            {
                if (!bg.activeInHierarchy)
                {
                    return bg;
                }
            }
            var gb = CreatNewTarget();
            return gb;
        }

        private GameObject CreatNewTarget()
        {
            var gb = GameObject.Instantiate(prefab);
            gb.SetActive(true);
            list.Add(gb);
            return gb;
        }

        /// <summary>
        /// 将对象池中的所有对象失活。
        /// </summary>
        public void ReSet()
        {
            foreach(var gb in list)
            {
                gb.SetActive(false);
            }
        }

        public void RemoveFromPool(GameObject gb)
        {
            list.Remove(gb);
        }
        public void AddToPool(GameObject gb)
        {
            list.Add(gb);
        }


    }
}
