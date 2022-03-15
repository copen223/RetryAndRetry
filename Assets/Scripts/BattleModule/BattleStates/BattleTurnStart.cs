using ActorModule.Core;
using OtherControllers;
using UnityEngine;

namespace BattleModule.BattleStates
{
    class BattleTurnStart : BattleState
    {
        public override void StateStart()
        {
            // 镜头移动到当前对象
            Camera.main.GetComponent<CameraController>().SetAndMoveToTarget(Manager.CurActorObject);

            // 判断对象
            var actor = Manager.CurActorObject.GetComponent<ActorController>();

            if (GameManager.GameManager.instance.IfDebug)
            {
                Manager.EventInvokeByState(BattleManager.BattleEvent.PlayerTurnStart);
                actor.OnTurnStart();
            }
            else
            {
                if (actor.group.IsPlayer)
                {
                    Manager.EventInvokeByState(BattleManager.BattleEvent.PlayerTurnStart);
                    actor.OnTurnStart();
                    /* Camera.main.GetComponent<CameraController>().Mode = CameraController.CameraMode.Freedom;*/
                }
                else if (actor.group.IsEnemy)
                {
                    Manager.EventInvokeByState(BattleManager.BattleEvent.ComputerTurnStart);
                    actor.OnTurnStart();
                    ChangeStateTo<BattleTurnAction>();
                }
            }
        }

        public void OnTurnStartOver(bool ifPlayer)
        {
            if (ifPlayer)
                ChangeStateTo<BattleTurnDraw>();
            else
                ChangeStateTo<BattleTurnAction>();
        }
    }
}
