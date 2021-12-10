using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.CardModule;

public class CardPoolUIController : MonoBehaviour
{
    public enum PoolType
    {
        Deck,
        Discard,
        Hand,
        UpChange
    }

    public PoolType poolType;

    public void ShowThisPool()
    {
        PlayerController player = BattleManager.instance.CurActorObject.GetComponent<PlayerController>();
        if (player == null)
            return;
        var window = CardSystem.instance.SelectionWindow;
        var windowCon = window.GetComponent<CardSelectionWindowController>();
        List<Card> cardsToShow = new List<Card>();

        bool ifSort = false;

        switch (poolType)
        {
            case PoolType.Deck:cardsToShow = player.deck.list;ifSort = true; break;
            case PoolType.Discard:cardsToShow = player.discard.list; break;
            case PoolType.Hand:cardsToShow = player.hand.list; break;
            case PoolType.UpChange:cardsToShow = player.upChangeDeck.list; break;
        }

        windowCon.ShowCardSelectionWindow(cardsToShow, gameObject, ifSort);


    }

}
