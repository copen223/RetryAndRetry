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
        Hand = transform.parent.transform.parent.gameObject;
        CardStates = new List<CardState>(transform.GetComponents<CardState>());
        currentState = CardStates[0];
        SpriteObject = transform.Find("Sprite").gameObject;
        SpriteController = SpriteObject.GetComponent<CardViewController>();
    }
    private void Update()
    {
        currentState.StateUpdate();
    }

    //---------------------事件--------------------//




    // 消息发送
    public void CardBroadcastMessage()
    {

    }


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

    private void OnCardReplaced(Card rep)
    {
        ToBeReplacedCard = rep;
        currentState.ChangeStateTo<CardPreReplaced>();
    }

    private void OnDiscard()
    {
        currentState.ChangeStateTo<CardDiscard>();
    }

    public void OnReset()
    {
        SpriteObject.SetActive(true);
        if (card != null && currentState != null)
        {
            if (card.situation == CardSituation.Idle)
                currentState.ChangeStateTo<CardIdle>();
            if (card.situation == CardSituation.Focused)
                currentState.ChangeStateTo<CardFocus>();
        }
        SpriteObject.GetComponent<CardViewController>().OnReset();
    }

    public void OnEnable()
    {
        SetInteractActive(true);
    }
}
