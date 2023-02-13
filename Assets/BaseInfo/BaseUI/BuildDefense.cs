using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildDefense : MonoBehaviour
{
    // Start is called before the first frame update


    public bool spawning;
    public GameObject objectSpawned;
    public GameObject defense;
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
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0;
            defense.transform.position = mousePosition;
            //Destroy(defense);
            //defense = null;
            defenseSprite.color = new Color(1f, 1f, 1f, 1f);
            spawning = false;
        }
    }

    private void LateUpdate()
    {
        if(defense != null && numberOfClicks == 0)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0;
            defense.transform.position = mousePosition;
            defenseSprite.color = new Color(1, 1, 1, .5f);
            spawning = true;
        }
    }

    public void BuyDefense()
    {
        defense = Instantiate(objectSpawned);
        defenseSprite = defense.GetComponent<SpriteRenderer>();
        numberOfClicks = 0;
    }
}
