using System.Collections.Generic;
using ActorModule.Core;
using ActorModule.UI.Status;
using BattleModule;
using CardModule;
using CardModule.Controllers;
using Tools;
using UI;
using UnityEngine;

namespace ActorModule.UI
{
    /// <summary>
    /// 统一管理所有actor的ui
    /// 包括fitui status
    /// </summary>
    public class ActorUIController : MonoBehaviour
    {
        // 对象池
        public GameObject FitUIPrefab;
        TargetPool fitsPool;
        public Transform FitUIParent;

   

        // 
        void Awake()
        {
        
        }

        void Start()
        {
            fitsPool = new TargetPool(FitUIPrefab);

            BattleManager.instance.ActorQueueChangeEvent += OnActorChanged;

            //abillityPool = new TargetPool(AbillityUIPrefab);
            statusPool = new TargetPool(StatusUIPrefab);
            triggerCardsUIPool = new TargetPool(TriggerCardsUIPrefab);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                if (!UIManager.instance.IfActiveUIInteraction)
                    return;

                var worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                var hits = Physics2D.RaycastAll(worldPos, Vector2.zero);
                foreach (var hit in hits)
                {
                    if (hit.collider == null)
                        return;


                    var screenUIPos = Camera.main.WorldToScreenPoint(worldPos += new Vector3(1, 0, 0));
                    if (hit.collider.transform.parent.tag == "Actor")
                    {
                        var con = hit.collider.transform.parent.GetComponent<ActorController>();

                        OnShowActorStatus(screenUIPos, con);
                    }
                }
            }
        }


        // 暂时使用遍历整个队列 更新UI的方法，后续为了优化应该对Actor的增减进行监听，event委托应为含有一个Actor对象参数的委托
        void OnActorChanged( List<ActorController> actors)
        {
            fitsPool.ReSet();
            foreach(var actor in actors )   //  为每个actor创建一个fitui
            {
                var gb = fitsPool.GetTarget(FitUIParent);
                gb.SetActive(true);
                var controller = gb.GetComponent<ActorFitUIController>();
                controller.Target = actor.gameObject;
            }
        }

        //#region Ability
        //public GameObject AbillityUIPrefab;
        //TargetPool abillityPool;
        //public Transform abillityUIParent;

        //private List<ActorAbility> actorAbilities = new List<ActorAbility>();
        //public void OnShowActorAbillity(Vector3 ScreenPos, ActorAbility actorAbility)
        //{
        //    if (actorAbilities.Contains(actorAbility))
        //        return;

        //    var gb = abillityPool.GetTarget(abillityUIParent);
        //    gb.transform.position = ScreenPos;

        //    var controller = gb.GetComponent<ActorAbilityUIController>();
        //    gb.SetActive(true);

        //    controller.UpdateValueByActor(actorAbility);

        //    controller.OnCloseWindowEvent += CloseActorAbillityUICallBack;
        //    actorAbilities.Add(actorAbility);

        //}

        //private void CloseActorAbillityUICallBack(ActorAbilityUIController who)
        //{
        //    who.OnCloseWindowEvent -= CloseActorAbillityUICallBack;
        //    actorAbilities.Remove(who.ability);
        //}
        //#endregion


        #region ActorStatus
        public GameObject StatusUIPrefab;
        TargetPool statusPool;
        public Transform statusUIParent;


        private List<ActorController> actorsShowingStatus = new List<ActorController>();
        public void OnShowActorStatus(Vector3 ScreenPos, ActorController actor)
        {
            if (actorsShowingStatus.Contains(actor))
                return;

            var gb = statusPool.GetTarget(statusUIParent);
            gb.transform.position = ScreenPos;

            var controller = gb.GetComponent<ActorStatusUI>();
            gb.SetActive(true);

            controller.UpdateValueByActor(actor); // 重点

            controller.OnCloseWindowEvent += CloseActorStatusUICallBack;
            actorsShowingStatus.Add(actor);

        }

        private void CloseActorStatusUICallBack(ActorStatusUI who)
        {
            who.OnCloseWindowEvent -= CloseActorStatusUICallBack;
            actorsShowingStatus.Remove(who.Actor);
        }
        #endregion

        #region 该单位触发卡牌
        public GameObject TriggerCardsUIPrefab;
        TargetPool triggerCardsUIPool;
        public Transform TriggerCardsUIParent;

        /// <summary>
        /// 显示触发的卡牌时每帧调用，每帧第一次显示前先调用Over函数进行清除
        /// </summary>
        /// <param name="cards"></param>
        /// <param name="actor"></param>
        /// <param name="ifSetLeft"></param>
        public void OnShowTriggerCards(List<Card> cards,ActorController actor,bool ifSetLeft)
        {
            if (cards.Count <= 0)
                return;

            var gb = triggerCardsUIPool.GetTarget(TriggerCardsUIParent);

            Vector3 dir = Vector3.zero;
            if (ifSetLeft)
                dir = Vector3.left;
            else
                dir = Vector3.right;

            Vector3 worldPos = actor.Sprite.transform.position + dir * actor.Sprite.GetComponent<Collider2D>().bounds.size.x;
            Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);

            gb.transform.position = screenPos;

            var cardSelectionWindowController = gb.GetComponent<CardSelectionWindowController>();
            cardSelectionWindowController.ShowCardSelectionWindow(cards, false);
        }

        /// <summary>
        /// 结束显示触发的卡牌时调用
        /// </summary>
        public void OnOverShowTriggerCards()
        {
            triggerCardsUIPool.ReSet();
        }



        #endregion

    }
}
