using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Tools;

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
    public int DrawLine(List<Vector3> points,int materialId,int linerId)
    {
        int id;
        GameObject drawerObject;

        if (linerId < 0)
        {
            drawerObject = DrawerObjects.GetTarget(out id);   // 第一次获取对象，则创建对象并获取ID
            var line = drawerObject.GetComponent<LineRenderer>();
            line.SetPositions(points.ToArray());        // 更新线点
            line.material = Materials[materialId];
            drawerObject.SetActive(true);
            return id;
        }
        else   // 非首次获取对象，则根据ID获取对象
        {
            drawerObject = DrawerObjects.GetTarget(linerId);
            var line2 = drawerObject.GetComponent<LineRenderer>();
            line2.SetPositions(points.ToArray());       // 更新线点
            line2.material = Materials[materialId];
            drawerObject.SetActive(true);
            return linerId;
        }
    }
    public void FinishDrawing(int id)
    {
        DrawerObjects.list[id].SetActive(false);
    }
}

