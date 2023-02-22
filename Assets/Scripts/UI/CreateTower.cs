using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateTower : MonoBehaviour
{
    [SerializeField] public KeyCode keyCode;

    private Platform platform;
    private GameObject tower;
    private Tower towerBase;
    private bool placing = false;

    private void Start()
    {
        platform = GameObject.Find("Platform").GetComponent<Platform>();
    }

    void Update()
    {
        if (Input.GetKeyDown(keyCode) && !placing)
        {
            tower = new GameObject("Tower");
            tower.transform.parent = platform.transform;

            towerBase = tower.AddComponent<Tower>();
            towerBase.parent = tower;

            placing = true;
        }

        if(placing && Input.GetMouseButtonDown(0)) //Left click
        {
            Tuple<Vector3, bool> placement = towerBase.place();
            Debug.Log(placement.Item1 + " " + placement.Item2);
            if(placement.Item2)
            {
                placing = false;
            }
        }
    }
}
