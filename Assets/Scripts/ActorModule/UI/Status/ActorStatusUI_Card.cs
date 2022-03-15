using ActorModule.Core;
using CardModule.Controllers;
using UnityEngine;

namespace ActorModule.UI.Status
{
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
                cardSelectionWindow.ShowCardSelectionWindow(player.hand.list, false);
            }
            else if(actor is EnemyController)
            {
                var enemy = actor as EnemyController;
                cardSelectionWindow.ShowCardSelectionWindow(enemy.AICards, true);
            }
        }
    }
}
