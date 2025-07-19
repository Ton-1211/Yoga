using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlesPlayer : MonoBehaviour
{
    [SerializeField]List<ParticleSystem> particles = new List<ParticleSystem>();
    void Start()
    {
        for(int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            // TryGetComponentでParcticleSystemを取得し、リストに追加する
            if(child.TryGetComponent<ParticleSystem>(out ParticleSystem particle))
            {
                particles.Add(particle);
            }
        }
    }

    public void Play()
    {
        foreach(ParticleSystem particle in particles)
        {
            particle.Play();
        }
    }
}
