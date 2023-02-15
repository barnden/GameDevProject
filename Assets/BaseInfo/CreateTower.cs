using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateTower : MonoBehaviour
{
    [SerializeField] public KeyCode keyCode;
    [SerializeField] public Tower.TowerSize towerSize;

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
            towerBase.towerSize = towerSize;

            placing = true;
        }

        if(placing && Input.GetMouseButtonDown(0)) //Left click
        {
            Vector3 placement = towerBase.place();
            placing = false;
        }
    }
}
