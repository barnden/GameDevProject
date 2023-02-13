using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemyBullet : MonoBehaviour
{
    //public GameObject projectile;
    [SerializeField] float lifeTime;
    [SerializeField] float Speed;
    [SerializeField] float damage;
    [SerializeField] float AOESize;
    private GameObject self; 
    private Vector2 direction;

    public void Init(GameObject self, float lifeTime, float speed, float damage, float AOESize, Vector2 direction)
    {
        this.lifeTime = lifeTime;
        this.Speed = speed;
        this.damage = damage;
        this.AOESize = AOESize;
        this.direction = direction;
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
        transform.Translate(Time.deltaTime * Speed * direction);
    }
}
