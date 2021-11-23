using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.ActorModule;

public class EnvirObjectController : ActorController
{
    private void Awake()
    {
        group = new ActorGroup("环境物体", 0, ActorGroup.GroupType.EnvirObject);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
