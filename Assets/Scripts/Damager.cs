using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Damager : MonoBehaviour {
	[System.Serializable] public class DamageEvent : UnityEvent<Damager, Damageable, float> { }
	[SerializeField] private float damage;
	
	public DamageEvent OnDamage;

	public void Damage(Damageable damageable)
	{
		// Apply damage to damageable.
		damageable.Damage(this, damage);
	}

	public void SetDamage(float damage)
	{
		this.damage = damage;
	}
}
