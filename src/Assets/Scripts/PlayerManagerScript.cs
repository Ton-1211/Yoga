using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManagerScript : MonoBehaviour
{
    int obtainedDamage;

    public int ObtainedDamage
    {
        get { return obtainedDamage; }
    }

    void Start()
    {
        obtainedDamage = 0;
    }

    /// <summary>
    /// ダメージ吸収値を増加
    /// </summary>
    public void ObtainAttack(int attackDamage)
    {
        obtainedDamage += attackDamage;
    }

    /// <summary>
    /// ダメージ吸収値を0にする
    /// </summary>
    public void ResetObtainedDamage()
    {
        obtainedDamage = 0;
    }

    public int GetBeamDamage(float damageMagnification)
    {
        return (int)(obtainedDamage * damageMagnification);
    }
    public int GetBeamDamage(float damageMagnification, int addDamage)
    {
        return (int)(obtainedDamage * damageMagnification) + addDamage;
    }
}
