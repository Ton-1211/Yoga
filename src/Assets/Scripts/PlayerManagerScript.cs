using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManagerScript : MonoBehaviour
{
    const float DefaultDamageMagnification = 1f;

    int obtainedDamage;
    int totalDamage;

    public int ObtainedDamage => obtainedDamage;

    public int TotalDamage => totalDamage;

    void Start()
    {
        obtainedDamage = 0;
        totalDamage = 0;
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
        if(obtainedDamage > 0)
        {
            totalDamage += obtainedDamage;
        }
        obtainedDamage = 0;
    }

    // ビームのダメージを取得
    public int GetBeamDamage(float damageMagnification = DefaultDamageMagnification)
    {
        return (int)(obtainedDamage * damageMagnification);
    }
    // ビームのダメージを取得（追加でダメージを加算する場合）
    public int GetBeamDamage(float damageMagnification, int addDamage)
    {
        return (int)(obtainedDamage * damageMagnification) + addDamage;
    }
}
