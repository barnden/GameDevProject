using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocomotionRotate : LocomotionSystem
{
    public override void Target()
    {
        Vector2 dir = target.transform.position - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(0, 0, angle);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 8.0f * Time.deltaTime);
    }
}
