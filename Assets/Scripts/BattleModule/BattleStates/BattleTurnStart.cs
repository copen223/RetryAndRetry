using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.BattleModule.BattleStates
{
    class BattleTurnStart : BattleState
    {
        public override void StateStart()
        {
            // 镜头移动到当前对象
            Camera.main.SendMessage("SetAndMoveToTarget", Manager.CurActorObject);

            // 判断对象
            var actor = Manager.CurActorObject.GetComponent<ActorController>();
            if (actor.group.IsPlayer) Manager.ManagerBroadcastMessage("OnPlayerTurnStart");
            else if (actor.group.IsEnemy) Manager.ManagerBroadcastMessage("OnEnemyTurnStart");
        }

        private void OnTurnStartOver()
        {
            ChangeStateTo<BattleTurnDraw>();
        }
    }
}
