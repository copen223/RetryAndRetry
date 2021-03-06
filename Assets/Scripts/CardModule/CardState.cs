using System.Collections;
using CardModule.Controllers;
using UnityEngine;

namespace CardModule
{
    public class CardState : MonoBehaviour
    {
        //------------------------------------事件订阅--------------------------------//
        /// <summary>
        /// 进入该状态的动画结束时调用 参数代表现在是动画开始还是结束时
        /// </summary>
        /// <param name="isStart"> </param>
        protected virtual void OnAnimationDo(bool isStart) { }

        public CardController Controller;
        public virtual void StateStart() { Controller.SpriteController.AddOberserver(OnAnimationDo,this); }

        public virtual void StateUpdate() { }

        public virtual void StateExit()
        {
            Controller.SpriteController.RemoveOberserver(OnAnimationDo,this); 
        }

        public void ChangeStateTo<T>()
        {
            foreach (CardState cardState in Controller.CardStates)
            {
                if (cardState is T)
                {
                    Controller.currentState.StateExit();
                    Controller.currentState = cardState;
                    Controller.currentState.StateStart();
                }
            }
        }

        public void ChangeStateTo<T>(System.Action onStateChange)
        {
            foreach (CardState cardState in Controller.CardStates)
            {
                if (cardState is T)
                {
                    Controller.currentState.StateExit();
                    onStateChange?.Invoke();
                    Controller.currentState = cardState;
                    Controller.currentState.StateStart();
                }
            }
        }

        public bool IsEventProtecting;

        public void SetEventProtect()
        {
            IsEventProtecting = true;
            StartCoroutine("DelayStopEventProtect");
        }

        IEnumerator DelayStopEventProtect()
        {
            yield return new WaitForSeconds(0.1f);
            IsEventProtecting = false;
        }

        private void OnEnable()
        {
            IsEventProtecting = false;
        }

        private void Start()
        {
            Controller = gameObject.GetComponent<CardController>();
        }
    }
}
