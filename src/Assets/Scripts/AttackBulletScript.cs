using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackBulletScript : MonoBehaviour
{
    [SerializeField] EnemyAttackManagerScript enemyAttackManager;
    [SerializeField] LayerMask playerLayer;
    [SerializeField] int damage;
    PlayerManagerScript playerManager;

    void Start()
    {
        playerManager = FindObjectOfType<PlayerManagerScript>();
    }
    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == playerLayer)
        {
            // 消去処理（エフェクト発生等も記述）
            playerManager.ObtainAttack(damage);
            Destroy(this.gameObject);// オブジェクトの破壊
        }
    }
}
