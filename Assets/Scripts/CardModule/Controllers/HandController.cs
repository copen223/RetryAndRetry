using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.CardModule.CardStates;
using Assets.Scripts.CardModule;
using Assets.Scripts.Tools;
using Assets.Scripts.BattleModule.BattleStates;

// 手卡控制器，同时是卡牌UI界面的控制者
public class HandController : MonoBehaviour
{
    public static HandController instance;
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
    public Transform CardsParent;
    public Transform ContainersParent;
    public float ShowHandTime;
    public Vector3 ShowHandPos;
    public Vector3 HideHandPos;
    private bool isCorouting = false;

    [Header("其他链接")]
    public GameObject Holder;
    public GameObject DeckObject;
    public GameObject DiscardObject;
    public GameObject DrawButtonObject;

    private Deck deck { get { return Holder.GetComponent<PlayerController>().deck; } }
    private DiscardPool discard { get { return Holder.GetComponent<PlayerController>().discard; } }
    private Hand hand { get { return Holder.GetComponent<PlayerController>().hand; } }
    private UpChangeDeck upChangeDeck { get { return Holder.GetComponent<PlayerController>().upChangeDeck; } }
    private List<Container> containers { get { return Holder.GetComponent<PlayerController>().containers; }set { Holder.GetComponent<PlayerController>().containers = value; } }

    [Header("订阅者")]
    public List<GameObject> ListenerObjectsList = new List<GameObject>();

