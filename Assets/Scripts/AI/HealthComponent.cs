using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class HealthComponent : MonoBehaviour, BaseAIComponent
{
    [SerializeField] public float maxHealth;
    [SerializeField] private float effectiveHealth;
    public FloatEvent OnDamageTaken;
    public FloatEvent OnHeal;
    public UnityEvent OnDeath;

    /// <summary>
    /// Initializes max health to current health.
    /// </summary>
    void Start()
    {
        effectiveHealth = maxHealth;
        RegCompToStatSystem();
    }

    /// <summary>
    /// Changes health by the value specified amount.
    /// </summary>
    /// <param name="amount">Net amount to change health by. Negative changes cause damage. Positive changes cause healing.</param>
    public void DamageStat(Stats statToDamage, float amount)
    {
        switch (statToDamage)
        {
            case Stats.HEALTH:
                effectiveHealth -= amount;

                // prevent healing past maxHealth
                // give small wiggle room to protect against float inaccuracies
                effectiveHealth = Mathf.Clamp(effectiveHealth, -0.1f, maxHealth);

                invokeReponse(amount);
                return;

            default:
                return;
        }
    }

    public void SetStat(Stats statToDamage, float value)
    {
        switch (statToDamage)
        {
            case Stats.HEALTH:
                effectiveHealth = value;

                // prevent healing past maxHealth
                // give small wiggle room to protect against float inaccuracies
                effectiveHealth = Mathf.Clamp(effectiveHealth, -0.1f, maxHealth);

                invokeReponse(value);
                return;

            default:
                return;
        }
    }

    public List<float> GetStat(Stats stat)
    {
        if (stat != Stats.HEALTH)
            return null;

        return new List<float> { effectiveHealth };
    }
    public void RegCompToStatSystem()
    {
        gameObject.GetComponent<StatusSystem>().RegisterAIComponent(this, Stats.HEALTH);
    }

    /// If healing occured the OnHeal event will be invoked.
    /// If health dropped below 0 OnDeath will be invoked.
    /// Otherwise if damage occured OnDamageTaken will be invoked.
    private void invokeReponse(float amount)
    {
        if (amount < 0)
        {
            OnHeal.Invoke(amount);
        }
        else if (effectiveHealth <= 0)
        {
            OnDeath.Invoke();
        }
        else if (amount > 0)
        {
            OnDamageTaken.Invoke(amount);
        }
    }
}
