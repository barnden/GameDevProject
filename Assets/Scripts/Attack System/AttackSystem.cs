using System; // contains [Serializable]
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackSystem : MonoBehaviour
{
    [Serializable]
    private class ProjectileParams
    {
        public GameObject projectile;
        public float aliveTime;
        public float Speed;
        public float damage;
        public float fireRate;
        public float AOESize;
        public float fireSpread;
    }


    private GameObject target = null;
    [SerializeField] ProjectileParams[] projectilesToFire;


    // Start is called before the first frame update
    void Start()
    {
        foreach (ProjectileParams currProjectile in projectilesToFire)
        {
            StartCoroutine(FireProjectile(currProjectile.fireRate));
        }
    }

    IEnumerator FireProjectile(float fireRate)
    {
        // keep the subrutine running so the enemy keeps firing
        while(true)
        {
            Debug.Log(fireRate.ToString());
            yield return new WaitForSeconds(fireRate);
        }
    }
}
