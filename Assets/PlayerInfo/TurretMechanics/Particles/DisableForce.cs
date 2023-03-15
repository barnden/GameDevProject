using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableForce : MonoBehaviour
{
    public LayerMask layer;
    public Collider2D[] GetOverlappingTriggers(ParticleSystem.Particle _particle, float _radius)
    {
        // TODO: It would be better not to use Physics here but to simply compare to a list we overlap with.

        var colliders = Physics2D.OverlapCircleAll(_particle.position, _radius, layer);
        return colliders;
    }

    void OnParticleTrigger()
    {
        ParticleSystem ps = GetComponent<ParticleSystem>();

        List<ParticleSystem.Particle> enter = new List<ParticleSystem.Particle>();
        List<ParticleSystem.Particle> exit = new List<ParticleSystem.Particle>();

        // get
        int numEnter = ps.GetTriggerParticles(ParticleSystemTriggerEventType.Enter, enter);
        int numExit = ps.GetTriggerParticles(ParticleSystemTriggerEventType.Exit, exit);

        Debug.Log(numEnter);
        // iterate
        for (int i = 0; i < numEnter; i++)
        {
            ParticleSystem.Particle p = enter[i];
            GetOverlappingTriggers(p, p.GetCurrentSize(ps));
            p.startColor = new Color32(255, 0, 0, 255);
            enter[i] = p;
        }
        for (int i = 0; i < numExit; i++)
        {
            ParticleSystem.Particle p = exit[i];
            p.startColor = new Color32(0, 255, 0, 255);
            exit[i] = p;
        }

        // set
        ps.SetTriggerParticles(ParticleSystemTriggerEventType.Enter, enter);
        ps.SetTriggerParticles(ParticleSystemTriggerEventType.Exit, exit);
    }
}