using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.CardModule.CardStates;
using Assets.Scripts.CardModule;
using Assets.Scripts.Tools;

// 手卡控制器，同时是卡牌UI界面的控制者
public class HandController : MonoBehaviour
{
    [Header("挂载所有卡牌对象")]
    public List<GameObject> CardObjects_list = new List<GameObject>();
    public List<GameObject> ContainerObjects_list = new List<GameObject>();

    public GameObject CardPrefab;
    public GameObject ContainerPrefab;
    // 对象池
    public TargetPool CardPool;
    public TargetPool ContainerPool;

    [Header("UI布局信息")]
    public Vector2 FirstPos;
    public float Spacing;
    public Vector2 ContainerOffset;

    [Header("其他链接")]
    public GameObject Holder;
    public GameObject DeckObject;
    public GameObject DiscardObject;
    public GameObject DrawButtonObject;

    private Deck deck;
    private DiscardPool discard;
    private Hand hand;
    private List<Container> containers = new List<Container>();

    [Header("订阅者")]
    public List<GameObject> ListenerObjectsList = new List<GameObject>();

    void Awake()
    {
        // 初始化对象池
        CardPool = new TargetPool(CardPrefab);
        ContainerPool = new TargetPool(ContainerPrefab);
    }
   
    void Start()
    {
        InitLayout(true);
    }

    // 卡牌布局
    private Vector3 GetCorrectCardPos(int index)
    {
        var pos = new Vector3(FirstPos.x + Spacing * index, FirstPos.y, 0);
        return pos;
    }
    private void InitLayout(bool ifInit)
    {
        for(int i = 0;i<CardObjects_list.Count;i++)
        {
            var card = CardObjects_list[i];
            if (ifInit) card.transform.position = GetCorrectCardPos(i);
            else card.SendMessage("StartMoveToCorrectPos", GetCorrectCardPos(i));
            if(i< ContainerObjects_list.Count)
            card.SendMessage("SetContainer", ContainerObjects_list[i]); // 链接卡牌与卡槽
        }
        for (int i = 0; i < ContainerObjects_list.Count; i++)
        {
            var card = ContainerObjects_list[i];
            card.transform.localPosition = GetCorrectCardPos(i);
            if(i < CardObjects_list.Count)
            card.SendMessage("SetCardObject", CardObjects_list[i]);     // 链接卡牌与卡槽
        }
    }

    //---------------卡牌资源循环----------------//
    public void DrawCard()
    {
        deck.TranslateCardTo(deck.GetFirstCard(),hand);

        for(int i=0;i<CardObjects_list.Count;i++)
        {
            if(CardObjects_list[i].GetComponent<CardController>().currentState is CardInactive)
            {
                CardObjects_list[i].GetComponent<CardController>().Card = hand.list[i];
                CardObjects_list[i].SendMessage("Active");
                return;
            }
        }
    }

    public void DiscardCard(Card card)
    {
        hand.TranslateCardTo(card, discard);
    }

    private void ReplaceCard(int index)
    {
        var container = ContainerObjects_list[index];
        container.SendMessage("OnCardReplaced");

    }


    //---------------消息发送------------------//
    public void HandBroadcastMessage(string message)
    {
        foreach (var gb in ListenerObjectsList) gb.SendMessage(message);
    }


    //---------------消息处理区----------------//
    private void OnCardDisappear(GameObject gb)
    {
        CardObjects_list.Remove(gb);
        CardObjects_list.Add(gb);
    }
    private void OnCardMake(GameObject gb)
    {
        foreach(var card in CardObjects_list)
        {
            if(card != gb)
            {
                card.SendMessage("HandleMessage", CardEvent.Message.CardIsMaking);
            }
        }
    }

    private void OnCardDiscard(GameObject gb)
    {
        //---------------数据---------------//
        DiscardCard(gb.GetComponent<CardController>().Card);
        //---------------显示---------------//
        OnCardDisappear(gb);
        InitLayout(false);
    }

