using UnityEngine;
using UnityEngine.UI;

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
