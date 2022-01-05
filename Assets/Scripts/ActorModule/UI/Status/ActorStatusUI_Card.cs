using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.CardModule;
using System;

public class ActorStatusUI_Card : MonoBehaviour
{
    [SerializeField]
    private CardSelectionWindowController cardSelectionWindow = null;

    /// <summary>
    /// 设置该UI
    /// </summary>
    /// <param name="cards"></param>
    /// <param name="finishSelectFunc"></param>
    /// <param name="player"></param>
    public void UpdateThisUI(ActorController actor)
    {
        if (actor is PlayerController)
        {
            var player = actor as PlayerController;
            cardSelectionWindow.ShowCardSelectionWindow(player.hand.list, player.gameObject, false);
        }
    }
}
