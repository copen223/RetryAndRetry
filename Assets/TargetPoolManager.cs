using System.Collections;
using System.Collections.Generic;
using Tools;
using UnityEngine;

public class TargetPoolManager : MonoBehaviour
{
    public GameObject FocusTrailPrefab;
    public TargetPool FocusTrailPool;
    void Start()
    {
        FocusTrailPool = new TargetPool(FocusTrailPrefab);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
