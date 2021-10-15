using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.BattleModule.BattleStates
{
    class BattleTurnAction : BattleState
    {
        public override void StateStart()
        {
            base.StateStart();

            if (GameManager.instance.IfDebug)
                Manager.EventInvokeByState(BattleManager.BattleEvent.PlayerActionStart);
            else
            {
                if (Manager.CurActorObject.GetComponent<ActorController>().group.IsPlayer)
                    Manager.EventInvokeByState(BattleManager.BattleEvent.PlayerActionStart);
                else
                {
                    Manager.EventInvokeByState(BattleManager.BattleEvent.ComputerActionStart);
                    Manager.CurActorObject.GetComponent<ActorController>().StartDoBattleAI();
                }
            }
        }


        public void OnTurnEnd()
        {
            if (Manager.currentState != this)
                return;
            ChangeStateTo<BattleTurnEnd>();
        }
    }

    
}
