using UnityEngine;

public class Tochka : MonoBehaviour
{
    /// <summary>
    /// 衝突したCubeのみを消滅させる
    /// </summary>
    public void TakeDamage(Collider other)
    {
        other.gameObject.SetActive(false);
    }
}
