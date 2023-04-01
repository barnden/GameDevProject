using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public GameObject parent;
    public Platform platform;

    private bool placing;
    private float towerBaseOffsetZ = -0.5f;
    private GameObject towerBase;

    private void Awake()
    {
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
            Tuple<Vector2, int, int> snap = platform.getSnap(mousePos);

            if(snap != null)
            {
                towerPos = new Vector3(snap.Item1.x, snap.Item1.y, towerBaseOffsetZ);
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
        Tuple<Vector2, int, int> snapPos = platform.getSnap(mousePos);

        if (snapPos != null && !platform.towerExists(snapPos.Item2, snapPos.Item3))
        {
            platform.place(obj, snapPos.Item2, snapPos.Item3);
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
