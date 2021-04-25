using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.ActorModule;
using Assets.Scripts.Tools;

public class ActorController : MonoBehaviour
{
    private void Start()
    {
    }


    public int advantage;   // 先攻
    public ActorGroup group;

    // 属性
    public float HealPoint;
    public float HealPoint_Max;

    //-----------------FocusTrail---------------------//
    [Header("需要挂载")]
    private List<GameObject> focusTrails = new List<GameObject>();  // 该单位存在的专注轨迹
    public void AddFocusTrail(GameObject gb) { focusTrails.Add(gb); gb.GetComponent<FocusTrailController>().Actor = gameObject; }
    public void RemoveFocusTrail(GameObject gb) { focusTrails.Remove(gb); gb.GetComponent<FocusTrailController>().Actor = null; }
    public void ShowAllFocusTrail(bool isActive)
    {
        foreach (var gb in focusTrails) gb.SetActive(isActive);
    }

    //----------------FocusTrail-END------------------//

    protected void OnTurnEnd()
    {
        if(BattleManager.instance.CurActorObject == gameObject)
            ShowAllFocusTrail(false);
    }
    protected void OnTurnStart()
    {
        if (BattleManager.instance.CurActorObject == gameObject)
            ShowAllFocusTrail(true);
    }



    // 事件响应

    private List<float> damageDatasList = new List<float>();
    public void OnBehit(float damage)
    {
        if (HealPoint - damage <= 0)
            damage = HealPoint;
        HealPoint -= damage;
        EventInvoke(ActorEvent.OnBehit);
    }
    public void OnHealUp(float heal)
    {
        if(HealPoint + heal >= HealPoint_Max)
            heal = HealPoint;
        HealPoint += heal;
        EventInvoke(ActorEvent.OnHealUp);
    }

    //-------------------事件订阅-------------------//
    public enum ActorEvent
    {
        OnBehit,
        OnHealUp
    }
    public event ActorHandle EventObserver_OnBehit;
    public event ActorHandle EventObserver_OnHealUp;


    public void AddEventObserver(ActorEvent actorEvent, ActorHandle handle)
    {
        switch (actorEvent)
        {
            case ActorEvent.OnBehit: EventObserver_OnBehit += handle; break;
            case ActorEvent.OnHealUp: EventObserver_OnHealUp += handle; break;

        }
    }
    public void RemoveEventObserver(ActorEvent actorEvent, ActorHandle handle)
    {
        switch (actorEvent)
        {
            case ActorEvent.OnBehit: EventObserver_OnBehit -= handle; break;
            case ActorEvent.OnHealUp: EventObserver_OnHealUp -= handle; break;

        }
    }

    public void EventInvoke(ActorEvent actorEvent)
    {
        switch (actorEvent)
        {
            case ActorEvent.OnBehit: EventObserver_OnBehit?.Invoke(gameObject); break;
            case ActorEvent.OnHealUp: EventObserver_OnHealUp?.Invoke(gameObject); break;
        }

    }

    //-----------------事件数据类型定义-------------------//
    public delegate void ActorHandle(GameObject actor);
    public delegate void HealHandle(HealData healData);
    public class HealData { public HealData(GameObject atk, GameObject dfd) { atker = atk;dfder = dfd;} GameObject atker; GameObject dfder; }


}

public class DamageData
{
    public float damage;
    public DamageData(float damage)
    {

    }
}


