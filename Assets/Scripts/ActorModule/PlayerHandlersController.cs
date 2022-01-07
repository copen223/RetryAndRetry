using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.CardModule;

public class PlayerHandlersController : MonoBehaviour
{
    /// <summary>
    /// 执行者
    /// </summary>
    public FocusTrailHandler FocusTrailHandler;

    public ActorController Actor;
    
    public void FocusTheCard(Card card,Vector2 dir)
    {
        FocusTrailHandler.SetFocusTrailHandler(Actor.gameObject, card, dir);
        StartCoroutine(StartFocusTheCardCallBack());
    }

    IEnumerator StartFocusTheCardCallBack()
    {
        yield return FocusTrailHandler.StartHandleFocusTrail();

        if (BattleManager.instance.CurActorObject == Actor.gameObject)
        {
            HandController.instance.ResetHandCards();
        }
    }

}
