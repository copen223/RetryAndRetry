using System;
using ActorModule.Core;
using UnityEngine;
using System.Collections.Generic;
using BattleModule;


namespace ActorModule.UI.QueueToken
{
    public class ActorQueueUI : MonoBehaviour
    {
        [SerializeField] private ActorQueueToken tokenPrefab = null;
        private List<ActorQueueToken> tokens = new List<ActorQueueToken>();

        [SerializeField] private Transform tokenParent = null;

        private void Start()
        {
            BattleManager.instance.ActorQueueChangeEvent += UpdateQueueUICallback;
            BattleManager.instance.TurnEndEvent += UpdateQueueHeadPointCallback;
        }

        private void UpdateQueueUICallback(List<ActorController> actors)
        {
            for (int i = 0; i < actors.Count; i++)
            {
                var actor = actors[i];
                
                if(i >= tokens.Count)
                    tokens.Add(Instantiate(tokenPrefab,tokenParent));
                
                tokens[i].gameObject.SetActive(true);
                tokens[i].Init(actor);
            }

            for (int i = actors.Count; i < tokens.Count; i++)
            {
                tokens[i].gameObject.SetActive(false);
            }
        }

        private void UpdateQueueHeadPointCallback(int curindex)
        {
            foreach (var token in tokens)
            {
                token.SetHeadPointActive(false);
            }
            tokens[curindex].SetHeadPointActive(true);
        }
    }
}
