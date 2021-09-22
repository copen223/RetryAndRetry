using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("相机移动")]
    public float Speed;
    public Vector3 Offset;

    public enum CameraMode
    {
        Freedom,
        Track
    }

    public CameraMode Mode = CameraController.CameraMode.Freedom;
    public GameObject Target;

    private void Update()
    {
        switch (Mode)
        {
            case (CameraMode.Track):
                if (Target != null) MoveToTarget();break;
            case (CameraMode.Freedom):PlayerControlCamera(); break;
        }
        
    }

    public void SetAndMoveToTarget(GameObject target)
    {
        Target = target;
        Mode = CameraMode.Track;
    }

    public void SetFreedomControl()
    {
        Mode = CameraMode.Freedom;
    }

    private void PlayerControlCamera()
    {
        Vector3 move = Vector3.zero;
        if(Input.GetKey(KeyCode.W))
            move += Vector3.up;
        if (Input.GetKey(KeyCode.S))
            move += Vector3.down;
        if (Input.GetKey(KeyCode.A))
            move += Vector3.left;
        if (Input.GetKey(KeyCode.D))
            move += Vector3.right;
        transform.Translate(move * Speed * Time.deltaTime);
    }


    private void MoveToTarget()
    {
        transform.position = Offset + new Vector3(Target.transform.position.x, Target.transform.position.y, transform.position.z);
    }

}
