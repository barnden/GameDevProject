using System; // contains [Serializable]
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileSystem : MonoBehaviour
{
    [Serializable]

    // These do nothing currently
    private class ProjectileParams
    {
        public GameObject projectile;
        public float distToSpawnProjectile;
        public float lifetime;
        public float speed;
        public float damage;
        public float fireRate;
        public float AOESize;
        public float fireSpread;
    }

    // Keep public so other systems may change these
    // attributes (such as status's)
    [Serializable]
    public struct AttackProperties
    {
        // We'll keep it as a gameObject if we want the system
        // to deploy more than just "projectiles"
        public GameObject projectile;
        public ProjectileProperties projectileProperties;
        public float projectileSpawnDistance;
        public float fireRate; // in seconds
        public float fireSpread; // in degrees
         
    }


    [SerializeField] AttackProperties[] projectiles;
    [SerializeField] ProjectileParams[] projectilesToFire;
    private GameObject target = null;
    private List<IEnumerator> coroutines;

    public void setTarget(GameObject target)
    {
        this.target = target;
    }

    public void Start()
    {
        coroutines = new List<IEnumerator>();

        foreach (AttackProperties currProjectile in projectiles)
        {
            var coroutine = FireProjectile(currProjectile);
            coroutines.Add(coroutine);
            StartCoroutine(coroutine);
        }
    }

    public void OnDestroy()
    {
        foreach (var coroutine in coroutines)
        {
            StopCoroutine(coroutine);
        }
    }

    IEnumerator FireProjectile(AttackProperties attack)
    {
        // keep the subrutine running so the enemy keeps firing
        while(true)
        {
            if(target)
            {
                Vector2 targetDir = target.transform.position - transform.position;
                targetDir.Normalize();

                // find front of enemy to instantiate bullet
                Vector2 front = new Vector2(transform.position.x + (targetDir.x * attack.projectileSpawnDistance),
                                            transform.position.y + (targetDir.y * attack.projectileSpawnDistance));

                // Instantiate the projectile with provided params
                GameObject projectile = Instantiate(attack.projectile, front, Quaternion.identity);

                /*projectile.GetComponent<BaseProjectile>().Init(projectile, param.lifetime, 
                                                            param.speed, param.damage, 
                                                            param.AOESize, targetDir);*/
                projectile.GetComponent<BaseProjectile>().Init(projectile);
                projectile.GetComponent<BaseProjectile>().setDirection(targetDir);
            }

            yield return new WaitForSeconds(attack.fireRate);
        }
    }
}
