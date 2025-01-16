using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System;

[Serializable]
public class Attack// ひとかたまり（発生位置が変化しない）攻撃
{
    public List<Vector2> positions = new List<Vector2>();// どの位置に攻撃を飛ばすか
    public float waitSeconds = 0f;// 攻撃前に待つ時間 
    public float seconds = 0f;// その位置に何秒攻撃を発生させるか
    public float judgeInterval = 0.1f;// 判定オブジェクトの間隔
}
[Serializable]
public class AttackListWrap// Jsonで攻撃のリストを扱うためのクラス（こうしないとリストがJsonに書き出されない）
{
    public List<Attack> attacks;
}
[Serializable]
public class AttackSet
{
    public List<AttackData> AttackDatas;
}
[Serializable]
public class AttackData
{
    public float spawnTiming;
    public Vector2 position;

    public AttackData()
    {
        spawnTiming = 0f;
        position = Vector2.zero;
    }

    public AttackData(float spawnTiming, Vector2 position)
    {
        this.spawnTiming = spawnTiming;
        this.position = position;
    }
}
[Serializable]
public class JsonPath
{
    [PathAttribute]public string path;
}
public class JSONReader : MonoBehaviour
{
    [SerializeField] GameObject bossAttack;
    [SerializeField] Transform bossAttackParent;
    [Header("ボスの攻撃Jsonのリスト"), PathAttribute, SerializeField] List<string> bossAttackJsonPaths;

    float timer = -1f;
    List<AttackData> attackList = new List<AttackData>();
    GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");
        //BossAttack("C:/Users/Ton/Documents/GitHub/Yoga/YogaDevelop/Develop/Yoga/src/Assets/BossAttacks/.json");
    }

    // Update is called once per frame
    void Update()
    {
        if (attackList.Count > 0 && timer >= 0f)
        {
            timer += Time.deltaTime;
            for (int i = attackList.Count - 1; i > 0; i--)// リストの要素を削除しながら見ていくので、順番がずれないために逆から見ている
            {
                if (attackList[i].spawnTiming < timer)
                {
                    GameObject attack = Instantiate(bossAttack, new Vector3(player.transform.position.x + attackList[i].position.x,
                        player.transform.position.y + attackList[i].position.y, 0f), Quaternion.identity, bossAttackParent);
                    attack.GetComponent<Rigidbody>().AddForce(bossAttackParent.forward * 2f, ForceMode.Impulse);

                    attackList.RemoveAt(i);// リストから削除
                }
            }
        }
    }

    public void BossAttack()
    {
        string attackJson = bossAttackJsonPaths[0];// １番先頭の要素を読み込む
        List<Attack> attacks = LoadAttackJson(attackJson);
        AttackSet attackSet = ConvertToAttackSet(attacks);

        attackList.Clear();// 念の為リストの中身をなくしている
        for (int i = 0; i < attackSet.AttackDatas.Count; i++)
        {
            attackList.Add(attackSet.AttackDatas[i]);
        }
        timer = 0f;
        attackJson.Remove(0);// リストから読み込んだ要素を削除
    }
    public static List<Attack> LoadAttackJson(string dataPath)
    {
        StreamReader streamReader = new StreamReader(dataPath);
        string dataString = streamReader.ReadToEnd();
        streamReader.Close();

        return JsonUtility.FromJson<AttackListWrap>(dataString).attacks;
    }

    void JsonTest()
    {
        AttackData data1 = new AttackData();

        data1.spawnTiming = 0.2f;
        data1.position = Vector2.one;

        string jsonData = JsonUtility.ToJson(data1);// JSONデータはC#上で文字列として扱われる

        Debug.Log("data1:" + jsonData);

        AttackData data2 = JsonUtility.FromJson<AttackData>(jsonData);
        Debug.Log("data2:" + "spawnTiming:" + data2.spawnTiming + " position:" + data2.position);
    }

    AttackSet ConvertToAttackSet(List<Attack> attackGroups)
    {
        AttackSet attackSet = new AttackSet();
        float totalSeconds = 0f;
        List<AttackData> attacks = new List<AttackData>();
        for (int i = 0; i < attackGroups.Count; i++)
        {
            totalSeconds += attackGroups[i].waitSeconds;// 待機時間
            int judgeNum = (int)(attackGroups[i].seconds / attackGroups[i].judgeInterval);// 判定オブジェクトの個数
            for (int j = 0; j < judgeNum; j++)
            {
                for (int k = 0; k < attackGroups[i].positions.Count; k++)// 攻撃箇所の個数（左右の手足で設定しているぶん）
                {
                    attacks.Add(new AttackData(totalSeconds + j * attackGroups[i].judgeInterval, attackGroups[i].positions[k]));
                }
            }
            totalSeconds += attackGroups[i].seconds;
        }

        attackSet.AttackDatas = attacks;// 判定ごとの攻撃のリストに反映
        return attackSet;
    }
}
