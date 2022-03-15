using System.Collections.Generic;
using ActorModule.Core;
using UnityEngine;

namespace EnvirObjcetModule.Handlers
{
    public class CreatFireEnhandler : EnvirObjectBeHitHandler
    {
        [SerializeField]
        private GameObject firePrefab = null;

        [SerializeField]
        private List<Transform> firePositions = new List<Transform>();

        private bool hasHandlered = false;
        public override void HandlerBehit(DamageData data)
        {
            if (hasHandlered)
                return;


            if(data.element == DamageElement.Fire)
            {
                foreach(var position in firePositions)
                {
                    var gb = Instantiate(firePrefab, Vector3.zero, Quaternion.identity, position);
                    gb.transform.localPosition = Vector3.zero;
                }
                hasHandlered = true;
            }
        }
    }
}
