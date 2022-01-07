﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.CardModule;
using Assets.Scripts.Tools;
using UnityEngine.UI;

public class ContainerController : MonoBehaviour,ITargetInPool
{
    // 链接
    public Image ContainerColorImage;
    public Color ActiveTypeColor;
    public Color PassiveTypeColor;


    //--------------属性--------------//
    public CardUseType Type { get { return container.type; } }

    public Container Container { get { return container; } set { container = value; OnContainerChanged(); } }
    private Container container;

    private void OnContainerChanged()
    {
        if (Type == CardUseType.Active) ContainerColorImage.color = ActiveTypeColor;
        else if (Type == CardUseType.Passive) ContainerColorImage.color = PassiveTypeColor;
    }

    public void OnReset()
    {
        
    }

    //------------消息响应------------//
}