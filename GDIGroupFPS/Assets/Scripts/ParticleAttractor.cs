using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleAttractor : MonoBehaviour
{
    public ParticleSystem particleSystem;
    public Transform target;
    public float speed = 5.0f;

    private ParticleSystem.Particle[] particles;

    void LateUpdate()
    {
        if (!enabled)
        return;

        int maxParticles = particleSystem.main.maxParticles;
        if (particles == null || particles.Length < maxParticles)
        {
            particles = new ParticleSystem.Particle[maxParticles];
        }

        int particleCount = particleSystem.GetParticles(particles);
        float step = speed * Time.deltaTime;
        Vector3 targetPosition = particleSystem.transform.InverseTransformPoint(target.position);  // Converting to local space if needed

        for (int i = 0; i < particleCount; i++)
        {
            Vector3 direction = (targetPosition - particles[i].position).normalized;
            particles[i].position += direction * step;
        }

        particleSystem.SetParticles(particles, particleCount);
    }
}

