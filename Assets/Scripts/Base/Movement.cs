using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.EventSystems;
using System;

public class Movement : MonoBehaviour
{
    [SerializeField] float speed = 1.0f;
    [SerializeField] Map map;
    [SerializeField] Platform platform;

    private bool mouseIsUp = true;
    private bool selectMode = false;
    private bool moveMode = false;
    private Vector3 destination;

    private GameObject lineDest;
    private DottedLine dottedLine;

    void Update()
    {
        if(selectMode)
        {
            if (mouseIsUp && Input.GetMouseButtonDown(0))
            {
                selectMode = false;
                Vector3 dest = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                Tuple<float, float, float, float> borders = map.getBorders();
                float left = borders.Item1;
                float right = borders.Item2;
                float bottom = borders.Item3;
                float top = borders.Item4;

                float baseRad = platform.getBaseRadius();

                dest.x = Math.Max(left + baseRad, Math.Min(right - baseRad, dest.x));
                dest.y = Math.Max(bottom + baseRad, Math.Min(top - baseRad, dest.y));

                destination = dest;
                destination.Set(destination.x, destination.y, 0.0f); //Z must be set to 0 to prevent object from moving into the background
                addLine(destination);
                moveMode = true;
            }
        }
        if(moveMode)
        {
            transform.position = Vector3.MoveTowards(transform.position, destination, speed * Time.deltaTime);
            if(transform.position == destination)
            {
                removeLine();
                moveMode = false;
            }
        }
    }

    private void OnMouseDown()
    {
        mouseIsUp = false;
        selectMode = true;
    }

    private void OnMouseUp()
    {
        mouseIsUp = true;
    }

    void addLine(Vector3 destination)
    {
        if(lineDest != null)
        {
            removeLine();
        }

        lineDest = new GameObject("Line Destination");
        lineDest.transform.position = destination;

        dottedLine = lineDest.AddComponent<DottedLine>();
        dottedLine.source = gameObject;
        dottedLine.destination = lineDest;
        dottedLine.lineMaterial = Resources.Load("BaseAssets/DottedLine", typeof(Material)) as Material;
        dottedLine.lineColor = Color.yellow;
    }

    void removeLine()
    {
        Destroy(lineDest);
    }
}
