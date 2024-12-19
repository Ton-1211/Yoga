using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerParticleManager : MonoBehaviour
{
    [SerializeField] List<ParticlesPlayer> particles = new List<ParticlesPlayer>();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void PlayParticle(Vector3 attackPosition)
    {
        ParticlesPlayer playParticle = new ParticlesPlayer();
        float minDistance = 0f;

        foreach(ParticlesPlayer particle in particles)
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
