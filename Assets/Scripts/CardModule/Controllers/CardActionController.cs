using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.CardModule;
using Assets.Scripts.CardModule.CardActions;
using System;
using Assets.Scripts.Tools;
using Assets.Scripts.Physics;

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
    private CardAction action;  // 当前的行动数据
    [SerializeField]private GameObject ContactPrefab = null;    // 接触点贴图
    private TargetPool contactPool;  //  接触点贴图对象池
    private List<Vector3> contactPoints = new List<Vector3>();  // 接触点位置
    private List<GameObject> contactObjects = new List<GameObject>();   // 接触的battleTrail
    private List<GameObject> targets = new List<GameObject>();  // 选中的对象
    private List<Combat> combats = new List<Combat>();

    [SerializeField]private GameObject FocusTrailPrefab = null;     // 专注轨迹预组

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
        holderPos = new Vector3(holderPos.x, holderPos.y, 1) + Controller.holder.GetComponent<ActorController>().CenterOffset;

        Vector3 startPos = holderPos;                 // 获取第一个点

        AttackTrail trail = action as AttackTrail;  // 获取AttackTrail信息

        while (true)
        {
            //---------------每帧重置数据-----------------//
            foreach (var target in targets)
            {
                target.GetComponent<ActorController>().ActiveAllFocusTrail(false);
                target.GetComponent<ActorController>().ShowFocusTrailCount(false);
            }
            contactPoints.Clear();
            contactObjects.Clear();
            targets.Clear();
            int backGroudhitNum = 0;
            combats.Clear();
            LineDrawer.instance.FinishAllDrawing(this);     // 清除上一帧的线
            //--------------------------------------------//

            //------------------画攻击线--------------//
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos = new Vector3(mousePos.x, mousePos.y, 1);
            Vector3 dir = (mousePos - startPos).normalized;    //  方向确定
            float dis = Vector2.Distance(mousePos, startPos);  // 距离确定

            Controller.holder.GetComponent<ActorController>().ChangeFaceTo(dir);

            // 读action信息求射线点
            // point2 取min、max间的线；

            if (dis > trail.Distance_max) dis = trail.Distance_max;
            else if (dis < trail.Distance_min) dis = trail.Distance_min;

            Vector3 endPos = startPos + dir * dis; //  point2确定

            //-----------------射线检测--------------//
            List<Vector2> tailPoints = trail.GetLinePoints(startPos, endPos);

            //Debug.Log(startPos + "" + endPos);

            RaycastHit2D[] hits = MyPhysics2D.RayCastAlongLine(tailPoints);

            for(int i =0; i < hits.Length; i++)
            {
                //-----------------------决定检测对象
                var hit = hits[i];
                var targetGo = hit.collider.gameObject;
                if(CheckLayerIfCanAttack(hit.transform.gameObject.layer)
                    && hit.collider.gameObject.name == "Sprite")
                {
                    targetGo = hit.transform.parent.gameObject;    // 射中对象的sprite，则主要信息在于其父对象上
                }

                // 检测对象是否为自己
                if(targetGo != Controller.holder.gameObject)
                {
                    if(hit.collider.tag == "Obstacle" && 
                        !CheckLayerIfCanAttack(hit.transform.gameObject.layer))  // 遇到障碍物，射线被阻断
                    {
                        endPos = hit.point;
                        break;
                    }
                    else if(CheckLayerIfCanAttack(hit.transform.gameObject.layer))  // 遇到可攻击对象
                    {
                        // 环境物体的处理方式,Obstacle与actor相同处理，但是Background和Platform要进行特殊处理
                        if (1 << targetGo.layer == LayerMask.GetMask("EnvirObject"))
                        {
                            if(targetGo.tag == "Background" || targetGo.tag == "Platform")
                            {
                                targets.Add(targetGo);
                                backGroudhitNum += 1;
                                continue;
                            }
                        }

                        if (targets.Count < trail.TargetNum + backGroudhitNum)
                        {
                            targets.Add(targetGo);   // 选中对象+1
                            if (targets.Count == trail.TargetNum + backGroudhitNum)
                            {
                                endPos = hits[i].point;
                                break;
                            }
                        }
                        else                                // 对象已满时
                        {
                            endPos = hits[i - 1].point;
                            break;
                        }
                    }
                }
            }
            //------------targets更新完毕----------------//
            //------------更新contacts和combat-------------------//
            foreach(var target in targets)
            {
                // 射中的目标激活专注轨迹，显示专注个数。专注轨迹的具体显示视情况而定
                target.GetComponent<ActorController>().ActiveAllFocusTrail(true);
                //target.GetComponent<ActorController>().ShowAllFocusTrail(false);
                target.GetComponent<ActorController>().ShowAllFocusTrail(true);
                target.GetComponent<ActorController>().ShowFocusTrailCount(true);
            }

            //-------------------检测射线碰撞的专注轨迹-------------//
            dis = Vector2.Distance(startPos, endPos);
            // RaycastHit2D[] contactHits = Physics2D.RaycastAll(startPos, dir, dis,LayerMask.GetMask("BattleTrail")); // 再射一次
            RaycastHit2D[] contactHits = MyPhysics2D.RayCastAlongLine(trail.GetLinePoints(startPos, endPos));
            //Debug.Log(startPos + "" + endPos);

            float rayCutOffLength = dis;
            float allowance = 0.05f;

            foreach (var con in contactHits)     // 完成contactPoints/Objects更新
            {
                if (con.collider.tag == "FocusTrail")
                {
                    float hitLength = Vector2.Distance(startPos, con.point);
                    if (hitLength > rayCutOffLength + allowance)
                        continue;

                    if (con.collider.gameObject.GetComponent<FocusTrailController>().Actor == Controller.holder)
                        continue;

                    if(con.collider.gameObject.GetComponent<FocusTrailController>().IfShow) // 只有与显示的轨迹相交才显示相交点
                        contactPoints.Add(con.point);

                    contactObjects.Add(con.transform.gameObject);

                    var controller = con.collider.gameObject.GetComponent<FocusTrailController>();
                    if (controller.CanCutOff)
                    {
                        rayCutOffLength = hitLength;
                    }
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
            //points = new List<Vector3> { startPos, endPos };

            var points2d = trail.GetLinePoints(startPos, endPos);
            points = new List<Vector3>();
            foreach(var p in points2d)
            {
                points.Add(p);
            }

            //Debug.Log("画攻击轨迹线");
            LineDrawer.instance.DrawLine(this, points, 0);  // 显示射线

            //-------------确认选择---------------------------------//
            //1.应用所有combat
            //2.通知action结束
            if (IfInputMouse0)
            {
                if (targets.Count > 0)
                {

                    Controller.Card.OnDoMake();

                    foreach (var combat in combats)
                    {
                        combat.StartDoCombat();
                    }
                    foreach (var target in targets)
                    {
                        target.GetComponent<ActorController>().ActiveAllFocusTrail(false);
                        target.GetComponent<ActorController>().ShowAllFocusTrail(false);
                        target.GetComponent<ActorController>().ShowFocusTrailCount(false);
                    }
                    SetContactPointPos(false);                      // 清除标记
                    LineDrawer.instance.FinishAllDrawing(this);     // 清除上一帧的线
                    OnActionOverEvent?.Invoke();    //  结束Action返回消息
                    break;
                }
            }
            if (IfInputMouse1)
            {
                // 取消输入
                foreach (var target in targets)
                {
                    target.GetComponent<ActorController>().ActiveAllFocusTrail(false);
                    target.GetComponent<ActorController>().ShowAllFocusTrail(false);
                    target.GetComponent<ActorController>().ShowFocusTrailCount(false);
                }
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
        var gb = Instantiate(FocusTrailPrefab, Controller.holder.transform.Find("FocusTrails")); // 生成的focustrial对象

        while (true)
        {
            //-------------------每帧更新-------------------
            points.Clear();
            //-------------------计算位置和点集------------------
            var mouseScreenPos = Input.mousePosition;
            var mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
            Vector2 dir = mouseWorldPos - Controller.holder.transform.position;    // 鼠标相对玩家位置
            dir = dir.normalized;

            FocusTrail trail = action as FocusTrail;
            trail.Actor = Controller.holder.GetComponent<ActorController>();

            List<Vector3> focusTrailOffsetPoints = trail.GetLineOffsetPoints(dir);
            List<Vector3> focusTrailWorldPoints = new List<Vector3>();
            foreach (var point in focusTrailOffsetPoints)
            {
                focusTrailWorldPoints.Add(point + Controller.holder.GetComponent<ActorController>().Sprite.transform.position);
            }

            //-------------------显示-------------------------
            gb.transform.localScale = new Vector3(1, 1, 1);
            gb.GetComponent<FocusTrailController>().IfShow = true;
            gb.GetComponent<FocusTrailController>().Seter = Controller.holder;
            gb.GetComponent<FocusTrailController>().SetPoints(focusTrailWorldPoints);
            gb.GetComponent<FocusTrailController>().SetOffsetPoints();  // 清空offset点集 防止使用offsetpoint决定线
            gb.GetComponent<FocusTrailController>().SetColor(true);
            gb.SetActive(true);

            //-------------------输入-------------------------
            if(IfInputMouse0 && !gb.GetComponent<FocusTrailController>().IfOccupied)
            {
                Vector2 scale_x = Controller.holder.transform.localScale;
                gb.GetComponent<FocusTrailController>().SetOffsetPoints(focusTrailOffsetPoints.ToArray());
                gb.GetComponent<FocusTrailController>().SetColor(false);
                Controller.holder.GetComponent<ActorController>().AddFocusTrail(gb);
                Controller.Card.SetFocusTrail(gb);
                gb.GetComponent<FocusTrailController>().IfShow = false;

                OnActionOverEvent?.Invoke();    // 通知action结束
                break;
            }
            if(IfInputMouse1)
            {
                // 取消输入
                Destroy(gb);

                OnActionCancleEvent?.Invoke();    // 通知action结束
                break;
            }

            yield return new WaitForEndOfFrame();
        }
    }

    /// <summary>
    /// 修正专注轨迹的相对point,通过人物朝向
    /// </summary>
    /// <returns></returns>
    private List<Vector3> ReviseOffsetPoints(List<Vector3> basePoints)
    {
        return null;
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
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0)) IfInputMouse0 = true; else IfInputMouse0 = false;
        if (Input.GetKeyDown(KeyCode.Mouse1)) IfInputMouse1 = true; else IfInputMouse1 = false;
    }
    
    // 工具
    private bool CheckLayerIfCanAttack(int layer)
    {
        int bitmask = 1 << layer;
        int layerCheck1 = LayerMask.GetMask("Actor");
        int layerCheck2 = LayerMask.GetMask("EnvirObject");
        int layerCheck = layerCheck1 | layerCheck2;
        int layerCheckEnd = bitmask & layerCheck;
        return 0 != (bitmask & layerCheckEnd);
    }
}
