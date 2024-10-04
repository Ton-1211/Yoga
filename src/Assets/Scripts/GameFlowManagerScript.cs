using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameFlowManagerScript : MonoBehaviour
{
    enum GameState
    {
        Start = 0,
        Drain = 1,
        BossAttack = 2,
        PlayerAttack = 3
    }

    [SerializeField] Animator bossAnimator;
    [SerializeField] GameObject enemyAttackPrefab;
    [Header("敵の攻撃を発生させる場所"), SerializeField] List<Transform> attackSpawnPoints;

    [SerializeField]GameState gameState;
    PlayerManagerScript playerManager;
    ScoreScript scoreScript;
    void Start()
    {
        playerManager = FindObjectOfType<PlayerManagerScript>();
        scoreScript = GetComponent<ScoreScript>();
    }

    void Update()
    {
        
    }

    private void OnValidate()
    {
        ChangeGameState(gameState);
    }

    void ChangeGameState(GameState state)
    {
        gameState = state;
        switch(state)
        {
            case GameState.Start:
                break;
            case GameState.Drain:
                break;
            case GameState.BossAttack:
                StartCoroutine(SummonBossAttack());
                break;
            case GameState.PlayerAttack:
                StartCoroutine(DamageBoss());
                break;
            default:
                break;
        }
    }

    IEnumerator SummonBossAttack()// アニメーションを再生するフラグを時間差で切るためにコルーチンにした
    {
        bossAnimator.SetBool("EnemyAttack", true);
        foreach (Transform attackSpawnPoint in attackSpawnPoints)
        {
            GameObject attack = Instantiate(enemyAttackPrefab, attackSpawnPoint.position, Quaternion.identity);
        }
        yield return new WaitForSeconds(0.5f);
        bossAnimator.SetBool("EnemyAttack", false);
        gameState = GameState.Drain;
    }
    
    IEnumerator DamageBoss()
    {
        bossAnimator.SetBool("Diagonal", true);
        yield return new WaitForSeconds(1f);
        
        int damage = playerManager.GetBeamDamage();
        scoreScript.AddScore(damage);

        yield return null;
    }
}
