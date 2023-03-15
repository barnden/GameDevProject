using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.EventSystems;

public class Movement : MonoBehaviour
{
    [SerializeField] float speed = 1.0f;

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
                destination = Camera.main.ScreenToWorldPoint(Input.mousePosition);
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
