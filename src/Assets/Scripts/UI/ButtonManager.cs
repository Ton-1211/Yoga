using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class ButtonManager : MonoBehaviour
{
    // メインシーンに切り替える関数
    public void SwitchToPrototype()
    {
        SceneManager.LoadScene("Prototype"); // 切り替え先のシーン名
    }

    public void SwitchToOpeningScene()
    {
        SceneManager.LoadScene("opaning");
    }

    public void BackToTitle()
    {
        SceneManager.LoadScene("Title");
    }

    public void EndProgram()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;// エディター上での再生停止
#endif
        Application.Quit();
    }
}
