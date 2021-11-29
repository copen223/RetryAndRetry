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
    [SerializeField] private Text cardName_text;
    [SerializeField] private Text cardDes_text;
    [SerializeField] private Text cardLevel_text;
    [SerializeField] private GameObject cardView_go;
    #endregion

    #region 储存信息
    private Card card;


    #endregion

    #region 对外方法
    public void SetCard(Card card)
    {
        this.card = card;
        UpdateView();
    }
    #endregion

    #region 内部方法
    private void UpdateView()
    {
        cardView_go.transform.localScale = new Vector3(1, 1, 1);
        cardName_text.text = card.name;
        cardDes_text.text = card.GetCardDescription();
        if(card.cardLevel != 0)
            cardLevel_text.text = card.cardLevel + "";
        else
            cardLevel_text.text = "";
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
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            OnCardDoSelectedEvent?.Invoke(card);
        }
    }
    #endregion
}
