using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Tools;
using Assets.Scripts.ActorModule;

public class FireElementController : MonoBehaviour
{
    [SerializeField]
    private float fireDamage = 2;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("sdfdsf");
        GameObject go = collision.gameObject.name == "Sprite" ? collision.transform.parent.gameObject : collision.gameObject;
        int layer = go.layer;
        if (MyTools.CheckLayerIfCanAttack(layer))
        {
            DamageData data = new DamageData(fireDamage, Vector2.zero, DamageSource.EnvirSurface);
            go.GetComponent<ICanBeHitObject>().OnInjured(data);
        }
    }
}
