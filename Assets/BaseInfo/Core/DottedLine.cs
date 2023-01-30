using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DottedLine : MonoBehaviour
{
    [SerializeField] public GameObject source;
    [SerializeField] public GameObject destination;
    [SerializeField] public Material lineMaterial;
    [SerializeField] public Color lineColor = Color.white;
    [SerializeField] public float lineWidth = 0.3f;

    private GameObject lineObject;
    private LineRenderer _renderer;
    
    void Start()
    {
        lineObject = new GameObject("MovementLine");
        _renderer = lineObject.AddComponent<LineRenderer>();
        _renderer.alignment = LineAlignment.View;

        _renderer.textureMode = LineTextureMode.Tile;
        _renderer.material = lineMaterial;
        _renderer.material.mainTextureScale = new Vector2(1.0f / lineWidth, 1.0f);

        _renderer.startColor = lineColor;
        _renderer.endColor = lineColor;

        _renderer.startWidth = lineWidth;
        _renderer.endWidth = lineWidth;
    }

    void Update()
    {
        if(source != null && destination != null)
        {
            _renderer.SetPositions(new Vector3[] { destination.transform.position, source.transform.position });
            Debug.Log(_renderer.startColor + " " + _renderer.endColor);
        }
    }

    private void OnDestroy()
    {
        Destroy(lineObject);
    }
}
