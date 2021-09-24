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
    public event Action OnActionCancleEvent;
    // 链接
    /// <summary>
    /// 主控制器
    /// </summary>
    public CardController Controller;
    // 数据
    private CardAction action;
    public GameObject ContactPrefab;    // 接触点贴图
    private TargetPool contactPool;  //  接触点贴图对象池
    private List<Vector3> contactPoints = new List<Vector3>();  // 接触点位置
    private List<GameObject> contactObjects = new List<GameObject>();   // 接触的battleTrail
    private List<GameObject> targets = new List<GameObject>();  // 选中的对象
    private List<Combat> combats = new List<Combat>();
    private Vector3 attackPosOffset = new Vector3(0, 0.4f, 0);

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
        if (action is AttackTrail)
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
        holderPos = new Vector3(holderPos.x, holderPos.y, 1) + attackPosOffset;

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
            RaycastHit2D[] hits = Physics2D.RaycastAll(point1, dir, dis);

            for(int i =0; i<hits.Length; i++)
            {
                //-----------------------决定检测对象
                var hit = hits[i];
                var targetGo = hit.collider.gameObject;
                if(hit.collider.tag == "Actor" && hit.collider.gameObject.name == "Sprite")
                {
                    targetGo = hit.transform.parent.gameObject;    // 射中对象的sprite，则主要信息在于其父对象上
                }

                // 检测对象是否为自己
                if(targetGo != Controller.holder.gameObject)
                {
                    if(hit.collider.tag == "Obstacle")  // 遇到障碍物，射线被阻断
                    {
                        point2 = hit.point;
                        break;
                    }
                    else if(hit.collider.tag == "Actor")  // 遇到对象
                    {
                        if (targets.Count < trail.TargetNum)
                        {
                            targets.Add(targetGo);   // 选中对象+1
                            if (targets.Count == trail.TargetNum)
                            {
                                point2 = hits[i].point;
                                break;
                            }
                        }
                        else                                // 对象已满时
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

            //-------------------检测射线碰撞的专注轨迹-------------//
            dis = Vector2.Distance(point1, point2); 
            RaycastHit2D[] contactHits = Physics2D.RaycastAll(point1, dir, dis,LayerMask.GetMask("BattleTrail")); // 再射一次
            foreach(var con in contactHits)     // 完成contactPoints/Objects更新
            {
                if (con.collider.tag == "FocusTrail")
                {
                    if (con.collider.gameObject.GetComponent<FocusTrailController>().Actor == Controller.holder)
                        continue;
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

            SetContactPointPos(true);                           // 显示接触点
            points = new List<Vector3> { point1, point2 };
            Debug.Log("画攻击轨迹线");
            LineDrawer.instance.DrawLine(this, points, 0);  // 显示射线

            //-------------确认选择---------------------------------//
            //1.应用所有combat
            //2.通知action结束
            if (IfInputMouse0)
            {
                if (targets.Count > 0)
                {
                    foreach(var combat in combats)
                    {
                        combat.StartDoCombat();
                    }
                    foreach (var target in targets)
                        target.GetComponent<ActorController>().ShowAllFocusTrail(false);
                    SetContactPointPos(false);                      // 清除标记
                    LineDrawer.instance.FinishAllDrawing(this);     // 清除上一帧的线
                    OnActionOverEvent?.Invoke();    //  结束Action返回消息
                    break;
                }
            }
            if (IfInputMouse1)
            {
                // 取消输入
                foreach(var target in targets)
                    target.GetComponent<ActorController>().ShowAllFocusTrail(false);
                SetContactPointPos(false);                      // 清除标记
                LineDrawer.instance.FinishAllDrawing(this);
                OnActionCancleEvent?.Invoke();    //  结束Action返回消息
                break;
            }

            yield return new WaitForEndOfFrame();
        }
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
            Vector3 point1_offset = new Vector3(x, 0);
            Vector3 point2_offset = new Vector3(x, y);
            Vector3 point3_offset = new Vector3(0, y);
            Vector3 point1 = point1_offset + Controller.holder.transform.Find("Sprite").transform.position;
            Vector3 point2 = point2_offset + Controller.holder.transform.Find("Sprite").transform.position;
            Vector3 point3 = point3_offset + Controller.holder.transform.Find("Sprite").transform.position;
            points.Add(point1); points.Add(point2); points.Add(point3);
            //-------------------显示-------------------------

            var gb = focusTrailPool.GetTarget(Controller.holder.transform.Find("FocusTrails"));
            gb.GetComponent<FocusTrailController>().Seter = Controller.holder;
            gb.GetComponent<FocusTrailController>().SetPoints(points);
            gb.GetComponent<FocusTrailController>().SetOffsetPoints();  // 清空offset点集 防止使用offsetpoint决定线
            gb.SetActive(true);

            //-------------------输入-------------------------
            if(IfInputMouse0 && !gb.GetComponent<FocusTrailController>().IfOccupied)
            {
                gb.GetComponent<FocusTrailController>().SetOffsetPoints( point1_offset,point2_offset,point3_offset); 
                focusTrailPool.RemoveFromPool(gb);
                Controller.holder.GetComponent<ActorController>().AddFocusTrail(gb);
                Controller.Card.SetFocusTrail(gb);

                OnActionOverEvent?.Invoke();    // 通知action结束
                break;
            }
            if(IfInputMouse1)
            {
                // 取消输入
                gb.SetActive(false);

                OnActionCancleEvent?.Invoke();    // 通知action结束
                break;
            }

            yield return new WaitForEndOfFrame();
        }

        

    }

    private void SetContactPointPos(bool active)
    {
        contactPool.ReSet();
        foreach(var pos in contactPoints)
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(pos);
            screenPos = new Vector3(screenPos.x, screenPos.y, 0);
            var contactGb = contactPool.GetTarget(GameObject.Find("Canvas").transform);
            contactGb.SetActive(active);
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
