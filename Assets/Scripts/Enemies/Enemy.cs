using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable, IEnemy
{
    // Fire時、自分のどの程度下にBeamを出現させるか
    private static readonly float BeamOffsetYRate = 1.5f;

    // デバッグ用
    public string Id { get; private set; }

    // 倒されたときにプレイヤーが得るスコア
    public int Score { get; private set; }

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

    // 移動時に動的にメッシュ差し替えを行う
    [SerializeField]
    private List<Mesh> meshes;

    // prefabs
    private GameObject beamPrefab;

    // components
    private MeshFilter meshFilter;

    // states
    private IEnumerator<Mesh> meshesRingBuffer;

    void Awake()
    {
        beamPrefab = (GameObject)Resources.Load("Prefabs/EnemyBeam");

        // （メッシュ）移動時に差し替える。
        meshFilter = transform.Find("Model").GetComponent<MeshFilter>();
        meshesRingBuffer = meshes.Repeat().GetEnumerator();
        meshesRingBuffer.MoveNext();
    }

    public void Initialize(string id, int score)
    {
        this.Alive = true;
        this.Id = id;
        this.Score = score;

        gameObject.ToggleMeshRenderer(true);
        gameObject.ToggleBoxCollider(true);
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

            // check other is Player or Tochka or not
            var parent = other.transform.parent;
            if (parent != null)
            {
                var damageable = parent.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    damageable.TakeDamage(gameObject, other);
                }
            }
        };
    }

    public void OnMoved()
    {
        // メッシュ差し替えでアニメーションさせる
        {
            // 循環バッファなので必ず取得できる想定
            if (!meshesRingBuffer.MoveNext())
                throw new MissingReferenceException("meshesRingBuffer.MoveNext() is FALSE");

            meshFilter.mesh = meshesRingBuffer.Current;
        }
    }

    public void TakeDamage(GameObject attacker, Collider collided)
    {
        this.Alive = false;
        this.StartCoroutine(StartDeadAnimation());
    }

    private IEnumerator StartDeadAnimation()
    {
        // ための一瞬
        yield return new WaitForSeconds(0.05f);

        ParticleManager.Instance.Play("Prefabs/Particles/EnemyDead",
            transform.position + (Vector3.up * 0.1f),
            meshFilter.GetComponent<Renderer>().material);

        yield return new WaitForSeconds(0.06f);

        gameObject.ToggleMeshRenderer(false);
        gameObject.ToggleBoxCollider(false);
    }
}
