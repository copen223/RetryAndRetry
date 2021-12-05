using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.CardModule;
using Assets.Scripts.CardModule.CardEffects;
using System.Linq;
using Assets.Scripts.SpaceModule.PathfinderModule;
using Assets.Scripts.ActorModule.ActorStates;
using Assets.Scripts.CardModule.CardActions;

namespace ActorModule.AI
{
    /// <summary>
    /// 就近攻击行动方案
    /// </summary>
    public class ProximityAttackAP : ActionPlan
    {
        [SerializeField]
        private GameObject debugLine = null;

        private Dictionary<GameObject, float> heatLevelOfEnemies_dic = new Dictionary<GameObject, float>();  // 威胁度字典

        private Card attackCard;
        private GameObject attackTarget;
        
        [SerializeField]
        private string attackCardInfo = null;

        [SerializeField]
        private string focusCardInfo = null;
        private Card focusCard;

        private bool ifAttackUp;    // 决定该单位攻击方向
        private Vector2 focusDir1;    // 决定该单位的专注方向，参数1代表是否面朝方向专注，参数2代表是否上方专注
        private Vector2 focusDir2;

        private void Start()
        {
            ifAttackUp = (Random.Range(0, 50f) <= 25f);

            int focusX = (int)Actor.GetComponent<ActorController>().FaceDir.x;
            int focusY = (Random.Range(50, 100f) <= 75f) ? 1 : -1;

            focusDir1 = new Vector2(focusX, focusY);

            int focusX2 = (Random.Range(100, 200f) <= 150f) ? 1 : -1;
            int focusY2 = (Random.Range(300, 400f) <= 350f) ? 1 : -1;
            if (focusX2 == focusX) { if (focusY2 == focusY) { focusX2 = -focusX2; } }

            focusDir2 = new Vector2(focusX2, focusY2);

        }

        /// <summary>
        /// 开始该行动
        /// </summary>
        public override void DoPlan()
        {
            if (attackCard == null || focusCard == null)
            {
                CardBuilder builder = new CardBuilder();
                attackCard = builder.CreatCardByName(attackCardInfo);
                attackCard.User = ActionMode.Actor.GetComponent<ActorController>();
                focusCard = builder.CreatCardByName(focusCardInfo);
                focusCard.User = ActionMode.Actor.GetComponent<ActorController>();
            }
            StartCoroutine(DoPlanCoroutine());
        }

        private IEnumerator DoPlanCoroutine()
        {
            //------------进行专注--------------
            FocusTrailHandler focusHandler = AI.GetComponent<FocusTrailHandler>();
            focusHandler.SetFocusTrailHandler(Actor, focusCard, focusDir1);
            yield return focusHandler.StartHandleFocusTrail();

            //focusHandler.SetFocusTrailHandler(Actor, focusCard, focusDir2);
            //yield return focusHandler.StartHandleFocusTrail();

            //------------进行移动--------------

            UpdateHeatLevels(); // 更新仇恨值

            var dicSort = from objDic in heatLevelOfEnemies_dic orderby objDic.Value descending select objDic;
            var target = dicSort.First().Key;   // 获取攻击对象
            attackTarget = target;

            GameObject selfGo = Actor;
            EnemyController selfController = Actor.GetComponent<EnemyController>();
            PathFinderComponent pathFinder = Actor.GetComponent<PathFinderComponent>();

            List<Node> canMoveNodes_list = pathFinder.SearchPathForm(selfGo.transform.position, CheckIfCanAttack); // 从自己开始进行寻路搜索
            var path = new List<Vector3>();
            foreach (var node in canMoveNodes_list)
            {
                Vector3 nodeInWorld = pathFinder.CellToWorld((node.x, node.y));
                path = pathFinder.GetPathFromTo(selfGo.transform.position, nodeInWorld);
                if (path.Count >= 1)
                    break;
            }

            // Cost限制移动
            int surplus = 0;
            int point = selfController.MovePoint;
            while (true)
            {
                if (path.Count <= 0)
                    break;
                int cost = Mathf.FloorToInt(pathFinder.GetPathCostToNode(path[path.Count - 1]));
                surplus = point - cost;
                if (surplus < 0)
                {
                    path.RemoveAt(path.Count - 1);
                    continue;
                }
                else
                    break;
            }

            selfController.StatesChild.GetComponent<ActorMoveByPath>().SetNodePath(pathFinder.VectorPath2NodePath(path));
            selfController.StatesChild.GetComponent<ActorActionIdle>().ChangeStateTo<ActorMoveByPath>();

            //----------------等待移动完成---------------------

            while (true)
            {
                if (selfController.currentState is ActorActionIdle)
                    break;
               
                yield return new WaitForSeconds(0.2f);
            }

            //----------------开始攻击------------
            AttackTrail trail = attackCard.CardAction as AttackTrail;
            Vector2 atkReferenceDir = target.GetComponent<ActorController>().CenterPos - selfController.CenterPos;

            Vector2 atkDir = Vector2.zero;
            AttackScanTarget(selfController.CenterPos, atkReferenceDir.normalized, trail.Distance_max, ifAttackUp, out atkDir);

            if (atkDir == Vector2.zero) InvokeActionPlanOverEvent();

            var combatHandler = AI.GetComponent<CombatHandler>();
            combatHandler.SetCombatHandler(Actor, attackCard, atkDir);
            combatHandler.StartHandleCombat(InvokeActionPlanOverEvent);


            //----------------随机下次攻击方向和专注方向------------
            ifAttackUp = (Random.Range(0, 100f) <= 50f);
            yield return new WaitForEndOfFrame();

            int focusX = (int)selfController.FaceDir.x;
            int focusY = (Random.Range(0, 100f) <= 50f)?1:-1;

            focusDir1 = new Vector2(focusX, focusY);

            yield return new WaitForEndOfFrame();

            int focusX2 = (Random.Range(0, 100f) <= 50f) ? 1 : -1; yield return new WaitForEndOfFrame();
            int focusY2 = (Random.Range(0, 100f) <= 50f) ? 1 : -1;
            if(focusX2 == focusX) { if(focusY2 == focusY) { focusX2 = -focusX2; } }

            focusDir2 = new Vector2(focusX2, focusY2);

        }

