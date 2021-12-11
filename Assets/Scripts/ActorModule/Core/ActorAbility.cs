using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorAbility : MonoBehaviour
{
    // DEBUG
    private void Update()
    {
        
    }
    //
    void Start()
    {
        UpdateValueByBase();
    }

    #region 能力属性列表
    public int BasicsAttack;  // 攻击力
    public int BasicsDodge;   // 闪避值
    public int BasicsHit;     // 命中值
    public int BasicsDefense; // 防御力
    public int BasicsHealPoint; // 生命值

    public ActorAbilityValue Attack;  // 攻击力
    public ActorAbilityValue Dodge;   // 闪避值
    public ActorAbilityValue Hit;     // 命中值
    public ActorAbilityValue Defense; // 防御力
    public ActorAbilityValue HealPoint;
    #endregion

    #region 能力加值与减值

    #endregion

    private void UpdateValueByBase()
    {
        Attack = ActorAbilityValue.InitOrUpdateBaseValue(Attack, BasicsAttack);
        Dodge = ActorAbilityValue.InitOrUpdateBaseValue(Dodge, BasicsDodge);
        Hit = ActorAbilityValue.InitOrUpdateBaseValue(Hit, BasicsHit);
        Defense = ActorAbilityValue.InitOrUpdateBaseValue(Defense, BasicsDefense);
        HealPoint = ActorAbilityValue.InitOrUpdateBaseValue(HealPoint, BasicsHealPoint);
    }

}

public class ActorAbilityValue
{
    /// <summary>
    /// 检查该值是否存在，如果不存在返回一个新值，否则修改。
    /// </summary>
    /// <param name="value"></param>
    /// <param name="baseValue"></param>
    /// <returns></returns>
    public static ActorAbilityValue InitOrUpdateBaseValue(ActorAbilityValue value,int baseValue)
    {
        if (value == null)
            return new ActorAbilityValue(baseValue);
        else
        {
            value.ChangeBaseValue(baseValue);
            return value;
        }
        
    }
    public ActorAbilityValue(int baseValue)
    {
        this.baseValue = baseValue;
        addValue = 0;
        multiValue = 1;
        min = 0;
        max = 9999;
    }
    public ActorAbilityValue(int baseValue,int min,int max)
    {
        this.baseValue = baseValue;
        addValue = 0;
        multiValue = 1;
        this.min = min;
        this.max = max;
    }

    int baseValue;  // 基础值
    int addValue;   // 加值
    float multiValue; // 乘值
    int min;
    int max;
    
    /// <summary>
    /// 改变该数值的加值
    /// </summary>
    /// <param name="value"></param>
    public void AddTheAddValue(int value)
    {
        addValue += value;
    }
    /// <summary>
    /// 改变该数值的乘值
    /// </summary>
    /// <param name="value"></param>
    public void AddTheMultiValue(float value)
    {
        multiValue += value;
        if (multiValue < 0)
            multiValue = 0;
    }

    public void ChangeBaseValue(int value)
    {
        baseValue = value;
    }
    

    /// <summary>
    /// 经过加值和乘值运算后的最终值
    /// </summary>
    public int FinalValue
    {
        get
        {
            float final = (baseValue + addValue) * multiValue;
            int finalInt = Mathf.RoundToInt(final);
            finalInt = Mathf.Clamp(finalInt, min, max);
            return finalInt;
        }
    }
}
