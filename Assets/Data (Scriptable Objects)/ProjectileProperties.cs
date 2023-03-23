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
}
