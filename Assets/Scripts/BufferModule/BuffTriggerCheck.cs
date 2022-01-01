using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.BufferModule
{
    /// <summary>
    /// 提供buff触发条件检测函数的接口，一般用于提供buffscriptableObject
    /// </summary>
    public abstract class BuffTriggerCheck: ScriptableObject
    {
       public abstract bool CheckIfCanTouchOff(BuffTouchOffEventArgs eventArgs);

    }
}
