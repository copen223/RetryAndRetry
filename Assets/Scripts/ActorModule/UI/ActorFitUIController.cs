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
    public GameObject FocusCountUIObject;

    /// <summary>
    /// 切换对象时调用，订阅事件
    /// </summary>
    private void OnSetTarget()
    {
        var actor = Target.GetComponent<ActorController>();
        actor.AddEventObserver(ActorController.ActorEvent.OnBehit, OnHealPointChanged);
        OnHealPointChanged(Target);

        actor.AddEventObserver(ActorController.ActorEvent.OnFoucusTrailChange, OnFoucsTrailChanged);
        OnFoucsTrailChanged(Target);

        actor.FocusCountUI_GO = FocusCountUIObject;
    }
    /// <summary>
    /// 移除委托调用
    /// </summary>
    private void RemoveAllSubject()
    {
        if (target != null)
        {
            Target.GetComponent<ActorController>().RemoveEventObserver(ActorController.ActorEvent.OnBehit, OnHealPointChanged);
            Target.GetComponent<ActorController>().RemoveEventObserver(ActorController.ActorEvent.OnFoucusTrailChange, OnFoucsTrailChanged);
        }
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

    public void OnFoucsTrailChanged(GameObject gb)
    {
        var con = transform.Find("ActorFitFocusTrailCount").GetComponent<ActorFitFocusTrailUI>();
        con.OnFocusTrailCountChanged(gb);
    }

    public void ActiveUI(GameObject gb)
    {

    }

}
