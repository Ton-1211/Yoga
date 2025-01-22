using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using Unity.VisualScripting;

public class ScoreScript : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI resultScoreText;

    int score;
    int previousValue;
    bool isCountUp;
    bool isResult;
    string number;
    DG.Tweening.Sequence sequence;

    public TextMeshProUGUI ScoreText => scoreText;
    // Start is called before the first frame update
    void Start()
    {
        isResult = false;
        previousValue = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(isCountUp)
        {
            number = previousValue.ToString();
            if (!isResult)
            {
                ShowScoreText(scoreText, number);
            }
            else
            {
                if (!resultScoreText.gameObject.activeSelf) resultScoreText.gameObject.SetActive(true);
                ShowScoreText(resultScoreText, number);
            }
        }
        if(!isCountUp)
        {
            //Debug.Log("previousValue:" + previousValue);
            //Debug.Log("number:" + number);
            previousValue = 0;
            if (scoreText.enabled)
            {
                StartCoroutine(DisableText(3f));
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
        previousValue = score;
        score += point;
        if(isCountUp)
        {
            sequence.Kill(true);
        }
        CountUpAnim();
    }

    void CountUpAnim()
    {
        isCountUp = true;
        sequence = DOTween.Sequence().Append(DOTween.To(() => previousValue, num => previousValue = num, score, 0.5f))// アニメーションの設定
            // 少し待機してから
            .AppendInterval(0.1f)
            // スコア表示の更新を停止
            .AppendCallback(() => isCountUp = false);
    }

    void ShowScoreText(TextMeshProUGUI targetText, string numberString)
    {
        targetText.text = "";
        for (int i = 0; i < numberString.Length; i++)
        {
            targetText.text += "<sprite=" + numberString[i] + ">";
        }
    }

    IEnumerator DisableText(float disableTime)
    {
        yield return new WaitForSeconds(disableTime);
        scoreText.enabled = false;
    }
}
