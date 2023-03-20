using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This will become an abstract class for both enemies and turrets to reference
public abstract class LocomotionSystem : MonoBehaviour, BaseAIComponent
{

    [SerializeField] protected float effectiveSpeed;
    [SerializeField] protected float maxSpeed;
    [SerializeField] protected bool isDirectMovement;
    [System.NonSerialized] protected GameObject target = null;

    // without bounds, used to calculate status changes, so stackable
    // status do not step on each others toes when reverting damage, ect
    public float rawSpeed;

    public abstract void Target();

    void Start()
    {
        rawSpeed = effectiveSpeed;
        GetComponent<StatusSystem>().RegisterAIComponent(this, Stats.SPEED);
    }

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
                rawSpeed -= amount;


                // prevent healing past maxHealth
                // give small wiggle room to protect against float inaccuracies
                effectiveSpeed = Mathf.Clamp(rawSpeed, 0.0f, maxSpeed);
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
                rawSpeed = value;

                // we don't want to have negative speed
                effectiveSpeed = Mathf.Clamp(rawSpeed, 0.0f, maxSpeed);
                return;

            default:
                return;
        }
    }

    public float GetStat(Stats stat)
    {
        switch (stat)
        {
            case Stats.SPEED:
                return effectiveSpeed;

            default:
                return 0f;
        }
    }
}
