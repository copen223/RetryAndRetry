using ActorModule.Core;
using BattleModule;
using UnityEngine;
using UnityEngine.UI;

namespace ActorModule.UI.QueueToken
{
    public class ActorQueueToken : MonoBehaviour
    {
        [SerializeField] private Image frameImage = null;
        [SerializeField] private Image tokenImage = null;
        [SerializeField] private GameObject pointHead = null; // 指向箭头

        private ActorController targetActor;
        
        // 对外方法
        
        /// <summary>
        /// 初始化 或者改变actor
        /// </summary>
        /// <param name="actor"></param>
        public void Init(ActorController actor)
        {
            if (actor.@group.IsEnemy)
                frameImage.color = Color.red;
            if (actor.@group.IsPlayer)
                frameImage.color = Color.cyan;
            
            tokenImage.sprite = actor.Token;
            targetActor = actor;

            if (BattleManager.instance.CurActorObject.GetComponent<ActorController>() != actor)
                pointHead.SetActive(false);
            else
                pointHead.SetActive(true);
            
            print(actor);
        }

        public void SetHeadPointActive(bool ifActive)
        {
            pointHead.SetActive(ifActive);
        }
    }
}
