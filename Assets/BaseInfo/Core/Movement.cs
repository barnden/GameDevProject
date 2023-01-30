using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.EventSystems;

public class Movement : MonoBehaviour
{
    private LineRenderer _renderer;
    private bool moveMode = false;

    void Start()
    {
        
    }

    void Update()
    {
        if(moveMode)
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Debug.Log(transform.position + " " + mouseWorldPos);
            Handles.DrawDottedLine(transform.position, mouseWorldPos, 10.0f);
        }
    }

    private void OnMouseDown()
    {
        Debug.Log("Clicked");
        moveMode = true;
    }
}
