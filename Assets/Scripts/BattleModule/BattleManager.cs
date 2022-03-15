using System;
using System.Collections.Generic;
using ActorModule.Core;
using BattleModule.BattleStates;
using CardModule.Controllers;
using UnityEngine;

namespace BattleModule
{
    public class BattleManager : MonoBehaviour
    {
        //---------------------链接-------------------//
        public HandController HandController => HandController.instance;

        public static BattleManager instance;

        private void Awake()
        {
            instance = this;
            //HandController = GameObject.Find("Hand").GetComponent<HandController>();
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

        public GameObject CurActorObject => ActorQueue[CurActorIndex].gameObject;

        [Header("当前回合行动人物")]
        public int CurActorIndex;

        [Header("状态机")]
        public BattleState currentState;
        public List<BattleState> BattleStates;

        [Header("战斗对象缓存与队列")]
        public List<ActorController> ActorList = new List<ActorController>();
        public List<ActorController> ActorQueue = new List<ActorController>();

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
        public event VoidHandle EventObserver_ComputerTurnStart;
        public event VoidHandle EventObserver_ComputerActionStart;

        public void AddEventObserver(BattleEvent battleEvent,VoidHandle handle)
        {
            switch (battleEvent)
            {
                case BattleEvent.PlayerTurnStart: EventObserver_PlayerTurnStart += handle;break;
                case BattleEvent.PlayerDrawStart: EventObserver_PlayerDrawStart += handle; break;
                case BattleEvent.PlayerActionStart: EventObserver_ActionStart += handle; break;
                case BattleEvent.ComputerTurnStart: EventObserver_ComputerTurnStart += handle; break;
                case BattleEvent.ComputerActionStart: EventObserver_ComputerActionStart += handle; break;
            }        
        }

        public void EventInvokeByState(BattleEvent battleEvent)
        {
            switch (battleEvent)
            {
                case BattleEvent.PlayerTurnStart: EventObserver_PlayerTurnStart?.Invoke(); break;
                case BattleEvent.PlayerDrawStart: EventObserver_PlayerDrawStart?.Invoke(); break;
                case BattleEvent.PlayerActionStart: EventObserver_ActionStart?.Invoke(); break;
                case BattleEvent.ComputerTurnStart: EventObserver_ComputerTurnStart?.Invoke(); break;
                case BattleEvent.ComputerActionStart: EventObserver_ComputerActionStart?.Invoke(); break;
            }

        }

        public event Action<List<ActorController>> ActorQueueChangeEvent;
        public event Action<int> TurnEndEvent;
        public void InvokeActorQueueChangeEventByState() { ActorQueueChangeEvent?.Invoke(ActorQueue); }
        public void InvokeTurnEndEventEventByState() { TurnEndEvent?.Invoke(CurActorIndex); }
        

        #region 外部通知
        public void OnActorDeath(GameObject actorObject)
        {
            // 当前的对象
            var curActor = CurActorObject;
            var deathActor = actorObject.GetComponent<ActorController>();
        
            ActorList.Remove(deathActor);
            ActorQueue.Remove(deathActor);


            // 如果当前回合对象死亡
            if(curActor == actorObject)
            {
                CurActorIndex--;
                currentState.ChangeStateTo<BattleTurnEnd>();
            }
            // 其他对象死亡
            else
            {
                CurActorIndex = ActorQueue.IndexOf(curActor.GetComponent<ActorController>());
            }
            
            ActorQueueChangeEvent?.Invoke(ActorQueue);
        }
        #endregion
    }
}
