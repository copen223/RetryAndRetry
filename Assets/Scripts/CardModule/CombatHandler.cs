using System;
using System.Collections;
using System.Collections.Generic;
using ActorModule.Core;
using CardModule.CardActions;
using CardModule.Controllers;
using OtherControllers;
using Physics;
using Tools;
using UnityEngine;

namespace CardModule
{
    public class CombatHandler:MonoBehaviour
    {
        private List<Combat> combats_list = new List<Combat>();

        private GameObject atker;
        private Card atkCard;
        //private Vector2 atkDir;
        private Vector2 atkTargetPoint; // 攻击目标点


        [SerializeField] private GameObject ContactPrefab = null;    // 接触点贴图
        private TargetPool contactPool;  //  接触点贴图对象池


        private void Start()
        {
            contactPool = new TargetPool(ContactPrefab);
        }

        /// <summary>
        /// 创建一个Combat处理者，基于一个atker向atkDir使用atkCard
        /// </summary>
        /// <param name="atker"></param>
        /// <param name="atkCard"></param>
        /// <param name="atkDir"></param>
        //public void SetCombatHandler(GameObject _atker,Card _atkCard,Vector2 _atkDir)
        //{
        //    atker = _atker; atkCard = _atkCard; atkDir = _atkDir;
        //}

        public void SetCombatHandler(GameObject _atker, Card _atkCard, Vector2 _atkTargetPoint)
        {
            atker = _atker; atkCard = _atkCard; atkTargetPoint = _atkTargetPoint;
        }

        /// <summary>
        /// 开始执行Combat,在处理完成后运行EndFunc
        /// </summary>
        public void StartHandleCombat(Action endFunc)
        {
            StartCoroutine(DoHandleCombat(endFunc));
        }

