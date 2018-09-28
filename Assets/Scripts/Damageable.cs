using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Damageable : MonoBehaviour
{
    [System.Serializable] public class DamageEvent : UnityEvent<Damageable, Damager, float> { }
    [System.Serializable] public class HealEvent : UnityEvent<Damageable, float> { }
    [System.Serializable] public class HealthSetEvent : UnityEvent<Damageable> { }
    [System.Serializable] public class DeathEvent : UnityEvent<Damageable, Damager> { }
    [SerializeField] public float maxHealth;
    [SerializeField] public float currentHealth;

    public DamageEvent OnTakeDamage;
    public HealEvent OnHeal;
    public HealthSetEvent OnHealthSet;
    public DeathEvent OnDie;

    public float HealthRatio
    {
        get
        {
            return currentHealth / maxHealth;
        }
    }

	public bool IsAlive
	{
		get
		{
			return currentHealth > 0;
		}
	}

    public void Heal(float amount)
    {
        // Add new health.
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);

        // Invoke healing event.
        OnHeal.Invoke(this, amount);
    }

    public void SetHealth(float amount)
    {
        // Set current health.
        currentHealth = Mathf.Min(amount, maxHealth);

        // Invoke health set event.
        OnHealthSet.Invoke(this);
    }

    public void Damage(Damager damager, float damage)
    {
		if (!IsAlive) return;

        // Take damage from health.
        currentHealth -= damage;

        // Invoke damage event.
        OnTakeDamage.Invoke(this, damager, damage);

        // Check for death.
        if (!IsAlive)
        {
            currentHealth = 0;

            // Invoke death event.
            OnDie.Invoke(this, damager);
        }
    }

    private void Start()
    {
        // Assign initial health.
        currentHealth = maxHealth;
    }
}
