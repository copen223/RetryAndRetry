using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.ActorModule;

public abstract class EnvirObjectBeHitHandler : MonoBehaviour
{
    public abstract void HandlerBehit(DamageData data);
}
