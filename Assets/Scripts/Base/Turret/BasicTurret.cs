using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicTurret : MonoBehaviour
{
    [Header("Turret Properties")]
    public GameObject turretBullet;
    public float turretBulletLifetime;
    public GameObject turretBulletPool;
    public Transform spawnPosition;
    public float turretRadius;
    public float fireRate;
    public Transform bulletPrefab;
    private float fireTimer;

    public GameObject currentTarget;
    
    // Start is called before the first frame update
    void Start()
    {
        fireTimer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        fireTimer -= Time.deltaTime;
        currentTarget = TrackNearestEnemy(turretRadius);
        RotateToNearestEnemy(currentTarget);

        if(fireTimer <= 0f)
        {
           // print("shot");
            Shoot();
            fireTimer = fireRate;
        }
        Debug.DrawRay(transform.position, 10f * transform.right, Color.green);

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
        if(currentTarget == null)
        {
            return;
        }

        RaycastHit2D turretRayInfo = Physics2D.Raycast(transform.position, transform.right, 10f);
        
        if (turretRayInfo.collider != null)
        {
            GameObject bullet = turretBulletPool.GetComponent<BulletPooling>().GetPooledObject();
            if(bullet != null)
            {
                bullet.transform.position = spawnPosition.transform.position;
                bullet.transform.rotation = transform.rotation;
                bullet.GetComponent<BaseBullet>().blifeTime = turretBulletLifetime;
                bullet.GetComponent<BaseBullet>().bMaxLifeTime = turretBulletLifetime;
                bullet.SetActive(true);
            }
        } 
    }
}
