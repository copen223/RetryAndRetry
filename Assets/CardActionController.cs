using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.CardModule;
using Assets.Scripts.CardModule.CardActions;

public class CardActionController : MonoBehaviour
{
    public CardController Controller;
    private CardAction action;
    public void StartAction(CardAction _action)
    {
        action = _action;
        if(action is BattleTrail)
        {
            StartCoroutine(DrawBattleTrail());
        }
    }

    IEnumerator DrawBattleTrail()
    {
        //--------------------画线初始化----------//
        List<Vector3> points = new List<Vector3>();
        Vector3 holderPos = Controller.holder.transform.position;
        holderPos = new Vector3(holderPos.x, holderPos.y, 1);
        points.Add(holderPos);
        points.Add(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        int lineId = -1;


        BattleTrail trail = action as BattleTrail;

        while (true)
        {
            //----------------射线----------------//

            //------------------画线--------------//
            lineId = LineDrawer.instance.DrawLine(points, 0, lineId);
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos = new Vector3(mousePos.x, mousePos.y, 1);
            Vector3 dir = (mousePos - holderPos).normalized;

            points[1] = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            points[1] = new Vector3(points[1].x, points[1].y, 1);

            //-----------------    --------------//
            

            yield return new WaitForEndOfFrame();
        }
    }

    private void EndDraw(int lineId)
    {
        LineDrawer.instance.FinishDrawing(lineId);
    }


    void Start()
    {
        
    }
    void Update()
    {
        
    }
}
