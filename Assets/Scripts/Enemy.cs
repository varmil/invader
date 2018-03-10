using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // Fire時、自分のどの程度下にBeamを出現させるか
    private static readonly float BeamOffsetYRate = 2.5f;

    // デバッグ用
    public string Id { get; set; }

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

    private GameObject beamPrefab;
    private GameObject deadEffect;
    private ParticleSystem deadEffectParticle;

    void Awake()
    {
        beamPrefab = (GameObject)Resources.Load("Prefabs/EnemyBeam");
        var deadEffectPrefab = (GameObject)Resources.Load("Prefabs/Particles/EnemyDead");

        // warm up particle, TODO: Use Particle Pooling
        deadEffect = (GameObject)Instantiate(deadEffectPrefab, transform.position, Quaternion.identity);
        deadEffectParticle = deadEffect.GetComponent<ParticleSystem>();

        this.Alive = true;
    }

    public void Fire()
    {
        var myPos = this.transform.position;
        var beamPos = myPos + (Vector3.down * Height * BeamOffsetYRate);
        var beam = ObjectPool.Instance.Get(beamPrefab, beamPos, Quaternion.identity);

        // beam callback
        beam.GetComponent<Beam>().OnCollided = (other) =>
        {
            ObjectPool.Instance.Release(beam);

            // check other is Player or not
            var parent = other.transform.parent;
            if (parent != null)
            {
                var player = parent.GetComponent<Player>();
                if (player != null)
                {
                    player.Die();
                }
            }
        };
    }

    public void Die()
    {
        this.Alive = false;
        this.StartCoroutine(StartDeadAnimation());
    }

    private IEnumerator StartDeadAnimation()
    {
        // ための一瞬
        yield return new WaitForSeconds(0.05f);

        deadEffect.transform.localPosition = transform.position;
        deadEffectParticle.Play();

        yield return new WaitForSeconds(0.06f);

        this.gameObject.SetActive(false);
    }
}
