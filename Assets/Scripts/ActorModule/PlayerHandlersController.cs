using System.Collections;
using ActorModule.Core;
using BattleModule;
using CardModule;
using CardModule.Controllers;
using UnityEngine;

namespace ActorModule
{
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
}
