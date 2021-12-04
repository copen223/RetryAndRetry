using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Assets.Scripts.CardModule.CardActions;
using Assets.Scripts.Tools;
using System.Collections;

namespace Assets.Scripts.CardModule
{
    public class CombatHandler:MonoBehaviour
    {
        private List<Combat> combats_list = new List<Combat>();

        private GameObject atker;
        private Card atkCard;
        private Vector2 atkDir;

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
        public void SetCombatHandler(GameObject _atker,Card _atkCard,Vector2 _atkDir)
        {
            atker = _atker; atkCard = _atkCard; atkDir = _atkDir;
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
            var atkerCon = atker.GetComponent<ActorController>();
            atkerCon.ChangeFaceTo(atkDir);

            //------1.发出射线,确定攻击将作用的对象们-------
            Vector2 startPos = atkerCon.CenterOffset + atker.transform.position;
            AttackTrail atkTrail = atkCard.CardAction as AttackTrail;
            float rayDistance = atkTrail.Distance_max;

            RaycastHit2D[] hits = Physics2D.RaycastAll(startPos, atkDir, rayDistance);
            List<GameObject> beHitTargets = new List<GameObject>();     // 储存hit的对象，是1.发出射线的关键结果

            Vector2 rayEndPoint = startPos;   // 显示射线的终点位置

            foreach(var hit in hits)
            {
                if (hit.collider == null) continue;
                if (hit.collider.tag == "Obstacle" && !CheckLayerIfCanAttack(hit.collider.gameObject.layer)) { rayEndPoint = hit.point; break; }
                if (CheckLayerIfCanAttack(hit.collider.gameObject.layer))
                {
                    var targetCon = hit.collider.transform.parent.GetComponent<ActorController>();
                    if (!(targetCon.group.IsPlayer ^ atkerCon.group.IsPlayer)) continue;    // 对象为友军，跳过
                    
                    if (beHitTargets.Count < atkTrail.TargetNum)    // 添加该对象，并检查是否超过攻击允许对象数，若超过，结束检测
                    {
                        beHitTargets.Add(targetCon.gameObject);
                        if (beHitTargets.Count == atkTrail.TargetNum)
                        {
                            rayEndPoint = hit.point;
                            break;
                        }
                    }
                }
            }

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

            RaycastHit2D[] contactHits = Physics2D.RaycastAll(startPos, (rayEndPoint-startPos).normalized, Vector2.Distance(startPos,rayEndPoint), LayerMask.GetMask("BattleTrail"));
            foreach(var hit in contactHits)
            {
                if (hit.collider.tag != "FocusTrail") continue;
                contacts.Add(hit.collider.gameObject);
                contactPoints.Add(hit.point);
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

            LineDrawer.instance.DrawLine(this, new List<Vector3> { startPos, rayEndPoint }, 0);  // 1显示射线
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
