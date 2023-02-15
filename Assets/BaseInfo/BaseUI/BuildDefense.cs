using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildDefense : MonoBehaviour
{
    // Start is called before the first frame update


    public bool spawning;
    public Transform parentBase;
    public GameObject objectSpawned;
    public GameObject defense;

    private Tower towerBase;
    public SpriteRenderer defenseSprite;
    [SerializeField] private int numberOfClicks = 0;

    void Start()
    {
      
    }

    // Update is called once per frame
    void Update()
    {
        // clicked again to spawn in // currently eliminates object
        if(Input.GetMouseButton(0) && spawning == true)
        {
            print("placed object");
            numberOfClicks = 1;
            //Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //mousePosition.z = 0;
            //defense.transform.position = mousePosition;
            Vector3 placement = towerBase.place();
            defenseSprite.color = new Color(0f, 0f, 0f, 1f);
            spawning = false;
        }
    }

    private void LateUpdate()
    {
        if(defense != null && numberOfClicks == 0)
        {
            //Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //mousePosition.z = 0;
            //defense.transform.position = mousePosition;

            defenseSprite.color = new Color(1, 1, 1, .5f);
            spawning = true;
        }
    }

    public void BuyDefense()
    {
        defense = Instantiate(objectSpawned);
        defense.transform.parent = parentBase;
        towerBase = defense.AddComponent<Tower>();
        towerBase.parent = defense;
        towerBase.towerSize = Tower.TowerSize.Medium;

        defenseSprite = defense.GetComponentInChildren<SpriteRenderer>();
        numberOfClicks = 0;
    }
}
