using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.ActorModule;

public class EnemyController : ActorController
{
    // Start is called before the first frame update
    void Awake()
    {
        advantage = 1;
        group = new ActorGroup("怪物", 1, ActorGroup.GroupType.Enemy);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
