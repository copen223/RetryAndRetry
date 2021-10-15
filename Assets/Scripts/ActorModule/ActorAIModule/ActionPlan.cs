using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActorModule.AI
{
    // 行动方案父类，具体的行动方案都派生自该类
    public class ActionPlan : MonoBehaviour
    {
        protected Dictionary<GameObject, int> heatLevelOfEnemies_list;  // 威胁度字典

    }
}
