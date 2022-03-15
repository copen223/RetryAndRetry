using ActorModule.Core;
using OtherControllers;
using UnityEngine;

namespace BattleModule.BattleStates
{
    class BattleTurnAction : BattleState
    {
        public override void StateStart()
        {
            base.StateStart();

            if (GameManager.GameManager.instance.IfDebug)
                Manager.EventInvokeByState(BattleManager.BattleEvent.PlayerActionStart);
            else
            {
                if (Manager.CurActorObject.GetComponent<ActorController>().group.IsPlayer)
                {
                    Manager.EventInvokeByState(BattleManager.BattleEvent.PlayerActionStart);
                    Camera.main.GetComponent<CameraController>().SetFreedomControl();
                }
                else
                {
                    Manager.EventInvokeByState(BattleManager.BattleEvent.ComputerActionStart);
                    Manager.CurActorObject.GetComponent<ActorController>().StartDoBattleAI();
                }
            }
        }

        /// <summary>
        /// 结束回合调用该函数
        /// </summary>
        public void OnTurnEnd()
        {
            if (Manager.currentState != this)
                return;
            ChangeStateTo<BattleTurnEnd>();
        }
    }

    
}
