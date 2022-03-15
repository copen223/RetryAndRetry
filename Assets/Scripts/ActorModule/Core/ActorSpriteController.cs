using System;
using UnityEngine;

namespace ActorModule.Core
{
    public class ActorSpriteController : MonoBehaviour
    {
        public event Action MouseEnterEvent;
        public event Action MouseExitEvent;

        public bool ifMouseStayOnSprite = false;

        void Update()
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var hits = Physics2D.OverlapPointAll(mousePosition);
            foreach(var hit in hits)
            {
                if(hit == null)
                {
                    continue;
                }
                if(hit.gameObject == gameObject)
                {
                    if(!ifMouseStayOnSprite)
                    {
                        MouseEnterEvent?.Invoke();
                    }
                    ifMouseStayOnSprite = true;
                    return;
                }
            }

            if(ifMouseStayOnSprite)
            {
                MouseExitEvent?.Invoke();
            }

            ifMouseStayOnSprite = false;
        }



        //public void OnMouseExit()
        //{
        //    MouseExitEvent?.Invoke();
        //}

        //public void OnPointerEnter(PointerEventData eventData)
        //{
        //    MouseEnterEvent?.Invoke();
        //}

        //public void OnPointerExit(PointerEventData eventData)
        //{
        //    MouseExitEvent?.Invoke();
        //}

        //private void OnMouseEnter()
        //{
        //    MouseEnterEvent?.Invoke();
        //}
    }
}
