using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class AttackSet
{
    public List<AttackData> AttackDatas;
}
[System.Serializable]
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

public class JSONReader : MonoBehaviour
{
    [SerializeField] GameObject bossAttack;
    [SerializeField] Transform bossAttackParent;

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

    public void BossAttack([PathReference] string attackJson)
    {
        AttackSet attacks = LoadAttackJson(attackJson);

        attackList.Clear();// 念の為リストの中身をなくしている
        for (int i = 0; i < attacks.AttackDatas.Count; i++)
        {
            attackList.Add(attacks.AttackDatas[i]);
        }
        timer = 0f;
    }
    AttackSet LoadAttackJson(string dataPath)
    {
        StreamReader streamReader = new StreamReader(dataPath);
        string dataString = streamReader.ReadToEnd();
        streamReader.Close();

        return JsonUtility.FromJson<AttackSet>(dataString);
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
}
