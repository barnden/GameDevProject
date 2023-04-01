using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class Map : MonoBehaviour
{
    [SerializeField] public Zone[] zones;
    [SerializeField] UpgradeTower[] upgradeTowers;
    [SerializeField] GameObject baseCore;

    public Zone getCurrentZone()
    {
        foreach(Zone zone in zones) {
            Vector3 basePos = baseCore.transform.position;
            Tuple<float, float, float, float> borders = zone.getBorders();
            float left = borders.Item1;
            float right = borders.Item2;
            float bottom = borders.Item3;
            float top = borders.Item4;
            if(left <= basePos.x && basePos.x <= right && bottom <= basePos.y && basePos.y <= top)
            {
                return zone;
            }
        }

        return null;
    }

    public UpgradeTower getCurrentUpgradeTower()
    {
        foreach (UpgradeTower upgradeTower in upgradeTowers)
        {
            Vector3 basePos = baseCore.transform.position;
            float baseTowerRad = Vector2.Distance(upgradeTower.transform.position, basePos);
            if(baseTowerRad <= upgradeTower.transform.GetChild(0).transform.localScale.x)
            {
                return upgradeTower;
            }
        }

        return null;
    }

    public Tuple<float, float, float, float> getBorders()
    {
        float minLeft = float.PositiveInfinity;
        float maxRight = float.NegativeInfinity;
        float minBottom = float.PositiveInfinity;
        float maxTop = float.NegativeInfinity;

        foreach(Zone zone in zones)
        {
            Tuple<float, float, float, float> borders = zone.getBorders();
            minLeft = Math.Min(minLeft, borders.Item1);
            maxRight = Math.Max(maxRight, borders.Item2);
            minBottom = Math.Min(minBottom, borders.Item3);
            maxTop = Math.Max(maxTop, borders.Item4);
        }

        return new Tuple<float, float, float, float>(minLeft, maxRight, minBottom, maxTop);
    }
}
