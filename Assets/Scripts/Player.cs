using System;
using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    // （デバッグ用）
    public bool Invincible;
    public bool RapidFire;

    // 移動速度
    public static readonly float Speed = 0.2F;
    // Fire時、自分のどの程度上にBeamを出現させるか
    private static readonly float BeamOffsetYRate = 0.1f;

    // 生存しているか
    public bool Alive
    {
        get;
        private set;
    }

    public float Height
    {
        get { return transform.localScale.y; }
    }

    // 敵を倒したときのcallback
    public Action<Enemy> OnEnemyDefeated { get; set; }

    private GameObject beamPrefab;

    void Awake()
    {
        beamPrefab = (GameObject)Resources.Load("Prefabs/PlayerBeam");

        // initialize
        this.Alive = true;
    }

    public void MoveRight()
    {
        transform.position += Vector3.right * Speed;
    }

    public void MoveLeft()
    {
        transform.position += Vector3.left * Speed;
    }

    public void Fire()
    {
        // the beam instance can only exist one at the same time
        if (!RapidFire && ObjectPool.Instance.CountActive(beamPrefab) != 0)
        {
            return;
        }

        var myPos = this.transform.position;
        var beamPos = myPos + (Vector3.up * Height * BeamOffsetYRate);
        var beam = ObjectPool.Instance.Get(beamPrefab, beamPos, Quaternion.identity);

        // beam callback
        beam.GetComponent<Beam>().OnCollided = (other) =>
        {
            ObjectPool.Instance.Release(beam);

            // check other is Enemy or Tochka or not
            var parent = other.transform.parent;
            if (parent != null)
            {
                Debug.Log(parent.name);

                var enemy = parent.GetComponent<Enemy>();
                if (enemy != null && enemy.Alive)
                {
                    OnEnemyDefeated(enemy);
                    enemy.Die();
                }

                var tochka = parent.GetComponent<Tochka>();
                if (tochka != null)
                {
                    tochka.TakeDamage(other);
                }
            }
        };
    }

    public void Die()
    {
        // 無敵なら死なない
        if (Invincible) return;

        this.Alive = false;
        this.StartCoroutine(StartDeadAnimation());
    }

    private IEnumerator StartDeadAnimation()
    {
        // ための一瞬
        yield return new WaitForSeconds(0.1f);

        ParticleManager.Instance.Play("Prefabs/Particles/PlayerDead",
            transform.position + (Vector3.down * 1f));

        yield return new WaitForSeconds(0.06f);

        this.gameObject.SetActive(false);
    }
}
