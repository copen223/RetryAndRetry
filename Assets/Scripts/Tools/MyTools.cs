using UnityEngine;

namespace Tools
{
    public class MyTools
    {
        /// <summary>
        /// 检测该目标是否处于可被攻击层
        /// </summary>
        /// <param name="layer"></param>
        /// <returns></returns>
        public static bool CheckLayerIfCanAttack(int layer)
        {
            int bitmask = 1 << layer;
            int layerCheck1 = LayerMask.GetMask("Actor");
            int layerCheck2 = LayerMask.GetMask("EnvirObject");
            int layerCheck = layerCheck1 | layerCheck2;
            int layerCheckEnd = bitmask & layerCheck;
            return 0 != (bitmask & layerCheckEnd);
        }
    }
}
