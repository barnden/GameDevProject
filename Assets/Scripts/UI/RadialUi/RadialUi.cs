using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RadialUi : MonoBehaviour
{
    [SerializeField] GameObject createButton;
    [SerializeField] GameObject upgradeButton;
    [SerializeField] GameObject moveButton;
    [SerializeField] GameObject deleteButton;
    [SerializeField] Platform platform;
    public GameObject uiWorldPos;

    [Header("What Player Mouse Selected")]
    public GameObject selectedGameObject;
    public GameObject UpgradeScreenPanel;
    public GameObject upgradeButtonPrefab;
    private bool open;

    private void Start()
    {
        createButton.GetComponent<CanvasGroup>().alpha = 0.0f;
        upgradeButton.GetComponent<CanvasGroup>().alpha = 0.0f;
        moveButton.GetComponent<CanvasGroup>().alpha = 0.0f;
        deleteButton.GetComponent<CanvasGroup>().alpha = 0.0f;

        disableInteraction();

        open = false;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && open)
        {
            setAlpha(0.0f);
            Destroy(uiWorldPos);
            open = false;
            BuildingPhaseOff();
        }
      
        
        if (Input.GetMouseButtonDown(1))
        {
            Vector2 cursorPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            uiWorldPos = new GameObject("Radial UI World Pos");
            uiWorldPos.transform.parent = platform.transform;

            if (platform.pointInBase(cursorPos))
            {
                platform.setCursorPos(cursorPos);
                Tuple<Vector2, float, int> snap = platform.getSnap();
                
                bool onCore = platform.pointInCore(cursorPos);
                upgradeButton.GetComponent<RadialUiButton>().coreClicked = onCore;
                if(onCore)
                {
                    uiWorldPos.transform.position = platform.transform.position;
                }
                else
                {
                    uiWorldPos.transform.position = snap.Item1;
                }

                bool onTower = platform.towerExists();
                if (onTower && !onCore)
                {
                    GameObject towerSelected = platform.getTower();
                    selectedGameObject = towerSelected;
                }

                createButton.GetComponent<Button>().interactable = !onTower && !onCore;
                upgradeButton.GetComponent<Button>().interactable = onTower || onCore;
                moveButton.GetComponent<Button>().interactable = onTower && !onCore;
                deleteButton.GetComponent<Button>().interactable = onTower && !onCore;
            }
            else
            {
                uiWorldPos.transform.position = cursorPos;
                createButton.GetComponent<Button>().interactable = true;
                upgradeButton.GetComponent<Button>().interactable = false;
                moveButton.GetComponent<Button>().interactable = false;
                deleteButton.GetComponent<Button>().interactable = false;
            }

            setAlpha(1.0f);
            open = true;
            BuildingPhaseOn();
        }

        if(uiWorldPos)
        {
            gameObject.transform.position = Camera.main.WorldToScreenPoint(uiWorldPos.transform.position);
        }
    }

    public void setAlpha(float alpha)
    {
        createButton.GetComponent<CanvasGroup>().alpha = alpha;
        upgradeButton.GetComponent<CanvasGroup>().alpha = alpha;
        moveButton.GetComponent<CanvasGroup>().alpha = alpha;
        deleteButton.GetComponent<CanvasGroup>().alpha = alpha;
    }

    public void disableInteraction()
    {
        createButton.GetComponent<Button>().interactable = false;
        upgradeButton.GetComponent<Button>().interactable = false;
        moveButton.GetComponent<Button>().interactable = false;
        deleteButton.GetComponent<Button>().interactable = false;
    }

    public void BuildingPhaseOn()
    {
        Time.timeScale = 0.0f;
    }
    public void BuildingPhaseOff()
    {
        Time.timeScale = 1f;
    }
    public void UpgradeModeOff()
    {
        // delete all upgrade choice buttons
        foreach (Transform child in UpgradeScreenPanel.transform)
        {
            if (child.name != "Close")
            {
                Destroy(child.gameObject);
            }

        }
    }
    
}
