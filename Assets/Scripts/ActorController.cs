using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.ActorModule;

public class ActorController : MonoBehaviour
{
    public int advantage;   // 先攻
    public ActorGroup group;

    // 属性
    private float healPoint;


    // 事件发放
    public List <GameObject> ListenersList = new List<GameObject>();
    public void ActorBroadcastMessage(string message,object data)
    {
        foreach(var lisetner in ListenersList)
        {
            lisetner.SendMessage(message,data);
        }
    }

    public void AddListener(GameObject gb)
    {
        ListenersList.Add(gb);
    }

    // 事件响应

    private List<float> damageDatasList = new List<float>();
    private void OnBehit(float damage)
    {
        if (healPoint - damage <= 0)
            damage = healPoint;
        healPoint -= damage;
        ActorBroadcastMessage("OnBehit", damage);
    }

}

public class DamageData
{
    public float damage;
    public DamageData(float damage)
    {

    }
}


