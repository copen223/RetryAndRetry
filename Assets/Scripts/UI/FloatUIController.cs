using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloatUIController : MonoBehaviour
{
    public Text text { get { return transform.Find("Text").GetComponent<Text>(); } }
    public float MoveSpeed = 2f;
    public Vector3 BaseWorldPos = Vector3.zero;
    public float StopTimeBiLi = 0.75f;
    private GameObject target;


    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartMoveToDir(Vector3 dir,float time)
    {
        StartCoroutine(IEMoveAnDisappearAfterTime(dir, time, false));
    }
    public void StartMoveToDir(GameObject _target, Vector3 dir, float time)
    {
        target = _target;
        StartCoroutine(IEMoveAnDisappearAfterTime(dir, time, true));
    }

    IEnumerator IEMoveAnDisappearAfterTime(Vector3 dir, float time, bool ifChaseTarget)
    {
        float timer = 0;
        while(timer <= time)
        {
            if (timer <= time * StopTimeBiLi)
            {
                Vector3 baseWorldPos = ifChaseTarget ? target.transform.position : BaseWorldPos;
                Vector3 newWorldPos = baseWorldPos + dir.normalized * MoveSpeed * timer;
                //Debug.Log(newWorldPos);
                Vector3 newScreenPos = Camera.main.WorldToScreenPoint(newWorldPos);

                transform.position = newScreenPos;
            }

            text.color = new Color(text.color.r, text.color.g, text.color.b, 1 * ((time - timer) / time));

            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime;
        }

        GetComponent<TargetInPool>().InActive();
    }
}