    [Header("卡牌列表显示")]
    public List<string> HandNameList = new List<string>();
    public List<string> DeckNameList = new List<string>();
    public List<string> DiscardNameList = new List<string>();

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
        // 初始化对象池
        CardPool = new TargetPool(CardPrefab);
        ContainerPool = new TargetPool(ContainerPrefab);
    }
   
    void Start()
    {
        // 订阅事件
        BattleManager.instance.AddEventObserver(BattleManager.BattleEvent.PlayerTurnStart, OnPlayerTurnStartCallBack);
        BattleManager.instance.AddEventObserver(BattleManager.BattleEvent.PlayerDrawStart, OnPlayerTurnDrawCallBack);
        BattleManager.instance.AddEventObserver(BattleManager.BattleEvent.TurnEnd, OnPlayerTurnEndCallBack);

        
        InitLayout(true);
    }

    private void Update()
    {
        if(!isCorouting)
        {
            // 卡牌移动动画进行
            if(MoveHandCoroutines.Count>0)
            {
                StartCoroutine(MoveHand(MoveHandCoroutines[0]));
            }
        }
    }

    #region 卡牌布局相关
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
            if (ifInit) card.transform.localPosition = GetCorrectCardPos(i);
            else if(card.activeInHierarchy) card.GetComponent<CardController>().StartMoveToCorrectPos(GetCorrectCardPos(i));
        }
        for (int i = 0; i < ContainerObjects_list.Count; i++)
        {
            var card = ContainerObjects_list[i];
            card.transform.localPosition = GetCorrectCardPos(i);
        }
    }
    #endregion

    #region 卡牌行为例如抽卡,还有冻结方法
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

    private void DiscardCard(Card card)
    {
        var player = BattleManager.instance.CurActorObject.GetComponent<PlayerController>();
        player.DiscardCard(card);
    }

    /// <summary>
    /// 卡牌被丢弃的时候，从列表的最开始放到列表的最后
    /// </summary>
    /// <param name="gb"></param>
    private void OnCardDisappear(GameObject gb)
    {
        CardObjects_list.Remove(gb);
        CardObjects_list.Add(gb);
    }
    /// <summary>
    /// 当正在处理某张卡时，你希望冻结其他卡的交互请使用这个方法,记得要解冻
    /// </summary>
    /// <param name="gb"></param>
    /// <param name="isStart"></param>
    public void OnCardMakeDo(GameObject gb,bool isStart)
    {
        foreach(var card in CardObjects_list)
        {
            if(card != gb && card.activeInHierarchy)
            {
                card.GetComponent<CardController>().SetInteractActive(!isStart);
                UIManager.instance.IfActiveUIInteraction = !isStart;
            }
        }
    }

    /// <summary>
    /// 卡牌对象被丢弃时调用该方法告诉hand进行队列的切换
    /// </summary>
    /// <param name="gb"></param>
    public void OnCardDiscard(GameObject gb)
    {
        //---------------数据---------------//
        DiscardCard(gb.GetComponent<CardController>().Card);
        //---------------显示---------------//
        OnCardDisappear(gb);
        InitLayout(false);
    }
    #endregion

    #region 卡牌移动事件排队进行

    IEnumerator MoveHand(MoveHandInfo info)
    {
        isCorouting = true;
        Vector3 startPos = transform.localPosition;
        Vector3 newPos = transform.localPosition;
        Vector3 pos = info.pos;
        float timer = 0;
        while (newPos != pos)
        {
            newPos = Vector3.Lerp(startPos, pos, timer / ShowHandTime);
            transform.localPosition = newPos;
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        if (info.isStart)
            BattleManager.instance.GetComponent<BattleTurnStart>().OnTurnStartOver(true);
        MoveHandCoroutines.RemoveAt(0);
        isCorouting = false;
    }
    class MoveHandInfo { public Vector3 pos; public bool isStart; public MoveHandInfo(Vector3 _pos, bool _isStart) { pos = _pos;isStart = _isStart; } }

    /// <summary>
    /// 移动手牌的事件队列
    /// </summary>
    private List<MoveHandInfo> MoveHandCoroutines = new List<MoveHandInfo>();
    
    IEnumerator StartCoroutineDelay(string _name,object value,float time)
    {
        float timer = 0;
        while(timer<time)
        {
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        StartCoroutine(_name,value);
    }

    #endregion

    #region 卡牌的刷新与创建

    /// <summary>
    /// 用新的数据层来刷新手卡显示层，是更新函数
    /// </summary>
    public void ResetHandCards()
    {
        for (int i= 0;i < hand.list.Count;i++)
        {
            if (i >= CardObjects_list.Count)
            {
                CreatNewCardView();
            }

            var card = CardObjects_list[i];

            card.GetComponent<CardController>().Card = hand.list[i];
            card.GetComponent<CardController>().OnReset();
        }
    }
    /// <summary>
    /// 上述函数的另一个声明
    /// </summary>
    public void UpdateHandCardsView()
    {
        ResetHandCards();
    }
    private void CreatNewCardView()
    {
        var cardView = CardPool.GetTarget(CardsParent);
        cardView.SetActive(true);
        CardObjects_list.Add(cardView);     // 更新显示层对象列表

        //cardView.GetComponent<CardController>().Card = hand.list[i];    // 更新显示层与数据链接
        cardView.transform.localPosition = GetCorrectCardPos(CardObjects_list.IndexOf(cardView));    // 根据列表Index更新位置
                                                                                                     //cardView.SendMessage("OnReset");
        cardView.GetComponent<CardController>().OnReset();
    }
    #endregion

    #region 回调函数，玩家回合开始、结束的响应,包含换卡的硬代码
    /// <summary>
    /// 回合开始时，根据玩家hand数据进行卡牌和卡槽对象的创建
    /// </summary>
    public void OnPlayerTurnStartCallBack()
    {
        //------------------hand显示--------------------//
        MoveHandCoroutines.Add(new MoveHandInfo(ShowHandPos, true));

        //------------------激活/重置-------------------//
        foreach (var gb in CardObjects_list) gb.SetActive(false);
        foreach (var gb in ContainerObjects_list) gb.SetActive(false);
        DeckObject.SetActive(true);
        DiscardObject.SetActive(true);
        //------------------设置-------------------//
        Holder = BattleManager.instance.CurActorObject; // 持有者设置
        PlayerController actor = Holder.GetComponent<PlayerController>();

        //-------------对象池与卡牌列表更新------------//
        // 有多少卡就创建多少卡牌对象
        CardObjects_list.Clear();
        for (int i = 0; i < hand.list.Count; i++)
        {
            var gb = CardPool.GetTarget(CardsParent);
            gb.SetActive(true);
            gb.GetComponent<CardController>().Card = hand.list[i];  // 更新显示与数据的链接
            gb.transform.localPosition = GetCorrectCardPos(i);   // 确定位置
            //gb.SendMessage("OnReset");
            gb.GetComponent<CardController>().OnReset();        // 应用更新后的card数据
            CardObjects_list.Add(gb);
        }
        // 有多少卡槽就创建多少卡槽对象
        ContainerObjects_list.Clear();
        for (int i = 0; i < containers.Count; i++)
        {
            var gb = ContainerPool.GetTarget(ContainersParent);
            gb.SetActive(true);
            gb.GetComponent<ContainerController>().Container = containers[i];
            ContainerObjects_list.Add(gb);
            gb.transform.localPosition = GetCorrectCardPos(i);  // 确定位置

            gb.GetComponent<ContainerController>().OnReset();
        }


    }
    /// <summary>
    /// 监听BattleManager的回合开始事件的回调函数
    /// </summary>
    public void OnPlayerTurnDrawCallBack()
    {
        bool isChanging = false;
        // 先弃掉不在卡槽中的卡
        for (int j = ContainerObjects_list.Count; j < CardObjects_list.Count; j++)
        {
            // 数据层
            var player = BattleManager.instance.CurActorObject.GetComponent<PlayerController>();
            DiscardCard(CardObjects_list[j].GetComponent<CardController>().Card);
            // 显示层
            CardObjects_list[j].GetComponent<CardController>().OnDiscard();
            CardObjects_list.RemoveAt(j);                   // 移出显示对象
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
                //----------抽卡显示层---------------//
                // 创建新卡
                var cardView = CardPool.GetTarget(CardsParent);
                cardView.SetActive(true);
                CardObjects_list.Add(cardView);     // 更新显示层对象列表
                cardView.GetComponent<CardController>().Card = hand.list[i];    // 更新显示层与数据链接
                cardView.transform.localPosition = GetCorrectCardPos(i);    // 根据列表Index更新位置
                //cardView.SendMessage("OnReset");
                cardView.GetComponent<CardController>().OnReset();
            }
            // 有卡则换
            else
            {
                isChanging = true;
                //-----------换牌数据层--------------//
                var rep = deck.GetFirstCard(containers[i].type);
                hand.ReplaceCardTo(i, rep, discard);
                deck.RemoveCard(rep);

                //discard.AddCard(hand.list[i]); // 弃牌区已更新
                //hand.list[i] = rep;  // 手牌已换
                //deck.RemoveCard(rep); // 卡组已更新

                containers[i].Card = rep; // 卡槽已更新 
                //-----------换牌显示层--------------//
                var cardView = CardObjects_list[i];
                cardView.GetComponent<CardController>().OnCardReplaced(hand.list[i]);
                //cardView.SendMessage("OnCardReplaced", hand.list[i]);
            }
        }
        InitLayout(true);

        foreach (var card in CardObjects_list)
        {
            card.GetComponent<CardController>().SetInteractActive(true);
        }

        HandNameList.Clear();
        foreach (var card in hand.list) { HandNameList.Add(card.name); }
        DeckNameList.Clear();
        foreach (var card in deck.list) { DeckNameList.Add(card.name); }
        DiscardNameList.Clear();
        foreach (var card in discard.list) { DiscardNameList.Add(card.name); }


        // 返回信息
        if (!isChanging)
            BattleManager.instance.GetComponent<BattleTurnDraw>().OnDrawOver();
    }
    private void OnPlayerTurnEndCallBack()
    {
        // 交互限制
        foreach(var card in CardObjects_list)
        {
            card.GetComponent<CardController>().SetInteractActive(false);
        }

        MoveHandCoroutines.Add(new MoveHandInfo(HideHandPos, false));
    }
    #endregion


}
