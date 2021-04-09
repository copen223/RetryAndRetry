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
        
        var drawerObject = DrawerObjects.GetTarget(out id);
        if (linerId >= 0)
        {
            drawerObject = DrawerObjects.GetTarget(linerId);
            var line2 = drawerObject.GetComponent<LineRenderer>();
            line2.SetPositions(points.ToArray());
            line2.material = Materials[materialId];
            drawerObject.SetActive(true);
            return linerId;
        }
        DrawerObjects.GetIndex(drawerObject);
        var line = drawerObject.GetComponent<LineRenderer>();
        line.SetPositions(points.ToArray());
        line.material = Materials[materialId];
        drawerObject.SetActive(true);
        return id;
    }
    public void FinishDrawing(int id)
    {
        DrawerObjects.list[id].SetActive(false);
    }
}

