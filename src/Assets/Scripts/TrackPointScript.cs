using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackPointScript : MonoBehaviour
{
    [Serializable]
    class Tracker
    {
        [Tooltip("動かす対象"), SerializeField] Transform trackObject;
        [Tooltip("追いかける対象"), SerializeField] Transform target;
        [Tooltip("追跡の倍率"), SerializeField] Vector2 trackSensitivity = new Vector2(1, 1);
        [Header("----- 肘のみ設定する -----\n肩"), SerializeField] Transform shoulder;
        [Header("手"), SerializeField] Transform hand;
        [Header("腕を完全に伸ばすしきい値"), SerializeField] float armExtendThreshold;
        Transform neck;

        public Transform Neck
        {
            get { return neck; }
            set { neck = value; }
        }

        public void Track()
        {
            float differenceY = target.position.y - neck.position.y;
            trackObject.position = new Vector3(target.position.x * trackSensitivity.x, target.position.y + differenceY * trackSensitivity.y, target.position.z);
            if (shoulder != null && hand != null)
            {
                ElbowCorrection();
            }
        }
        public void ChangeSensitivityX(float Num)
        {
            trackSensitivity.x = Num;
        }
        public void ChangeSensitivityY(float Num)
        {
            trackSensitivity.y = Num;
        }
        void ElbowCorrection()
        {
            float distance = Vector3.Distance(shoulder.position, hand.position);
            Debug.Log(distance);
            if (distance < armExtendThreshold)
            {
                return;
            }

            trackObject.position = (shoulder.position + hand.position) * 0.5f;// 肩と手の中間点に配置
        }
    }

    [SerializeField] Tracker[] trackers;

    // Start is called before the first frame update
    void Start()
    {
        foreach (Tracker tracker in trackers)
        {
            tracker.Neck = GameObject.FindWithTag("Neck").transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        foreach (Tracker tracker in trackers)
        {
            tracker.Track();
        }
    }
}
