using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.CardModule;
using Assets.Scripts.CardModule.CardActions;
using Assets.Scripts.CardModule.CardEffects;
using Assets.Scripts.ActorModule;
using Assets.Scripts.ActorModule.ActorStates;
using UnityEngine.EventSystems;

public class PlayerController : ActorController
{
    /// <summary>
    /// 测试用标签，用于设定不同的阵营
    /// </summary>
    [Header("测试用标签，用于设定不同的阵营")]
    public bool IfEnemy; 

    public Hand hand;
    public DiscardPool discard;
    public Deck deck;
    public UpChangeDeck upChangeDeck;
    public List<Container> containers = new List<Container>();

    [Header("牌组信息")]
    public DeckInfo DeckInfo;
    public DeckInfo UpChangeDeckInfo;

    [Header("属性")]
    private int CardDrawNum;
    public int ActionPoint { set { ChangeActionPoint(value); }get { return actionPoint; } }
    private int actionPoint;
    [Header("属性设置")]
    public int ActionPoint_Max;
    public int ActionPoint_Resume;
    public int MovePoint { set { ChangeMovePoint(value); } get { return movePoint; } }
    private int movePoint;
    public int MovePoint_Max;
    public int MovePoint_Resume;

    #region 属性改变方法

    // debuge
    public List<string> deckList_debug;
    public List<string> handList_debug;
    public List<string> discardList_debug;
    public List<string> upChangeList_debug;

    private void ChangeActionPoint(int value)
    {
        //if (BattleManager.instance.CurActorObject != gameObject)
        //    return;
        if (value < 0)
            value = 0;
        if (value > ActionPoint_Max)
            value = ActionPoint_Max;
        actionPoint = value;
        var ui = UIManager.instance.transform.Find("PlayerResource").transform.Find("ActionPoint").GetComponent<TextUIController>();
        ui.ChangeValue(value);

    }
    private void ChangeMovePoint(int value)
    {
        if (BattleManager.instance.CurActorObject == gameObject)
        {
            if (value < 0)
                value = 0;
            if (value > MovePoint_Max)
                value = MovePoint_Max;
            movePoint = value;
            var ui = UIManager.instance.transform.Find("PlayerResource").transform.Find("MovePoint").GetComponent<TextUIController>();
            ui.ChangeValue(value);
        }
    }
    #endregion


    // 进入战斗时，监听战斗管理器的回合开始与回合结束事件
    public override void OnEnterBattle()
    {
        
    }

    public override void OnTurnEnd()
    {
        base.OnTurnEnd();
        ActionPoint = 0;
        currentState.ChangeStateTo<ActorNoActionIdle>();
        ShowAllFocusTrail(false);
    }

    public override void OnTurnStart()
    {
        base.OnTurnStart(); 
        currentState.ChangeStateTo<ActorActionIdle>();
        // ShowAllFocusTrail(true);
        // 恢复各项数值
        ActionPoint += ActionPoint_Resume;
        MovePoint += MovePoint_Resume;
    }

    #region 卡牌相关
    /// <summary>
    /// 丢弃手卡
    /// </summary>
    /// <param name="card"></param>
    public void DiscardCard(Card card)
    {
        if(hand.list.Contains(card))
        {
            if (card.situation == CardSituation.Focused)
            {
                // 遇到专注的卡牌要进行轨迹、卡牌、人物的解绑和销毁专注轨迹
                card.situation = CardSituation.Idle;
                var focusObject = card.CancleFocusTrail();
                RemoveFocusTrail(focusObject);
                Destroy(focusObject);
            }

            hand.TranslateCardTo(card, discard);
        }
    }
    #endregion

    #region 生命周期

    public Transform StatesChild = null;
    private void Start()
    {
        ActorStates = new List<ActorState>(StatesChild.GetComponents<ActorState>());
        currentState = ActorStates.Find((x) => { return (x is ActorNoActionIdle); });

        GameManager.instance.AddListener(GameManager.GameEvent.EnterBattle, OnEnterBattle);
        Sprite.GetComponent<ActorSpriteController>().MouseEnterEvent += OnMouseEnterCallback;
        Sprite.GetComponent<ActorSpriteController>().MouseExitEvent += OnMouseExitCallback;
    }

    private void Awake()
    {
        containers = new List<Container>() { new Container(CardUseType.Active), new Container(CardUseType.Active | CardUseType.Passive), new Container(CardUseType.Active | CardUseType.Passive), new Container(CardUseType.Active | CardUseType.Passive) };
        //deck = new Deck(this, new List<Card> { new Card("打击", CardType.Active, new List<CardEffect>(){new NomalDamage(2f,EffectTrigger.OnCombatAtk)}, new AttackTrail(0, 5, 1)), new Card("闪躲", CardType.Passive, new List<CardEffect> { new NomalDodge() },new FocusTrail(1,1)), new Card("火焰冲击", CardType.Active, null), new Card("肉蛋葱鸡", CardType.Active, null), new Card("无视", CardType.Active, null), new Card("原声大碟", CardType.Active, null), new Card("嘿嘿嘿", CardType.Passive, null), new Card("不会吧", CardType.Passive, null) });
        // BattleReady 决定Deck
        hand = new Hand(this, new List<Card>());
        discard = new DiscardPool(this, new List<Card>());
        //advantage = 3;
        // 设定阵营
        if (!IfEnemy)
            group = new ActorGroup("主角", 0, ActorGroup.GroupType.Player);
        else group = new ActorGroup("敌人", 0, ActorGroup.GroupType.Enemy);
    }

    private void Update()
    {
        currentState.StateUpdate();
        // debug
        deckList_debug.Clear();
        handList_debug.Clear();
        discardList_debug.Clear();
        upChangeList_debug.Clear();
        if (deck == null || hand == null || discard == null || upChangeDeck == null)
            return;
        foreach (var card in deck.list)
        {
            deckList_debug.Add(card.name);
        }
        foreach (var card in hand.list)
        {
            handList_debug.Add(card.name);
        }
        foreach (var card in discard.list)
        {
            discardList_debug.Add(card.name);
        }
        foreach (var card in upChangeDeck.list)
        {
            upChangeList_debug.Add(card.name);
        }

    }
    #endregion

    #region 状态切换事件
    public void OnSelectMoveTarget()
    {
        currentState.ChangeStateTo<ActorSelectMoveTarget>();
    }


    #endregion

    #region 流程事件
    //public void OnPlayerTurnStart()
    //{
    //    // 恢复各点数
    //    ActionPoint += ActionPoint_Resume;
    //    MovePoint += MovePoint_Resume;
    //}
    #endregion

    // 监听sprite鼠标进入事件 来实现focustrail的开关
    void OnMouseEnterCallback()
    {
        ShowAllFocusTrail(true);
    }
    void OnMouseExitCallback()
    { 
        ShowAllFocusTrail(false);
    }
}
