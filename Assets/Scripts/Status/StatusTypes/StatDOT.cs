using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* As of right now, DOTs are on a per-second basis
 * so every second the damage/healing will be applied
 * to keep computation power to a minimum
 */

[CreateAssetMenu(fileName = "DmgDOT", menuName = "StatusTypes/StatDOT", order = 1)]
public class StatDOT : BaseStatusEffect
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
            compToEffect.DamageStat(statToEffect, amount);
            counter--;
            yield return new WaitForSeconds(1.0f);
        }

        entityStatSysToEffect.RemoveEffect(this);
    }
}
