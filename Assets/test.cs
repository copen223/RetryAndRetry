using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    public Vector3 dir;
    public float angle;
    public Vector3 newDir;
    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Quaternion rotation = Quaternion.Euler(0, 0, angle);
            newDir = rotation * dir;
        }
    }
}
