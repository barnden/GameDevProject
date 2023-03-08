using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This will become an abstract class for both enemies and turrets to reference
public abstract class LocomotionSystem : MonoBehaviour, BaseAIComponent
{

    [SerializeField] protected float speed;
    [SerializeField] protected float maxSpeed;
    [SerializeField] protected bool isDirectMovement;
    protected GameObject target = null;

    public abstract void Target();


    // Update is called once per frame
    void Update()
    {
        if (target != null)
        {
            Target();
        }
    }

    public void setTarget(GameObject target)
    {
        this.target = target;
    }

    public void DamageStat(Stats statToDamage, float amount)
    {
        switch (statToDamage)
        {
            case Stats.SPEED:
                speed -= amount;

                // prevent healing past maxHealth
                // give small wiggle room to protect against float inaccuracies
                speed = Mathf.Clamp(speed, 0.0f, maxSpeed);
                return;

            default:
                return;
        }
    }

    public void SetStat(Stats statToDamage, float value)
    {
        switch (statToDamage)
        {
            case Stats.SPEED:
                speed = value;

                // we don't want to have negative speed
                speed = Mathf.Clamp(speed, 0.0f, maxSpeed);
                return;

            default:
                return;
        }
    }
}
