using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloatUIController : MonoBehaviour
{
    public Text text { get { return transform.Find("Text").GetComponent<Text>(); } }
    public float MoveSpeed;


    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartMoveToDir(Vector3 dir,float time)
    {
        StartCoroutine(IEMoveAnDisappearAfterTime(dir, time));
    }

    IEnumerator IEMoveAnDisappearAfterTime(Vector3 dir, float time)
    {
        float timer = 0;
        while(timer <= time)
        {
            transform.Translate(dir.normalized * MoveSpeed * Time.deltaTime);

            text.color = new Color(text.color.r, text.color.g, text.color.b, 1 * ((time - timer) / time));

            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime;
        }

        GetComponent<TargetInPool>().InActive();
    }
}
