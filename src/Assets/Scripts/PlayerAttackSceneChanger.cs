using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerAttackSceneChanger : MonoBehaviour
{
    // プレイヤーの攻撃のシーンに無理やり遷移させている
    // できる限り早くTimeLineで表示できるようにし、このスクリプトを削除する

    [SerializeField] int SceneNum;

    public void SceneChange()
    {
        SceneManager.LoadScene(SceneNum);
    }
}
