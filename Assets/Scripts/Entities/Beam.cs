using System;
using UnityEngine;

/**
 * Enemy, Player が射出するビーム
 */
public class Beam : MonoBehaviour, IDamageable
{
    // ここまで到達したらビームをプールに戻す
    [SerializeField]
    private float releaseYPosAbs = 20f;

    // 弾速
    [SerializeField]
    private float speed = 0.2f;

    // ビームが進む方向
    [SerializeField]
    private BeamVector Direction;

    enum BeamVector : byte
    {
        Up,
        Down
    }

    public Action<Collider> OnCollided { get; set; }

    void Update()
    {
        var vector = (Direction == BeamVector.Down) ? Vector3.down : Vector3.up;
        transform.position += vector * speed;

        // release self
        if (Mathf.Abs(transform.position.y) > releaseYPosAbs)
        {
            ObjectPool.Instance.Release(gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        OnCollided(other);
    }

    public void TakeDamage(GameObject attacker, Collider collided)
    {
        // TODO: play beam explosion effect
        // Debug.Log("TakeDamage attacker is " + attacker.name);
    }
}