        private IEnumerator DoHandleCombat(Action endFunc)
        {
            var atkDir = atkTargetPoint - (Vector2)atker.GetComponent<ActorController>().Sprite.transform.position;

            var atkerCon = atker.GetComponent<ActorController>();
            atkerCon.ChangeFaceTo(atkDir);

            //------1.发出射线,确定攻击将作用的对象们-------
            Vector2 startPos = atkerCon.CenterOffset + atker.transform.position;
            AttackTrail atkTrail = atkCard.CardAction as AttackTrail;
            //float rayDistance = atkTrail.Distance_max;

            List<Vector2> rayLinePoints = atkTrail.GetLinePoints(startPos, atkTargetPoint);
            RaycastHit2D[] hits = MyPhysics2D.RayCastAlongLine(rayLinePoints);

            //RaycastHit2D[] hits = Physics2D.RaycastAll(startPos, atkDir, rayDistance);

            List<GameObject> beHitTargets = new List<GameObject>();     // 储存hit的对象，是1.发出射线的关键结果
            int backgroundHitNum = 0;

            Vector2 rayEndPoint = startPos;   // 显示射线的终点位置

            foreach(var hit in hits)
            {
                if (hit.collider == null) 
                    continue;
                if (hit.collider.tag == "Obstacle" && !CheckLayerIfCanAttack(hit.collider.gameObject.layer)) 
                { 
                    rayEndPoint = hit.point;
                    break; 
                }
                if (CheckLayerIfCanAttack(hit.collider.gameObject.layer))
                {
                    var targetCon = hit.collider.transform.parent.GetComponent<ActorController>();
                    var targetGo = hit.collider.transform.parent.gameObject;

                    if (targetCon.gameObject == atker) continue; // 是自己 跳过

                    //if (!(targetCon.group.IsPlayer ^ atkerCon.group.IsPlayer)) continue;    // 对象为友军，跳过
                    //if (targetCon.group.type == atkerCon.group.type) continue; // 对象为友军，跳过


                    // 环境物体的处理方式,Obstacle与actor相同处理，但是Background和Platform要进行特殊处理
                    if (1 << targetGo.layer == LayerMask.GetMask("EnvirObject"))
                    {
                        if (targetGo.tag == "Background" || targetGo.tag == "Platform")
                        {
                            beHitTargets.Add(targetGo);
                            backgroundHitNum += 1;
                            continue;
                        }
                    }

                    if (beHitTargets.Count < atkTrail.TargetNum + backgroundHitNum)    // 添加该对象，并检查是否超过攻击允许对象数，若超过，结束检测
                    {
                        beHitTargets.Add(targetCon.gameObject);
                        if (beHitTargets.Count == atkTrail.TargetNum + backgroundHitNum)
                        {
                            rayEndPoint = hit.point;
                            break;
                        }
                    }
                }
            }

            atker.GetComponent<ActorController>().ActiveAllFocusTrail(false);   // 放置射线接触到自身trail

            //------2.激活对象的专注轨迹，再次进行射线检测，构建combat------
            foreach (var target in beHitTargets)
            {
                // 射中的目标激活专注轨迹，显示专注个数。专注轨迹的具体显示视情况而定
                target.GetComponent<ActorController>().ActiveAllFocusTrail(true);
                target.GetComponent<ActorController>().ShowAllFocusTrail(true);     // 显示专注轨迹
                //target.GetComponent<ActorController>().ShowFocusTrailCount(true);
            }

            List<GameObject> contacts = new List<GameObject>(); // 记录触发的专注轨迹
            List<Vector2> contactPoints = new List<Vector2>();  // 记录触发点的位置

            // RaycastHit2D[] contactHits = Physics2D.RaycastAll(startPos, (rayEndPoint-startPos).normalized, Vector2.Distance(startPos,rayEndPoint), LayerMask.GetMask("BattleTrail"));
            RaycastHit2D[] contactHits = MyPhysics2D.RayCastAlongLine(atkTrail.GetLinePoints(startPos, rayEndPoint)); 
            float rayCutOffLength = Vector2.Distance(startPos, rayEndPoint);    // 用于实现截断攻击轨迹的专注轨迹机制
            float allowance = 0.1f;

            foreach (var hit in contactHits)
            {
                if (hit.collider.tag != "FocusTrail") continue;

                float hitLength = Vector2.Distance(startPos, hit.point);
                if (hitLength > rayCutOffLength + allowance)
                    continue;

                contacts.Add(hit.collider.gameObject);
                contactPoints.Add(hit.point);

                var con = hit.collider.gameObject.GetComponent<FocusTrailController>();
                if(con.CanCutOff)
                {
                    rayCutOffLength = hitLength;
                }
            }

            combats_list.Clear();   // 记录本次攻击带来的combat

            foreach(var target in beHitTargets) 
            {
                List<Card> activeFoucsCards = new List<Card>();
                foreach(var contact in contacts) 
                { 
                    var focusTrail = contact.GetComponent<FocusTrailController>();
                    if (focusTrail.Actor == target) activeFoucsCards.Add(focusTrail.Card);
                }
                var combat = new Combat(atkCard, activeFoucsCards, atker, target);
                combats_list.Add(combat);
            }

            //-------3.显示层，示意攻击方向以及播放动画--------
            var points2d = atkTrail.GetLinePoints(startPos, rayEndPoint);
            var points = new List<Vector3>();
            foreach (var p in points2d)
            {
                points.Add(p);
            }

            LineDrawer.instance.DrawLine(this, points, 0);  // 1显示射线

            // LineDrawer.instance.DrawLine(this, new List<Vector3> { startPos, rayEndPoint }, 0);  // 1显示射线
            SetContactPointPos(true, contactPoints);    // 2显示接触点

            float animationTime = 0.75f;float animationTimer = 0;
            while (animationTimer < animationTime)
            {
                yield return new WaitForEndOfFrame();
                animationTimer += Time.deltaTime;
            }

            LineDrawer.instance.FinishAllDrawing(this);
            SetContactPointPos(false, contactPoints);

            //------4.应用combat，结束攻击--------
            if (beHitTargets.Count > 0)
            {
                foreach (var target in beHitTargets)
                {
                    target.GetComponent<ActorController>().ActiveAllFocusTrail(false);
                    target.GetComponent<ActorController>().ShowAllFocusTrail(false);
                }
                foreach (var combat in combats_list)
                {
                    combat.StartDoCombat();
                }
            }

            atkCard.OnDoMake(); // 卡牌使用完成
            endFunc(); // 结束调用endFunc

        }

        /// <summary>
        /// 在对应位置上显示接触点,每帧调用
        /// </summary>
        /// <param name="active"></param>
        /// <param name="contactPoints"></param>
        private void SetContactPointPos(bool active,List<Vector2> contactPoints)
        {
            contactPool.ReSet();
            foreach (var pos in contactPoints)
            {
                Vector3 screenPos = Camera.main.WorldToScreenPoint(pos);
                screenPos = new Vector3(screenPos.x, screenPos.y, 0);
                var contactGb = contactPool.GetTarget(GameObject.Find("Canvas").transform);
                contactGb.SetActive(active);
                contactGb.transform.position = screenPos;
            }
        }

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
}
