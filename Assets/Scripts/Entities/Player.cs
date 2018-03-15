using System;
using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour, IDamageable
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
    public Action<IEnemy> OnEnemyDefeated { get; set; }

    // 自機の死亡処理が始まった際のcallback
    public Action OnDeadAnimationStart { get; set; }

    // callback when the object should be invisible
    public Action OnDead { get; set; }

    // 自機の死亡処理が終わった後のcallback
    public Action OnDeadAnimationEnd { get; set; }

    private GameObject beamPrefab;
    private MeshFilter meshFilter;

    void Awake()
    {
        beamPrefab = (GameObject)Resources.Load("Prefabs/PlayerBeam");

        // get MeshFilter, its renderer material is used when Player is dead
        meshFilter = transform.GetComponentInChildren<MeshFilter>();

        Initialize();
    }

    public void Initialize()
    {
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
            // check other is Enemy or Tochka or not
            var parent = other.transform.parent;
            if (parent != null)
            {
                var damageable = parent.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    damageable.TakeDamage(gameObject, other);
                }

                var enemy = parent.GetComponent<IEnemy>();
                if (enemy != null)
                {
                    OnEnemyDefeated(enemy);
                }
            }

            ObjectPool.Instance.Release(beam);
        };
    }

    public void TakeDamage(GameObject attacker, Collider collided)
    {
        StartCoroutine(Die());
    }

    private IEnumerator Die()
    {
        // 無敵なら死なない
        if (Invincible)
            yield break;

        this.Alive = false;

        OnDeadAnimationStart();
        yield return StartCoroutine(StartDeadAnimation());
        OnDeadAnimationEnd();
    }

    private IEnumerator StartDeadAnimation()
    {
        // material is switched Red color temporally
        ParticleManager.Instance.Play("Prefabs/Particles/PlayerDead",
            transform.position + (Vector3.down * 1f),
            meshFilter.GetComponent<Renderer>().material);

        yield return new WaitForSeconds(0.06f);

        OnDead();

        // アニメーション終了まで適当に待つ
        yield return new WaitForSeconds(1f);
    }
}
