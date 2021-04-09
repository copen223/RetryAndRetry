using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Assets.Scripts.CardModule;
using Assets.Scripts.CardModule.CardStates;
using Assets.Scripts.Tools;

public class CardController : MonoBehaviour,ITargetInPool
{
    public List<CardState> CardStates = new List<CardState>();
    public CardState currentState;



    [Header("移动速度")]
    public float Speed;

    // 链接
    public GameObject SpriteObject;
    public GameObject holder
    {
        get { return transform.parent.GetComponent<HandController>().Holder; }
        set { }
    }
    private Card card;
    public Card ToBeReplacedCard;
    public Card Card { set { card = value; SpriteObject.SendMessage("OnCardChanged", card); } get { return card; } }

    private GameObject container;
    private void SetContainer(GameObject _container) { container = _container; }

    // 控制参量
    bool canInteract = true;
    bool isMaking = false;

    private void Start()
    {
        CardStates = new List<CardState>(transform.GetComponents<CardState>());
        currentState = CardStates[0];
    }
    private void Update()
    {
        currentState.StateUpdate();
    }

    // 消息发送
    public void CardBroadcastMessage()
    {

    }


    //---------------------------消息响应-------------------------------------//
    private void StartMoveToCorrectPos(Vector3 pos)
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

    private void HandleMessage(CardEvent.Message message)
    {
        switch (message)
        {
            case CardEvent.Message.CardIsMaking: SetInteractActive(false); break;
            case CardEvent.Message.CardMakingOver: SetInteractActive(true); break;
        }
    }

    private void OnAnimationStart()
    {
        transform.parent.GetComponent<HandController>().HandBroadcastMessage("OnAnimationStart");
    }
    private void OnAnimationOver() 
    {
        transform.parent.GetComponent<HandController>().HandBroadcastMessage("OnAnimationOver"); 
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
        currentState.ChangeStateTo<CardIdle>();
    }
}
