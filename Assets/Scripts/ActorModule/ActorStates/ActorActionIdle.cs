using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.ActorModule.ActorStates
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
            if(Input.GetKeyDown(KeyCode.Mouse0) && (Controller.group.IsPlayer || GameManager.instance.IfDebug))
            {
                var mousePos_world = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                var hit = Physics2D.Raycast(mousePos_world, Vector2.zero,4f,LayerMask.GetMask("Actor"));
                //Debug.LogError(hit.collider);
                if (hit.collider == null)
                    return;
                if (hit.collider.gameObject == Controller.gameObject || hit.transform.parent?.gameObject == Controller.gameObject)
                {
                    ChangeStateTo<ActorSelectMoveTarget>();
                }
                
            }
        }
        public override void StateExit()
        {
            
        }
    }
}
