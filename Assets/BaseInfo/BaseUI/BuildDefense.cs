using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildDefense : MonoBehaviour
{
    // Start is called before the first frame update


    public bool spawning;
    public GameObject objectSpawned;
    public GameObject defense;
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
            spawning = true;
        }
    }

    public void BuyDefense()
    {
        defense = Instantiate(objectSpawned);
        numberOfClicks = 0;
    }
}
