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
    private StatusSystem statusSystem;

    public void setTarget(GameObject target)
    {
        this.target = target;
    }

    public void Start()
    {
        statusSystem = gameObject.GetComponent<StatusSystem>();
        if (statusSystem == null)
        {
            Debug.Log("ERROR: Status system not found as component");
            gameObject.SetActive(false);
        }

        RegCompToStatSystem();

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
                                                               gameObject,
                                                               properties.lifeTime, 
                                                               properties.damage, 
                                                               properties.speed, 
                                                               properties.scaleModifier, 
                                                               target, 
                                                               targetDir);

                // propagate the status of this object to the spawning children
                foreach (BaseStatusEffect currEffect in statusSystem.activeEffects)
                {
                    if(currEffect.propagateToChildren)
                    {
                        // Since we are unsure that the projectile will register the component
                        // quick enough, we need to do that here before we apply the effect.
                        StatusSystem projStatusSystem = projectile.GetComponent<StatusSystem>();
                        if (projStatusSystem != null)
                        {
                            BaseAIComponent aiProjComp = projectile.GetComponent<ProjectileSystem>();

                            projStatusSystem.RegisterAIComponent(aiProjComp, Stats.FIRERATE);
                            projStatusSystem.RegisterAIComponent(aiProjComp, Stats.PROJECTILE_SCALE);
                            projStatusSystem.RegisterAIComponent(aiProjComp, Stats.PROJECTILE_SPEED);

                            projStatusSystem.ApplyEffect(currEffect);
                        }
                    }
                }
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
                                                                   gameObject,
                                                                   properties.lifeTime,
                                                                   properties.damage,
                                                                   properties.speed,
                                                                   properties.scaleModifier,
                                                                   target,
                                                                   targetDir);

                    // propagate the status of this object to the spawning children
                    foreach (BaseStatusEffect currEffect in statusSystem.activeEffects)
                    {
                        if (currEffect.propagateToChildren)
                        {
                            // Since we are unsure that the projectile will register the component
                            // quick enough, we need to do that here before we apply the effect.
                            StatusSystem projStatusSystem = projectile.GetComponent<StatusSystem>();
                            if (projStatusSystem != null)
                            {
                                BaseAIComponent aiProjComp = projectile.GetComponent<ProjectileSystem>();

                                projStatusSystem.RegisterAIComponent(aiProjComp, Stats.FIRERATE);
                                projStatusSystem.RegisterAIComponent(aiProjComp, Stats.PROJECTILE_SCALE);
                                projStatusSystem.RegisterAIComponent(aiProjComp, Stats.PROJECTILE_SPEED);

                                projStatusSystem.ApplyEffect(currEffect);
                            }
                        }
                    }
                }
            }
        }
    }

    // Status system dependent functions
    public void DamageStat(Stats statToDamage, float amount)
    {
        foreach (AttackProperties currProjectile in projectiles)
        {
            switch (statToDamage)
            {
                case Stats.FIRERATE:
                    currProjectile.fireRate -= amount;

                    // give small wiggle room to protect against float inaccuracies
                    currProjectile.fireRate = Mathf.Clamp(currProjectile.fireRate, 0.0f, float.MaxValue);
                    return;

                case Stats.PROJECTILE_SCALE:
                    currProjectile.projectileProperties.scaleModifier -= amount;

                    // give small wiggle room to protect against float inaccuracies
                    currProjectile.projectileProperties.scaleModifier = 
                        Mathf.Clamp(currProjectile.projectileProperties.scaleModifier, 0.0f, float.MaxValue);
                    return;

                case Stats.PROJECTILE_SPEED:
                    currProjectile.projectileProperties.speed -= amount;

                    // give small wiggle room to protect against float inaccuracies
                    currProjectile.projectileProperties.speed =
                        Mathf.Clamp(currProjectile.projectileProperties.speed, 0.0f, float.MaxValue);
                    return;

                case Stats.PROJECTILE_LIFETIME:
                    currProjectile.projectileProperties.lifeTime -= amount;

                    // give small wiggle room to protect against float inaccuracies
                    currProjectile.projectileProperties.lifeTime =
                        Mathf.Clamp(currProjectile.projectileProperties.lifeTime, 0.0f, float.MaxValue);
                    return;

                case Stats.PROJECTILE_DAMAGE:
                    currProjectile.projectileProperties.damage -= amount;

                    // give small wiggle room to protect against float inaccuracies
                    currProjectile.projectileProperties.damage =
                        Mathf.Clamp(currProjectile.projectileProperties.damage, float.MinValue, float.MaxValue);
                    return;


                default:
                    return;
            }
        }
    }

    public void SetStat(Stats statToDamage, float value)
    {
        foreach (AttackProperties currProjectile in projectiles)
        {
            switch (statToDamage)
            {
                case Stats.FIRERATE:

                    currProjectile.fireRate = value;

                    // give small wiggle room to protect against float inaccuracies
                    currProjectile.fireRate = Mathf.Clamp(currProjectile.fireRate, 0.0f, float.MaxValue);
                    return;

                case Stats.PROJECTILE_SCALE:

                    currProjectile.projectileProperties.scaleModifier = value;

                    // give small wiggle room to protect against float inaccuracies
                    currProjectile.projectileProperties.scaleModifier = 
                        Mathf.Clamp(currProjectile.projectileProperties.scaleModifier, 0.0f, float.MaxValue);
                    return;

                case Stats.PROJECTILE_SPEED:

                    currProjectile.projectileProperties.speed = value;

                    // give small wiggle room to protect against float inaccuracies
                    currProjectile.projectileProperties.speed =
                        Mathf.Clamp(currProjectile.projectileProperties.speed, 0.0f, float.MaxValue);
                    return;

                case Stats.PROJECTILE_LIFETIME:

                    currProjectile.projectileProperties.lifeTime = value;

                    // give small wiggle room to protect against float inaccuracies
                    currProjectile.projectileProperties.lifeTime =
                        Mathf.Clamp(currProjectile.projectileProperties.lifeTime, 0.0f, float.MaxValue);
                    return;

                case Stats.PROJECTILE_DAMAGE:

                    currProjectile.projectileProperties.damage = value;

                    // give small wiggle room to protect against float inaccuracies
                    currProjectile.projectileProperties.damage =
                        Mathf.Clamp(currProjectile.projectileProperties.damage, float.MinValue, float.MaxValue);
                    return;

                default:
                    return;
            }
        }
    }

    public List<float> GetStat(Stats stat)
    {
        List<float> propertyList = new List<float>();

        foreach (AttackProperties currProjectile in projectiles)
        {
            float statVal = 0.0f;
            switch (stat)
            {
                case Stats.FIRERATE:
                    statVal = currProjectile.fireRate;
                    propertyList.Add(statVal);

                    break;

                case Stats.PROJECTILE_SCALE:
                    statVal = currProjectile.projectileProperties.scaleModifier;
                    propertyList.Add(statVal);

                    break;

                case Stats.PROJECTILE_SPEED:
                    statVal = currProjectile.projectileProperties.speed;
                    propertyList.Add(statVal);

                    break;

                case Stats.PROJECTILE_LIFETIME:
                    statVal = currProjectile.projectileProperties.lifeTime;
                    propertyList.Add(statVal);

                    break;

                case Stats.PROJECTILE_DAMAGE:
                    statVal = currProjectile.projectileProperties.damage;
                    propertyList.Add(statVal);

                    break;

                default:
                    return null;
            }
        }
        return propertyList;
    }

    public void RegCompToStatSystem()
    {
        statusSystem.RegisterAIComponent(this, Stats.FIRERATE);
        statusSystem.RegisterAIComponent(this, Stats.PROJECTILE_SCALE);
        statusSystem.RegisterAIComponent(this, Stats.PROJECTILE_SPEED);
        statusSystem.RegisterAIComponent(this, Stats.PROJECTILE_LIFETIME);
        statusSystem.RegisterAIComponent(this, Stats.PROJECTILE_DAMAGE);
    }

    public void OnDestroy()
    {
        foreach (var coroutine in coroutines)
        {
            StopCoroutine(coroutine);
        }
    }
}
