using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Base abstract class for all status effects to inherit from
public abstract class BaseStatusEffect : ScriptableObject
{
    public Stats statToEffect;
    public bool isStackable; // can targets have multiple of these statuses

    // may be removed in the future if all status effects do
    // not require this
    public float amount;  

    public abstract void Apply( StatusSystem entityStatSysToEffect, 
                                BaseAIComponent compToEffect);
}
