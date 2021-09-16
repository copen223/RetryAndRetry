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
        ActorRayDrawer rayDrawer;
        public override void StateStart()
        {
            rayDrawer = gameObject.GetComponent<ActorRayDrawer>();
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
            {
                rayDrawer.EndDraw();
                ChangeStateTo<ActorActionIdle>();
            }
        }
        public override void StateExit()
        {
            
        }
    }
}
