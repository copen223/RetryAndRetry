using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.CardModule;
using Assets.Scripts.CardModule.CardActions;
using Assets.Scripts.CardModule.CardEffects;
using Assets.Scripts.ActorModule;

public class PlayerController : ActorController
{
    public Hand hand;
    public DiscardPool discard;
    public Deck deck;
    public List<Container> containers = new List<Container>();

    [Header("属性")]
    public int CardDrawNum;

    private void Start()
    {
        BattleManager.instance.AddEventObserver(BattleManager.BattleEvent.PlayerTurnStart, OnTurnStart);
        BattleManager.instance.AddEventObserver(BattleManager.BattleEvent.TurnEnd, OnTurnEnd);
    }

    private void Awake()
    {
        containers = new List<Container>() { new Container(CardType.Active), new Container(CardType.Active), new Container(CardType.Passive)};
        deck = new Deck(this, new List<Card> { new Card("打击", CardType.Active, new List<CardEffect>(){new NomalDamage(2f,EffectTrigger.OnCombatAtk)}, new AttackTrail(0, 5, 1)), new Card("闪躲", CardType.Passive, null,new FocusTrail(1,1)), new Card("火焰冲击", CardType.Active, null), new Card("肉蛋葱鸡", CardType.Active, null), new Card("无视", CardType.Active, null), new Card("原声大碟", CardType.Active, null), new Card("嘿嘿嘿", CardType.Passive, null), new Card("不会吧", CardType.Passive, null) });
        hand = new Hand(this, new List<Card>());
        discard = new DiscardPool(this, new List<Card>());
        //advantage = 3;
        group = new ActorGroup("主角", 0, ActorGroup.GroupType.Player);
    }
}
