using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackManagerScript : MonoBehaviour
{
    // joyconのトラッキングができたらこちらに変更
    // [SerializeField] TrackManagerScript trackManager;
    [SerializeField] Transform[] ikPoints;// 仮置き
    
    public GameObject GetNearestTrackPoint(Transform from)
    {
        GameObject nearest = null;
        float minDistance = 0f;
        float distance = 0f;

        // joyconのトラッキングができたらこちらに変更
        //foreach(TrackPoint t in trackManager.TrackPoints)
        //{
        //    distance = (from.position - t.GetTransform().position).sqrMagnitude;// 2乗した値で距離を出す
        //    if(minDistance > distance)
        //    {
        //        nearest = t.GetTransform().gameObject;
        //        minDistance = distance;
        //    }
        //}

        // 仮置き
        foreach(Transform t in ikPoints)
        {
            distance = (from.position - t.position).sqrMagnitude;
            if(minDistance > distance)
            {
                nearest = t.gameObject;
                minDistance = distance;
            }
        }

        return nearest;
    }
}
