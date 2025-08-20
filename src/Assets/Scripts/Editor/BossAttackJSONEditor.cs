using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEditor.UIElements;
using System;

public class BossAttackJSONEditor : EditorWindow
{
#if UNITY_EDITOR

    const float ElementMaxWidth = 480f;
    const float DefaultX = 0f;
    const float DefaultY = 0f;
    const float DefaultMaxWaitSeconds = 10f;
    const float DefaultMaxSeconds = 0f;
    const float DefaultMaxInterval = 0f;

    const int FontSize = 16;
    static readonly Vector2 DefaultPosition = (0, 0);

    [MenuItem("Tools/攻撃エディター")]
    static void OpenWindow()
    {
        GetWindow<BossAttackJSONEditor>("攻撃エディター");
    }

    int index = 0;// Listの何番目の要素か
    List<Attack> attackGroups = new List<Attack>();// ひとかたまりの攻撃のList
    string fileName;// 保存する名前

    float elementMaxWidth = ElementMaxWidth;// 要素の幅の最大値

    /* OnGUIは毎フレーム呼ばれるため念の為ここで宣言している */
    float x = DefaultX;
    float y = DefaultY;
    float maxWaitSeconds = DefaultMaxWaitSeconds;
    float maxSeconds = DefaultMaxSeconds;
    float maxInterval = DefaultMaxInterval;
    void OnGUI()
    {
        GUIStyle descriptionStyle = new GUIStyle(EditorStyles.label);// 他のラベルに影響させないためコピーしている
        descriptionStyle.fontSize = FontSize;
        descriptionStyle.fontStyle = FontStyle.Bold;
        using (new EditorGUILayout.VerticalScope())
        {
            EditorGUILayout.LabelField("ボスの攻撃を作成します", descriptionStyle);
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("現在編集中:" + fileName);

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("新しく攻撃のポーズを追加"))
                {
                    attackGroups.Add(new Attack());
                }
                if(GUILayout.Button("JSONファイルを編集"))
                {
                    LoadJson();
                }
                
            }

            if (attackGroups.Count > 0)
            {
                index = EditorGUILayout.IntSlider("インデックス", index, 0, attackGroups.Count - 1, GUILayout.MaxWidth(elementMaxWidth));
                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.LabelField((index + 1) + "番目のポーズ");
                    if (GUILayout.Button("削除"))
                    {
                        attackGroups.RemoveAt(index);
                    }
                    using(var change = new EditorGUI.ChangeCheckScope())
                    { 
                    }
                }
                using (new GUILayout.VerticalScope("Box"))
                {
                    if (GUILayout.Button("場所を追加"))
                    {
                        attackGroups[index].positions.Add(DefaultPosition);
                    }

                    if (attackGroups[index].positions.Count > 0)
                    {
                        for (int i = 0; i < attackGroups[index].positions.Count; i++)
                        {
                            using (new EditorGUILayout.HorizontalScope())
                            {
                                EditorGUILayout.LabelField((i + 1) + "番目");
                                if (GUILayout.Button("削除"))
                                {
                                    attackGroups[index].positions.RemoveAt(i);
                                }
                            }
                            using (new EditorGUI.IndentLevelScope(1))// 字下げ
                            {
                                x = attackGroups[index].positions[i].x;
                                y = attackGroups[index].positions[i].y;

                                using (var change = new EditorGUI.ChangeCheckScope())
                                {
                                    x = EditorGUILayout.FloatField("x座標", x, GUILayout.MaxWidth(elementMaxWidth));
                                    y = EditorGUILayout.FloatField("y座標", y, GUILayout.MaxWidth(elementMaxWidth));
                                    if (change.changed)// 変更があったとき
                                    {
                                        attackGroups[index].positions[i] = new Vector2(x, y);// Listの要素を直接変更することが難しかったため後から保存している
                                    }
                                }
                            }
                        }
                    }

                    attackGroups[index].waitSeconds = EditorGUILayout.Slider("待機時間", attackGroups[index].waitSeconds, 0f, maxWaitSeconds, GUILayout.MaxWidth(elementMaxWidth));
                    attackGroups[index].seconds = EditorGUILayout.Slider("継続時間", attackGroups[index].seconds, 0f, maxSeconds, GUILayout.MaxWidth(elementMaxWidth));
                    attackGroups[index].judgeInterval = EditorGUILayout.Slider("判定オブジェクトの間隔", attackGroups[index].judgeInterval,
                        0.1f, maxInterval, GUILayout.MaxWidth(elementMaxWidth));
                }
            }
        }

        fileName = EditorGUILayout.TextField("保存名", fileName, GUILayout.MaxWidth(elementMaxWidth));

        if (GUILayout.Button("JSONファイルに書き出し"))
        {
            ExportJson();
        }
    }

    /// <summary>
    /// Jsonファイルへの書き出しを行う
    /// </summary>
    void ExportJson()
    {
        string filePath = Application.dataPath + "/StreamingAssets/BossAttacks/" + fileName + ".json";// 書き出す場所とファイル名の設定
        if(System.IO.File.Exists(filePath))// 同じ名前のファイルが存在していないか
        {
            if(!EditorUtility.DisplayDialog("同名のファイルが既にあります！", "上書き保存しますか？\n" + "（不安なら「キャンセル」してファイルをコピーしておいてください）","はい", "キャンセル"))// 「はい」を押したときのみ上書き保存
            {
                return;
            }
            else Debug.Log("上書き保存します...");
        }

        AttackListWrap attackList = new AttackListWrap();
        attackList.attacks = attackGroups;

        // Jsonファイルへの書き出し部分
        using (FileStream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
        {
            using (StreamWriter streamWriter = new StreamWriter(fileStream))
            {
                streamWriter.WriteLine(JsonUtility.ToJson(attackList));// Json形式に変換、書き込み
                streamWriter.Flush();
                streamWriter.Close();
            }
            Debug.Log("書き出し完了! 書き出されたファイルの場所は:" + filePath + "です!");
            fileStream.Close();
        }
    }

    /// <summary>
    /// Jsonファイルを読み込む
    /// </summary>
    void LoadJson()
    {
        string path = EditorUtility.OpenFilePanel("JSONファイルを選択", "Assets/StreamingAssets/BossAttacks", "json");
        if(string.IsNullOrEmpty(path))// ファイルを選択しなかったり、Nullのとき
        {
            return;// 何も行わない
        }

        // 読み込んだJsonファイルから設定を反映
        attackGroups = JSONReader.LoadAttackJson(path);
        fileName = Path.GetFileNameWithoutExtension(path);
    }

    /// <summary>
    /// 攻撃の場所に含まれているか調べる
    /// </summary>
    bool CheckAttackPositionIncluded(Attack attack, AttackData attackData)
    {
        for(int i = 0; i < attack.positions.Count; i++)
        {
            // 含まれていたとき
            if (attack.positions[i] == attackData.position)
            {
                return true;
            }
        }

        // 含まれていないとき
        return false;
    }
#endif
}
