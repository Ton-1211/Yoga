using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackBulletScript : MonoBehaviour
{
    [SerializeField] EnemyAttackManagerScript enemyAttackManager;
    [SerializeField] string playerTag;
    [SerializeField] int damage;
    [SerializeField] GameObject preditionPoint;
    [SerializeField] float minPreditionDistance;
    [SerializeField] float maxLifeTime = 10f;
    PlayerManagerScript playerManager;
    Rigidbody rb;
    PlayerParticleManager playerParticleManager;
    float timer;

    public int Damage => damage;
    void Start()
    {
        timer = 0f;
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

        timer += Time.deltaTime;
        if(timer > maxLifeTime)
        {
            Destroy(gameObject);
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
