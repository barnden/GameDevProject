using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// An interface for all AI components to derive from
public interface BaseAIComponent
{
    // Damages (or heals if negative) a given stat
    void DamageStat(Stats statToAdjust, float amount);
    
    // Sets a given stat to a specified value
    void SetStat(Stats statToAdjust, float amount);
}
