using System;
using UnityEngine;

namespace GameManager
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager instance;

        public GameState State;

        public bool IfDebug;

        public enum GameState
        {
            Explore,
            Battle
        }



        public void Awake()
        {
            if (instance == null)
                instance = this;
            else
                Destroy(gameObject);

            State = GameState.Explore;
        }


        #region 事件
        // --------------响应---------------------
        public void OnEnterBattle()
        {
            State = GameState.Battle;
            EventObserver_EnterBattle?.Invoke();
        }
    
        // --------------事件4件套----------------
        private event Action EventObserver_EnterBattle;
        public enum GameEvent
        {
            EnterBattle
        }

        public void AddListener(GameEvent gameEvent, Action handle)
        {
            switch (gameEvent)
            {
                case GameEvent.EnterBattle: EventObserver_EnterBattle += handle; break;
                default: break;
            }
        }

        public void RemoveListener(GameEvent gameEvent, Action handle)
        {
            switch (gameEvent)
            {
                case GameEvent.EnterBattle: EventObserver_EnterBattle -= handle; break;
                default: break;
            }
        }


        #endregion
    }
}
