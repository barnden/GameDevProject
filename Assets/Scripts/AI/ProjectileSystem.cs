using System; // contains [Serializable]
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* NOTES:
 * 
 * We might want to enable and disable coroutines for cleaner code
 * instead of using the target variable to decide if we instantiate
 * the projectile
 */

public class ProjectileSystem : MonoBehaviour, BaseAIComponent
{
    // Keep public so other systems may change these
    // attributes (such as status's)
    [Serializable]
    public class AttackProperties
    {
        // We'll keep it as a gameObject if we want the system
        // to deploy more than just "projectiles"
        public GameObject projectile;
        public ProjectileProperties projectileProperties;
        public float projectileSpawnDistance;
        public float fireRate; // in seconds
        public float fireSpread; // in degrees     
    }

    [SerializeField] List<AttackProperties> projectiles;
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
            var coroutine = FireProjectileCoroutine(currProjectile);
            coroutines.Add(coroutine);
            StartCoroutine(coroutine);
        }
    }

    IEnumerator FireProjectileCoroutine(AttackProperties attack)
    {
        // keep the subroutine running so the enemy keeps firing
        while (true)
        {
            // if(eventTrigger == EventType.DISABLED) { break; }
            
            if(target)
            {
                Vector2 targetDir = target.transform.position - transform.position;
                targetDir.Normalize();

                // find front of enemy to instantiate bullet
                Vector2 front = new Vector2(transform.position.x + (targetDir.x * attack.projectileSpawnDistance),
                                            transform.position.y + (targetDir.y * attack.projectileSpawnDistance));

                // Instantiate the projectile with provided params
                GameObject projectile = Instantiate(attack.projectile, front, Quaternion.identity);

                ProjectileProperties properties = attack.projectileProperties;
                projectile.GetComponent<BaseProjectile>().Init(projectile, 
                                                               properties.lifeTime, 
                                                               properties.damage, 
                                                               properties.speed, 
                                                               properties.scaleModifier, 
                                                               target, 
                                                               targetDir);

            }
            yield return new WaitForSeconds(attack.fireRate);
        }
    }

    // Fires all projectiles in projectiles array
    public void FireProjectiles(int numberOfTimes)
    {
        foreach (AttackProperties currProjectile in projectiles)
        {
            if (target)
            {
                for (int i = 0; i < numberOfTimes; i++)
                {
                    Vector2 targetDir = target.transform.position - transform.position;
                    targetDir.Normalize();

                    // find front of enemy to instantiate bullet
                    Vector2 front = new Vector2(transform.position.x + (targetDir.x * currProjectile.projectileSpawnDistance),
                                                transform.position.y + (targetDir.y * currProjectile.projectileSpawnDistance));

                    // Instantiate the projectile with provided params
                    GameObject projectile = Instantiate(currProjectile.projectile, front, Quaternion.identity);

                    ProjectileProperties properties = currProjectile.projectileProperties;
                    projectile.GetComponent<BaseProjectile>().Init(projectile,
                                                                   properties.lifeTime,
                                                                   properties.damage,
                                                                   properties.speed,
                                                                   properties.scaleModifier,
                                                                   target,
                                                                   targetDir);
                }
            }
        }
    }

    // Status system dependent functions
    public void DamageStat(Stats statToDamage, float amount)
    {
        switch (statToDamage)
        {
            case Stats.FIRERATE:
                foreach(AttackProperties currProjectile in projectiles)
                {
                    currProjectile.fireRate -= amount;

                    // give small wiggle room to protect against float inaccuracies
                    currProjectile.fireRate = Mathf.Clamp(currProjectile.fireRate, 0.0f, float.MaxValue);
                }
                return;

            default:
                return;
        }
    }

    public void SetStat(Stats statToDamage, float value)
    {
        switch (statToDamage)
        {
            case Stats.FIRERATE:
                foreach (AttackProperties currProjectile in projectiles)
                {
                    currProjectile.fireRate = value;

                    // give small wiggle room to protect against float inaccuracies
                    currProjectile.fireRate = Mathf.Clamp(currProjectile.fireRate, 0.0f, float.MaxValue);
                }
                return;

            default:
                return;
        }
    }

    public List<float> GetStat(Stats stat)
    {
        List<float> propertyList = new List<float>();

        foreach (AttackProperties currProjectile in projectiles)
        {
            switch (stat)
            {
                case Stats.FIRERATE:
                    float statVal = currProjectile.fireRate;
                    propertyList.Add(statVal);

                    break;

                default:
                    return null;
            }
        }
        return propertyList;
    }
    public void OnDestroy()
    {
        foreach (var coroutine in coroutines)
        {
            StopCoroutine(coroutine);
        }
    }
}
