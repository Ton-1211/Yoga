using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class ScoreScript : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI scoreText;

    int score;
    int previousValue;
    bool isCountUp;
    Sequence sequence;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(isCountUp)
        {
            scoreText.SetText("{0:0}",previousValue);
        }
        if(!isCountUp && scoreText.enabled)
        {
            StartCoroutine(DisableText(3f));
        }
    }

    public void AddScore(int point)
    {
        if(!scoreText.enabled)
        {
            scoreText.enabled = true;
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

    IEnumerator DisableText(float disableTime)
    {
        yield return new WaitForSeconds(disableTime);
        scoreText.enabled = false;
    }
}
