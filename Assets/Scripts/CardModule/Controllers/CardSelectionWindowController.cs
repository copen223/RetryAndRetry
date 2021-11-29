using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.CardModule;
using System;

public class CardSelectionWindowController : MonoBehaviour
{
    [SerializeField]
    private GameObject cardPrefab;
    [SerializeField] Transform cardParent;

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
    public void ShowCardSelectionWindow(List<Card> cards, Action<Card> finishSelectFunc)
    {
        gameObject.SetActive(true);
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
}
