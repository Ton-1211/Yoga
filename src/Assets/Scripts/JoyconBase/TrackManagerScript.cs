using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/* https://www.mof-mof.co.jp/tech-blog/unity-joycon-introduceのスクリプトをもとにしている */

[Serializable]
class TrackPoint
{
    [Header("トラッキングで動かすオブジェクト"), SerializeField] Transform TrackTransform;
    public S_Joycon TrackJoycon { get; set; }

    public Transform GetTransform() { return TrackTransform; }

    public void AddPosition(Vector3 vector3)
    {
        TrackTransform.position += vector3;
    }
}
public class TrackManagerScript : MonoBehaviour
{
    [SerializeField] TrackPoint[] trackPoints;

    private static readonly S_Joycon.Button[] m_buttons =
        Enum.GetValues(typeof(S_Joycon.Button)) as S_Joycon.Button[];

    private List<S_Joycon> m_joycons;
    private S_Joycon m_joyconL;
    private S_Joycon m_joyconR;
    private S_Joycon.Button? m_pressedButtonL;
    private S_Joycon.Button? m_pressedButtonR;
    private void Start()
    {
        SetControllers();
        SetTrackPoints();
    }

    private void FixedUpdate()
    {
        foreach (TrackPoint trackPoint in trackPoints)
        {
            Vector3 accel = trackPoint.TrackJoycon.GetAccelWithoutGravity();
            //accel -= trackPoint.GetTransform().eulerAngles.normalized;
            Vector3 moveAmount = new Vector3(-accel.y, accel.z, -accel.x) * Time.deltaTime * Time.deltaTime * 9.8f * 300f;
            //Vector3 moveAmount = new Vector3(-trackPoint.TrackJoycon.GetAccel().y, -trackPoint.TrackJoycon.GetAccel().x, -trackPoint.TrackJoycon.GetAccel().z)
            //    * Time.deltaTime * Time.deltaTime * 1000f;// ジョイコンの加速度センサはxが縦、yが横方向だったり+-がUnityと逆だったりするのでUnityに揃えた
            //                                           // 倍率の数字は後で変数にする

            Quaternion orientation = trackPoint.TrackJoycon.GetVector();
            //orientation = new Quaternion(orientation.x, orientation.z, orientation.y, orientation.w);
            trackPoint.GetTransform().rotation = orientation;
            trackPoint.GetTransform().Rotate(90, 0, 0, Space.World);

            trackPoint.AddPosition(moveAmount);
        }
    }

    private void Update()
    {
        m_pressedButtonL = null;
        m_pressedButtonR = null;
        if (m_joycons == null || m_joycons.Count <= 0) return;
        SetControllers();
        SetTrackPoints();
        foreach (var button in m_buttons)
        {
            if (m_joyconL.GetButton(button))
            {
                m_pressedButtonL = button;
                if(m_pressedButtonL == S_Joycon.Button.DPAD_DOWN)
                {
                    ResetTrackPosition(m_joyconL);
                }
            }
            if (m_joyconR.GetButton(button))
            {
                m_pressedButtonR = button;
            }
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            m_joyconL.SetRumble(160, 320, 0.6f, 200);
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            m_joyconR.SetRumble(160, 320, 0.6f, 200);
        }
    }

    private void OnGUI()
    {
        var style = GUI.skin.GetStyle("label");
        style.fontSize = 24;
        if (m_joycons == null || m_joycons.Count <= 0)
        {
            GUILayout.Label("Joy-Con が接続されていません");
            return;
        }
        if (!m_joycons.Any(c => c.isLeft))
        {
            GUILayout.Label("Joy-Con (L) が接続されていません");
            return;
        }
        if (!m_joycons.Any(c => !c.isLeft))
        {
            GUILayout.Label("Joy-Con (R) が接続されていません");
            return;
        }
        GUILayout.BeginHorizontal(GUILayout.Width(960));
        foreach (var trackPoint in trackPoints)
        {
            var isLeft = trackPoint.TrackJoycon.isLeft;
            var name = isLeft ? "Joy-Con (L)" : "Joy-Con (R)";
            var key = isLeft ? "Z キー" : "X キー";
            var button = isLeft ? m_pressedButtonL : m_pressedButtonR;
            var stick = trackPoint.TrackJoycon.GetStick();
            var gyro = trackPoint.TrackJoycon.GetGyro();
            Vector3 gyroRaw = trackPoint.TrackJoycon.GetGyroRaw();
            var accel_NoG = trackPoint.TrackJoycon.GetAccelWithoutGravity();
            accel_NoG = new Vector3(-accel_NoG.y, -accel_NoG.z, -accel_NoG.x);
            var accel = trackPoint.TrackJoycon.GetAccel();
            accel = new Vector3(-accel.y, -accel.z, -accel.x);
            var orientation = trackPoint.TrackJoycon.GetVector();
            Vector3 angle = trackPoint.TrackJoycon.GetVector().eulerAngles;
            GUILayout.BeginVertical(GUILayout.Width(480));
            GUILayout.Label(name);
            GUILayout.Label(key + "：振動");
            GUILayout.Label("押されているボタン：" + button);
            GUILayout.Label(string.Format("スティック：({0}, {1})", stick[0], stick[1]));
            GUILayout.Label("ジャイロ：" + gyro);
            //GUILayout.Label("生ジャイロ:" + gyroRaw);
            //GUILayout.Label("加速度：" + accel);
            GUILayout.Label("加速度：" + accel);
            GUILayout.Label("補正後：" + accel_NoG);
            GUILayout.Label("傾き：" + orientation);
            GUILayout.Label("オイラー角" + angle);
            GUILayout.EndVertical();
        }
        GUILayout.EndHorizontal();
    }
    public void SetControllers()
    {
        m_joycons = S_JoyconManager.Instance.j;
        if (m_joycons == null || m_joycons.Count <= 0) return;
        m_joyconL = m_joycons.Find(c => c.isLeft);
        m_joyconR = m_joycons.Find(c => !c.isLeft);
    }

    void SetTrackPoints()
    {
        // トラッキングの設定量と接続されたジョイコンの数の小さい方を設定
        int loopLength = trackPoints.Length < m_joycons.Count ? trackPoints.Length : m_joycons.Count;

        for (int i = 0; i < loopLength; i++)
        {
            trackPoints[i].TrackJoycon = m_joycons[i];
        }
    }

    void ResetTrackPosition(S_Joycon joycon)
    {
        foreach(TrackPoint trackpoint in trackPoints)
        {
            if(trackpoint.TrackJoycon == joycon)
            {
                trackpoint.GetTransform().position = Vector3.zero;// 位置のリセット
                return;
            }
        }
    }
}