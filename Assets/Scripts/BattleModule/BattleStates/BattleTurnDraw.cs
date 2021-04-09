using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.BattleModule.BattleStates
{
    class BattleTurnDraw : BattleState
    {
        public override void StateStart()
        {
            var actor = Manager.CurActorObject.GetComponent<ActorController>();
            if (actor.group.IsPlayer)
                Manager.ManagerBroadcastMessage("OnPlayerTurnDraw");
            else if(actor.group.IsEnemy)
                ChangeStateTo<BattleTurnAction>();
        }

        private void OnDrawOver()
        {
            if (Manager.currentState != this) return;
            ChangeStateTo<BattleTurnAction>();
        }
    }
}
