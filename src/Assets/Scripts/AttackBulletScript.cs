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
    }
    void FixedUpdate()
    {
        // 到着予測地点の表示
        Ray ray = new Ray(transform.position, -transform.forward);
        RaycastHit hit;

        // 到着予測地点の表示地点から一定以上近づいたら
        if (Physics.Raycast(ray, out hit, minPreditionDistance, LayerMask.GetMask("PreditionPanel")))
        {
            if (!preditionPoint.activeSelf)
            {
                preditionPoint.SetActive(true);// 予測表示を有効化
            }
            preditionPoint.transform.position = hit.point;// 予測地点を表示
        }
        // 近い攻撃がなくなったら表示を消す
        else if (preditionPoint.activeSelf)
        {
            preditionPoint.SetActive(false);
        }

        // 一定時間を超えたらオブジェクトを消去
        timer += Time.deltaTime;
        if(timer > maxLifeTime)
        {
            Destroy(gameObject);
        }
    }
    void OnTriggerEnter(Collider other)
    {
        // 攻撃が吸収されたとき
        if (other.gameObject.CompareTag(playerTag))
        {
            // 消去処理（エフェクト発生等も記述）
            playerManager.ObtainAttack(damage);// 吸収ダメージを加算
            playerParticleManager.PlayParticle(transform.position);// 吸収エフェクトを再生
            Destroy(this.gameObject);// オブジェクトの破壊
        }
    }
}
