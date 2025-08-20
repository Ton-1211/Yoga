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
        phaseCounter = 0;
        playerManager = FindObjectOfType<PlayerManagerScript>();
        inGameUI.SetActive(false);
    }

    /// <summary>
    /// 次のゲーム状態へと移る、基本的にTimelineのシグナルから呼び出す
    /// </summary>
    public void ChangeGameFlow()
    {
        switch(state)
        {
            case GameState.Opening:// 敵の攻撃召喚に移る
                // ゲームの状態を変更
                state = GameState.SummonAttack;
                phaseCounter++;

                // 見本ポーズの表示
                if (!poseImage.gameObject.activeSelf) poseImage.gameObject.SetActive(true);
                if(!inGameUI.activeSelf) inGameUI.SetActive(true);
                poseImage.sprite = jsonReader.PoseImage;// 見本のポーズの画像を指定

                //ボスの攻撃召喚を開始
                if (bossSummonDirector != null) bossSummonDirector.Play();// Timelineを再生
                break;

            case GameState.SummonAttack:// 攻撃を生成した後、プレイヤーの攻撃に移る
                jsonReader.BossAttack();// ボスの攻撃を生成
                StartCoroutine(WaitAndRunPlayerAttack(playerAttackWaitTime)); // 一定時間後にプレイヤーの攻撃に映る
                break;

            case GameState.PlayerAttack:// 敵の攻撃召喚か撃破に移る
                foreach(PlayableDirector director in playerAttackDirectors)
                {
                    if(director.state == PlayState.Playing || director.state == PlayState.Paused)
                    {
                        director.Stop();// 再生を止める
                    }
                }

                // ボスの攻撃の残り状況から、次の状態を指定（ボスの攻撃召喚フェーズか、ゲーム終了フェーズかを指定する）
                state = jsonReader.RemainBossAttack != 0 ? GameState.Opening : GameState.BossDefeat;
                if(state == GameState.Opening)
                {
                    inGameObject.SetActive(true);
                    if (animationObject.activeSelf) animationObject.SetActive(false);
                }
                ChangeGameFlow();// 次のフェーズに移行する
                break;

            case GameState.BossDefeat: // ボスの撃破ムービー
                state = GameState.Result;// ムービーの終わりにリザルト表示するため
                if(winDirector != null) winDirector.Play();// Timelineを再生
                break;

            case GameState.Result:// リザルト画面
                scoreScript.AddScore(playerManager.TotalDamage, true);// スコア表示
                //scoreScript.AddScore(250, true);// デバッグで使用した
                float acculate = jsonReader.PossibleDamage != 0 ? (playerManager.TotalDamage / jsonReader.PossibleDamage) * 100f : 100f;// 精度
                // 精度によって表示するランクを変更
                if(acculate >= 90)
                {
                    rank.rankImage.sprite = rank.s;// Sランク表示
                }
                else if(acculate >= 70)
                {
                    rank.rankImage.sprite = rank.a;// Aランク表示
                }
                else
                {
                    rank.rankImage.sprite = rank.b;// Bランク表示
                }
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 指定された時間後にプレイヤーの攻撃フェーズへと移行
    /// </summary>
    /// <param name="waitTime">攻撃を吸収できる時間</param>
    /// <returns>処理が完了するまでのコルーチン</returns>
    IEnumerator WaitAndRunPlayerAttack(float waitTime)
    {
        // 吸収ターン
        float timer = 0f;
        while(timer < waitTime)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        // 吸収ターン終了、プレイヤーの攻撃フェーズへ
        state = GameState.PlayerAttack;// ゲームの状態を変更

        // ムービー再生用の設定
        inGameObject.SetActive(false);// ムービー再生時にゲーム内オブジェクトの一部を非表示
        if (poseImage.gameObject.activeSelf) poseImage.gameObject.SetActive(false);// 見本ポーズを非表示
        if (!animationObject.activeSelf) animationObject.SetActive(true);// ムービー用のオブジェクトを表示
        if (inGameUI.activeSelf) inGameUI.SetActive(false);// ゲーム内UIを非表示

        // 後処理
        int attackMovieIndex = playerAttackDirectors.Count >= phaseCounter ? phaseCounter - 1 : playerAttackDirectors.Count - 1;// 何回目の攻撃かを調べる
        if (playerAttackDirectors.Count != 0) playerAttackDirectors[attackMovieIndex].Play();// 回数に応じた攻撃ムービーを再生
        scoreScript.AddScore(playerManager.GetBeamDamage());// 直前の攻撃で吸収したダメージ量を表示
        playerManager.ResetObtainedDamage();// 一回の攻撃あたりの吸収量をリセット
    }
}
