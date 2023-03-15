using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Handles an AI status's such as damage, DOT, ect...
public class StatusSystem : MonoBehaviour
{
    [System.NonSerialized]
    public GameObject entityRoot;
    public List<BaseStatusEffect> activeEffects = new List<BaseStatusEffect>();

    void Start()
    {
        entityRoot = gameObject;
    }

    // calls the se and has it apply it's effect to the correct componen
    public void ApplyEffect(BaseStatusEffect se)
    {
        // No need to keep track of the se if it's only going
        // to apply perminant damage once and never again
        if (se is StatDmgInstant || se.isStackable)
        {
            se.Apply(this, getEffectedComponent(se.statToEffect));
        }

        else if (!activeEffects.Contains(se))
        {
            activeEffects.Add(se);
            se.Apply(this, getEffectedComponent(se.statToEffect));
        }
    }

    public void RemoveEffect(BaseStatusEffect se)
    {
        activeEffects.Remove(se);
    }


    // Helpers

    // Adjust this function to get the correct component if a
    // new stat is being added!
    public BaseAIComponent getEffectedComponent(Stats stat)
    {
        switch (stat)
        {
            case Stats.HEALTH:
                return GetComponent<HealthComponent>();
            
            case Stats.SPEED:
                return GetComponent<LocomotionSystem>();

            /*
            case Stats.FIRERATE:
                return GetComponent<AttackSystem_Base>();
            */
            default:
                return null;
        
        }
    }
}
