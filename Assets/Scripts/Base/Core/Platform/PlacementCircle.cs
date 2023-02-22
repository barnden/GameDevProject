using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class PlacementCircle : MonoBehaviour
{
    [SerializeField] public Color lineColor = Color.blue;
    [SerializeField] public float lineWidth = 0.05f;
    [SerializeField] public float lineZ = 0.0f;
    [SerializeField] public int pointsOnCircle = 128;
    [SerializeField] public float radius = 1.0f;
    [SerializeField] public GameObject parentObject;

    private GameObject lineObject;
    private LineRenderer _renderer;

    void Start()
    {
        lineObject = new GameObject("PlacementCircle");
        lineObject.transform.parent = parentObject.transform;
        Vector3 parentPosition = parentObject.transform.position;
        lineObject.transform.position = new Vector3(parentPosition.x, parentPosition.y, lineZ);

        _renderer = lineObject.AddComponent<LineRenderer>();
        _renderer.useWorldSpace = false;
        _renderer.alignment = LineAlignment.View;

        _renderer.startColor = lineColor;
        _renderer.endColor = lineColor;

        _renderer.startWidth = lineWidth;
        _renderer.endWidth = lineWidth;

        _renderer.positionCount = pointsOnCircle;
        _renderer.loop = true;

        Vector3[] points = new Vector3[pointsOnCircle];
        for(int i = 0; i < pointsOnCircle; i++)
        {
            float theta = i * ((2.0f * Mathf.PI) / pointsOnCircle);
            points[i] = new Vector3(Mathf.Sin(theta) * radius, Mathf.Cos(theta) * radius, 0.0f);
        }
        _renderer.SetPositions(points);
    }

    private void OnDestroy()
    {
        Destroy(lineObject);
    }
}
