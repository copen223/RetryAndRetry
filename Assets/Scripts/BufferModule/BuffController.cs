using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.BufferModule;
using System;
using Assets.Scripts.CardModule;

public class BuffController : MonoBehaviour
{
    /// <summary>
    /// 这个buff管理器所在的对象
    /// </summary>
    public ActorController Actor { get { return transform.parent.gameObject.GetComponent<ActorController>(); } }

    private void Start()
    {
        buffParent = transform;
    }

    private Transform buffParent;

    public List<Buff> buffs = new List<Buff>();

    /// <summary>
    /// 给人物添加buff，参数是buff的原型
    /// </summary>
    /// <param name="buffPrefab"></param>
    public void AddBuff(Buff buffPrefab)
    {
        var newBuff = Instantiate(buffPrefab,buffParent);
        newBuff.controller = this;
        newBuff.Target = transform.parent.GetComponent<ActorController>();
        newBuff.Init();

        buffs.Add(newBuff);

        if (newBuff.ActiveType == BuffActiveType.Sustainable)
            newBuff.LauchBuff();

        OnBuffChangeEvent?.Invoke(transform.parent.gameObject);
    }
    /// <summary>
    /// 给人物添加buff，参数是buff的原型，并将该buff与card绑定
    /// </summary>
    /// <param name="buffPrefab"></param>
    /// <param name="card"></param>
    public void AddBuff(Buff buffPrefab, Card card)
    {
        var newBuff = Instantiate(buffPrefab, buffParent);
        newBuff.controller = this;
        newBuff.Target = transform.parent.GetComponent<ActorController>();
        newBuff.Init();

        buffs.Add(newBuff);

        if (newBuff.DurationType == BuffDurationType.Focus)
            newBuff.LinkToCard(card);

        if (newBuff.ActiveType == BuffActiveType.Sustainable)
            newBuff.LauchBuff();

        OnBuffChangeEvent?.Invoke(transform.parent.gameObject);
    }

    /// <summary>
    /// 将buff移出列表，但buff物体尚未摧毁;由buff生命周期结束时的OnbuffFinished调用
    /// </summary>
    /// <param name="buff"></param>
    public void RemoveBuff(Buff buff)
    {
        buffs.Remove(buff);

        OnBuffChangeEvent?.Invoke(transform.parent.gameObject);
    }
    /// <summary>
    /// 驱散buff，强制移除某buff
    /// </summary>
    /// <param name="buff"></param>
    public void DisperseBuff(Buff buff)
    {
        buff.OnBuffFinished();
    }
    /// <summary>
    /// 驱散符合checkFunc条件的buff，最多驱散maxNum个
    /// </summary>
    /// <param name="checkFunc"></param>
    /// <param name="maxNum"></param>
    public void DisperseBuff(DisperseBuffCheckFunc checkFunc,int maxNum)
    {
        int dispersedNum = 0;
        for(int i=0;i<buffs.Count;i++)
        {
            if(checkFunc(buffs[i]))
            {
                DisperseBuff(buffs[i]);
                i--;
                dispersedNum++;
                if(dispersedNum >= maxNum)
                {
                    return;
                }
            }
        }
    }

    public delegate bool DisperseBuffCheckFunc(Buff buff); 


    /// <summary>
    /// 触发buff1
    /// 该单位回合开始时调用，触发回合生效型buff，持续回合数-1
    /// </summary>
    public void OnTurnStart()
    {
        for(int i =0;i<buffs.Count;i++)
        {
            if (!buffs[i].IfAcitve)
                continue;

            if (buffs[i].ActiveType == BuffActiveType.Turn)
                buffs[i].LauchBuff();

            if (buffs[i].ReduceTime(1))
                i--;
        }
    }

    /// <summary>
    /// 触发buff2
    /// 按照时点buffTriggerType进行buff触发
    /// </summary>
    /// <param name="buffTriggerType"></param>
    /// <param name="eventArgs"></param>
    public void TouchOffBuff(BuffTriggerType buffTriggerType,BuffTouchOffEventArgs eventArgs)
    {
        foreach(var buff in buffs)
        {
            if (!buff.IfAcitve)
                return;

            if(buff.TriggerType == buffTriggerType)
            {
                buff.CheckAndTouchOffBuff(eventArgs);
            }
        }
    }


    #region 事件
    /// <summary>
    /// buff改变时触发的事件，用于通知UI变化
    /// </summary>
    public event Action<GameObject> OnBuffChangeEvent;
    #endregion

}
