using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    static public BattleManager instance;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        BattleStates = new List<BattleState>(GetComponents<BattleState>());
        currentState = BattleStates[2];
        foreach (var state in BattleStates) state.Manager = this;
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

    public void ManagerBroadcastMessage(string message)
    {
        foreach (var listener in ListenersList)
            listener.SendMessage(message);
    }
}
