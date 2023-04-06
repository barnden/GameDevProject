using System;
using UnityEngine;

[Serializable]
public struct ProjectileProperties
{
    public float speed;
    public float scaleModifier;
    public float lifeTime;
    public float damage;
    public bool dieOnCollision;

    // Only used to pass down target information to projectiles
    // which spawn other projectiles
    [NonSerialized] public GameObject target;
}
