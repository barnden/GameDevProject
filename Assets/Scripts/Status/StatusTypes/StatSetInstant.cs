using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// The stat is set to a value instantly
[CreateAssetMenu(fileName = "Set", menuName = "StatusTypes/StatSetInstant", order = 2)]
public class StatSetInstant : BaseStatusEffect
{
    public override void Apply(StatusSystem entityStatSysToEffect,
                                BaseAIComponent compToEffect)
    {
        compToEffect.SetStat(statToEffect, amount);
    }
}
