using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.CardModule;
using Assets.Scripts.CardModule.CardActions;
using System;
using Assets.Scripts.Tools;

public class CardActionController : MonoBehaviour
{
    // 事件
    public event Action OnActionOverEvent;
    // 链接
    public CardController Controller;
    // 数据
    private CardAction action;
    public GameObject ContactPrefab;    // 接触点贴图
    private TargetPool contactPool;  //  接触点贴图对象池
    private List<Vector3> contactPoints = new List<Vector3>();  // 接触点位置
    private List<GameObject> contactObjects = new List<GameObject>();   // 接触的battleTrail
    private List<GameObject> targets = new List<GameObject>();  // 选中的对象
    private List<Combat> combats = new List<Combat>();

    public GameObject FocusTrailPrefab;     // 专注轨迹预组
    private TargetPool focusTrailPool;      // 专注轨迹池

    public void AddNewTrailToPool(GameObject gb) { focusTrailPool.AddToPool(gb); }
    // 控制参量
    public bool IsAction;

    private bool IfInputMouse0;
    private bool IfInputMouse1;
    public void StartAction(CardAction _action)
    {
        IsAction = true;
        action = _action;
        if(action is AttackTrail)
        {
            StartCoroutine(DrawAttackTrail());
        }
        else if(action is FocusTrail)
        {
            StartCoroutine(DrawFocusTrail());
        }

    }

    IEnumerator DrawAttackTrail() 
    {
        //--------------------画线初始化----------//
        List<Vector3> points = new List<Vector3>(); // linerenderer用点集

        Vector3 holderPos = Controller.holder.transform.position;
        holderPos = new Vector3(holderPos.x, holderPos.y, 1);

        Vector3 point1 = holderPos;                 // 获取第一个点

        AttackTrail trail = action as AttackTrail;  // 获取AttackTrail信息

        while (true)
        {
            //---------------每帧重置数据-----------------//
            foreach (var target in targets) target.GetComponent<ActorController>().ShowAllFocusTrail(false);
            contactPoints.Clear();
            contactObjects.Clear();
            targets.Clear();
            combats.Clear();
            LineDrawer.instance.FinishAllDrawing(this);     // 清除上一帧的线
            //--------------------------------------------//

            //------------------画线--------------//
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos = new Vector3(mousePos.x, mousePos.y, 1);
            Vector3 dir = (mousePos - point1).normalized;    //  方向确定
            float dis = Vector2.Distance(mousePos, point1);  // 距离确定

            // 读action信息求射线点
            // point2 取min、max间的线；
            float trueDis = dis;
            if (dis > trail.Distance_max) trueDis = trail.Distance_max;
            else if (dis < trail.Distance_min) trueDis = trail.Distance_min;

            Vector3 point2 = point1 + dir * dis; //  point2确定

            //-----------------射线检测--------------//
            RaycastHit2D[] hits = Physics2D.RaycastAll(Controller.holder.transform.position, dir, dis);

            for(int i =0; i<hits.Length; i++)
            {
                var hit = hits[i];

                if(hit.collider.gameObject != Controller.holder.gameObject)
                {
                    if(hit.collider.tag == "Obstacle")  // 遇到障碍物，跳出遍历
                    {
                        point2 = hit.point;
                        break;
                    }
                    else if(hit.collider.tag == "Actor")  // 否则遇到对象
                    {
                        if (targets.Count < trail.TargetNum)
                            targets.Add(hit.collider.gameObject);   // 选中对象+1
                        else                                // 否则对象已满
                        {
                            point2 = hits[i - 1].point;
                            break;
                        }
                    }
                }
            }
            //------------targets更新完毕----------------//
            //------------更新contacts和combat-------------------//
            foreach(var target in targets)
            {
                target.GetComponent<ActorController>().ShowAllFocusTrail(true);
            }

            RaycastHit2D[] contactHits = Physics2D.RaycastAll(Controller.holder.transform.position, dir, dis); // 再射一次
            foreach(var con in contactHits)     // 完成contactPoints/Objects更新
            {
                if (con.collider.tag == "FocusTrail")
                {
                    contactPoints.Add(con.point);
                    contactObjects.Add(con.transform.gameObject);
                }
            }

            foreach(var target in targets)
            {
                // 获取每个对象触发的专注卡牌
                List<Card> focusCards = new List<Card>();
                foreach(var con in contactObjects)
                {
                    var focusTrail = con.GetComponent<FocusTrailController>();
                    if (focusTrail.Actor == target)
                    {
                        focusCards.Add(focusTrail.Card);
                    }
                }
                var combat = new Combat(Controller.Card,focusCards,Controller.holder,target);
                combats.Add(combat);
            }

            //------------contacts和combats更新完毕-------------------//
            //----------------------显示------------------------------//

            SetContactPointPos();                           // 显示接触点
            points = new List<Vector3> { point1, point2 };
            LineDrawer.instance.DrawLine(this, points, 0);  // 显示射线

            //-------------确认选择---------------------------------//
            if (IfInputMouse0)
            {
                if (targets.Count > 0)
                {
                    foreach(var combat in combats)
                    {
                        combat.StartDoCombat();
                    }
                    break;
                }
            }
            if (IfInputMouse1)
            {
                // 取消输入
                LineDrawer.instance.FinishAllDrawing(this);
                break;
            }

            yield return new WaitForEndOfFrame();
        }

        LineDrawer.instance.FinishAllDrawing(this);     // 清除上一帧的线
        OnActionOverEvent?.Invoke();    //  结束Action返回消息
    }

