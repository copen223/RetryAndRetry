using System.Collections.Generic;
using System.Threading.Tasks;
using ActorModule.Core;
using SpaceModule.PathfinderModule;
using UI;
using UnityEngine;

namespace ActorModule.ActorStates
{
    public class ActorSelectMoveTarget : ActorState
    {
        private ActorMoveComponent moveComponent;
        private PathFinderComponent pathfinderComponent;
        private ActorRayDrawer rayDrawer;
        public override void StateStart()
        {
            moveComponent = Controller.GetComponent<ActorMoveComponent>();
            pathfinderComponent = Controller.GetComponent<PathFinderComponent>();
            pathfinderComponent.SearchPathFrom(gameObject.transform.position);
            rayDrawer = Controller.GetComponent<ActorRayDrawer>();

            UIManager.instance.IfActiveUIInteraction = false;
        }
        public override void StateUpdate()
        {
            // 1.路径获取
            var mousePos_world = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //var path = pathfinderComponent.GetPathFromTo(gameObject.transform.position, mousePos_world);
            bool ifCanFindPath = 
                pathfinderComponent.GetPathFromToNearst(gameObject.transform.position, mousePos_world,
                ((PlayerController) Controller).MovePoint,  out List<Vector3> path);
            var nodePath = pathfinderComponent.VectorPath2NodePath(path);
            List<Vector3> linePath = new List<Vector3>();
            foreach(var node in nodePath)
            {
                foreach (var passPos in node.PrePassWorldPositions) linePath.Add(new Vector3(passPos.Item1,passPos.Item2,0));
                linePath.Add(new Vector3(node.worldX, node.worldY, 0));
            }

            // 2.移动点数消耗判断,修正path
            int surplus = 0;
            int point = ((PlayerController) Controller).MovePoint;
            while (true)
            {
                int cost = Mathf.FloorToInt(pathfinderComponent.GetPathCostToNode(path[path.Count - 1]));
                surplus = point - cost;
                if (surplus < 0)
                {
                    path.RemoveAt(path.Count - 1);
                    continue;
                }
                else
                    break;
            }

            // 3.点数消耗显示与路径显示
            var ui = UIManager.instance.transform.Find("PlayerResource").transform.Find("MovePoint").GetComponent<TextUIController>();
            ui.ChangeText(surplus + "", Color.red);
            rayDrawer.DrawLine(linePath);

            // 4.输入处理，左键确认，右键取消
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                rayDrawer.EndDraw();
                ChangeStateTo<ActorActionIdle>();
                return;
            }
            if(Input.GetKeyDown(KeyCode.Mouse0))
            {
                (Controller as PlayerController).MovePoint = surplus;    // 应用消耗值
                (Controller as PlayerController).StatesChild.GetComponent<ActorMoveByPath>().SetNodePath(nodePath); // 设置路径
                ChangeStateTo<ActorMoveByPath>();   // 开始移动
                return;
            }
            
        }
        public override void StateExit()
        {
            var ui = UIManager.instance.transform.Find("PlayerResource").transform.Find("MovePoint").GetComponent<TextUIController>();
            ui.ChangeText((Controller as PlayerController).MovePoint + "", Color.black);
            rayDrawer.EndDraw();

            DelayUIInteractionActiveAsync();
        }

        private async void DelayUIInteractionActiveAsync()
        {
            await Task.Delay(1);
            UIManager.instance.IfActiveUIInteraction = true;
        }

    }
}
