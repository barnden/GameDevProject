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

    void Update()
    {
        if(selectMode)
        {
            if (mouseIsUp && Input.GetMouseButtonDown(0))
            {
                selectMode = false;
                destination = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                destination.Set(destination.x, destination.y, 0.0f); //Z must be set to 0 to prevent object from moving into the background
                moveMode = true;
            }
        }
        else if(moveMode)
        {
            transform.position = Vector3.MoveTowards(transform.position, destination, speed * Time.deltaTime);
            if(transform.position == destination)
            {
                moveMode = false;
            }
        }
    }

    private void OnMouseDown()
    {
        mouseIsUp = false;
        Debug.Log("Clicked");
        selectMode = true;
    }

    private void OnMouseUp()
    {
        mouseIsUp = true;
    }
}
