using System.Collections.Generic;
using ActorModule.UI;
using Prefabs.UI;
using Tools.NewTools;
using UI.ActionTip;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    public class UIManager : MonoBehaviour
    {
        //------------链接----------------
        public PlayerResourceUI UI_PlayerResource;

        public ActionTipsUI UI_ActionTips;


        /// <summary>
        /// 是否允许UI交互,UI响应时要读取该变量进行判断
        /// </summary>
        public bool IfActiveUIInteraction = true;

        /// <summary>
        /// 鼠标是否位于UI对象上
        /// </summary>
        public bool IfMouseOnUI
        {
            get 
            {
                PointerEventData eventdata = new PointerEventData(EventSystem.current)
                {
                    position = new Vector2(Input.mousePosition.x, Input.mousePosition.y)
                };

                var results = new List<RaycastResult>();

                EventSystem.current.RaycastAll(eventdata, results);

                return results.Count > 0;
            }
        }

        /// <summary>
        /// 鼠标位于的UI对象
        /// </summary>
        /// <returns></returns>
        public List<RaycastResult> UIsHitByMouse()
        {
            PointerEventData eventdata = new PointerEventData(EventSystem.current)
            {
                position = new Vector2(Input.mousePosition.x, Input.mousePosition.y)
            };

            var results = new List<RaycastResult>();

            EventSystem.current.RaycastAll(eventdata, results);

            return results;
        }

        // 漂浮UI
        TargetPool floatUIPool;
        public GameObject floatUIPrefab;
        public Transform floatUIParent;

        // 其他链接
        public ActorUIController ActorUI { get { return transform.Find("Actor").GetComponent<ActorUIController>(); } }
        public BattleMessagesConsoler MessagesConsoler { get { return transform.Find("BattleMessages").GetComponent<BattleMessagesConsoler>(); } }

        public static UIManager instance;
        // Start is called before the first frame update
        void Awake()
        {
            if (instance == null)
                instance = this;
            else
                Destroy(this);
        }

        private void Start()
        {
            floatUIPool = new TargetPool(floatUIPrefab, floatUIParent);
        }


        #region 外部调用函数
        /// <summary>
        /// 在世界坐标的位置创建一个漂浮UI，向dir方向移动，time后消失。
        /// </summary>
        /// <param name="worldPos"></param>
        /// <param name="dir"></param>
        /// <param name="time"></param>
        public GameObject CreatFloatUIAt(Vector3 worldPos,Vector2 dir,float time, Color color, string text)
        {
            var gb = floatUIPool.GetInstance();
            var screenPos = Camera.main.WorldToScreenPoint(worldPos);
            gb.transform.position = screenPos;
            gb.transform.GetComponent<FloatUIController>().BaseWorldPos = worldPos;
            gb.transform.GetComponent<FloatUIController>().text.color = color;
            gb.transform.GetComponent<FloatUIController>().text.text = text;
            gb.GetComponent<FloatUIController>().StartMoveToDir(dir, time);

            return gb;
        }

        public GameObject CreatFloatUIAt(GameObject target, Vector2 dir, float time, Color color, string text)
        {
            var gb = floatUIPool.GetInstance();
            var screenPos = Camera.main.WorldToScreenPoint(target.transform.position);
            gb.transform.position = screenPos;
            gb.transform.GetComponent<FloatUIController>().text.color = color;
            gb.transform.GetComponent<FloatUIController>().text.text = text;
            gb.GetComponent<FloatUIController>().StartMoveToDir(target, dir, time);

            return gb;
        }
        #endregion

    }
}
