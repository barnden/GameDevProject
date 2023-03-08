using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Damage is applied instantly
[CreateAssetMenu(fileName = "Dmg", menuName = "StatusTypes/StatDmgInstant", order = 1)]
public class StatDmgInstant : BaseStatusEffect
{
    public override void Apply(StatusSystem entityStatSysToEffect,
                                BaseAIComponent compToEffect)
    {
        compToEffect.DamageStat(statToEffect, amount);
    }
}
