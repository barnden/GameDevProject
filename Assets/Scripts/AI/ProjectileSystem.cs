using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* NOTES:
 * 
 * We might want to enable and disable coroutines for cleaner code
 * instead of using the target variable to decide if we instantiate
 * the projectile
 */

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

public class ProjectileSystem : MonoBehaviour, BaseAIComponent
{
    [SerializeField] public List<AttackProperties> projectiles;
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
        ResetSystem();
    }

    public void ResetSystem()
    {
        if (coroutines != null && coroutines.Count > 0)
        {
            // Destroy existing coroutines
            coroutines.ForEach(routine => StopCoroutine(routine));
        }

        coroutines = new List<IEnumerator>();

        foreach (AttackProperties currProjectile in projectiles)
        {
            var coroutine = FireProjectileCoroutine(currProjectile);
            coroutines.Add(coroutine);
            StartCoroutine(coroutine);
        }
    }

    private void SpawnProjectile(GameObject projectileReference, ProjectileProperties projectileProperties, float projectileSpawnDistance)
    {
        Vector2 targetDir = (target.transform.position - transform.position).normalized;

        // find front of enemy to instantiate bullet
        // Note: Vector3 cast into Vector2 implicitly drops z dimension, which is what we want here.
        Vector2 front = (Vector2) transform.position + targetDir * projectileSpawnDistance;

        // Instantiate the projectile with provided params
        GameObject projectile = Instantiate(projectileReference, front, Quaternion.identity);

        ProjectileProperties properties = projectileProperties;
        projectile.GetComponent<BaseProjectile>().Init(gameObject,
                                                        properties.lifeTime,
                                                        properties.damage,
                                                        properties.speed,
                                                        properties.scaleModifier,
                                                        properties.dieOnCollision,
                                                        target,
                                                        targetDir);

        // propagate the status of this object to the spawning children
        statusSystem.activeEffects.ForEach(effect =>
        {
            if (!effect.propagateToChildren)
                return;

            // Since we are unsure that the projectile will register the component
            // quick enough, we need to do that here before we apply the effect.
            if (projectile.GetComponent<StatusSystem>() is StatusSystem status)
            {
                // FIXME: Do we really need to RegisterAIComponent() here? Start() should be called on Instantiate<T>()
                //status.RegisterAIComponent(typeof(ProjectileSystem), Stats.FIRERATE, Stats.PROJECTILE_SCALE, Stats.PROJECTILE_SPEED);
                status.ApplyEffect(effect);
            }
        });
    }

    IEnumerator FireProjectileCoroutine(AttackProperties attack)
    {
        // keep the subroutine running so the enemy keeps firing
        while (true)
        {
            // if(eventTrigger == EventType.DISABLED) { break; }
            if (target)
                SpawnProjectile(attack.projectile, attack.projectileProperties, attack.projectileSpawnDistance);

            yield return new WaitForSeconds(attack.fireRate);
        }
    }

    // Fires all projectiles in projectiles array
    public void FireProjectiles(int numberOfTimes)
    {
        foreach (AttackProperties projectile in projectiles)
        {
            if (!target)
                continue;

            for (int i = 0; i < numberOfTimes; i++)
                SpawnProjectile(projectile.projectile, projectile.projectileProperties, projectile.projectileSpawnDistance);
        }
    }

    // Status system dependent functions
    private void DamageStatValue(ref float stat, float value)
    {
        stat -= value;
        stat = Mathf.Clamp(stat, 0f, float.MaxValue);
    }
    public void DamageStat(Stats stat, float amount)
    {
        foreach (AttackProperties projectile in projectiles)
        {
            switch (stat)
            {
                case Stats.FIRERATE:
                    DamageStatValue(ref projectile.fireRate, amount);
                    return;

                case Stats.PROJECTILE_SCALE:
                    DamageStatValue(ref projectile.projectileProperties.scaleModifier, amount);
                    return;

                case Stats.PROJECTILE_SPEED:
                    DamageStatValue(ref projectile.projectileProperties.speed, amount);
                    return;

                case Stats.PROJECTILE_LIFETIME:
                    DamageStatValue(ref projectile.projectileProperties.lifeTime, amount);
                    return;

                case Stats.PROJECTILE_DAMAGE:
                    DamageStatValue(ref projectile.projectileProperties.damage, amount);
                    return;

                default:
                    return;
            }
        }
    }

    private void SetStatValue(ref float stat, float value)
    {
        stat = Mathf.Clamp(value, 0f, float.MaxValue);
    }
    public void SetStat(Stats stat, float value)
    {
        foreach (AttackProperties projectile in projectiles)
        {
            switch (stat)
            {
                case Stats.FIRERATE:
                    SetStatValue(ref projectile.fireRate, value);
                    return;

                case Stats.PROJECTILE_SCALE:
                    SetStatValue(ref projectile.projectileProperties.scaleModifier, value);
                    return;

                case Stats.PROJECTILE_SPEED:
                    SetStatValue(ref projectile.projectileProperties.speed, value);
                    return;

                case Stats.PROJECTILE_LIFETIME:
                    SetStatValue(ref projectile.projectileProperties.lifeTime, value);
                    return;

                case Stats.PROJECTILE_DAMAGE:
                    SetStatValue(ref projectile.projectileProperties.damage, value);
                    return;

                default:
                    return;
            }
        }
    }

    public List<float> GetStat(Stats stat)
    {
        List<float> propertyList = new();

        foreach (AttackProperties currProjectile in projectiles)
        {
            switch (stat)
            {
                case Stats.FIRERATE:
                    propertyList.Add(currProjectile.fireRate);
                    break;

                case Stats.PROJECTILE_SCALE:
                    propertyList.Add(currProjectile.projectileProperties.scaleModifier);
                    break;

                case Stats.PROJECTILE_SPEED:
                    propertyList.Add(currProjectile.projectileProperties.speed);
                    break;

                case Stats.PROJECTILE_LIFETIME:
                    propertyList.Add(currProjectile.projectileProperties.lifeTime);
                    break;

                case Stats.PROJECTILE_DAMAGE:
                    propertyList.Add(currProjectile.projectileProperties.damage);
                    break;

                default:
                    return null;
            }
        }

        return propertyList;
    }

    public void RegCompToStatSystem()
    {
        statusSystem.RegisterAIComponent(this, Stats.FIRERATE,
                                               Stats.PROJECTILE_SCALE,
                                               Stats.PROJECTILE_SPEED,
                                               Stats.PROJECTILE_LIFETIME,
                                               Stats.PROJECTILE_DAMAGE);
    }

    public void OnDestroy()
    {
        coroutines.ForEach(routine => StopCoroutine(routine));
    }
}
