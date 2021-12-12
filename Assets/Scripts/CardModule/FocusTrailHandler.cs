using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Tools;
using Assets.Scripts.CardModule.CardActions;

namespace Assets.Scripts.CardModule
{
    public class FocusTrailHandler : MonoBehaviour
    {
        private TargetPool focusTrailPool;
        [SerializeField]
        private GameObject focusTrailPrefab = null;
        // Start is called before the first frame update

        private Card focusCard;
        private Vector2 focusWorldDir;
        private GameObject focuser;

        void Start()
        {
            focusTrailPool = new TargetPool(focusTrailPrefab);
        }

        public void SetFocusTrailHandler(GameObject _focuser, Card _focusCard,Vector2 _focusWorldDir)
        {
            focuser = _focuser; focusCard = _focusCard; focusWorldDir = _focusWorldDir;
        }

        public void StartHandleFocusTrailNoCallBack()
        {
            StartCoroutine(DoHandleFocusTrail());
        }

        public IEnumerator StartHandleFocusTrail()
        {
            return DoHandleFocusTrail();
        }

        private IEnumerator DoHandleFocusTrail()
        {
            //List<Vector3> points = new List<Vector3>();
            float focusTime = 0.25f;
         
            //-------------------每帧更新-------------------
            //points.Clear();
            focusTrailPool.ReSet();
            //-------------------计算位置和点集------------------

            FocusTrail trail = focusCard.CardAction as FocusTrail;
            trail.Actor = focuser.GetComponent<ActorController>();

            List<Vector3> focusTrailOffsetPoints = trail.GetLineOffsetPoints(focusWorldDir);
            List<Vector3> focusTrailWorldPoints = new List<Vector3>();
            foreach (var point in focusTrailOffsetPoints)
            {
                focusTrailWorldPoints.Add(point + focuser.GetComponent<ActorController>().Sprite.transform.position);
            }

            //-------------------显示-------------------------

            var gb = focusTrailPool.GetTarget(focuser.transform.Find("FocusTrails"));
            gb.transform.localScale = new Vector3(1, 1, 1);
            gb.GetComponent<FocusTrailController>().Seter = focuser;
            gb.GetComponent<FocusTrailController>().SetPoints(focusTrailWorldPoints);
            gb.GetComponent<FocusTrailController>().SetOffsetPoints();  // 清空offset点集 防止使用offsetpoint决定线
            gb.SetActive(true);

            yield return new WaitForSeconds(focusTime);

            //-------------------确定--------------------------
            Vector2 scale_x = focuser.transform.localScale;
            gb.GetComponent<FocusTrailController>().SetOffsetPoints(focusTrailOffsetPoints.ToArray());
            focusTrailPool.RemoveFromPool(gb);
            focuser.GetComponent<ActorController>().AddFocusTrail(gb);
            focusCard.SetFocusTrail(gb);

            focuser.GetComponent<ActorController>().ShowAllFocusTrail(false);
            focuser.GetComponent<ActorController>().ActiveAllFocusTrail(true);
        }
    }
}
