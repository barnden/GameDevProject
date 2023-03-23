using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UpgradeChoice : MonoBehaviour
{
    public RadialUi radialUIref;
    public Upgrade chosenUpgrade;
   

    private void Start()
    {
        if(chosenUpgrade != null)
        {
            this.transform.GetComponentInChildren<TextMeshProUGUI>().text = chosenUpgrade.title;
        }
       
    }
    public void ApplyUpgrade()
    {
        // apply it one time
        print("applied upgrade: " + chosenUpgrade.title);
        radialUIref.selectedGameObject.GetComponent<TowerUpgrade>().BuyUpgrade(chosenUpgrade);
        
    }
}
