using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;

public class AttackBulletScript : MonoBehaviour
{
    [SerializeField] EnemyAttackManagerScript enemyAttackManager;
    [SerializeField] string playerTag;
    [SerializeField] int damage;
    PlayerManagerScript playerManager;
    Rigidbody rb;

    void Start()
    {
        playerManager = FindObjectOfType<PlayerManagerScript>();
        rb = GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * 2, ForceMode.Impulse);
    }
    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag(playerTag))
        {
            // 消去処理（エフェクト発生等も記述）
            playerManager.ObtainAttack(damage);
            Destroy(this.gameObject);// オブジェクトの破壊
        }
    }
}
