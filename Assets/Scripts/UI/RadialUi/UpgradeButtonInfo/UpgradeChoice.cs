using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeChoice : MonoBehaviour
{
    public RadialUi radialUIref;
    public Upgrade chosenUpgrade;
   

    private void Start()
    {
        if(chosenUpgrade != null)
        {
            transform.GetComponentInChildren<TextMeshProUGUI>().text = "";
            gameObject.GetComponent<Image>().sprite = chosenUpgrade.icon;
        }
       
    }
    public void ApplyUpgrade()
    {
        // apply it one time
        print("applied upgrade: " + chosenUpgrade.title);
        radialUIref.selectedGameObject.GetComponent<TurretUpgrade>().BuyUpgrade(chosenUpgrade);
    }
}
