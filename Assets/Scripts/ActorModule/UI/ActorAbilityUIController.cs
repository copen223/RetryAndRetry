using ActorModule.Core;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ActorModule.UI
{
    public class ActorAbilityUIController : MonoBehaviour,IDragHandler,IBeginDragHandler
    {
        public Text Attack;
        public Text Dfence;
        public Text Hit;
        public Text Dodge;

        public ActorAbility ability;

        /// <summary>
        /// 更新该UI的数值显示
        /// </summary>
        /// <param name="actorAbility"></param>
        public void UpdateValueByActor(ActorAbility actorAbility)
        {
            ability = actorAbility;
            Attack.text = actorAbility.Attack.FinalValue.ToString();
            Dfence.text = actorAbility.Defense.FinalValue.ToString();
            Hit.text = actorAbility.Hit.FinalValue.ToString();
            Dodge.text = actorAbility.Dodge.FinalValue.ToString();
        }
        /// <summary>
        /// 关闭按钮调用
        /// </summary>
        public void OnCloseWindowCallBack()
        {
            gameObject.SetActive(false);
            OnCloseWindowEvent?.Invoke(this);
        }

        Vector3 offset;

        public void OnBeginDrag(PointerEventData eventData)
        {
            offset = transform.position - Input.mousePosition;
        }

        public void OnDrag(PointerEventData eventData)
        {
            transform.position = offset + Input.mousePosition;
        }

        public event System.Action<ActorAbilityUIController> OnCloseWindowEvent;
    }
}
