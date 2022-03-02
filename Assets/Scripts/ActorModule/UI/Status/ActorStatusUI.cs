using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ActorStatusUI : MonoBehaviour, IDragHandler, IBeginDragHandler
{
    public ActorController Actor;

    [SerializeField]
    private GameObject Ability = null;
    [SerializeField]
    private GameObject Card = null;
    [SerializeField]
    private GameObject Buff = null;

    /// <summary>
    /// 仅仅作为Tabs点击调用
    /// </summary>
    /// <param name="index"></param>
    private void OnTabClick(int index)
    {
        switch(index)
        {
            case 0: Ability.SetActive(true); Card.SetActive(false); Buff.SetActive(false);break;
            case 1: Ability.SetActive(false); Card.SetActive(true); Buff.SetActive(false); break;
            case 2: Ability.SetActive(false); Card.SetActive(false); Buff.SetActive(true); break;
        }    
    }

    private void OnEnable()
    {
        Ability.SetActive(true);
        Card.SetActive(false);
        Buff.SetActive(false);
    }

    public void UpdateValueByActor(ActorController actor)
    {
        Actor = actor;
        Ability.GetComponent<ActorStatusUI_Ability>().UpdateValueByActor(actor.Ability);
        Card.GetComponent<ActorStatusUI_Card>().UpdateThisUI(actor);
        Buff.GetComponent<ActorStatusUI_Buff>().UpdateThisUI(actor.BuffCon);
    }


    /// <summary>
    /// 关闭按钮调用
    /// </summary>
    public void OnCloseWindowCallBack()
    {
        gameObject.SetActive(false);
        OnCloseWindowEvent?.Invoke(this);
    }

    Vector3 offset;

    public void OnBeginDrag(PointerEventData eventData)
    {
        offset = transform.position - Input.mousePosition;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = offset + Input.mousePosition;
    }

    public event System.Action<ActorStatusUI> OnCloseWindowEvent;


}
