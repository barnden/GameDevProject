using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemyMove : MonoBehaviour
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
