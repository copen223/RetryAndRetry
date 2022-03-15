using System.Collections.Generic;
using UnityEngine;

namespace ActorModule
{
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
            //for (int i = 0; i < linePoints.Count; i++) { linePoints[i] += new Vector3(0, 0, 0); }
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
}
