using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.CardModule;
using System;
using System.Linq;

public class CardSelectionWindowController : MonoBehaviour
{
    [Header("是否作为StatusUI模块")]
    public bool IsStatusMode;

    [SerializeField]
    private GameObject cardPrefab = null;
    [SerializeField] Transform cardParent = null;
    [SerializeField] GameObject CloseButton = null;

    List<GameObject> cards_list = new List<GameObject>();

    public event Action CancleUpChangeEvent;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Mouse1))
        {
            CancleUpChangeEvent?.Invoke();
        }
    }

    /// <summary>
    /// 显示卡牌选择界面
    /// </summary>
    /// <param name="cards"></param>
    public void ShowCardSelectionWindow(List<Card> cards, Action<Card> finishSelectFunc, GameObject player)
    {
        gameObject.SetActive(true);
        CloseButton.SetActive(false);

        int i = 0;
        for (; i < cards.Count; i++)
        {
            var card = cards[i];
            GameObject go;
            if (i < cards_list.Count)
            {
                go = cards_list[i];
                go.SetActive(true);
            }
            else
            {
                go = Instantiate(cardPrefab, cardParent);
                cards_list.Add(go);
            }

            var con = go.GetComponent<CardInWindowController>();
            con.SetCard(card);
            con.SetSelector(player);
            con.OnCardDoSelectedEvent += finishSelectFunc;
        }
        for (; i < cards_list.Count; i++)
        {
            cards_list[i].SetActive(false);
        }
    }

    /// <summary>
    /// 结束选择窗口的同时，结束监听
    /// </summary>
    /// <param name="finishSelectFunc"></param>
    public void EndWindowShow(Action<Card> finishSelectFunc)
    {
        gameObject.SetActive(false);
        foreach(var card in cards_list)
        {
            card.GetComponent<CardInWindowController>().OnCardDoSelectedEvent -= finishSelectFunc;
        }
    }

    /// <summary>
    /// 仅作查看时使用
    /// </summary>
    /// <param name="cards"></param>
    /// <param name="player"></param>
    public void ShowCardSelectionWindow(List<Card> cards, GameObject player,bool ifSort)
    {
        gameObject.SetActive(true);

        if(!IsStatusMode)
            CloseButton.SetActive(true);

        int i = 0;

        List<Card> waitToShow = new List<Card>(cards);

        if (ifSort)
        {
            waitToShow.Sort(
                (Card card1, Card card2) => 
                {
                    if (card1.cardLevel == card2.cardLevel) return 0;
                    if (card1.cardLevel > card2.cardLevel) return 1;
                    else return -1;
                }
                );
            waitToShow.Sort(
                (Card card1, Card card2) =>
                {
                    return card1.name.CompareTo(card2.name);
                }
                );
        }

        for (; i < waitToShow.Count; i++)
        {
            var card = waitToShow[i];
            GameObject go;
            if (i < cards_list.Count)
            {
                go = cards_list[i];
                go.SetActive(true);
            }
            else
            {
                go = Instantiate(cardPrefab, cardParent);
                cards_list.Add(go);
            }

            var con = go.GetComponent<CardInWindowController>();
            con.SetCard(card);
            con.SetSelector(player);
            con.IsStatusMode = IsStatusMode;
        }
        for (; i < cards_list.Count; i++)
        {
            cards_list[i].SetActive(false);
        }


    }


    /// <summary>
    /// 关闭按钮触发
    /// </summary>
    public void EndWindowShow()
    {
        gameObject.SetActive(false);
    }
}
