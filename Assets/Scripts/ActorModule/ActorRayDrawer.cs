using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorRayDrawer : MonoBehaviour
{
    public GameObject RayPrefab;
    private GameObject RayObject;
    private LineRenderer lineRenderer;
    // Start is called before the first frame update
    void Start()
    {
        if (RayObject == null)
            RayObject = GameObject.Instantiate(RayPrefab);
        lineRenderer = RayObject.GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DrawLine( List<Vector3> linePoints)
    {
        RayObject.SetActive(true);
        lineRenderer.positionCount = linePoints.Count;
        lineRenderer.SetPositions(linePoints.ToArray());
        lineRenderer.Simplify(0);
    }
    public void EndDraw()
    {
        RayObject.SetActive(false);
    }


}
