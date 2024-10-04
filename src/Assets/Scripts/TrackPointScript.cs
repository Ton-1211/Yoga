using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackPointScript : MonoBehaviour
{
    [Tooltip("追いかける対象"), SerializeField] Transform target;
    [Tooltip("追跡の倍率"), SerializeField] Vector2 trackMagnification;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Track()
    {
        transform.position = new Vector3(target.position.x * trackMagnification.x, target.position.y * trackMagnification.y, target.position.z);
    }

    void ChangeMagnificationX(float Num)
    {
        trackMagnification.x = Num;
    }
    void ChangeMagnificationY(float Num)
    {
        trackMagnification.y = Num;
    }
}
