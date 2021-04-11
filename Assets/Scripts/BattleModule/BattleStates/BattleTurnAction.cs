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
            if(Manager.CurActorObject.GetComponent<ActorController>().group.IsPlayer)
                Manager.EventInvokeByState(BattleManager.BattleEvent.PlayerActionStart);
            else
                Manager.EventInvokeByState(BattleManager.BattleEvent.ComputerActionStart);
        }


        public void OnTurnEnd()
        {
            if (Manager.currentState != this)
                return;
            ChangeStateTo<BattleTurnEnd>();
        }
    }

    
}
