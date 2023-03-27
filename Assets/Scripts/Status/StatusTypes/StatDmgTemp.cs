using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Damage is applied to the given stat and reverted
// after a certain amount of time
[CreateAssetMenu(fileName = "DmgTemp", menuName = "StatusTypes/StatDmgTemp", order = 1)]
public class StatDmgTemp : BaseStatusEffect
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
            //compToEffect.DamageStat(statToEffect, amount);
            if (isPercentageBased)
            {
                float statVal = entityStatSysToEffect.GetStat(statToEffect);
                entityStatSysToEffect.DamageStat(statToEffect, statVal * amount);
            }
            else
            {
                entityStatSysToEffect.DamageStat(statToEffect, amount);
            }
            counter--;
            yield return new WaitForSeconds(1.0f);
        }

        // revert the damage
        //compToEffect.DamageStat(statToEffect, -amount);
        if (isPercentageBased)
        {
            float statVal = entityStatSysToEffect.GetStat(statToEffect);
            entityStatSysToEffect.DamageStat(statToEffect, statVal * amount);
        }
        else
        {
            entityStatSysToEffect.DamageStat(statToEffect, -amount);
        }
        entityStatSysToEffect.RemoveEffect(this);
    }
}
