using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleEvent : MonoBehaviour
{

    private ParticleSystem system;
    // Start is called before the first frame update
    void Start()
    {
        system = GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// OnParticleTrigger is called when any particles in a particle system
    /// meet the conditions in the trigger module.
    /// </summary>
    void OnParticleTrigger()
    {
        ParticleSystem.Particle[] m_Particles;
        m_Particles = new ParticleSystem.Particle[system.main.maxParticles]; 
        
        int numParticlesAlive = system.GetParticles(m_Particles);

        // Change only the particles that are alive
        for (int i = 0; i < numParticlesAlive; i++)
        {
            //dust at particle
            DustManager.Instance.AddDust(m_Particles[i].position,1);
        }
        system.Clear();
    }
}
