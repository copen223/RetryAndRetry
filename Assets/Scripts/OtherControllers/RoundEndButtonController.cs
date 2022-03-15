using BattleModule;
using BattleModule.BattleStates;
using UnityEngine;

namespace OtherControllers
{
    public class RoundEndButtonController : MonoBehaviour
    {
        private bool isUsable;

        void Start()
        {
            BattleManager.instance.AddEventObserver(BattleManager.BattleEvent.PlayerActionStart, OnPlayerTurnDrawOver);
            BattleManager.instance.AddEventObserver(BattleManager.BattleEvent.ComputerTurnStart, OnComputerTurnStart);
        }

        private void SetUsable(bool usable)
        {
            isUsable = usable;
        }
        public void TurnEnd()
        {
            if (isUsable)
            {
                isUsable = false;
                BattleManager.instance.GetComponent<BattleTurnAction>().OnTurnEnd();
            }
        
        }

        private void OnPlayerTurnDrawOver()
        {
            isUsable = true;
        }

        private void OnComputerTurnStart()
        {
            isUsable = true;
        }
    }
}
