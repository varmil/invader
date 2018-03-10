using System;
using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    // 移動速度
    public static readonly float Speed = 0.2F;
    // Fire時、自分のどの程度上にBeamを出現させるか
    private static readonly float BeamOffsetYRate = 0.1f;

    // （デバッグ用）
    public bool Invincible;
    public bool RapidFire;

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
    private GameObject deadEffect;
    private ParticleSystem deadEffectParticle;

    void Awake()
    {
        beamPrefab = (GameObject)Resources.Load("Prefabs/PlayerBeam");
        var deadEffectPrefab = (GameObject)Resources.Load("Prefabs/Particles/PlayerDead");

        // warm up particle
        deadEffect = (GameObject)Instantiate(deadEffectPrefab, transform.position, Quaternion.identity);
        deadEffectParticle = deadEffect.GetComponent<ParticleSystem>();

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

            // check other is Enemy or not
            var enemy = other.gameObject.GetComponent<Enemy>();
            if (enemy != null)
            {
                OnEnemyDefeated(enemy);
                enemy.Die();
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

        deadEffect.transform.localPosition = transform.localPosition;
        deadEffectParticle.Play();

        yield return new WaitForSeconds(0.06f);

        this.gameObject.SetActive(false);
    }
}
