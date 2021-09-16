using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.ActorModule.ActorStates
{
    public class ActorSelectMoveTarget : ActorState
    {
        private ActorMoveComponent moveComponent;
        private PathFinderComponent pathfinderComponent;
        private ActorRayDrawer rayDrawer;
        public override void StateStart()
        {
            moveComponent = gameObject.GetComponent<ActorMoveComponent>();
            pathfinderComponent = gameObject.GetComponent<PathFinderComponent>();
            pathfinderComponent.SearchPathFrom(gameObject.transform.position);
            rayDrawer = gameObject.GetComponent<ActorRayDrawer>();
        }
        public override void StateUpdate()
        {
            if(Input.GetKeyDown(KeyCode.Mouse1))
            {
                rayDrawer.EndDraw();
                ChangeStateTo<ActorActionIdle>();
                return;
            }
            if(Input.GetKeyDown(KeyCode.Mouse0))
            {
                ChangeStateTo<ActorMoveByPath>();
                return;
            }
            var mousePos_world = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //var path = pathfinderComponent.GetPathFromTo(gameObject.transform.position, mousePos_world);
            var path = pathfinderComponent.GetPathFromToNearst(gameObject.transform.position, mousePos_world);

            rayDrawer.DrawLine(path);
        }
        public override void StateExit()
        {
            rayDrawer.EndDraw();
        }
    }
}
