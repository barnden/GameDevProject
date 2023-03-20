using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocomotionTranslate : LocomotionSystem
{
    public override void Target()
    {
        transform.Translate(effectiveSpeed * Time.deltaTime * (target.transform.position - transform.position).normalized);
    }
}
