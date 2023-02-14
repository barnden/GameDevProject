using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Draggable : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public PointerEventData.InputButton button = PointerEventData.InputButton.Left;
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

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == button)
            mMouseDown = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button == button)
            mMouseDown = false;
    }
}