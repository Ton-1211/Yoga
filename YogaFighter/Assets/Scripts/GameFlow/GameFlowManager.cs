using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

[Serializable]
public class Rank
{
    public Image rankImage;
    public Sprite s;
    public Sprite a;
    public Sprite b;
}
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
    [SerializeField] GameObject inGameUI;
    [SerializeField] JSONReader jsonReader;
    [SerializeField] ScoreScript scoreScript;
    [SerializeField] Image poseImage;
    [Header("ボスの攻撃生成からプレイヤーの攻撃に移るまでの時間"), SerializeField] float playerAttackWaitTime = 10f;
    [Header("ボスの攻撃召喚ムービーのDirector"), SerializeField] PlayableDirector bossSummonDirector;
    [Header("プレイヤーの攻撃ムービーのDirectorたち"), SerializeField] List<PlayableDirector> playerAttackDirectors;
    [Header("ボスの撃破ムービーのDirector"), SerializeField] PlayableDirector winDirector;
    [SerializeField] Rank rank;

    [SerializeField] GameState state = GameState.Opening;
    int phaseCounter;
    PlayerManagerScript playerManager;
    
    void Start()
    {
        //state = GameState.Opening;
        phaseCounter = 0;
        playerManager = FindObjectOfType<PlayerManagerScript>();
        inGameUI.SetActive(false);
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
                if (!poseImage.gameObject.activeSelf) poseImage.gameObject.SetActive(true);
                if(!inGameUI.activeSelf) inGameUI.SetActive(true);
                poseImage.sprite = jsonReader.PoseImage;// 見本のポーズの画像を指定

                if(bossSummonDirector != null) bossSummonDirector.Play();
                break;

            case GameState.SummonAttack:// 攻撃を生成した後、プレイヤーの攻撃に移る
                jsonReader.BossAttack();
                StartCoroutine(WaitAndRunPlayerAttack(playerAttackWaitTime)); // 一定時間後にプレイヤーの攻撃に映る
                break;

            case GameState.PlayerAttack:// 敵の攻撃召喚か撃破に移る
                foreach(PlayableDirector director in playerAttackDirectors)
                {
                    if(director.state == PlayState.Playing || director.state == PlayState.Paused)// 再生を止める
                    {
                        director.Stop();
                    }
                }
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
                scoreScript.AddScore(playerManager.TotalDamage, true);// スコア表示
                //scoreScript.AddScore(250, true);// デバッグで使用した
                float acculate = jsonReader.PossibleDamage != 0 ? (playerManager.TotalDamage / jsonReader.PossibleDamage) * 100f : 100f;// 精度
                // 精度によって表示するランクを変更
                if(acculate >= 90)
                {
                    rank.rankImage.sprite = rank.s;
                }
                else if(acculate >= 70)
                {
                    rank.rankImage.sprite = rank.a;
                }
                else
                {
                    rank.rankImage.sprite = rank.b;
                }
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
        if (poseImage.gameObject.activeSelf) poseImage.gameObject.SetActive(false);
        if (!animationObject.activeSelf) animationObject.SetActive(true);
        if (inGameUI.activeSelf) inGameUI.SetActive(false);

        int attackMovieIndex = playerAttackDirectors.Count >= phaseCounter ? phaseCounter - 1 : playerAttackDirectors.Count - 1;
        if (playerAttackDirectors.Count != 0) playerAttackDirectors[attackMovieIndex].Play();
        scoreScript.AddScore(playerManager.GetBeamDamage());
        playerManager.ResetObtainedDamage();
    }
}
