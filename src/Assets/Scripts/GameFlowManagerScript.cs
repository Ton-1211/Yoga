using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// ===============================================================
// 古いバージョン、現在は「GameFlow->GameFlowManager.cs」になっている
// ===============================================================

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

    [SerializeField] GameState gameState;
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

    public void OnChangeGameState(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            switch (gameState)
            {
                case GameState.Start:
                    ChangeGameState(GameState.BossAttack);
                    break;
                case GameState.BossAttack:
                    ChangeGameState(GameState.Drain);
                    break;
                case GameState.Drain:
                    ChangeGameState(GameState.PlayerAttack);
                    break;
                case GameState.PlayerAttack:
                    bossAnimator.SetBool("Diagonal", false);
                    ChangeGameState(GameState.BossAttack);
                    break;
                default:
                    break;
            }
        }
    }

    void ChangeGameState(GameState state)
    {
        gameState = state;
        switch (state)
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
        yield return new WaitForSeconds(0.3f);
        foreach (Transform attackSpawnPoint in attackSpawnPoints)
        {
            GameObject attack = Instantiate(enemyAttackPrefab, attackSpawnPoint.position, attackSpawnPoint.rotation);
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

        yield return new WaitForSeconds(5f);

        bossAnimator.SetBool("Damaged", true);
    }
}
