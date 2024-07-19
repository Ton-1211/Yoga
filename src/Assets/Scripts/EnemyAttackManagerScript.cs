using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackManagerScript : MonoBehaviour
{
    [SerializeField] TrackManagerScript trackManager;
    
    public GameObject GetNearestTrackPoint(Transform from)
    {
        GameObject nearest = null;
        float minDistance = 0f;
        float distance = 0f;

        foreach(TrackPoint t in trackManager.TrackPoints)
        {
            distance = (from.position - t.GetTransform().position).sqrMagnitude;// 2乗した値で距離を出す
            if(minDistance > distance)
            {
                nearest = t.GetTransform().gameObject;
                minDistance = distance;
            }
        }
        return nearest;
    }
}
