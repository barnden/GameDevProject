using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UpgradeOverlay : MonoBehaviour
{
    public PauseMenu pauseRef;
    public bool isUpgrading;
    public bool isScreen;
    // Start is called before the first frame update

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(1))
        {
            BuildingPause();
            // make 2D ray, needs a 2D collider on the object that the ray will hit
            RaycastHit2D rayIntersect = Physics2D.GetRayIntersection(Camera.main.ScreenPointToRay(Input.mousePosition));
            if(rayIntersect.collider != null)
            {
                if(rayIntersect.collider.CompareTag("Base"))
                {
                    // show base exclusive upgrades
                    print("Hit Base or Turrets");
                }
            }
            // right click hit the screen
            else
            {
                print("hit the screen");
            }
            
            if(isScreen) 
            {

            }
            else
            {

            }
        }
    }

    public void BuildingPause()
    {
        if(isUpgrading == false)
        {
            Time.timeScale = 0f;
            isUpgrading = true;
        }
        
        else
        {
            Time.timeScale = 1f;
            isUpgrading = false;
        }
    }

}
