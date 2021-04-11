using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.CardModule;
using Assets.Scripts.Tools;

public class ContainerController : MonoBehaviour,ITargetInPool
{
    // 链接

    //--------------属性--------------//
    public CardType Type { get { return container.type; } }

    private Container container;
    public Container Container { get { return container; } set { container = value; } }
    
    //------------消息响应------------//
    public void SetCard(Card card)
    {
        container.Card = card;
    }

    public void OnReset()
    {

    }
}
