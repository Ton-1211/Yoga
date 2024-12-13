using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerParticleManager : MonoBehaviour
{
    [SerializeField] List<ParticleSystem> particles = new List<ParticleSystem>();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void PlayParticle(Vector3 attackPosition)
    {
        ParticleSystem playParticle = new ParticleSystem();
        float minDistance = 0f;

        foreach(ParticleSystem particle in particles)
        {
            float distance = Vector3.Distance(attackPosition, particle.transform.position);
            if(minDistance > distance)
            {
                minDistance = distance;
                playParticle = particle;
            }
        }

        playParticle.Play();
    }
}
