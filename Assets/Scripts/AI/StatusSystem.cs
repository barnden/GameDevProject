using System;
using System.Collections.Generic;
using UnityEngine;

// Handles an AI status's such as damage, DOT, ect...
public class StatusSystem : MonoBehaviour
{
    [System.NonSerialized]
    public GameObject entityRoot;
    public List<BaseStatusEffect> activeEffects = new List<BaseStatusEffect>();
    private Dictionary<Stats, Type> handlers = new Dictionary<Stats, Type>();

    void Start()
    {
        entityRoot = gameObject;
    }

    // Apply status effect to the affected component
    public void ApplyEffect(BaseStatusEffect se)
    {
        // No need to keep track of the se if it's only going
        // to apply permanent damage once and never again
        if ((se is StatDmgInstant || se.isStackable) && !activeEffects.Contains(se))
            activeEffects.Add(se);

        se.Apply(this, GetComponent(GetAIComponentType(se.statToEffect)) as BaseAIComponent);
    }

    public void RemoveEffect(BaseStatusEffect se)
    {
        activeEffects.Remove(se);
    }

    // Helpers
    public void RegisterAIComponent<T>(T component, params Stats[] stats) where T : BaseAIComponent
    {
        foreach (Stats stat in stats)
        {
            handlers[stat] = typeof(T);
        }
    }

    public Type GetAIComponentType(Stats stat)
    {
        if (!handlers.ContainsKey(stat)) {
            Debug.LogError($"Attempted to retrieve nonexistent AI component for {stat}.");
            return null;
        }

        return handlers[stat];
    }

    public void DamageStat(BaseAIComponent component, Stats stat, float val)
    {
        if (component == null)
            return;

        component.DamageStat(stat, val);
    }

    public void DamageStat(Stats stat, float val) => DamageStat(GetAIComponent(stat), stat, val);
    public void DamageStatPercent(Stats stat, float val)
    {
        float statValue = GetStat(stat);
        DamageStat(stat, statValue * val);
    }

    public void SetStat(BaseAIComponent component, Stats stat, float val)
    {
        if (component == null)
            return;

        component.SetStat(stat, val);
    }

    public void SetStat(Stats stat, float val) => SetStat(GetAIComponent(stat), stat, val);

    public float GetStat(BaseAIComponent component, Stats stat, float defaultValue=0f)
    {
        if (component == null)
            return defaultValue;

        return component.GetStat(stat);
    }

    public float GetStat(Stats stat, float defaultValue=0f) => GetStat(GetAIComponent(stat), stat, defaultValue);


    // Helpers
    public BaseAIComponent GetAIComponent(Stats stat)
    {
        var componentType = GetAIComponentType(stat);

        if (componentType == null)
            return null;

        return GetComponent(componentType) as BaseAIComponent;
    }

    public float this[Stats stat]
    {
        get => GetStat(stat, 0f);
        set => SetStat(stat, value);
    }
}
