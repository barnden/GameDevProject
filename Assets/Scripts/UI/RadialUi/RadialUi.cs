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
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Tuple<Vector2, int, int> snap = platform.getSnap(mousePos);
 
            uiWorldPos = new GameObject("Radial UI World Pos");
            uiWorldPos.transform.parent = platform.transform;

            bool onCore = platform.pointInCore(mousePos);
            upgradeButton.GetComponent<RadialUiButton>().coreClicked = onCore;
            bool onTower = (snap != null && platform.towerExists(snap.Item2, snap.Item3));
            moveButton.GetComponent<RadialUiButton>().snap = snap;
            deleteButton.GetComponent<RadialUiButton>().snap = snap;

            if (onCore)
            {
                uiWorldPos.transform.position = platform.transform.position;
            }
            else if(onTower)
            {
                uiWorldPos.transform.position = snap.Item1;
                GameObject towerSelected = platform.getTower(snap.Item2, snap.Item3);
                selectedGameObject = towerSelected;
            }
            else
            {
                uiWorldPos.transform.position = mousePos;
            }

            createButton.GetComponent<Button>().interactable = !onTower && !onCore;
            upgradeButton.GetComponent<Button>().interactable = onTower || onCore;
            moveButton.GetComponent<Button>().interactable = onTower;
            deleteButton.GetComponent<Button>().interactable = onTower;

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
