using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.SpaceModule.PathfinderModule;
using System;
using Assets.Scripts.ActorModule;

public class ActorMoveComponent : MonoBehaviour
{
    public GameObject Actor { get { return gameObject; } }

    //-----------属性-----------
    public float moveSpeed;
    private float FallDamagePerUnit = 0.4f;

    //------------标识---------
    private bool isMoving;
    private bool ifMoveNext;

    /// <summary>
    /// 当路径移动完成时，该项为false否则为true
    /// </summary>
    public bool ifFinishMoving;

    /// <summary>
    /// 开始按路径list移动
    /// </summary>
    /// <param name="path"></param>
    public void StartMoveByPathList(List<Vector3> path)
    {
        ifFinishMoving = false;
        if (!isMoving)
            StartCoroutine(MoveByPathListCouroutine(path, true));
    }
    /// <summary>
    /// 按路径移动的Node版本
    /// </summary>
    /// <param name="path"></param>
    public void StartMoveByNodePathList(List<Node> path)
    {
        ifFinishMoving = false;
        if (!isMoving)
            StartCoroutine(MoveByPathListCouroutine(path, true));
    }
    /// <summary>
    /// 强制性移动，特点就是不改变朝向
    /// </summary>
    /// <param name="path"></param>
    public void StartForceMoveByPathList(List<Vector3> path)
    {
        ifFinishMoving = false;
        if (!isMoving)
            StartCoroutine(MoveByPathListCouroutine(path, false));
    }
    /// <summary>
    /// 强制性移动的node版本
    /// </summary>
    /// <param name="path"></param>
    public void StartForceMoveByPathList(List<Node> path)
    {
        ifFinishMoving = false;
        if (!isMoving)
            StartCoroutine(MoveByPathListCouroutine(path, false));
    }
    /// <summary>
    /// 按路径移动协程
    /// </summary>
    /// <param name="path"></param>
    /// <param name="ifChangeFace"></param>
    /// <returns></returns>
    private IEnumerator MoveByPathListCouroutine(List<Vector3> path, bool ifChangeFace)
    {
        int i = 0;
        ifMoveNext = true;
        var last = transform.position;
        while (true)
        {
            if (ifMoveNext)
            {
                i++;
                if (i > path.Count) break;

                ifMoveNext = false;

                var point = path[i - 1];
                var dir = point - last;

                if (ifChangeFace) Actor.GetComponent<ActorController>().ChangeFaceTo(dir);
                StartCoroutine(MoveToPointCouroutine(point));

                last = point;
            }
            yield return new WaitForEndOfFrame();
        }
        ifFinishMoving = true;
    }
    /// <summary>
    /// 按路径协程的node版本
    /// </summary>
    /// <param name="path"></param>
    /// <param name="ifChangeFace"></param>
    /// <returns></returns>
    private IEnumerator MoveByPathListCouroutine(List<Node> path, bool ifChangeFace)
    {
        int i = 0;
        ifMoveNext = true;
        var last = transform.position;
        while (true)
        {
            //if (ifMoveNext)
            //{
            if (i >= 1) ArrivalTheNode(path[i - 1]);   // 到达上一个节点，进行结果处理

            i++;
            if (i > path.Count) // 路径完成退出协程
            {
                break;
            }

            ifMoveNext = false;

            var curTargetNode = path[i - 1];
            Vector3 curTargetPos = new Vector3(curTargetNode.worldX, curTargetNode.worldY, 0);
            // 方向改变
            var dir = curTargetPos - last;
            if (ifChangeFace) Actor.GetComponent<ActorController>().ChangeFaceTo(dir);
            // 改变动画，开始移动到该节点
            HandleActionToNode(curTargetNode);
            yield return StartCoroutine(MoveToNodeCouroutine(curTargetNode));

            last = curTargetPos;
            //}
            //yield return new WaitForEndOfFrame();
        }
        ifFinishMoving = true;
    }
    private IEnumerator MoveToPointCouroutine(Vector3 point_target)
    {
        isMoving = true;

        Vector2 target_pos = (Vector2)point_target;
        Vector2 start_pos = (Vector2)Actor.transform.position;
        float dis = Vector2.Distance(target_pos, start_pos);
        float time = dis / moveSpeed;
        float timer = 0;

        while (timer < time)
        {
            timer += Time.deltaTime;
            if (timer >= time) timer = time;
            float x = Mathf.Lerp(start_pos.x, target_pos.x, timer / time);
            float y = Mathf.Lerp(start_pos.y, target_pos.y, timer / time);
            Vector3 curPos = new Vector3(x, y, 0);
            Actor.transform.position = curPos;
            yield return new WaitForEndOfFrame();
        }

        isMoving = false;
        ifMoveNext = true;
    }

    private IEnumerator MoveToNodeCouroutine(Node node)
    {
        foreach (var pos in node.PrePassWorldPositions)
        {
            yield return StartCoroutine(MoveToPointCouroutine(new Vector3(pos.Item1, pos.Item2, 0)));
        }
        yield return StartCoroutine(MoveToPointCouroutine(new Vector3(node.worldX, node.worldY, 0)));
    }

    //-------------------路径上上人物行为处理----------------
    private ActorActionToNode lastAction = ActorActionToNode.None;
    private void HandleActionToNode(Node node)
    {
        if (node.ActionToNode == lastAction) return; // 相同的节点行为类型，不用做出动画变更

        switch (node.ActionToNode)
        {
            case ActorActionToNode.Fall:
                // 切换下落动画
                break;
            default: break;
        }
    }

    private void ArrivalTheNode(Node node)
    {
        switch (node.ActionToNode)
        {
            case ActorActionToNode.Fall:
                HandleFallDamage(node);
                HandleBeatBack(node);
                break;
            default: break;
        }
        node.InvokeAriiveThisNodeEvent();   // 触发到达事件
    }

    private void HandleFallDamage(Node node)
    {
        if (node.FallCount <= 6) return;    // 6格即1.5格下落不受伤
        float fallDamge = Mathf.FloorToInt(node.FallCount * FallDamagePerUnit);
        Actor.GetComponent<ActorController>().OnInjured(new DamageData(fallDamge, Vector2.zero,50));
    }
    private void HandleBeatBack(Node node)
    {
        if (node.BeatBackInfomation == null) return;

        BeatBackInfomation information = node.BeatBackInfomation;
        information.target.GetComponent<ActorController>().OnBeatBack(information.dir, information.dis);

        node.BeatBackInfomation = null;
    }
}
