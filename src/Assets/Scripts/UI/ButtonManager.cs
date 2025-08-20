using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class ButtonManager : MonoBehaviour
{
    // メインシーンへと切り替える
    public void SwitchToPrototype()
    {
        SceneManager.LoadScene("Prototype");// 切り替え
    }

    // オープニングシーンへと切り替える
    public void SwitchToOpeningScene()
    {
        SceneManager.LoadScene("opaning");// 切り替え
    }

    // タイトルシーンへと切り替える
    public void BackToTitle()
    {
        SceneManager.LoadScene("Title");// 切り替え
    }

    // ゲーム終了ボタンを押されたときの処理
    public void EndProgram()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;// エディター上での再生停止
#endif
        Application.Quit();// アプリの終了
    }
}
