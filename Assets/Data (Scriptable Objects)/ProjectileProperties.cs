using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct ProjectileProperties
{
    [SerializeField] float speed;
    [SerializeField] float scaleModifier;
    [SerializeField] float lifeTime;
    [SerializeField] float damage;
}
