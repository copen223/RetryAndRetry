using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugLineController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    Vector3 start;
    Vector3 end;

    public void DrawLine(Vector3 _start,Vector3 _end)
    {
        start = _start;
        end = _end;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawLine(start, end, Color.red);
    }
}
