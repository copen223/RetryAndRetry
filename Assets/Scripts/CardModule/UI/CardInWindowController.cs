using System;
using ActorModule.Core;
using CardModule.Controllers;
using UI;
using UI.ActionTip;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CardModule.UI
{
    public class CardInWindowController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler,IPointerClickHandler
    {
        #region UI对象链接
        [SerializeField] private GameObject cardView_go = null;
        #endregion
        /// <summary>
        /// 是否用于Status面板
        /// </summary>
        public bool IsStatusMode;
        /// <summary>
        /// 是否点击后存在比例变化
        /// </summary>
        public bool IsTriggerMode;
        /// <summary>
        /// 是否点击后拥有反馈
        /// </summary>
        public bool IfCallBackMode;



        #region 储存信息
        private Card card;
        private Card basedCard;
        /// <summary>
        /// 正在进行选择的单位
        /// </summary>
        private GameObject selector;

        private ActorController user;


        #endregion

        #region 对外方法
        /// <summary>
        /// 显示的卡牌，由哪张卡牌升级，设置者的go
        /// </summary>
        /// <param name="card"></param>
        /// <param name="basedCard"></param>
        /// <param name="selector"></param>
        public void Init(Card card,Card basedCard,GameObject selector)
        {
            this.card = card;
            this.user = card.User;
            this.selector = selector;
            this.basedCard = basedCard;
            UpdateView();
        }
        public void SetCard(Card card)
        {
            this.card = card;
            this.user = card.User;
            UpdateView();
        }
        public void SetSelector(GameObject selector)
        {
            this.selector = selector;
        }
        #endregion

        #region 内部方法
        /// <summary>
        /// 更新显示层
        /// </summary>
        private void UpdateView()
        {
            cardView_go.GetComponent<CardViewController>().OnCardChanged(card);
        }

        #endregion

        #region 事件
        public void OnPointerEnter(PointerEventData eventData)
        {
            if(!IsTriggerMode)
                cardView_go.transform.localScale = new Vector3(1.2f, 1.2f, 1);
        

            if (IsStatusMode)
            {
                if (card.focusTrail != null)
                {
                    card.focusTrail.GetComponent<FocusTrailController>().IfShow = true;
                    card.focusTrail.SetActive(true);
                }
            }

            if(IfCallBackMode)
            {
                int surplusAP = (user as PlayerController).ActionPoint - (card.cardLevel - basedCard.cardLevel);
                Color color = (card.cardLevel - basedCard.cardLevel) > 0 ? Color.red : Color.green;
                UIManager.instance.UI_PlayerResource.ActionPointUI.ChangeText(surplusAP + "", color);
                UIManager.instance.UI_ActionTips.SetActionTip(ActionTipType.Left,"确认",true);
            }

        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!IsTriggerMode)
                cardView_go.transform.localScale = new Vector3(1, 1, 1);

            if (IsStatusMode)
            {
                if (card.focusTrail != null)
                {
                    card.focusTrail.GetComponent<FocusTrailController>().IfShow = false;
                    card.focusTrail.SetActive(false);
                }
            }

            if(IfCallBackMode)
            {
                int surplusAP = (user as PlayerController).ActionPoint;
                UIManager.instance.UI_PlayerResource.ActionPointUI.ChangeText(surplusAP + "", Color.black);
                UIManager.instance.UI_ActionTips.SetActionTip(ActionTipType.Left,"确认",false);
            }
        }

        public event Action<Card> OnCardDoSelectedEvent;

        public void OnPointerClick(PointerEventData eventData)
        {
            if(IsStatusMode || IsTriggerMode)
            {
                return;
            }

            if (eventData.button == PointerEventData.InputButton.Left)
            {
                int surplusAP = (user as PlayerController).ActionPoint;
                UIManager.instance.UI_PlayerResource.ActionPointUI.ChangeText(surplusAP + "", Color.black);


                OnCardDoSelectedEvent?.Invoke(card);
                //else
                //    UIManager.instance.CreatFloatUIAt(selector, Vector2.zero, 2f, Color.black, "行动点数不足！");
            }
        }
        #endregion
    }
}
