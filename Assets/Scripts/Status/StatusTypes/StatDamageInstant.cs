using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Damage Instant", menuName = "StatusTypes/StatDamageInstant", order = 1)]
public class StatDamageInstant : BaseStatusEffect
{
    public override void Apply(StatusSystem entityStatSysToEffect,
                                BaseAIComponent compToEffect)
    {
        compToEffect.DamageStat(statToEffect, amount);   
    }
}
