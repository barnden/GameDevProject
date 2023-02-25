using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This will become an abstract class for both enemies and turrets to reference
public abstract class LocomotionSystem : MonoBehaviour
{

    [SerializeField] protected float baseEnemySpeed;
    [SerializeField] protected bool isDirectMovement;
    protected GameObject target = null;

    public abstract void Target();


    // Update is called once per frame
    void Update()
    {
        if (target != null)
        {
            Target();
        }
    }

    public void setTarget(GameObject target)
    {
        this.target = target;
    }
}