        #region 威胁度更新
        /// <summary>
        /// 更新威胁度字典
        /// </summary>
        private void UpdateHeatLevels()
        {
            heatLevelOfEnemies_dic.Clear();
            foreach(var go in BattleManager.instance.ActorQueue)
            {
                if(go.GetComponent<ActorController>().group.type != Actor.GetComponent<ActorController>().group.type)
                    heatLevelOfEnemies_dic.Add(go, UpdateHeatLevel(go));
            }
        }

        /// <summary>
        /// UpdateHeatLevels调用，计算单个对象的威胁度
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        private float UpdateHeatLevel(GameObject target)
        {
            float heatLevel = 0;

            float dis = Vector2.Distance(Actor.transform.position, target.transform.position);
            float healPoint_value = GetTargetHealPointValue(target);
            

            return heatLevel;
        }

        private bool CheckIfCanAttack(Node curNode)
        {
            GameObject selfGo = transform.parent.parent.gameObject;
            PathFinderComponent pathFinder = selfGo.GetComponent<PathFinderComponent>();
            var curWorldPos = pathFinder.CellToWorld((curNode.x, curNode.y));
            var targetPos = attackTarget.transform.position;

            // 射线检测进行判断,向目标位置进行一次60度锥角的扫描，命中目标说明可以攻击中。
            var curPoint = selfGo.GetComponent<ActorController>().CenterOffset + curWorldPos;
            var targetPoint = targetPos + attackTarget.GetComponent<ActorController>().CenterOffset;
            var dir = targetPoint - curPoint;
            Vector2 dir2D = ((Vector2)dir).normalized;

            AttackTrail trail = attackCard.CardAction as AttackTrail;

            Vector2 attackDir;
            return AttackScanTarget(curPoint, dir2D, trail.Distance_max, true, out attackDir);
        }

        private bool AttackScanTarget(Vector3 curPoint,Vector2 referenceDir,float distance,bool Up2Down, out Vector2 attackDir)
        {
            int scanDirValue = Up2Down ? 1 : -1;
            float startAngle = -25, endAngle = 25; int rayCount = 10;
            for (float a = startAngle; a <= endAngle; a += (endAngle - startAngle) / rayCount)
            {
                Quaternion rotation = Quaternion.Euler(0, 0, scanDirValue * a);
                Vector3 dir3D = (Vector3)referenceDir;
                Vector3 rayDir = rotation * dir3D;
                var rayDir2D = ((Vector2)rayDir).normalized;

                // debug观察扫描线情况
                //var debugLineGo = GameObject.Instantiate(debugLine);
                //debugLineGo.GetComponent<DebugLineController>().DrawLine(curPoint, curPoint + rayDir * distance);

                var hits = Physics2D.RaycastAll(curPoint, rayDir2D, distance);

                foreach (var hit in hits)
                {
                    if (hit.collider == null) continue;
                    
                    if (hit.collider.transform.parent.gameObject == attackTarget)
                    {
                        attackDir = rayDir2D;
                        return true;
                    }

                    if (hit.collider.tag == "Obstacle")
                        break;
                }
            }
            attackDir = Vector2.zero;
            return false;
        }

        private float GetTargetHealPointValue(GameObject target)
        {
            float v = 0;
            var actor = target.GetComponent<ActorController>();
            float hp = 100f * actor.HealPoint/actor.HealPoint_Max;
            v += (100f - hp) * 0.1f;

            return v;
        }
        #endregion
    }
}
