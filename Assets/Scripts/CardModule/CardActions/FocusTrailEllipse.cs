using Assets.Scripts.CardModule.CardEffects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.CardModule.CardActions
{
    [CreateAssetMenu(fileName = "FocusTrailEllipse", menuName = "MyInfo/使用形式/专注轨迹_椭圆")]
    public class FocusTrailEllipse : FocusTrail
    {
        public float angle;

        public float a;
        public float b;

        public bool IfSetABByCollider;

        public override List<Vector3> GetLineOffsetPoints(Vector2 setDir)
        {
            if(IfSetABByCollider)
            {
                float allowance = 0.05f;
                a = Actor.Sprite.GetComponent<CapsuleCollider2D>().size.y/2 + allowance;
                b = Actor.Sprite.GetComponent<CapsuleCollider2D>().size.x/2 + allowance;
            }


            List<Vector3> offsetPoints = new List<Vector3>();

            Vector3 referenceDir = setDir;    // 基准方向点

            float startAngle = -angle/2, endAngle = angle/2; int rayCount = 10;

            for (float angle = startAngle; angle <= endAngle; angle += (endAngle - startAngle) / rayCount)
            {
                Quaternion rotation = Quaternion.Euler(0, 0, angle);
                Vector3 dir3D = (Vector3)referenceDir;
                Vector3 thisDir = (rotation * dir3D).normalized;

                if (Mathf.Abs( thisDir.x) < Mathf.Epsilon) 
                {
                    float ty = thisDir.y > 0 ? thisDir.y : -thisDir.y;
                    offsetPoints.Add(new Vector3(0, ty, 0));
                    continue;
                }

                float yrx = thisDir.y / thisDir.x;

                float x = a * a * b * b / (yrx * yrx * b * b + a * a);
                x = Mathf.Sqrt(x);
                x = thisDir.x > 0 ? x : -x;
                float y = yrx * x;

                offsetPoints.Add(new Vector3(x,y,0));
            }

            return offsetPoints;
        }
    }
}
