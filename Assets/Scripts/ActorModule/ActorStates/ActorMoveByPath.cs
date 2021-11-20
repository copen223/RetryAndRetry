using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Assets.Scripts.SpaceModule.PathfinderModule;

namespace Assets.Scripts.ActorModule.ActorStates
{
    public class ActorMoveByPath : ActorState
    {
        ActorMoveComponent moveComponent;
        ActorRayDrawer rayDrawer;
        private List<Vector3> path = new List<Vector3>();
        private List<Node> nodePath = new List<Node>();
        
        public void SetPath(List<Vector3> path_list)
        {
            path = path_list;
        }
        public void SetNodePath(List<Node> nodes_list)
        {
            nodePath = nodes_list;
        }

        public override void StateStart()
        {
            rayDrawer = gameObject.GetComponent<ActorRayDrawer>();
            //var pathfinder = gameObject.GetComponent<PathFinderComponent>();
            //var path = pathfinder.CurPath;
            moveComponent = gameObject.GetComponent<ActorMoveComponent>();
            //moveComponent.StartMoveByPathList(path);
            moveComponent.StartMoveByNodePathList(nodePath);    // 把按一般路劲修改成了node信息
            
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
