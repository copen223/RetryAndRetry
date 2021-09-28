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

            if (GameManager.instance.IfDebug)
                Manager.EventInvokeByState(BattleManager.BattleEvent.PlayerDrawStart);
            else
            {
                if (actor.group.IsPlayer)
                    Manager.EventInvokeByState(BattleManager.BattleEvent.PlayerDrawStart);
                else if (actor.group.IsEnemy)
                    ChangeStateTo<BattleTurnAction>();
            }
        }

        private void OnDrawOver()
        {
            if (Manager.currentState != this) return;
            ChangeStateTo<BattleTurnAction>();
        }
    }
}
