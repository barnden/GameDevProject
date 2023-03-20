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

        se.Apply(this, GetComponent(GetAIComponent(se.statToEffect)) as BaseAIComponent);
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

    public Type GetAIComponent(Stats stat)
    {
        if (!handlers.ContainsKey(stat))
            return null;

        return handlers[stat];
    }

    public void DamageStat(BaseAIComponent component, Stats stat, float val)
    {
        if (component == null)
            return;

        component.DamageStat(stat, val);
    }

    public void DamageStat(Stats stat, float val) => DamageStat(GetComponent(GetAIComponent(stat)) as BaseAIComponent, stat, val);

    public void SetStat(BaseAIComponent component, Stats stat, float val)
    {
        if (component == null)
            return;

        component.SetStat(stat, val);
    }

    public void SetStat(Stats stat, float val) => SetStat(GetComponent(GetAIComponent(stat)) as BaseAIComponent, stat, val);

    public float GetStat(BaseAIComponent component, Stats stat, float defaultValue=0f)
    {
        if (component == null)
            return defaultValue;

        return component.GetStat(stat);
    }

    public float GetStat(Stats stat, float defaultValue=0f) => GetStat(GetComponent(GetAIComponent(stat)) as BaseAIComponent, stat, defaultValue);

    public float this[Stats stat]
    {
        get => GetStat(stat, 0f);
        set => SetStat(stat, value);
    }
}
