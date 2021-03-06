using System.Collections.Generic;
using Tools;
using UI;
using UnityEngine;

namespace OtherControllers
{
    public class LineDrawer : MonoBehaviour
    {
        public static LineDrawer instance;

        public GameObject DrawerPrefab;
        public List<Material> Materials;
        public TargetPool DrawerObjects;

        void Awake()
        {
            instance = this;
        }
        void Start()
        {
            DrawerObjects = new TargetPool(DrawerPrefab);
        }
        //public int DrawLine(List<Vector3> points,int materialId,int linerId)
        //{
        //    int id;
        //    GameObject drawerObject;

        //    if (linerId < 0)
        //    {
        //        drawerObject = DrawerObjects.GetTarget(out id);   // 第一次获取对象，则创建对象并获取ID
        //        var line = drawerObject.GetComponent<LineRenderer>();
        //        line.SetPositions(points.ToArray());        // 更新线点
        //        line.material = Materials[materialId];
        //        drawerObject.SetActive(true);
        //        return id;
        //    }
        //    else   // 非首次获取对象，则根据ID获取对象
        //    {
        //        drawerObject = DrawerObjects.GetTarget(linerId);
        //        var line2 = drawerObject.GetComponent<LineRenderer>();
        //        line2.SetPositions(points.ToArray());       // 更新线点
        //        line2.material = Materials[materialId];
        //        drawerObject.SetActive(true);
        //        return linerId;
        //    }
        //}

        /// <summary>
        /// 开始画线，实际是创建一个画线物体，所以不需要每帧调用
        /// </summary>
        /// <param name="who"></param>
        /// <param name="points"></param>
        /// <param name="materialId"></param>
        public void DrawLine(Object who,List<Vector3> points, int materialId)
        {
            var gb = DrawerObjects.GetTarget();
            gb.GetComponent<LineObjectController>().User = who;

            var line = gb.GetComponent<LineRenderer>();
            line.positionCount = points.Count;
            line.SetPositions(points.ToArray());        // 更新线点
            line.Simplify(0);
            line.material = Materials[materialId];

            gb.SetActive(true);
            //Debug.Log(line.gameObject + "<points:" + points[0] + "至" + points[1]);
        }
        //public void FinishDrawing(int id)
        //{
        //    DrawerObjects.list[id].SetActive(false);
        //}
        /// <summary>
        /// 取消画线
        /// </summary>
        /// <param name="who"></param>
        public void FinishAllDrawing(Object who)
        {
            foreach (var g in DrawerObjects.list)
            {
                var c = g.GetComponent<LineObjectController>();
                if (c.User == who)
                    c.gameObject.SetActive(false);
            }
        }

        //public SpriteShapeController shapeController;
        //public void DrawLineBySpriteShape()
        //{
        //    shapeController.spriteShapeRenderer.
        //}
    }
}

