using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private Camera cameraObject;
    [SerializeField] private GameObject baseObject;

    [SerializeField] private bool baseCentered = true;
    [SerializeField] private bool scrollBase = true;
    [SerializeField] private float baseCenteredSize;
    [SerializeField] private float baseCenteringSpeed;

    [SerializeField] private float cameraMaxSize;
    [SerializeField] private float cameraMinSize;
    [SerializeField] private float cameraSpeed;

    void Update()
    {
        if(Input.mouseScrollDelta.y != 0)
        {
            scrollBase = false;
            cameraObject.orthographicSize = Mathf.Max(Mathf.Min(cameraObject.orthographicSize - Input.mouseScrollDelta.y, cameraMaxSize), cameraMinSize);
        }

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        gameObject.transform.position += new Vector3(horizontal * cameraSpeed, vertical * cameraSpeed, 0.0f);

        if(horizontal != 0.0f || vertical != 0.0f)
        {
            scrollBase = false;
            baseCentered = false;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            baseCentered = true;
            scrollBase = true;
        }
        
        if (baseCentered)
        {
            Vector3 pos = gameObject.transform.position;
            Vector3 basePos = baseObject.transform.position;

            Vector3 cameraDelta = new Vector3((basePos.x - pos.x) / baseCenteringSpeed, (basePos.y - pos.y) / baseCenteringSpeed, 0.0f);
            gameObject.transform.position += cameraDelta;
        }

        if(scrollBase)
        {
            cameraObject.orthographicSize += (baseCenteredSize - cameraObject.orthographicSize) / baseCenteringSpeed;
            if (baseCenteredSize == cameraObject.orthographicSize)
            {
                scrollBase = false;
            }
        }
    }
}