    IEnumerator DrawFocusTrail()
    {
        List<Vector3> points = new List<Vector3>();
        while (true)
        {
            //-------------------每帧更新-------------------
            points.Clear();
            focusTrailPool.ReSet();
            //-------------------计算位置和点集------------------
            var mouseScreenPos = Input.mousePosition;
            var mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
            Vector2 dir = mouseWorldPos - Controller.holder.transform.position;    // 鼠标相对玩家位置
            dir = dir.normalized;

            FocusTrail trail = action as FocusTrail;

            float x = trail.Distance_X * (dir.x < 0 ? (-1) : 1);
            float y = trail.Distance_Y * (dir.y < 0 ? (-1) : 1);

            // 获得世界坐标
            Vector3 point1 = new Vector3(x, 0, 0) + Controller.holder.transform.position;
            Vector3 point2 = new Vector3(x, y, 0) + Controller.holder.transform.position;
            Vector3 point3 = new Vector3(0, y, 0) + Controller.holder.transform.position;
            points.Add(point1); points.Add(point2); points.Add(point3);
            //-------------------显示-------------------------

            var gb = focusTrailPool.GetTarget(Controller.holder.transform.Find("FocusTrails"));
            gb.GetComponent<FocusTrailController>().Seter = Controller.holder;
            gb.GetComponent<FocusTrailController>().SetPoints(points);
            gb.SetActive(true);

            //-------------------输入-------------------------
            if(IfInputMouse0 && !gb.GetComponent<FocusTrailController>().IfOccupied)
            {
                focusTrailPool.RemoveFromPool(gb);
                Controller.holder.GetComponent<ActorController>().AddFocusTrail(gb);
                Controller.Card.SetFocusTrail(gb);
                break;
            }
            if(IfInputMouse1)
            {
                // 取消输入
                gb.SetActive(false);
                break;
            }

            yield return new WaitForEndOfFrame();
        }

        OnActionOverEvent?.Invoke();    // 通知action结束

    }

    private void SetContactPointPos()
    {
        contactPool.ReSet();
        foreach(var pos in contactPoints)
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(pos);
            screenPos = new Vector3(screenPos.x, screenPos.y, 0);
            var contactGb = contactPool.GetTarget();
            contactGb.transform.position = screenPos;
        }
    }


    void Start()
    {
        Controller = transform.parent.GetComponent<CardController>();
        contactPool = new TargetPool(ContactPrefab);

        focusTrailPool = new TargetPool(FocusTrailPrefab);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0)) IfInputMouse0 = true; else IfInputMouse0 = false;
        if (Input.GetKeyDown(KeyCode.Mouse1)) IfInputMouse1 = true; else IfInputMouse1 = false;
    }
}
