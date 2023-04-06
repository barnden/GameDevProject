using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Upgrade
{
    // Wrapper class for UpgradeNode with immutable members
    public string title { get; }
    public string description { get; }
    public double cost { get; }
    public int index { get; }
    public Sprite sprite { get;  }
    public Sprite icon { get;  }

    public Upgrade(UpgradeNode node, int index)
    {
        title = node.title;
        description = node.description;
        cost = node.cost;
        sprite = node.sprite;
        icon = node.icon;
        
        this.index = index;
    }
};

public class TurretUpgrade : MonoBehaviour
{
    // Reference Upgrade Tree
    [SerializeField]
    private UpgradeTree upgradeTree;

    [SerializeField]
    private CoreData coreData;

    // Private instance of upgrade tree
    private UpgradeTree tree = null;

    public bool BuyUpgrade(int idx, bool free=false) {
        if (tree == null)
        {
            Debug.LogWarning("Attempted to buy upgrade before tree instantiation.");
            return false;
        }

        UpgradeNode node = tree[idx];

        Debug.Log($"Buying: \"{node.title}\"");

        // Get StatusSystem component
        var statusSystem = gameObject.GetComponent<StatusSystem>();
        if (!statusSystem)
        {
            Debug.LogError("No status system found on tower game object.");
            return false;
        }

        // Set node as owned
        node.bought = true;

        // Remove power
        if (!free && coreData.getEnergy() < node.cost)
        {
            coreData.removeEnergy((float)node.cost);
        }

        // Resolve effective sprite
        if (tree.GetEffectiveSprite(node) is Sprite effectiveSprite)
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = effectiveSprite;
        }

        foreach (Modifier mod in node.modifiers)
        {
            var amount = mod.amount;

            if (mod.action == ModifierAction.MULTIPLY)
            {
                foreach (float currStat in statusSystem.GetStat(mod.stat))
                {
                    statusSystem.SetStat(mod.stat, (float)(currStat * mod.amount));
                }
            }
            else
            {
                statusSystem.DamageStat(mod.stat, (float) amount);
            }

            Debug.Log($"[Upgrade] {node.title} Applying modifier {mod.stat} {mod.action} {mod.amount}. Updated value: {statusSystem.GetStat(mod.stat)}");
        }

        if (gameObject.GetComponent<ProjectileSystem>() is ProjectileSystem projectileSystem)
        {
            // Clear all projectiles on turret and add effective projectiles
            ref List<AttackProperties> projectiles = ref projectileSystem.projectiles;
            projectiles.Clear();

            foreach (AttackProperties p in tree.GetEffectiveProjectiles(node))
            {
                // Add projectile from effective projectiles on upgrade

                projectiles.Add(p);

                // FIXME: Find some way to implement damage on projectiles.
                //// Create status effect for damaging health.
                //// Value of this status effect is PROJECTILE_DAMAGE
                //if (p.projectile.GetComponent<BaseProjectile>() is BaseProjectile projectile)
                //{
                //    // Add projectile damage here?
                //}
            }

            projectileSystem.ResetSystem();
        }


        return true;
    }

    public void BuyUpgrade(Upgrade upgrade) => BuyUpgrade(upgrade.index);

    public List<Upgrade> GetBuyableUpgrades(bool checkPower = false)
    {
        var buyableIndicies = tree.GetBuyable();

        // If checkPower is true then return only the upgrades that can be bought for the current amount of power
        if (checkPower)
            buyableIndicies.RemoveAll(i => tree[i].cost <= coreData.getEnergy());

        // Extract UpgradeNode information to immutable Upgrade wrapper
        // FIXME: Is this even needed?
        var buyable = buyableIndicies.Select(i => new Upgrade(tree[i], i)).ToList();

        return buyable;
    }

    void Start()
    {
        // Hydrate new instance of UpgradeTree with existing upgrade tree
        tree = ScriptableObject.CreateInstance<UpgradeTree>();
        tree.Init(upgradeTree);

        GetComponent<SpriteRenderer>().sprite = tree[0].sprite;
    }
}
