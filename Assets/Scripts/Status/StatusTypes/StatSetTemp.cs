using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A set value is applied to the given stat and reverted
// after a certain amount of time
[CreateAssetMenu(fileName = "SetTemp", menuName = "StatusTypes/StatSetTemp", order = 2)]
public class StatSetTemp : BaseStatusEffect
{
    public int Duration;
    public override void Apply(StatusSystem entityStatSysToEffect,
                                BaseAIComponent compToEffect)
    {
        IEnumerator coroutine = DoEffect(entityStatSysToEffect, compToEffect);
        
        // attach the coroutine to the object, so if the object dies
        // the coroutine is halted
        entityStatSysToEffect.StartCoroutine(coroutine);
    }
    private IEnumerator DoEffect(StatusSystem entityStatSysToEffect, BaseAIComponent compToEffect)
    {
        int counter = this.Duration;
        while (counter > 0)
        {
            compToEffect.SetStat(statToEffect, amount);
            counter--;
            yield return new WaitForSeconds(1.0f);
        }

        // revert the damage
        compToEffect.SetStat(statToEffect, -amount);
        entityStatSysToEffect.RemoveEffect(this);
    }
}
