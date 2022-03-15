using ActorModule.Core;
using UnityEngine;

namespace EnvirObjcetModule
{
    public abstract class EnvirObjectBeHitHandler : MonoBehaviour
    {
        public abstract void HandlerBehit(DamageData data);
    }
}
