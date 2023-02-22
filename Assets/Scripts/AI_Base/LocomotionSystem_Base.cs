using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This will become an abstract class for both enemies and turrets to reference
public class LocomotionSystem_Base : MonoBehaviour
{

    [SerializeField] float baseEnemySpeed;
    [SerializeField] bool isDirectMovement;
    private GameObject target = null;

    // Update is called once per frame
    void Update()
    {
        if (target != null)
        {
            transform.Translate(baseEnemySpeed * Time.deltaTime * (target.transform.position - transform.position).normalized);
        }
    }

    public void setTarget(GameObject target)
    {
        this.target = target;
    }
}
