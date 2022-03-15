using System.Collections.Generic;
using UnityEngine;

namespace Tools.NewTools
{
    public class TargetPool
    {
        public List<GameObject> Targets_Active_List = new List<GameObject>();
        public List<GameObject> Targets_Inactive_List = new List<GameObject>();

        public GameObject prefab;
        public Transform trans;

        public TargetPool(GameObject _prefab, Transform _trans)
        {
            prefab = _prefab;
            trans = _trans;
        }

        public GameObject GetInstance()
        {
            for (int i = 0; i < Targets_Inactive_List.Count; i++)
            {
                if (Targets_Inactive_List[i] != null)
                {
                    var go2 = Targets_Inactive_List[i];
                    go2.GetComponent<TargetInPool>().Active();
                    return go2;
                }
            }

            var go = CreatInstance();
            go.GetComponent<TargetInPool>().Active();
            return go;
        }

        private GameObject CreatInstance()
        {
            GameObject go = GameObject.Instantiate(prefab, trans);
            go.SetActive(false);
            go.AddComponent<TargetInPool>();
            go.GetComponent<TargetInPool>().pool = this;
            go.GetComponent<TargetInPool>().InActive();
            return go;
        }


    }
}
