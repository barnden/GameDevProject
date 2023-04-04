using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

// Handles an AI status's such as damage, DOT, ect...
public class StatusSystem : MonoBehaviour
{
    [NonSerialized]
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
        if (se is not StatDmgInstant && (se.isStackable && !activeEffects.Contains(se)))
        {
            activeEffects.Add(se);
        }

        se.Apply(this, GetComponent(GetAIComponentType(se.statToEffect)) as BaseAIComponent);
    }

    public void RemoveEffect(BaseStatusEffect se)
    {
        activeEffects.Remove(se);
    }

    // Helpers
    public void RegisterAIComponent(Type componentType, params Stats[] stats)
    {
        foreach (Stats stat in stats)
        {
            handlers[stat] = componentType;
        }
    }

    public void RegisterAIComponent<T>(T _, params Stats[] stats) where T : BaseAIComponent => RegisterAIComponent(typeof(T), stats);

    public Type GetAIComponentType(Stats stat)
    {
        if (!handlers.ContainsKey(stat))
        {
            Debug.LogError($"Attempted to retrieve nonexistent AI component for {stat} on {gameObject}.");
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
        List<float> values = GetStat(stat);

        /**
         * FIXME: Does this actually do what we want?
         * 
         * The Linq code has functional equivalence to the original foreach:
         *   List<float> statsToEffect = GetStat(stat);
         *   foreach (float statValue in statsToEffect)
         *   {
         *       DamageStat(stat, statValue * val);
         *   }
        */
        values.ForEach(x => DamageStat(stat, x * val));

    }

    public void SetStat(BaseAIComponent component, Stats stat, float val)
    {
        if (component == null)
            return;

        component.SetStat(stat, val);
    }

    public void SetStat(Stats stat, float val) => SetStat(GetAIComponent(stat), stat, val);

    public List<float> GetStat(BaseAIComponent component, Stats stat, float defaultValue = 0f)
    {
        if (component == null)
            return new List<float> { defaultValue };

        return component.GetStat(stat);
    }

    public List<float> GetStat(Stats stat, float defaultValue = 0f) => GetStat(GetAIComponent(stat), stat, defaultValue);


    // Helpers
    public BaseAIComponent GetAIComponent(Stats stat)
    {
        var componentType = GetAIComponentType(stat);

        if (componentType == null) {
            Debug.LogError($"Could not find component associated with {stat} on {gameObject}.");
            return null;
        }

        return GetComponent(componentType) as BaseAIComponent;
    }
}