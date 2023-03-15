using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Numerics;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
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
    private float towerRadius = 1.0f;
    private float ringIncrement = 1.0f; //Towers take up 1.0f radius
    private int startingSections = 4; //4,8,12 towers in radius 0,1,2

    private List<PlacementLine> placementLines = new List<PlacementLine>();
    private List<PlacementCircle> placementCircles = new List<PlacementCircle>();

    private Vector2 cursorPos;
    private CursorInfo cursorInfo;
    
    //private HashSet<(int ringNum, int section)> placedTowers = new HashSet<(int ringNum, int section)>();
    private Dictionary<(int ringNum, int section), GameObject> placedTowers = new Dictionary<(int ringNum, int section), GameObject>();

    struct CursorInfo
    {
        public float cursorRadius;

        public int ringNum;
        public float innerRadius;
        public float outerRadius;
        public float snapRadius;

        public float closestTheta;
        public int closestSection;
        public float snapAngle;

        public CursorInfo(float cursorRad, int ringN, float innerRad, float outerRad, float snapRad, float closestThet, int closestSec, float snapAngl)
        {
            cursorRadius = cursorRad;
            ringNum = ringN;
            innerRadius = innerRad;
            outerRadius = outerRad;
            snapRadius = snapRad;
            closestTheta = closestThet;
            closestSection = closestSec;
            snapAngle = snapAngl;
        }
    }

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
        
        if(Input.GetMouseButtonDown(1))
        {
            Vector2 cursorPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if(pointInBase(cursorPos))
            {
                setCursorPos(cursorPos);
            }
        }
    }

    private void setCursorInfo()
    {
        Vector2 corePos = gameObject.transform.position;
        float cursorRadius = Vector2.Distance(cursorPos, corePos);
        
        int ringNum = (int)((cursorRadius - coreRadius) / ringIncrement);
        float innerRadius = Mathf.Max(coreRadius + ringIncrement * ringNum, coreRadius);
        float outerRadius = innerRadius + towerRadius;
        float snapRadius = (innerRadius + outerRadius) / 2.0f;

        int sections = startingSections + (int)(startingSections * (innerRadius - coreRadius));
        float closestTheta = 0.0f;
        int closestSection = 0;

        float cursorAngle = Mathf.Atan2(snapRadius, 0.0f) - Mathf.Atan2(cursorPos.y - corePos.y, cursorPos.x - corePos.x);
        if (cursorAngle < 0.0f)
        {
            cursorAngle += 2.0f * Mathf.PI;
        }

        for (int section = 0; section < sections; section++) //I bet there is some math to do this in O(1)
        {
            float theta = section * ((2.0f * Mathf.PI) / sections);
            if (theta < cursorAngle)
            {
                closestTheta = theta;
                closestSection = section;
            }
        }

        float snapAngle = closestTheta + ((2.0f * Mathf.PI) / sections) / 2.0f;
        
        cursorInfo = new CursorInfo(cursorRadius, ringNum, innerRadius, outerRadius, snapRadius, closestTheta, closestSection, snapAngle);
    }

    public void setCursorPos(Vector2 cursor)
    {
        cursorPos = cursor;
        setCursorInfo();
    }

    public Tuple<Vector2, float, int> getSnap()
    {
        Vector2 corePos = gameObject.transform.position;
        cursorInfo.snapAngle = 2.0f * Mathf.PI - cursorInfo.snapAngle; //Mirror it
        Vector2 snapPoint = rotatePoint(new Vector2(0.0f, cursorInfo.snapRadius), cursorInfo.snapAngle);
        return new Tuple<Vector2, float, int>(snapPoint + corePos, cursorInfo.snapAngle, cursorInfo.ringNum);
    }

    public void enableRing()
    {
        displayRing = true;
    }

    public void disableRing()
    {
        displayRing = false;
    }

    public bool towerExists()
    {
        return placedTowers.ContainsKey((cursorInfo.ringNum, cursorInfo.closestSection));
    }

    public void place(GameObject obj)
    {
        placedTowers.Add((cursorInfo.ringNum, cursorInfo.closestSection), obj);
    }

    public GameObject delete()
    {
        GameObject obj = placedTowers[(cursorInfo.ringNum, cursorInfo.closestSection)];
        placedTowers.Remove((cursorInfo.ringNum, cursorInfo.closestSection));
        return obj;
    }

    public bool pointInBase(Vector2 point)
    {
        Vector2 corePosition = gameObject.transform.position;
        float pointRadius = Vector2.Distance(point, corePosition);
        float baseRadius = baseRadii[coreData.getLevel()];
        return pointRadius < baseRadius;
    }
}
