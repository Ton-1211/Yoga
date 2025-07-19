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

    [MenuItem("Tools/攻撃エディター")]
    static void OpenWindow()
    {
        GetWindow<BossAttackJSONEditor>("攻撃エディター");
    }

    int index = 0;// Listの何番目の要素か
    List<Attack> attackGroups = new List<Attack>();// ひとかたまりの攻撃のList
    string fileName;// 保存する名前

    float elementMaxWidth = 480f;// 要素の幅の最大値

    /* OnGUIは毎フレーム呼ばれるため念の為ここで宣言している */
    float x = 0f;
    float y = 0f;
    float maxWaitSeconds = 10f;
    float maxSeconds = 10f;
    float maxInterval = 10f;
    void OnGUI()
    {
        GUIStyle descriptionStyle = new GUIStyle(EditorStyles.label);// 他のラベルに影響させないためコピーしている
        descriptionStyle.fontSize = 16;
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
                        attackGroups[index].positions.Add(new Vector2(0, 0));
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
    /// JSONファイルへの書き出しを行う
    /// </summary>
    void ExportJson()
    {
        string filePath = Application.dataPath + "/StreamingAssets/BossAttacks/" + fileName + ".json";// 書き出す場所とファイル名の設定
        if(System.IO.File.Exists(filePath))// 同じ名前のファイルが存在していないか
        {
            //Debug.LogWarning("ファイル名が同じです！上書きします...");
            if(!EditorUtility.DisplayDialog("同名のファイルが既にあります！", "上書き保存しますか？\n" + "（不安なら「キャンセル」してファイルをコピーしておいてください）","はい", "キャンセル"))// 「はい」を押したときのみ上書き保存
            {
                return;
            }
            else Debug.Log("上書き保存します...");
        }

        //AttackSet attackSet = new AttackSet();
        //float totalSeconds = 0f;
        //List<AttackData> attacks = new List<AttackData>();
        //for (int i = 0; i < attackGroups.Count; i++)
        //{
        //    totalSeconds += attackGroups[i].waitSeconds;// 待機時間
        //    int judgeNum = (int)(attackGroups[i].seconds / attackGroups[i].judgeInterval);// 判定オブジェクトの個数
        //    for (int j = 0; j < judgeNum; j++)
        //    {
        //        for (int k = 0; k < attackGroups[i].positions.Count; k++)// 攻撃箇所の個数（左右の手足で設定しているぶん）
        //        {
        //            attacks.Add(new AttackData(totalSeconds + j * attackGroups[i].judgeInterval, attackGroups[i].positions[k]));
        //        }
        //    }
        //    totalSeconds += attackGroups[i].seconds;
        //}

        //attackSet.AttackDatas = attacks;// 書き出すクラスに反映
        AttackListWrap attackList = new AttackListWrap();
        attackList.attacks = attackGroups;

        using (FileStream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
        {
            using (StreamWriter streamWriter = new StreamWriter(fileStream))
            {
                //streamWriter.WriteLine(JsonUtility.ToJson(attackSet));
                streamWriter.WriteLine(JsonUtility.ToJson(attackList));
                streamWriter.Flush();
                streamWriter.Close();
            }
            Debug.Log("書き出し完了! 書き出されたファイルの場所は:" + filePath + "です!");
            fileStream.Close();
        }
    }

    void LoadJson()
    {
        string path = EditorUtility.OpenFilePanel("JSONファイルを選択", "Assets/StreamingAssets/BossAttacks", "json");
        if(string.IsNullOrEmpty(path))// ファイルを選択しなかったり、Nullのとき
        {
            return;
        }
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
            if (attack.positions[i] == attackData.position)
            {
                return true;
            }
        }
        return false;
    }
#endif
}
