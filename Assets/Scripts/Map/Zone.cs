using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zone : MonoBehaviour
{
    [SerializeField] float difficultyMultiplier;

    public Tuple<float, float, float, float> getBorders()
    {
        Vector3 pos = transform.position;
        Vector3 scale = transform.localScale;
        return new Tuple<float, float, float, float>(pos.x - scale.x / 2.0f, pos.x + scale.x / 2.0f, pos.y - scale.y / 2.0f, pos.y + scale.y / 2.0f);
    }
}
