using UnityEngine;

namespace OtherControllers
{
    public class EnterBattleButtonController : MonoBehaviour
    {
        // 点击响应
        public void EnterBattle()
        {
            GameManager.GameManager.instance.OnEnterBattle();
            gameObject.SetActive(false);
        }
    }
}
