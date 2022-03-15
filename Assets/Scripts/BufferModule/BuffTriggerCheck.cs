using UnityEngine;

namespace BufferModule
{
    /// <summary>
    /// 提供buff触发条件检测函数的接口，一般用于提供buffscriptableObject
    /// </summary>
    public abstract class BuffTriggerCheck: ScriptableObject
    {
       public abstract bool CheckIfCanTouchOff(BuffTouchOffEventArgs eventArgs);

    }
}
