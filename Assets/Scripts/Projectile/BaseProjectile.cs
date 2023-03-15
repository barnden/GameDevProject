using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// To replace old BaseProjectile once completed
public class BaseProjectile : MonoBehaviour
{
    [SerializeField] List<string> targetTags; // Tags that the bullet will check collision with

    [SerializeField] List<BaseStatusEffect> effects;
    /* A projectile in which this projectile will spawn
     * useful for AOE explosions, or projectiles firing more
     * projectiles 
     */
    [SerializeField] GameObject recursiveProjectile;
    [SerializeField] bool dieOnCollision;
    [SerializeField] float lifeTime;
    [SerializeField] float speed;
    [SerializeField] float scale; // Changes the scale from the default object
    

    private GameObject self; 
    private Vector2 direction;

    // Will be added once subparams within arrays can be displayed
    /*
    public void Init(GameObject self, float lifeTime, float speed, float damage, float AOESize, Vector2 direction)
    {
        this.lifeTime = lifeTime;
        this.speed = speed;
        this.damage = damage;
        this.scale = AOESize;
        this.direction = direction;
        this.self = self;
    }
    */
    public void Init(GameObject self)
    {
        this.self = self;
    }

    // Update is called once per frame
    void Update()
    {
        lifeTime -= Time.deltaTime;
        if (lifeTime <= 0)
        {
            Destroy(self);
        }
        transform.Translate(Time.deltaTime * speed * direction);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (checkCollisionTags(collision))
        {
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