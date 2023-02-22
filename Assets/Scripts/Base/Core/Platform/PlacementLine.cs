using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlacementLine : MonoBehaviour
{
    [SerializeField] public Vector3 startPoint;
    [SerializeField] public Vector3 endPoint;
    [SerializeField] public Color lineColor = Color.blue;
    [SerializeField] public float lineWidth = 0.05f;
    [SerializeField] public float lineZ = 0.0f;
    [SerializeField] public GameObject parentObject;

    private GameObject lineObject;
    private LineRenderer _renderer;

    void Start()
    {
        lineObject = new GameObject("PlacementLine");
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

        _renderer.SetPositions(new Vector3[] { startPoint, endPoint });
    }

    private void OnDestroy()
    {
        Destroy(lineObject);
    }
}
