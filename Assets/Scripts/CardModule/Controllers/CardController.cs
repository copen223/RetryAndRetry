using System.Collections;
using System.Collections.Generic;
using CardModule.CardStates;
using Tools;
using UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CardModule.Controllers
{
    public class CardController : MonoBehaviour,ITargetInPool,IPointerEnterHandler,IPointerExitHandler
    {
        #region Debug
        public GameObject Container;
        #endregion
    
    
        public bool ifMouseSelectThis = false;

        [Header("链接")]
        public List<CardState> CardStates = new List<CardState>();
        public CardState currentState;
        public GameObject Hand;

        // 链接
        public GameObject SpriteObject;
        public CardViewController SpriteController;
        public GameObject ActionObject;
        public CardActionController ActionController;
        public GameObject SelectionWindow;
        /// <summary>
        /// 卡牌的持有者
        /// </summary>
        public GameObject holder
        {
            get { return Hand.GetComponent<HandController>().Holder; }
            set { }
        }
        [SerializeField]
        private Card card;
        public Card Card { set { card = value; SpriteObject.GetComponent<CardViewController>().OnCardChanged(Card); } get { return card; } }



        [Header("移动速度")]
        public float Speed;

        // 缓存
        public Card ToBeReplacedCard;

        // 控制参量
        public bool canInteract = true;

        private void Start()
        {
            // 链接初始化
            var CardSystemTF = transform.parent.parent.parent;
            Hand = CardSystem.instance.Hand;
            SelectionWindow = CardSystem.instance.SelectionWindow;
            CardStates = new List<CardState>(transform.GetComponents<CardState>());
            currentState = CardStates[0];
            SpriteObject = transform.Find("Sprite").gameObject;
            SpriteController = SpriteObject.GetComponent<CardViewController>();
            ActionObject = transform.Find("Action").gameObject;
            ActionController = ActionObject.GetComponent<CardActionController>();
        
        }
        private void Update()
        {
            currentState.StateUpdate();

            // debug内容
            if(card != null)
            {
                foreach(var container in HandController.instance.ContainerObjects_list)
                {
                    if(container.GetComponent<ContainerController>().Container == card.Container)
                    {
                        Container = container;
                    }
                }
            }    


        }

        //---------------------事件--------------------//


        //---------------------------消息响应-------------------------------------//
        public void StartMoveToCorrectPos(Vector3 pos)
        {
            StartCoroutine(MoveToTargetPos(pos));
        }
        private IEnumerator MoveToTargetPos(Vector3 targetPos)
        {
            SetInteractActive(false);
            Vector3 curPos = transform.localPosition;
            while (curPos != targetPos)
            {
                curPos = Vector3.MoveTowards(curPos, targetPos, Speed * Time.deltaTime);
                transform.localPosition = curPos;
                yield return new WaitForEndOfFrame();
            }
            SetInteractActive(true);
        }

        /// <summary>
        /// 是否可交互
        /// </summary>
        /// <param name="active"></param>
        public void SetInteractActive(bool active)
        {
            canInteract = active;
        }



        #region 状态切换事件
        public void OnCardReplaced(Card rep)
        {
            ToBeReplacedCard = rep;
            currentState.ChangeStateTo<CardPreReplaced>();
        }

        public void OnDiscard()
        {
            Debug.Log(card.name + "discard");
            currentState.ChangeStateTo<CardDiscard>();
        }

        /// <summary>
        /// 切换card数据时进行刷新,在handcontroller中以sendmessage调用
        /// </summary>
        public void OnReset()
        {
            SpriteObject.SetActive(true);
            if (card != null && currentState != null)
            {
                if (card.Situation == CardSituation.Idle)
                    currentState.ChangeStateTo<CardIdle>();
                if (card.Situation == CardSituation.Focused)
                {
                    //Debug.LogError("刷新时发现有专注状态卡牌，设为专注：" + card.name);
                    if (!(currentState is CardFocus))
                    {
                        currentState.ChangeStateTo<CardFocus>();
                        return;
                    }
                    else
                    {
                        //SpriteObject.GetComponent<CardViewController>().SetFocusRotation();
                        SpriteController.SetFocusMask();
                        return;
                    }
                }
            }
            SpriteObject.GetComponent<CardViewController>().OnReset();
        }
        //public void OnSelectedWindow()
        //{
        //    currentState.ChangeStateTo<CardSelectionWindow>();
        //}
        #endregion
        public void OnEnable()
        {
            SetInteractActive(true);
            ifMouseSelectThis = false;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            ifMouseSelectThis = true;
            UIManager.instance.IfActiveUIInteraction = false;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            ifMouseSelectThis = false;
            UIManager.instance.IfActiveUIInteraction = true;
        }
    }
}
