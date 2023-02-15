using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Numerics;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngineInternal;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class Platform : MonoBehaviour
{
    [SerializeField] private CoreData coreData;
    [SerializeField] private float coreRadius;
    [SerializeField] private float[] baseRadii;

    [SerializeField] private bool displayRing = false;
    [SerializeField] private int defaultStartingSections;
    [SerializeField] private float ringZ;
    private float ringIncrement;
    private int startingSections;

    private List<PlacementLine> placementLines = new List<PlacementLine>();
    private List<PlacementCircle> placementCircles = new List<PlacementCircle>();

    private Vector2 cursorPos;
    private float towerRadius;

    private Vector2 rotatePoint(Vector2 point, float theta)
    {
        float newX = point.x * Mathf.Cos(theta) - point.y * Mathf.Sin(theta);
        float newY = point.y = point.x * Mathf.Sin(theta) + point.y * Mathf.Cos(theta);
        return new Vector2(newX, newY);
    }

    private void drawCircle(float radius)
    {
        PlacementCircle circle = gameObject.AddComponent<PlacementCircle>();
        circle.lineZ = ringZ;
        circle.parentObject = gameObject;
        circle.radius = radius;
        placementCircles.Add(circle);
    }

    private void drawLine(float startRadius, float endRadius, float theta)
    {
        PlacementLine line = gameObject.AddComponent<PlacementLine>();
        line.lineZ = ringZ;
        line.parentObject = gameObject;
        line.startPoint = rotatePoint(new Vector2(0.0f, startRadius), theta);
        line.endPoint = rotatePoint(new Vector2(0.0f, endRadius), theta);
        placementLines.Add(line);
    }

    private void drawRing(float startRadius, float endRadius)
    {
        drawCircle(startRadius);
        drawCircle(endRadius);
        int sections = startingSections + (int)(startingSections * (startRadius - coreRadius));
        for (int section = 0; section < sections; section++)
        {
            float theta = section * ((2.0f * Mathf.PI) / sections);
            drawLine(startRadius, endRadius, theta);
        }
    }

    private void deleteRing()
    {
        foreach (PlacementLine placementLine in placementLines)
        {
            Destroy(placementLine);
        }
        placementLines.Clear();

        foreach (PlacementCircle placementCircle in placementCircles)
        {
            Destroy(placementCircle);
        }
        placementCircles.Clear();
    }

    private void Update()
    {
        float baseRadius = baseRadii[coreData.getLevel()];
        gameObject.transform.localScale = new Vector3(baseRadius * 2, baseRadius * 2, 1.0f);

        if(placementLines.Count > 0 || placementCircles.Count > 0)
        {
            deleteRing(); //This is really inefficient
        }

        if(displayRing)
        {
            Vector2 corePos = gameObject.transform.position;
            float cursorRadius = Vector2.Distance(cursorPos, corePos);
            int ringNum = (int)((cursorRadius - coreRadius) / ringIncrement);
            float innerRadius = Mathf.Max(coreRadius + ringIncrement * ringNum, coreRadius);
            float outerRadius = innerRadius + towerRadius;
            if (outerRadius <= baseRadius)
            {
                drawRing(innerRadius, outerRadius);
            }
        }
    }

    public void enableRing()
    {
        displayRing = true;
    }

    public void disableRing()
    {
        displayRing = false;
    }

    public void setTowerRadius(float tower)
    {
        towerRadius = tower;
        ringIncrement = towerRadius;
        startingSections = (int)(defaultStartingSections / towerRadius);
    }

    public void setCursorPos(Vector2 cursor)
    {
        cursorPos = cursor;
    }

    public bool pointInBase(Vector2 point)
    {
        Vector2 corePosition = gameObject.transform.position;
        float pointRadius = Vector2.Distance(point, corePosition);
        float baseRadius = baseRadii[coreData.getLevel()];
        return pointRadius < baseRadius;
    }

    public Tuple<Vector2, float, int> getSnap()
    {
        Vector2 corePos = gameObject.transform.position;
        float cursorRadius = Vector2.Distance(cursorPos, corePos);
        int ringNum = (int)((cursorRadius - coreRadius) / ringIncrement);
        float innerRadius = Mathf.Max(coreRadius + ringIncrement * ringNum, coreRadius);
        float outerRadius = innerRadius + towerRadius;

        float snapRadius = (innerRadius + outerRadius) / 2.0f;

        float angle = Mathf.Atan2(snapRadius, 0.0f) - Mathf.Atan2(cursorPos.y - corePos.y, cursorPos.x - corePos.x);
        if(angle < 0.0f)
        {
            angle += 2.0f * Mathf.PI;
        }

        int sections = startingSections + (int)(startingSections * (innerRadius - coreRadius));
        float closestTheta = 0.0f;
        for (int section = 0; section < sections; section++) //I bet there is some math to do this in O(1)
        {
            float theta = section * ((2.0f * Mathf.PI) / sections);
            if(theta < angle)
            {
                closestTheta = theta;
            }
        }

        float snapAngle = closestTheta + ((2.0f * Mathf.PI) / sections) / 2.0f;
        snapAngle = 2.0f * Mathf.PI - snapAngle; //Mirror it
        Vector2 snapPoint = rotatePoint(new Vector2(0.0f, snapRadius), snapAngle);
        return new Tuple<Vector2, float, int>(snapPoint + corePos, snapAngle, ringNum);
    }
}
