using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Tools.NewTools;

public class TargetInPool : MonoBehaviour
{
    public TargetPool pool;

    public void InActive()
    {
        gameObject.SetActive(false);
        pool.Targets_Active_List.Remove(gameObject);
        pool.Targets_Inactive_List.Add(gameObject);
    }

    public void Active()
    {
        gameObject.SetActive(true);
        pool.Targets_Active_List.Add(gameObject);
        pool.Targets_Inactive_List.Remove(gameObject);
    }
}
