using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.ActorModule
{
    /// <summary>
    /// 可交互对象接口，该对象能被交互
    /// </summary>
    interface IInteractiveObject
    {
        /// <summary>
        /// 受伤方法
        /// </summary>
        /// <param name="data"></param>
        void OnBehit(DamageData data);
    }
}
