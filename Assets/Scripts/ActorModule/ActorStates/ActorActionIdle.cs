using UI;
using UnityEngine;

namespace ActorModule.ActorStates
{

    // 战斗中轮到该玩家行动时的idle状态
    public class ActorActionIdle:ActorState
    {
        public override void StateStart()
        {
            
        }
        public override void StateUpdate()
        {
            if (Controller.currentState != this)
                return;
            if(Input.GetKeyDown(KeyCode.Mouse0) && (Controller.group.IsPlayer || GameManager.GameManager.instance.IfDebug))
            {
                var mousePos_world = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                if (UIManager.instance.IfMouseOnUI)
                    return;


                var hits = Physics2D.RaycastAll(mousePos_world, Vector2.zero,4f);

                foreach (var hit in hits)
                {
                    if (hit.collider == null)
                        break;
                    if (hit.collider.gameObject == Controller.gameObject || hit.transform.parent?.gameObject == Controller.gameObject)
                    {
                        ChangeStateTo<ActorSelectMoveTarget>();
                        return;
                    }
                }
            }
        }
        public override void StateExit()
        {
            
        }
    }
}
