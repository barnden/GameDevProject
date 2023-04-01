using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildDefense : MonoBehaviour
{
    public bool spawning;
    public Transform parentBase;
    [SerializeField] Platform platform;
    public GameObject objectSpawned;
    public GameObject defense;

    private Tower towerBase;
    public SpriteRenderer defenseSprite;
    [SerializeField] private int numberOfClicks = 0;

    void Update()
    {
        if(Input.GetMouseButton(0) && spawning == true)
        {
            Tuple<Vector3, bool> placement = towerBase.place(defense);
            if (placement.Item2) //Placed successfully
            {
                numberOfClicks = 1;
                defense.GetComponent<AI_Base>().enabled = true; //Turn the AI on once placed
                defenseSprite.color = new Color(1f, 1f, 1f, 1f);
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
        if (!spawning)
        {
            defense = Instantiate(objectSpawned);

            towerBase = defense.AddComponent<Tower>();
            towerBase.parent = parentBase.gameObject;
            towerBase.platform = platform;
            defense.transform.parent = towerBase.getTowerBase().transform;

            defense.GetComponent<AI_Base>().enabled = false; //Turn off the AI while placing

            defenseSprite = defense.GetComponentInChildren<SpriteRenderer>();
            numberOfClicks = 0;
        }
    }

    public void MoveDefense(GameObject def)
    {
        defense = def;
        towerBase = defense.GetComponent<Tower>();
        towerBase.setPlacing(true);
        defenseSprite = defense.GetComponentInChildren<SpriteRenderer>();
        numberOfClicks = 0;
    }
}
