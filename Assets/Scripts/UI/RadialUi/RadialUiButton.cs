using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class RadialUiButton : MonoBehaviour
{
    enum ButtonType { Create, Upgrade, Move, Delete };

    [SerializeField] ButtonType buttonType;
    [SerializeField] Platform platform;
    [SerializeField] RadialUi radialUi;
    [SerializeField] CoreData coreData;

    public bool coreClicked = false;
    public Tuple<Vector2, int, int> snap;

    void Start()
    {
        gameObject.GetComponent<Button>().image.alphaHitTestMinimumThreshold = 0.5f;
    }

    public void clicked()
    {
        if (buttonType == ButtonType.Create)
        {
            gameObject.GetComponent<BuildDefense>().BuyDefense();
        }
        else if(buttonType == ButtonType.Upgrade)
        {
            if (coreClicked)
            {
                coreData.levelUp();
            }
            else
            {
                radialUi.UpgradeScreenPanel.SetActive(true);
                radialUi.BuildingPhaseOn();
                // then create amount of buttons available in tower's upgrade tree
                List<Upgrade> upgrades = radialUi.selectedGameObject.GetComponent<TurretUpgrade>().GetBuyableUpgrades();
                
                // FIXME: code better positioning of choices
                float ypos = 225f;
                float xpos = 0;
                for(int i = 0; i < upgrades.Count; ++i)
                {
                    if(ypos <= -225f)
                    {
                        ypos = 150f;
                        xpos += 100;
                    }
                    // instantiate upgrade choice buttons prefab
                    GameObject createdButton = Instantiate(radialUi.upgradeButtonPrefab, radialUi.UpgradeScreenPanel.transform);
                    createdButton.transform.localPosition = new Vector3(xpos, ypos, 0);

                    // FIXME: Figure out how to scale the icons properly
                    createdButton.transform.localScale = new Vector3(1f, 3.5f, 1f);
                    UpgradeChoice button_Upgrade = createdButton.GetComponent<UpgradeChoice>();
                    button_Upgrade.chosenUpgrade = upgrades[i];
                    button_Upgrade.radialUIref = radialUi;
                    ypos -= 125f;
                }
            
            }
        }
        if (buttonType == ButtonType.Move)
        {
            if(snap != null && platform.towerExists(snap.Item2, snap.Item3))
            {
                GameObject tower = platform.delete(snap.Item2, snap.Item3);
                gameObject.GetComponent<BuildDefense>().MoveDefense(tower);
            }
        }
        else if(buttonType == ButtonType.Delete)
        {
            if(snap != null && platform.towerExists(snap.Item2, snap.Item3))
            {
                GameObject tower = platform.delete(snap.Item2, snap.Item3);
                Destroy(tower);
            }
        }
        
        radialUi.disableInteraction(); //After click is finished processing, make the UI non-interactable
    }
}
