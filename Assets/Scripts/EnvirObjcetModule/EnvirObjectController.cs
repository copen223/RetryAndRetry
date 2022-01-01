using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.ActorModule;

public class EnvirObjectController : ActorController
{
    private void Awake()
    {
        group = new ActorGroup("环境", 0, ActorGroup.GroupType.EnvirObject);
    }

    protected override void OnAfterInjured(DamageData data)
    {
       // if(data.source == DamageSource.Actor && )
    }
    [SerializeField]
    private EnvirObjectBeHitHandler beHitHandler = null;
    public override void OnInjured(DamageData data)
    {
        Sprite.GetComponent<Animator>().SetTrigger("Behit");

        beHitHandler.HandlerBehit(data);
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
