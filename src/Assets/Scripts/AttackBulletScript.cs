using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;

public class AttackBulletScript : MonoBehaviour
{
    [SerializeField] EnemyAttackManagerScript enemyAttackManager;
    [SerializeField] string playerTag;
    [SerializeField] int damage;
    [SerializeField] GameObject preditionPoint;
    [SerializeField] float minPreditionDistance;
    PlayerManagerScript playerManager;
    Rigidbody rb;
    PlayerParticleManager playerParticleManager;

    void Start()
    {
        playerManager = FindObjectOfType<PlayerManagerScript>();
        playerParticleManager = FindObjectOfType<PlayerParticleManager>();
        //rb = GetComponent<Rigidbody>();
        //rb.AddForce(transform.forward * 2, ForceMode.Impulse);
    }
    void FixedUpdate()
    {
        Ray ray = new Ray(transform.position, -transform.forward);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, minPreditionDistance, LayerMask.GetMask("PreditionPanel")))
        {
            if(!preditionPoint.activeSelf)
            {
                preditionPoint.SetActive(true);
            }
            preditionPoint.transform.position = hit.point;
        }
        else if(preditionPoint.activeSelf)
        {
            preditionPoint.SetActive(false);
        }
    }
    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag(playerTag))
        {
            // 消去処理（エフェクト発生等も記述）
            playerManager.ObtainAttack(damage);
            playerParticleManager.PlayParticle(transform.position);
            Destroy(this.gameObject);// オブジェクトの破壊
        }
    }
}
