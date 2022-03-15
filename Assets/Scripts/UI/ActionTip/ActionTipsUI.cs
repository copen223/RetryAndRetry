using System;
using System.Collections.Generic;
using UnityEngine;

namespace UI.ActionTip
{
    public class ActionTipsUI : MonoBehaviour
    {
        [SerializeField] private ActionTipUI LeftMouseTip = null;
        [SerializeField] private ActionTipUI RightMouseTip = null;

        private List<ActionTipUI> activeActionTips = new List<ActionTipUI>();

        private void Start()
        {
            activeActionTips = new List<ActionTipUI>() {LeftMouseTip, RightMouseTip};
            SetAllActionTipsActive(false);
        }

        public void SetActionTip(ActionTipType type,string tipText,bool ifActive)
        {
            ActionTipUI selectedTip = null;
            switch (type)
            {
                case ActionTipType.Left: selectedTip = LeftMouseTip;
                    break;
                case ActionTipType.Right: selectedTip = RightMouseTip;
                    break;
            }

            if (selectedTip == null) return;
            
            selectedTip.Init(tipText);
            selectedTip.SetActive(ifActive);
        }

        public void SetAllActionTipsActive(bool ifAcive)
        {
            foreach (var actionTip in activeActionTips)
            {
                actionTip.SetActive(ifAcive);
            }
        }

    }
    
    public enum ActionTipType
    {
        Left,
        Right
    }
}
