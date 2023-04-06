using System;
using System.Collections.Generic;
using UnityEngine;

public enum ModifierAction
{
    ADD,
    MULTIPLY
}

[Serializable]
public struct Modifier
{
    public Stats stat;
    public ModifierAction action;
    public double amount;
}

[Serializable]
public class ModifierSO : ScriptableObject
{
    [SerializeField]
    public List<Modifier> modifiers;

    public void Init()
    {
        modifiers = new List<Modifier>();
    }
}

[Serializable]
public class AttackPropertiesSO : ScriptableObject
{
    [SerializeField]
    public List<AttackProperties> projectiles;

    public void Init()
    {
        projectiles = new List<AttackProperties>();
    }
}

[System.Serializable]
public class UpgradeNode
{
    public string title;
    public string description;
    public Sprite sprite;
    public Sprite icon;
    public double cost;
    public List<int> prerequisites;
    public List<int> exclusion;
    public bool bought;
    public GameObject zone;
    public bool inheritProjectile;
    public List<AttackProperties> projectiles;

    public List<Modifier> modifiers;

    public Vector2 position;
    public int zindex;
    public int pindex;
    public bool inheritSprite;

    public UpgradeNode(string title, string description, Sprite sprite, Sprite icon, double cost, List<int> prerequisites, List<int> exclusion, bool bought, bool inheritProjectile, GameObject zone, List<AttackProperties> projectiles, Vector2 position)
    {
        this.title = title;
        this.description = description;
        this.sprite = sprite;
        this.prerequisites = prerequisites;
        this.exclusion = exclusion;
        this.bought = bought;
        this.position = position;
        this.cost = cost;
        this.icon = icon;
        this.zone = zone;
        this.inheritProjectile = inheritProjectile;
        this.projectiles = projectiles;

        modifiers = new List<Modifier>();
        zindex = 0;
        pindex = 0;
        inheritSprite = false;
    }

    public UpgradeNode(UpgradeNode other)
    {
        // FIXME: Is there a better way to deep-copy?
        title = other.title;
        description = other.description;
        sprite = other.sprite;
        icon = other.icon;
        cost = other.cost;

        modifiers = other.modifiers;

        prerequisites = new List<int>();
        exclusion = new List<int>();

        foreach (var upgrade in other.prerequisites)
            prerequisites.Add(upgrade);

        foreach (var upgrade in other.exclusion)
            exclusion.Add(upgrade);

        foreach (var projectile in other.projectiles)
            projectiles.Add(projectile);

        position = other.position;

        bought = other.bought;

        zone = other.zone;

        inheritProjectile = other.inheritProjectile;
        projectiles = other.projectiles;

        zindex = other.zindex;
        pindex = other.pindex;
        inheritSprite = other.inheritSprite;
    }
}