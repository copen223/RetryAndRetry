using System.Collections;
using Tools.NewTools;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class FloatUIController : MonoBehaviour
    {
        public Text text { get { return transform.Find("Text").GetComponent<Text>(); } }
        public float MoveSpeed = 2f;
        public Vector3 BaseWorldPos = Vector3.zero;
        public float StopTimeBiLi = 0.75f;
        private GameObject target;
        private Camera _camera;


        void Awake()
        {
            _camera = Camera.main;
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        public void StartMoveToDir(Vector3 dir,float time)
        {
            StartCoroutine(IEMoveAnDisappearAfterTime(dir, time, false));
        }
        public void StartMoveToDir(GameObject _target, Vector3 dir, float time)
        {
            target = _target;
            StartCoroutine(IEMoveAnDisappearAfterTime(dir, time, true));
        }

        IEnumerator IEMoveAnDisappearAfterTime(Vector3 dir, float time, bool ifChaseTarget)
        {
            float timer = 0;
            Vector3 stopWorldPos = Vector3.zero;
            while(timer <= time)
            {
                if (timer <= time * StopTimeBiLi)
                {
                    Vector3 baseWorldPos = ifChaseTarget ? target.transform.position : BaseWorldPos;
                    Vector3 newWorldPos = stopWorldPos = baseWorldPos + dir.normalized * MoveSpeed * timer;
                    //Debug.Log(newWorldPos);
                    Vector3 newScreenPos = _camera.WorldToScreenPoint(newWorldPos);
                    
                    transform.position = newScreenPos;
                }
                
                transform.position = _camera.WorldToScreenPoint(stopWorldPos);
                text.color = new Color(text.color.r, text.color.g, text.color.b, 1 * ((time - timer) / time));

                yield return new WaitForEndOfFrame();
                timer += Time.deltaTime;
            }

            GetComponent<TargetInPool>().InActive();
        }
    }
}
