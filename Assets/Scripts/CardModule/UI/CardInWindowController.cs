using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.CardModule;
using UnityEngine.EventSystems;
using System;

public class CardInWindowController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler,IPointerClickHandler
{
    #region UI对象链接
    [SerializeField] private GameObject cardView_go = null;
    #endregion

    public bool IsStatusMode;


    #region 储存信息
    private Card card;
    /// <summary>
    /// 正在进行选择的单位
    /// </summary>
    private GameObject selector;


    #endregion

    #region 对外方法
    public void SetCard(Card card)
    {
        this.card = card;
        UpdateView();
    }
    public void SetSelector(GameObject selector)
    {
        this.selector = selector;
    }
    #endregion

    #region 内部方法
    /// <summary>
    /// 更新显示层
    /// </summary>
    private void UpdateView()
    {
        cardView_go.GetComponent<CardViewController>().OnCardChanged(card);
    }
    private bool IfCanUpChange()
    {
        var playerCon = selector.GetComponent<PlayerController>();
        if (playerCon.ActionPoint >= card.cardLevel)
            return true;
        return false;
    }
    #endregion

    #region 事件
    public void OnPointerEnter(PointerEventData eventData)
    {
        cardView_go.transform.localScale = new Vector3(1.2f, 1.2f, 1);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        cardView_go.transform.localScale = new Vector3(1, 1, 1);
    }

    public event Action<Card> OnCardDoSelectedEvent;

    public void OnPointerClick(PointerEventData eventData)
    {
        if(IsStatusMode)
        {
            return;
        }

        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (IfCanUpChange())
                OnCardDoSelectedEvent?.Invoke(card);
            else
                UIManager.instance.CreatFloatUIAt(selector, Vector2.zero, 2f, Color.black, "行动点数不足！");
        }
    }
    #endregion
}
