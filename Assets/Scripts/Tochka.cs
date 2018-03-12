using UnityEngine;

public class Tochka : MonoBehaviour, IDamageable
{
    /// <summary>
    /// 衝突したCubeのみを消滅させる
    /// </summary>
	public void TakeDamage(GameObject attacker, Collider other)
    {
        other.gameObject.SetActive(false);
    }
}
