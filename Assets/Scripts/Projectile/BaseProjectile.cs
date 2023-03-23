using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

// To replace old BaseProjectile once completed
public class BaseProjectile : MonoBehaviour
{
    // Tags that the bullet will check collision with
    [SerializeField] List<string> targetTags; 

    [SerializeField] List<BaseStatusEffect> effects;
    /* A projectile in which this projectile will spawn
     * useful for AOE explosions, or projectiles firing more
     * projectiles 
     */
    [SerializeField] GameObject recursiveProjectile;
    [SerializeField] bool dieOnCollision;

    public UnityEvent onCollision;

    /* Since projectile systems deal with how projectiles
     * act, with projectiles just being payloads for effects
     * and in charge of collision events, we don't get to
     * modify the properties here
     */
    private ProjectileProperties properties;

    private GameObject self; 
    private Vector2 direction;
    private ProjectileSystem projectileSysComponent;
    private void Start()
    {
        projectileSysComponent = GetComponent<ProjectileSystem>();
    }

    // Will be added once subparams within arrays can be displayed
    public void Init(GameObject self, float lifeTime, float damage, float speed, float scaleMod, GameObject target, Vector2 direction)
    {
        this.self = self;
        properties.target = target;

        properties.lifeTime = lifeTime;
        properties.damage = damage;
        properties.speed = speed;
        properties.scaleModifier = scaleMod;

        this.direction = direction;
    }

    // Legacy function
    public void Init(GameObject self)
    {
        this.self = self;
    }

    // Update is called once per frame
    void Update()
    {
        properties.lifeTime -= Time.deltaTime;
        if (properties.lifeTime <= 0)
        {
            Destroy(self);
        }
        transform.Translate(Time.deltaTime * properties.speed * direction);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (checkCollisionTags(collision))
        {
            onCollision.Invoke();

            // pass target information to projectileSystem (if there is one)
            if (projectileSysComponent != null)
            {
                projectileSysComponent.setTarget(properties.target);
                projectileSysComponent.FireProjectiles(1);
            }

            if (recursiveProjectile)
            {
                GameObject projectile = Instantiate(recursiveProjectile, transform.position, Quaternion.identity);
                projectile.GetComponent<BaseProjectile>().Init(projectile); // To be removed once custom UI for inspector is made
                projectile.GetComponent<BaseProjectile>().setDirection(new Vector3(0.0f, 0.0f, 0.0f)); // To be removed once custom UI for inspector is made
            }
            if (dieOnCollision)
            {
                Destroy(self);
            }

            //collision.GetComponent<HealthComponent>().DamageStat(Stats.HEALTH, damage);
            StatusSystem statSys = collision.GetComponent<StatusSystem>();
            if (statSys)
            {
                foreach (BaseStatusEffect currEffect in effects)
                {
                    statSys.ApplyEffect(currEffect);
                }
            }
        }
    }

    private bool checkCollisionTags(Collider2D collision)
    {
        foreach (string currTag in targetTags)
        {
            if (collision.CompareTag(currTag)) 
            { 
                return true; 
            }
        }
        return false;
    }

    public void setDirection(Vector2 direction)
    {
        this.direction = direction;
    }
}