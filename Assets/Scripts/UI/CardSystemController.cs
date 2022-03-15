using UnityEngine;

namespace UI
{
    public class CardSystemController : MonoBehaviour
    {
        public void SetAllButtonActive(bool ifAcive)
        {
            int childCount = transform.childCount;
            for(int i = 0; i<childCount;i++)
            {
                transform.GetChild(i).gameObject.SetActive(ifAcive);
            }
            transform.Find("SelectionWindow").gameObject.SetActive(false);
        }
    
        public void OnEnterBattle()
        {
            SetAllButtonActive(true);
        }

        private void Start()
        {
            SetAllButtonActive(false);
            GameManager.GameManager.instance.AddListener(GameManager.GameManager.GameEvent.EnterBattle, OnEnterBattle);
        }

    }
}
