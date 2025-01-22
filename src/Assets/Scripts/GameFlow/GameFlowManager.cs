using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class GameFlowManager : MonoBehaviour
{
    enum GameState
    {
        Opening = 0,
        SummonAttack = 1,
        PlayerAttack = 2,
        BossDefeat = 3,
        Result = 4
    }

    [SerializeField] GameObject inGameObject;
    [SerializeField] GameObject animationObject;
    [SerializeField] JSONReader jsonReader;
    [SerializeField] ScoreScript scoreScript;
    [Header("ボスの攻撃生成からプレイヤーの攻撃に移るまでの時間"), SerializeField] float playerAttackWaitTime = 10f;
    [Header("ボスの攻撃召喚ムービーのDirector"), SerializeField] PlayableDirector bossSummonDirector;
    [Header("プレイヤーの攻撃ムービーのDirectorたち"), SerializeField] List<PlayableDirector> playerAttackDirectors;
    [Header("ボスの撃破ムービーのDirector"), SerializeField] PlayableDirector winDirector;

    [SerializeField] GameState state = GameState.Opening;
    int phaseCounter;
    PlayerManagerScript playerManager;
    
    void Start()
    {
        //state = GameState.Opening;
        phaseCounter = 0;
        playerManager = FindObjectOfType<PlayerManagerScript>();
    }

    /// <summary>
    /// 次のゲーム状態へと移る、基本的にTimelineのシグナルで呼び出したい
    /// </summary>
    public void ChangeGameFlow()

    {
        switch(state)
        {
            case GameState.Opening:// 敵の攻撃召喚に移る
                state = GameState.SummonAttack;
                phaseCounter++;
                if(bossSummonDirector != null) bossSummonDirector.Play();
                break;

            case GameState.SummonAttack:// プレイヤーの攻撃に移る
                jsonReader.BossAttack();
                StartCoroutine(WaitAndRunPlayerAttack(playerAttackWaitTime)); // 一定時間後にプレイヤーの攻撃に映る
                break;

            case GameState.PlayerAttack:// 敵の攻撃召喚か撃破に移る
                state = jsonReader.RemainBossAttack != 0 ? GameState.Opening : GameState.BossDefeat;
                if(state == GameState.Opening)
                {
                    inGameObject.SetActive(true);
                    if (animationObject.activeSelf) animationObject.SetActive(false);
                }
                ChangeGameFlow();
                break;

            case GameState.BossDefeat: // ボスの撃破ムービー
                state = GameState.Result;// ムービーの終わりにリザルト表示するため
                if(winDirector != null) winDirector.Play();
                break;

            case GameState.Result:// リザルト画面
                break;
            default:
                break;
        }
    }

    IEnumerator WaitAndRunPlayerAttack(float waitTime)
    {
        float timer = 0f;
        while(timer < waitTime)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        state = GameState.PlayerAttack;
        inGameObject.SetActive(false);
        int attackMovieIndex = playerAttackDirectors.Count >= phaseCounter ? phaseCounter - 1 : playerAttackDirectors.Count - 1;
        if (playerAttackDirectors.Count != 0) playerAttackDirectors[attackMovieIndex].Play();
        scoreScript.AddScore(playerManager.GetBeamDamage());
    }
}
