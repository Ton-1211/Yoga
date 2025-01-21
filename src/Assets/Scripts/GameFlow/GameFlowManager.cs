using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class GameFlowManager : MonoBehaviour
{
    [SerializeField] JSONReader jsonReader;
    [Header("ダメージ表示に使用するUI"), SerializeField] GameObject damageDisplayUI;
    [Header("ボスの攻撃召喚ムービーのDirector"), SerializeField] PlayableDirector bossSummonDirector;
    [Header("プレイヤーの攻撃ムービーのDirectorたち"), SerializeField] List<PlayableDirector> playerAttackDirectors;
    [Header("ボスの撃破ムービーのDirector"), SerializeField] PlayableDirector winDirector;

    enum GameState
    {
        Opening = 0,
        SummonAttack = 1,
        PlayerAttack = 2,
        Result = 3
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void GameFlowBehavior()

    {

    }
}