    private void OnDrawCard(Card card)
    {

    }
    private void OnCardMakingOver(GameObject gb)
    {
        foreach (var card in CardObjects_list)
        {
            if (card != gb)
            {
                card.SendMessage("HandleMessage", CardEvent.Message.CardMakingOver);
            }
        }
    }

    private void OnPlayerTurnStart()
    {
        //------------------激活-------------------//
        foreach (var gb in CardObjects_list) gb.SetActive(true);
        foreach (var gb in ContainerObjects_list) gb.SetActive(true);
        DeckObject.SetActive(true);
        DiscardObject.SetActive(true);
        //------------------设置-------------------//
        Holder = BattleManager.instance.CurActorObject;
        PlayerController actor = Holder.GetComponent<PlayerController>();
        deck = actor.deck;
        hand = actor.hand;
        discard = actor.discard;
        containers = actor.containers;
        //-------------对象池与卡牌列表更新------------//
        // 有多少卡就创建多少卡牌对象
        CardObjects_list.Clear();
        for (int i =0;i<hand.list.Count;i++)
        {
            var gb = CardPool.GetTarget(transform);
            gb.SetActive(true);
            gb.GetComponent<CardController>().Card = hand.list[i];
            gb.transform.position = GetCorrectCardPos(i);
            gb.SendMessage("OnReset");
            CardObjects_list.Add(gb);
        }
        // 有多少卡槽就创建多少卡槽对象
        ContainerObjects_list.Clear();
        for (int i = 0;i<containers.Count;i++)
        {
            var gb = ContainerPool.GetTarget(transform);
            gb.SetActive(true);
            gb.GetComponent<ContainerController>().Container = containers[i];
            ContainerObjects_list.Add(gb);
            gb.transform.localPosition = GetCorrectCardPos(i);
            if (i<CardObjects_list.Count)
                gb.GetComponent<ContainerController>().CardObject = CardObjects_list[i];
            else
                gb.GetComponent<ContainerController>().CardObject = null;
            gb.SendMessage("OnReset");
        }

        BattleManager.instance.gameObject.SendMessage("OnTurnStartOver");
    }

    private void OnEnemyTurnStart()
    {
        foreach (var gb in CardObjects_list) gb.SetActive(false);
        foreach (var gb in ContainerObjects_list) gb.SetActive(false);
        DeckObject.SetActive(false);
        DiscardObject.SetActive(false);
    }

    private void OnPlayerTurnDraw()
    {
        // 先弃卡
        for(int j = ContainerObjects_list.Count;j < CardObjects_list.Count;j++)
        {
            hand.TranslateCardTo(hand.list[j], discard);
            CardObjects_list[j].SendMessage("OnDiscard");
            CardObjects_list.RemoveAt(j);
            j--;
        }

        // 再换卡
        int i;
        for (i = 0; i < containers.Count; i++)
        { 
            var container = containers[i];
            // 无卡则抽卡
            if (container.Card == null)
            {
                //----------抽卡数据层---------------//
                deck.TranslateCardTo(deck.GetFirstCard(container.type), hand);
                container.Card = hand.list[i];
                //----------抽卡显示层---------------//
                // 创建新卡
                var cardView = CardPool.GetTarget(transform);
                cardView.SetActive(true);
                CardObjects_list.Add(cardView);
                cardView.transform.localPosition = GetCorrectCardPos(i);
                cardView.GetComponent<CardController>().Card = container.Card;

            }
            // 有卡则换
            else
            {
                //-----------换牌数据层--------------//
                var rep = deck.GetFirstCard(containers[i].type);
                hand.list[i] = rep;  // 手牌已换
                discard.AddCard(rep); // 弃牌区已更新
                deck.RemoveCard(rep); // 卡组已更新
                containers[i].Card = rep; // 卡槽已更新 
                //-----------换牌显示层--------------//
                var cardView = CardObjects_list[i];
                cardView.SendMessage("OnCardReplaced", hand.list[i]);
            }
        }
        InitLayout(false);
    }
}
