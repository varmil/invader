using UnityEngine;

// can take damage
public interface IDamageable
{
	void TakeDamage(GameObject attacker, Collider collided);
}
