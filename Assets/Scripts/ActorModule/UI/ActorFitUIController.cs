using ActorModule.Core;
using BufferModule;
using Tools;
using UnityEngine;

namespace ActorModule.UI
{
    public class ActorFitUIController : MonoBehaviour,ITargetInPool
    {
        // 设置参数

        // 链接
        public GameObject Target { set { RemoveAllSubject(); target = value; OnSetTarget(); } get { return target; } } //  目标
        private GameObject target;

        private Vector2 targetSize { get { return target.GetComponent<ActorController>().Sprite.GetComponent<Collider2D>().bounds.size; } }

        public GameObject HealPointUIObject;
        public GameObject FocusCountUIObject;
        public GameObject BuffsUIObjcet;

        /// <summary>
        /// 切换对象时调用，订阅事件
        /// </summary>
        private void OnSetTarget()
        {
            var actor = Target.GetComponent<ActorController>();
            actor.AddEventObserver(ActorController.ActorEvent.OnBehit, OnHealPointChanged);
            OnHealPointChanged(Target);

            actor.AddEventObserver(ActorController.ActorEvent.OnFoucusTrailChange, OnFoucsTrailChanged);
            OnFoucsTrailChanged(Target);

            actor.BuffCon.OnBuffChangeEvent += OnBuffChanged;
            OnBuffChanged(Target);

            UpdateChildPoisitionAndScale();

            actor.FocusCountUI_GO = FocusCountUIObject;

            actor.OnDeathEvent += OnActorDeathCallBack;
        }
        /// <summary>
        /// 移除委托调用
        /// </summary>
        private void RemoveAllSubject()
        {
            if (target != null)
            {
                Target.GetComponent<ActorController>().RemoveEventObserver(ActorController.ActorEvent.OnBehit, OnHealPointChanged);
                Target.GetComponent<ActorController>().RemoveEventObserver(ActorController.ActorEvent.OnFoucusTrailChange, OnFoucsTrailChanged);
                Target.GetComponent<ActorController>().BuffCon.OnBuffChangeEvent -= OnBuffChanged;
            }
        }
        public void OnReset()
        {

        }

        void Update()
        {
            UpdatePosition();   // 更新位置
        }

        void UpdatePosition()
        {
            var worldPos = Target.transform.position;
            var scrrenPos = Camera.main.WorldToScreenPoint(worldPos);
            transform.position = scrrenPos;
        }

        void UpdateChildPoisitionAndScale()
        {
            Vector3 targetScrrenSize = Camera.main.WorldToScreenPoint(targetSize) - Camera.main.WorldToScreenPoint(Vector3.zero);
            float sizeY = targetScrrenSize.y;
            float bottomY = sizeY * 3 / 8;
            float upperY = sizeY * 4 / 8;
            HealPointUIObject.transform.localPosition = new Vector3(0, -bottomY, 0);
            BuffsUIObjcet.transform.localPosition = new Vector3(0, -bottomY - 25, 0);
            FocusCountUIObject.transform.localPosition = new Vector3(0, upperY, 0);
        }

        // 血量变化时改变血条显示
        public void OnHealPointChanged(GameObject gb)
        {
            var con = HealPointUIObject.GetComponent<ActorFitHealPointUI>();
            con.OnHealPointChanged(gb);
        }

        public void OnFoucsTrailChanged(GameObject gb)
        {
            var con = transform.Find("ActorFitFocusTrailCount").GetComponent<ActorFitFocusTrailUI>();
            con.OnFocusTrailCountChanged(gb);
        }

        public void OnBuffChanged(GameObject gb)
        {
            var con = BuffsUIObjcet.GetComponent<BuffUIController>();
            con.UpdateBuffUIs(gb.transform.Find("Buffs").GetComponent<BuffController>());
        }

        public void ActiveUI(GameObject gb)
        {

        }

        // 回调函数
        private void OnActorDeathCallBack(GameObject actorGO)
        {
            actorGO.GetComponent<ActorController>().OnDeathEvent -= OnActorDeathCallBack;
            gameObject.SetActive(false);    //  设置为false 等待对象池调用
        }

    }
}
