using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.ActorModule;
using Assets.Scripts.ActorModule.ActorStates;

public class EnemyController : ActorController
{
    // Start is called before the first frame update

    public int MaxMovePoint = 20;
    public int MovePoint { get { return movePoint; } set { movePoint = Mathf.Clamp(value,0,MaxMovePoint); } }
    private int movePoint;

    [Header("存放状态机的子物体")]
    public Transform StatesChild = null;


    void Awake()
    {
        advantage = 1;
        group = new ActorGroup("怪物", 1, ActorGroup.GroupType.Enemy);
    }

    private void Start()
    {
        ActorStates = new List<ActorState>(StatesChild.GetComponents<ActorState>());
        currentState = ActorStates.Find((x) => { return (x is ActorActionIdle); });
    }

    // Update is called once per frame
    void Update()
    {
        currentState.StateUpdate();
    }

    #region 流程事件
    public override void OnTurnStart()
    {
        base.OnTurnStart();
        currentState.ChangeStateTo<ActorActionIdle>();
        RemoveAllFocusTrail();  // 清除所有专注
        MovePoint = MaxMovePoint;   // 恢复点数
    }

    public override void OnTurnEnd()
    {
        base.OnTurnEnd();
        currentState.ChangeStateTo<ActorNoActionIdle>();
    }


    #endregion

}
