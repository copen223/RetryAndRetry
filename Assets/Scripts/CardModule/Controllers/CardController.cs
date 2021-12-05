using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Assets.Scripts.CardModule;
using Assets.Scripts.CardModule.CardStates;
using Assets.Scripts.Tools;

public class CardController : MonoBehaviour,ITargetInPool
{

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

    public void SetInteractActive(bool active)
    {
        canInteract = active;
    }

    private void OnAnimationStart()
    {
        Hand.GetComponent<HandController>().HandBroadcastMessage("OnAnimationStart");
    }
    private void OnAnimationOver() 
    {
        Hand.GetComponent<HandController>().HandBroadcastMessage("OnAnimationOver"); 
    }


    #region 状态切换事件
    private void OnCardReplaced(Card rep)
    {
        ToBeReplacedCard = rep;
        currentState.ChangeStateTo<CardPreReplaced>();
    }

    public void OnDiscard()
    {
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
            if (card.situation == CardSituation.Idle)
                currentState.ChangeStateTo<CardIdle>();
            if (card.situation == CardSituation.Focused)
            {
                //Debug.LogError("刷新时发现有专注状态卡牌，设为专注：" + card.name);
                if (!(currentState is CardFocus))
                {
                    currentState.ChangeStateTo<CardFocus>();
                    return;
                }
                else
                {
                    SpriteObject.GetComponent<CardViewController>().SetFocusRotation();
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
    }
}
