using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;


public class RadialUiButton : MonoBehaviour
{
    enum ButtonType { Create, Upgrade, Move, Delete };

    [SerializeField] ButtonType buttonType;
    [SerializeField] Platform platform;
    [SerializeField] RadialUi radialUi;
    [SerializeField] CoreData coreData;

    public bool coreClicked = false;

    void Start()
    {
        gameObject.GetComponent<Button>().image.alphaHitTestMinimumThreshold = 0.5f;
    }

    public void clicked()
    {
        if(buttonType == ButtonType.Create)
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
                List<Upgrade> upgrades = radialUi.selectedGameObject.GetComponent<TowerUpgrade>().GetBuyableUpgrades();
                
                // FIXME: code better positioning of choices
                float ypos = 150f;
                float xpos = 0;
                for(int i = 0; i < upgrades.Count; ++i)
                {
                    if(ypos <= -150f)
                    {
                        ypos = 150f;
                        xpos += 100;
                    }
                    // instantiate upgrade choice buttons prefab
                    GameObject createdButton = Instantiate(radialUi.upgradeButtonPrefab, radialUi.UpgradeScreenPanel.transform);
                    createdButton.transform.localPosition = new Vector3(xpos, ypos, 0);
                    UpgradeChoice button_Upgrade = createdButton.GetComponent<UpgradeChoice>();
                    button_Upgrade.chosenUpgrade = upgrades[i];
                    button_Upgrade.radialUIref = radialUi;
                    ypos -= 75f;
                }
            
            }
        }
        if (buttonType == ButtonType.Move)
        {
            if(platform.towerExists())
            {
                GameObject tower = platform.delete();
                gameObject.GetComponent<BuildDefense>().MoveDefense(tower);
            }
        }
        else if(buttonType == ButtonType.Delete)
        {
            if(platform.towerExists())
            {
                GameObject tower = platform.delete();
                Destroy(tower);
            }
        }
        
        radialUi.disableInteraction(); //After click is finished processing, make the UI non-interactable
    }
}
