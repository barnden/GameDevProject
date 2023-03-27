using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct ProjectileProperties
{
    [SerializeField] public float speed;
    [SerializeField] public float scaleModifier;
    [SerializeField] public float lifeTime;
    [SerializeField] public float damage;

    // Only used to pass down target information to projectiles
    // which spawn other projectiles
    [HideInInspector] public GameObject target;
}
