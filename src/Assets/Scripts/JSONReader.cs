using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System;

[Serializable]
public class Attack// ひとかたまり（発生位置が変化しない）攻撃
{
    const float DefaultWaitSeconds = 0f;
    const float DefaultAttackSeconds = 0f;
    const float DefaultJudgeInterval = 0.1f;

    public List<Vector2> positions = new List<Vector2>();// どの位置に攻撃を飛ばすか
    public float waitSeconds = DefaultWaitSeconds;// 攻撃前に待つ時間 
    public float seconds = DefaultAttackSeconds;// その位置に何秒攻撃を発生させるか
    public float judgeInterval = DefaultJudgeInterval;// 判定オブジェクトの間隔
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
    const float DefaultSpawnTiming = 0f;
    public float spawnTiming;
    public Vector2 position;

    public AttackData()
    {
        spawnTiming = DefaultSpawnTiming;
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
    //#if UNITY_EDITOR
    //    [PathAttribute]// エディターの機能を使用しているのでビルド時に属性を除外しようとしたが、ビルドエラーが起きるのでコメントアウト
    //    [SerializeField] public string path;
    //#endif
    [SerializeField] public string path;
    [Header("ポーズのときに表示する見本の画像"), SerializeField] public Sprite poseImage;
}
public class JSONReader : MonoBehaviour
{
    const float InitTimerSecond = -1f;
    const float InitTotalSeconds = 0f;
    [SerializeField] GameObject bossAttack;
    [SerializeField] Transform bossAttackParent;
    [Header("プレイヤーに対する攻撃の発生距離"), SerializeField] float attackDistance = 6f;
    [Header("ボスの攻撃のスピード"), SerializeField] float attackSpeed = 2f;
    [Header("ボスの攻撃Jsonのリスト"), SerializeField] List<JsonPath> bossAttackJsonPaths;

    float timer = InitTimerSecond;
    List<AttackData> attackList = new List<AttackData>();
    [SerializeField] GameObject player;
    int possibleDamageMax;

    public int RemainBossAttack => bossAttackJsonPaths.Count;
    public Sprite PoseImage => bossAttackJsonPaths[0].poseImage;
    public int PossibleDamage => possibleDamageMax;

    void Start()
    {
        //player = GameObject.FindWithTag("Player");
        possibleDamageMax = 0;
        //BossAttack("C:/Users/Ton/Documents/GitHub/Yoga/YogaDevelop/Develop/Yoga/src/Assets/BossAttacks/.json");
    }

    void Update()
    {
        if (attackList.Count > 0 && timer >= 0f)
        {
            timer += Time.deltaTime;
            for (int i = attackList.Count - 1; i > 0; i--)// リストの要素を削除しながら見ていくので、順番がずれないために逆から見ている
            {
                // 攻撃生成
                if (attackList[i].spawnTiming < timer)
                {
                    // 攻撃のオブジェクトを生成
                    GameObject attack = Instantiate(bossAttack, new Vector3(player.transform.position.x + attackList[i].position.x,
                        player.transform.position.y + attackList[i].position.y, player.transform.position.z + attackDistance), Quaternion.identity, bossAttackParent);

                    // 攻撃のオブジェクトを動かす
                    attack.GetComponent<Rigidbody>().AddForce(bossAttackParent.forward * attackSpeed, ForceMode.Impulse);

                    // 吸収可能な最大ダメージ量を増やす（リザルトの判定に使う）
                    possibleDamageMax += attack.GetComponent<AttackBulletScript>().Damage;

                    attackList.RemoveAt(i);// リストから削除
                }
            }
        }
    }

    // ボスの攻撃を登録されているJsonファイルのリストから読み込んで生成、開始タイミングで呼び出される
    public void BossAttack()
    {
        string attackJson = Application.streamingAssetsPath + "/" + bossAttackJsonPaths[0].path;// １番先頭の要素を読み込む
        List<Attack> attacks = LoadAttackJson(attackJson);
        AttackSet attackSet = ConvertToAttackSet(attacks);

        attackList.Clear();// 念の為リストの中身をなくしている
        for (int i = 0; i < attackSet.AttackDatas.Count; i++)
        {
            attackList.Add(attackSet.AttackDatas[i]);// 攻撃の内容を登録
        }
        timer = 0f;
        bossAttackJsonPaths.Remove(bossAttackJsonPaths[0]);// リストから読み込んだ要素を削除
    }

    /// <summary>
    /// 攻撃のJsonファイルを読み込んで攻撃のクラスに変換
    /// </summary>
    /// <param name="dataPath">Jsonファイルのパス</param>
    /// <returns>その攻撃で生成するオブジェクトとタイミングのリスト</returns>
    public static List<Attack> LoadAttackJson(string dataPath)
    {
        StreamReader streamReader = new StreamReader(dataPath);
        string dataString = streamReader.ReadToEnd();
        streamReader.Close();

        return JsonUtility.FromJson<AttackListWrap>(dataString).attacks;
    }

    /// <summary>
    /// 攻撃の情報から、実際に攻撃の生成で使用する情報のリストへと変換する
    /// </summary>
    /// <param name="attackGroups">その攻撃で生成するオブジェクトとタイミングのリスト</param>
    /// <returns>攻撃の情報リスト</returns>
    AttackSet ConvertToAttackSet(List<Attack> attackGroups)
    {
        AttackSet attackSet = new AttackSet();
        float totalSeconds = InitTotalSeconds;
        List<AttackData> attacks = new List<AttackData>();

        // 変換
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
            totalSeconds += attackGroups[i].seconds;// 時間を経過（生成タイミングを計算するため）
        }

        attackSet.AttackDatas = attacks;// 判定ごとの攻撃のリストに反映
        return attackSet;
    }
}
