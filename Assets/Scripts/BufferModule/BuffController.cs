using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.BufferModule;

public class BuffController : MonoBehaviour
{
    private void Start()
    {
        buffParent = transform;
    }

    private Transform buffParent;

    public List<Buff> buffs = new List<Buff>();
    public void AddBuff(Buff buffPrefab)
    {
        var newBuff = Instantiate(buffPrefab,buffParent);
        newBuff.controller = this;
        buffs.Add(newBuff);

        if (newBuff.ActiveType == BuffActiveType.Sustainable)
            newBuff.LauchBuff();
    }

    public void RemoveBuff(Buff buff)
    {
        buffs.Remove(buff);
    }
}
