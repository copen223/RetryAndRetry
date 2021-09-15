using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.ActorModule.ActorStates
{
    public class ActorMoveByPath : ActorState
    {
        ActorMoveComponent moveComponent;
        public override void StateStart()
        {
            var pathfinder = gameObject.GetComponent<PathFinderComponent>();
            var path = pathfinder.CurPath;
            moveComponent = gameObject.GetComponent<ActorMoveComponent>();
            moveComponent.StartMoveByPathList(path);
            
        }
        public override void StateUpdate()
        {
            if (Controller.currentState != this)
                return;
            if (moveComponent.ifFinishMoving)
                ChangeStateTo<ActorActionIdle>();
        }
        public override void StateExit()
        {
            
        }
    }
}
