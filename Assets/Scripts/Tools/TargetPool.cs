using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Tools
{
    public class TargetPool
    {
        public List<GameObject> list = new List<GameObject>();
        private GameObject prefab;

        public TargetPool(GameObject _prefab)
        {
            prefab = _prefab;
        }

        public int GetIndex(GameObject obj)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (obj == list[i])
                {
                    return i;
                }
            }
            return 0;
        }
        public GameObject GetTarget(out int index)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].activeInHierarchy)
                    continue;
                else
                {
                    index = i;
                    return list[i];
                }
            }
            index = list.Count;
            return CreatNewTarget();
        }
        public GameObject GetTarget(int index)
        {
            return list[index];
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

        public void ReSet()
        {
            foreach(var gb in list)
            {
                gb.SetActive(false);
            }
        }
    }
}
