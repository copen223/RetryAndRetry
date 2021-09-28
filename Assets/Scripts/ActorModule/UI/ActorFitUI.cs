using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorFitUI : MonoBehaviour
{
    [Header("UI相对位移")]
    public Vector3 Offset;

    public void UpdatePosition()
    {
        transform.localPosition = Offset;
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
