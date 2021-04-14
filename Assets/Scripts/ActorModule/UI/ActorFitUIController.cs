using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Tools;

public class ActorFitUIController : MonoBehaviour,ITargetInPool
{
    // 设置参数

    // 链接
    public GameObject Target { set { target = value; OnSetTarget(); } get { return target; } } //  目标
    private GameObject target;

    public GameObject HealPointUIObject;

    private void OnSetTarget()
    {

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


}
