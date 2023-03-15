using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocomotionTranslate : LocomotionSystem
{
    public override void Target()
    {
        transform.Translate(speed * Time.deltaTime * (target.transform.position - transform.position).normalized);
    }
}
