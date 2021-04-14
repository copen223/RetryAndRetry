using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Tools;

public class ActorFitUIController : MonoBehaviour,ITargetInPool
{
    // 设置参数

    // 链接
    public GameObject Target { set { RemoveAllSubject(); target = value; OnSetTarget(); } get { return target; } } //  目标
    private GameObject target;

    public GameObject HealPointUIObject;

    private void OnSetTarget()
    {
        Target.GetComponent<ActorController>().AddEventObserver(ActorController.ActorEvent.OnBehit, OnHealPointChanged);
        OnHealPointChanged(Target);
    }
    private void RemoveAllSubject()
    { 
        if( target != null)
            Target.GetComponent<ActorController>().RemoveEventObserver(ActorController.ActorEvent.OnBehit, OnHealPointChanged); 
    }
    public void OnReset()
    {

    }

    void Update()
    {
        UpdatePosition();   // 更新位置
    }

    void UpdatePosition()
    {
        var worldPos = Target.transform.position;
        var scrrenPos = Camera.main.WorldToScreenPoint(worldPos);
        transform.position = scrrenPos;
    }

    // 血量变化时改变血条显示
    public void OnHealPointChanged(GameObject gb)
    {
        var con = HealPointUIObject.GetComponent<ActorFitHealPointUI>();
        con.OnHealPointChanged(gb);
    }


}
