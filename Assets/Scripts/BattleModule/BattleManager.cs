using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.BattleModule.BattleStates;

public class BattleManager : MonoBehaviour
{
    //---------------------链接-------------------//
    public HandController HandController;

    static public BattleManager instance;

    private void Awake()
    {
        instance = this;
        HandController = GameObject.Find("Hand").GetComponent<HandController>();
    }

    private void Start()
    {
        BattleStates = new List<BattleState>(GetComponents<BattleState>());
        currentState = BattleStates.Find(x => { return (x is BattleRest); });
        foreach (var state in BattleStates) state.Manager = this;
        currentState.StateStart();
    }

    private void Update()
    {
        currentState.StateUpdate();
    }

    public GameObject CurActorObject { get { return ActorQueue[CurActorIndex]; } }
    [Header("当前回合行动人物")]
    public int CurActorIndex;

    [Header("状态机")]
    public BattleState currentState;
    public List<BattleState> BattleStates;

    [Header("战斗对象缓存与队列")]
    public List<GameObject> ActorList = new List<GameObject>();
    public List<GameObject> ActorQueue = new List<GameObject>();

    [Header("事件订阅者")]
    public List<GameObject> ListenersList = new List<GameObject>();

    //-----------------------事件--------------------------------//
    public enum BattleEvent
    {
        // 流程事件
        PlayerTurnStart,
        PlayerDrawStart,
        PlayerActionStart,
        ComputerTurnStart,
        ComputerActionStart,
        TurnEnd,
        // 其他事件
        ActorQueueCountChange
    }

    public delegate void VoidHandle();
    public event VoidHandle EventObserver_PlayerTurnStart;
    public event VoidHandle EventObserver_PlayerDrawStart;
    public event VoidHandle EventObserver_ActionStart;
    public event VoidHandle EventObserver_TurnEnd;
    public event VoidHandle EventObserver_ComputerTurnStart;
    public event VoidHandle EventObserver_ComputerActionStart;
    public event VoidHandle EventObserver_ActorQueueCountChange;

    public void AddEventObserver(BattleEvent battleEvent,VoidHandle handle)
    {
        switch (battleEvent)
        {
            case BattleEvent.PlayerTurnStart: EventObserver_PlayerTurnStart += handle;break;
            case BattleEvent.PlayerDrawStart: EventObserver_PlayerDrawStart += handle; break;
            case BattleEvent.PlayerActionStart: EventObserver_ActionStart += handle; break;
            case BattleEvent.TurnEnd: EventObserver_TurnEnd += handle; break;
            case BattleEvent.ComputerTurnStart: EventObserver_ComputerTurnStart += handle; break;
            case BattleEvent.ComputerActionStart: EventObserver_ComputerActionStart += handle; break;
            case BattleEvent.ActorQueueCountChange:EventObserver_ActorQueueCountChange += handle;break;
        }        
    }

    public void EventInvokeByState(BattleEvent battleEvent)
    {
        switch (battleEvent)
        {
            case BattleEvent.PlayerTurnStart: EventObserver_PlayerTurnStart?.Invoke(); break;
            case BattleEvent.PlayerDrawStart: EventObserver_PlayerDrawStart?.Invoke(); break;
            case BattleEvent.PlayerActionStart: EventObserver_ActionStart?.Invoke(); break;
            case BattleEvent.TurnEnd: EventObserver_TurnEnd?.Invoke(); break;
            case BattleEvent.ComputerTurnStart: EventObserver_ComputerTurnStart?.Invoke(); break;
            case BattleEvent.ComputerActionStart: EventObserver_ComputerActionStart?.Invoke(); break;
            case BattleEvent.ActorQueueCountChange: EventObserver_ActorQueueCountChange?.Invoke(); break;
        }

    }
}
