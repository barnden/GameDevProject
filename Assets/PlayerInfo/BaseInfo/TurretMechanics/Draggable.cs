using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Draggable : MonoBehaviour
{
    private bool mMouseDown = false;

    void Update()
    {
        Vector3 worldCoord;

        if (mMouseDown)
        {
            worldCoord = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            worldCoord.z = 0.0f;

            transform.position = worldCoord;
        }
    }

    void OnMouseDown()
    {
        mMouseDown = true;
    }

    void OnMouseUp()
    {
        mMouseDown = false;
    }
}
