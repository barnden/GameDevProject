using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public GameObject parent;
    public bool placing;
    private float towerBaseOffsetZ = -0.5f;

    private Platform platform;
    private GameObject towerBase;

    private void Awake()
    {
        platform = GameObject.Find("Platform").GetComponent<Platform>();
        towerBase = new GameObject("Tower Base");
        placing = true;
    }

    private void Start()
    {
        towerBase.transform.parent = parent.transform;
    }

    private void Update()
    {
        if (placing)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 towerPos = new Vector3(mousePos.x, mousePos.y, towerBaseOffsetZ);

            if (platform.pointInBase(mousePos))
            {
                platform.setCursorPos(mousePos);
                platform.enableRing();
                
                Tuple<Vector2, float, int> snapPos = platform.getSnap();
                towerPos = new Vector3(snapPos.Item1.x, snapPos.Item1.y, towerBaseOffsetZ);

                float snapPosDeg = snapPos.Item2 / (2.0f * Mathf.PI) * 360;
                towerBase.transform.eulerAngles = new Vector3(0.0f, 0.0f, snapPosDeg);

                int ringNum = snapPos.Item3;
            }
            else
            {
                towerBase.transform.eulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
                platform.disableRing();
            }
            
            towerBase.transform.position = towerPos;
        }
    }

    public GameObject getTowerBase()
    {
        return towerBase;
    }

    public void setPlacing(bool p)
    {
        placing = p;
    }

    public Tuple<Vector3, bool> place(GameObject obj)
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        
        if (platform.pointInBase(mousePos) && !platform.towerExists())
        {
            platform.disableRing();
            platform.place(obj);
            placing = false;
            return new Tuple<Vector3, bool>(towerBase.transform.position, true);
        }

        return new Tuple<Vector3, bool>(new Vector3(), false);
    }

    private void OnDestroy()
    {
        Destroy(towerBase);
    }
}
