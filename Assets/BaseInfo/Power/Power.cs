using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Power", fileName ="Power")]
public class Power : ScriptableObject
{
    private float currentPower = 0.0f;
    [SerializeField]
    private float maxPower = 50.0f;

    void OnEnable()
    {
        currentPower = 0.0f;
    }

    public float getPower()
    {
        return currentPower;
    }

    public float getMaxPower()
    {
        return maxPower;
    }

    public void addPower(float power)
    {
        currentPower += power;
        currentPower = Math.Min(currentPower, maxPower); //Don't go over max power
    }

    public void removePower(float power)
    {
        if(currentPower >= power) //Don't go under 0 power
        {
            currentPower -= power;
        }
    }
}
