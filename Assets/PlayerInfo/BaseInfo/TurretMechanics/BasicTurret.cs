using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicTurret : MonoBehaviour
{
    [Header("Turret Properties")]
    public GameObject turretBullet;
    public float turretRadius;


    public GameObject currentTarget;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        currentTarget = TrackNearestEnemy(turretRadius);
        RotateToNearestEnemy(currentTarget);
    }


    public GameObject TrackNearestEnemy(float radius)
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        GameObject nearestEnemy = null;
        float distance = Mathf.Infinity;
        
        foreach (GameObject enemy in enemies)
        {
            Vector3 diff = enemy.transform.position - transform.position;
            float curDistance = diff.sqrMagnitude;

            if (curDistance < distance)
            {
                nearestEnemy = enemy;
                distance = curDistance;
            }
        }

        return nearestEnemy;
    }

    public void RotateToNearestEnemy(GameObject target)
    {
        if(target == null)
        {
            return;
        }
        Vector2 dir = target.transform.position - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(0, 0, angle);
        //Quaternion targetRotation = new Quaternion(0,0, angle,0);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 8.0f * Time.deltaTime);

    }

    public void Shoot()
    {
        
    }
}
