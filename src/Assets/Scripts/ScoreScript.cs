using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using Unity.VisualScripting;

public class ScoreScript : MonoBehaviour
{
    const int InitPreviousValue = 0;
    const float DisableTextTime = 3f;
    const float ShowTime = 0.5f;
    const float AppendIntervalTime = 0.1f;
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI resultScoreText;
    [SerializeField] TextMeshProUGUI totalDamageText;

    int score;
    int previousValue;
    bool isCountUp;
    bool isResult;
    string number;
    DG.Tweening.Sequence sequence;

    public TextMeshProUGUI ScoreText => scoreText;
    
    void Start()
    {
        isResult = false;
        previousValue = InitPreviousValue;
    }

    void Update()
    {
        if(isCountUp)
        {
            number = previousValue.ToString();
            // リザルト以外のとき（ゲーム中）
            if (!isResult)
            {
                ShowScoreText(scoreText, number);// 吸収したダメージ量を表示
            }
            // リザルトのとき
            else
            {
                if (!resultScoreText.gameObject.activeSelf) resultScoreText.gameObject.SetActive(true);// スコアオブジェクトを表示

                // スコア表示
                ShowScoreText(resultScoreText, number);
                ShowScoreText(totalDamageText, number);
            }
        }
        if(!isCountUp)
        {
            previousValue = 0;
            if (scoreText.enabled)
            {
                StartCoroutine(DisableText(DisableTextTime));
            }
        }
    }

    public void AddScore(int point, bool resultMode = false)
    {
        isResult = resultMode;
        if(!resultMode && !scoreText.enabled)
        {
            scoreText.enabled = true;
        }
        else if(resultMode && !resultScoreText.enabled)
        {
            resultScoreText.enabled = true;
        }
        if(resultMode && !totalDamageText.enabled)
        {
            totalDamageText.enabled = true;
        }
        previousValue = score;
        score = point;
        if(isCountUp)
        {
            sequence.Kill(true);
        }
        CountUpAnim();
    }

    void CountUpAnim()
    {
        isCountUp = true;
        sequence = DOTween.Sequence().Append(DOTween.To(() => previousValue, num => previousValue = num, score, ShowTime))// アニメーションの設定
            // 少し待機してから
            .AppendInterval(AppendIntervalTime)
            // スコア表示の更新を停止
            .AppendCallback(() => isCountUp = false);
    }

    // スコアテキストを表示（フォントではなくスプライトの数字を使用）
    void ShowScoreText(TextMeshProUGUI targetText, string numberString)
    {
        targetText.text = "";
        for (int i = 0; i < numberString.Length; i++)
        {
            targetText.text += "<sprite=" + numberString[i] + ">";// スプライトの数字を用いたスコアの表示
        }
    }

    /// <summary>
    /// テキストを指定時間後に非表示にする
    /// </summary>
    /// <param name="disableTime">非表示にするまでの時間</param>
    /// <returns>処理完了までのコルーチン</returns>
    IEnumerator DisableText(float disableTime)
    {
        yield return new WaitForSeconds(disableTime);
        scoreText.enabled = false;
    }
}
