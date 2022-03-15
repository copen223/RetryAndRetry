using System.Collections.Generic;
using UnityEngine;

namespace Physics
{
    public class MyPhysics2D
    {
        /// <summary>
        /// 沿着Line进行射线检测
        /// </summary>
        /// <param name="linePoints"></param>
        /// <param name="mask"></param>
        /// <returns></returns>
        public static RaycastHit2D[] RayCastAlongLine(List<Vector2> linePoints,LayerMask mask)
        {
            List<RaycastHit2D> hit2Ds = new List<RaycastHit2D>();
            List<GameObject> hitTargets = new List<GameObject>();

            for(int i=0;i < linePoints.Count - 1;i++)
            {
                Vector2 curPoint = linePoints[i];
                Vector2 nextPoint = linePoints[i];

                RaycastHit2D[] hitsByOneLine = Physics2D.LinecastAll(curPoint, nextPoint, mask);

                foreach(var hitByOneLine in hitsByOneLine)
                {
                    if (hitByOneLine.collider == null)
                        continue;

                    if(!hitTargets.Contains(hitByOneLine.collider.gameObject))
                    {
                        hitTargets.Add(hitByOneLine.collider.gameObject);
                        hit2Ds.Add(hitByOneLine);
                    }
                }

            }

            return hit2Ds.ToArray();
            
        }

        public static RaycastHit2D[] RayCastAlongLine(List<Vector2> linePoints)
        {
            List<RaycastHit2D> hit2Ds = new List<RaycastHit2D>();
            List<GameObject> hitTargets = new List<GameObject>();

            for (int i = 0; i < linePoints.Count - 1; i++)
            {
                Vector2 curPoint = linePoints[i];
                Vector2 nextPoint = linePoints[i + 1];

                RaycastHit2D[] hitsByOneLine = Physics2D.LinecastAll(curPoint, nextPoint);

                foreach (var hitByOneLine in hitsByOneLine)
                {
                    if (hitByOneLine.collider == null)
                        continue;

                    if (!hitTargets.Contains(hitByOneLine.collider.gameObject))
                    {
                        hitTargets.Add(hitByOneLine.collider.gameObject);
                        hit2Ds.Add(hitByOneLine);
                    }
                }

            }

            return hit2Ds.ToArray();

        }
    }
}
