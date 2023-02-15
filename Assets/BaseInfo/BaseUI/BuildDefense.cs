using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildDefense : MonoBehaviour
{
    public bool spawning;
    public Transform parentBase;
    public GameObject objectSpawned;
    public GameObject defense;

    private Tower towerBase;
    public SpriteRenderer defenseSprite;
    [SerializeField] private int numberOfClicks = 0;

    void Update()
    {
        if(Input.GetMouseButton(0) && spawning == true)
        {
            Tuple<Vector3, bool> placement = towerBase.place();
            if (placement.Item2) //Placed successfully
            {
                numberOfClicks = 1;
                defenseSprite.color = new Color(0f, 0f, 0f, 1f);
                spawning = false;
            }
        }
    }

    private void LateUpdate()
    {
        if(defense != null && numberOfClicks == 0)
        {
            defenseSprite.color = new Color(1, 1, 1, .5f);
            spawning = true;
        }
    }

    public void BuyDefense()
    {
        defense = Instantiate(objectSpawned);

        towerBase = defense.AddComponent<Tower>();
        towerBase.parent = parentBase.gameObject;
        defense.transform.parent = towerBase.getTowerBase().transform;

        defenseSprite = defense.GetComponentInChildren<SpriteRenderer>();
        numberOfClicks = 0;
    }
}
