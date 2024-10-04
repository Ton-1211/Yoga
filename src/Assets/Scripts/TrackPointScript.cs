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
        public void Track()
        {
            trackObject.position = new Vector3(target.position.x * trackSensitivity.x, target.position.y * trackSensitivity.y, target.position.z);
        }
        public void ChangeSensitivityX(float Num)
        {
            trackSensitivity.x = Num;
        }
        public void ChangeSensitivityY(float Num)
        {
            trackSensitivity.y = Num;
        }
    }

    [SerializeField] Tracker[] trackers;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        foreach(Tracker tracker in trackers)
        {
            tracker.Track();
        }
    }
}
