using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Upgrade
{
    public string title { get;  set; }
    public string description { get; set; }
    public double cost { get; set; }
    public int index { get; set; }

    public Upgrade(UpgradeNode node)
    {
        title = node.title;
        description = node.description;
        cost = node.cost;
    }
};

public class TowerUpgrade : MonoBehaviour
{
    // Reference Upgrade Tree
    [SerializeField]
    private UpgradeTree upgradeTree;

    [SerializeField]
    private CoreData coreData;

    // Private instance of upgrade tree
    private UpgradeTree tree = null;

    public void BuyUpgrade(int idx) {
        if (tree == null)
        {
            Debug.LogWarning("Attempted to buy upgrade before tree instantiation.");
            return;
        }

        UpgradeNode node = tree[idx];

        Debug.Log($"Buying: \"{node.title}\"");

        // Remove power
        // TODO: Should we error out if cost > energy?
        coreData.removeEnergy((float) node.cost);

        // Set node as owned
        node.bought = true;

        // Resolve effective sprite
        int parent = -1;

        if (node.inheritSprite)
            parent = tree.GetParentSprite(idx);


        Sprite effectiveSprite = (parent == -1) ? node.sprite : tree[parent].sprite;

        if (effectiveSprite != null)
            gameObject.GetComponent<SpriteRenderer>().sprite = effectiveSprite;

        // TODO: Apply upgrade status effects
    }

    public void BuyUpgrade(Upgrade upgrade) => BuyUpgrade(upgrade.index);

    public List<Upgrade> GetBuyableUpgrades(bool checkPower = false)
    {
        var buyableIndicies = tree.GetBuyable();

        // If checkPower is true then return only the upgrades that can be bought for the current amount of power
        if (checkPower)
            buyableIndicies.RemoveAll(i => tree[i].cost <= coreData.getEnergy());

        // Extract information
        var buyable = buyableIndicies.Select(i => {
            var node = tree[i];

            var upgrade = new Upgrade(node);
            upgrade.index = i;

            return upgrade;
        }).ToList();

        return buyable;
    }

    // Start is called before the first frame update
    void Start()
    {
        // Hydrate new instance of UpgradeTree with existing upgrade tree
        tree = ScriptableObject.CreateInstance<UpgradeTree>();
        tree.Init(upgradeTree);
    }
}
