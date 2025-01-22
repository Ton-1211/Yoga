using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class TimelineTestScript : MonoBehaviour
{
    [SerializeField] PlayableDirector director;
    [SerializeField] TimelineAsset timeline;
    // Start is called before the first frame update
    void Start()
    {
        director.playableAsset = timeline;
        director.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
