using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorMoveComponent : MonoBehaviour
{
    public GameObject Actor { get { return gameObject; }  }

    //-----------属性-----------
    public float moveSpeed;

    //------------标识---------
    public bool isMoving;
    public bool ifMoveNext;
    public bool ifFinishMoving;


    public void StartMoveByPathList(List<Vector3> path)
    {
        ifFinishMoving = false;
        if (!isMoving)
            StartCoroutine(MoveByPathListCouroutine(path));
    }

    private IEnumerator MoveByPathListCouroutine(List<Vector3> path)
    {
        int i = 0;
        ifMoveNext = true;
        while(i < path.Count)
        {
            if (ifMoveNext)
            {
                ifMoveNext = false;
                var point = path[i];
                StartCoroutine(MoveToPointCouroutine(point));
                i++;
            }
            yield return new WaitForEndOfFrame();
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
            float x = Mathf.Lerp( start_pos.x, target_pos.x, timer / time);
            float y = Mathf.Lerp( start_pos.y, target_pos.y, timer / time);
            Vector3 curPos = new Vector3(x, y, 0);
            Actor.transform.position = curPos;
            yield return new WaitForEndOfFrame();
        }

        isMoving = false;
        ifMoveNext = true;
    }
}
