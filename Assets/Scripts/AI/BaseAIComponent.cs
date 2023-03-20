using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// An interfacefor all AI components to derive from
public interface BaseAIComponent
{
    // Damages (or heals if negative) a given stat
    void DamageStat(Stats statToDamage, float amount);

    // Sets a given stat to a specified value
    void SetStat(Stats statToSet, float value);

    // Retrieves value of a given stat
    float GetStat(Stats stats);
}
