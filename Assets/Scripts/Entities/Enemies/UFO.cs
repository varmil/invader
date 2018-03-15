using System;
using System.Collections;
using UnityEngine;

public class UFO : MonoBehaviour, IDamageable, IEnemy
{
    /// <summary>
    /// 移動速度
    /// </summary>
    private static readonly float Speed = .08F;

    /// <summary>
    /// この中からランダムでスコアを決定する
    /// </summary>
    private static readonly int[] RandomScores = new int[] { 300, 150, 100, 50 };

    /// <summary>
    /// UFOの得点は擬似乱数
    /// </summary>
    public int Score { get; private set; }

    /// <summary>
    /// 生存しているか
    /// </summary>
    public bool Alive { get; private set; }

    /// <summary>
    /// UFOが生存したまま逃げてしまったときのcallback
    /// </summary>
    public Action OnGone { get; set; }

    /// <summary>
    /// callback when the object should be invisible
    /// </summary>
    public Action OnDead { get; set; }

    private Renderer ufoRenderer;
    private bool canMove;
    private Vector3 movingVector;

    void Awake()
    {
        ufoRenderer = GetComponentInChildren<MeshRenderer>();
    }

    void Update()
    {
        // 移動
        if (Alive && canMove)
        {
            transform.position += movingVector * Speed;
        }

        // 生きたまま画面外に出たらcallback（絶対値比較なので便宜的にRightEndを用いる）
        if (Alive && Mathf.Abs(transform.position.x) > Constants.Stage.UFORightEnd)
        {
            OnGone();
        }
    }

    public void Initialize(Vector3 movingVector)
    {
        this.Alive = true;
        this.Score = RandomScores[UnityEngine.Random.Range(0, RandomScores.Length)];

        this.canMove = false;
        this.movingVector = movingVector;
    }

    public void TakeDamage(GameObject attacker, Collider collided)
    {
        this.Alive = false;
        this.StartCoroutine(StartDeadAnimation());
    }

    public void StartMoving()
    {
        canMove = true;
    }

    private IEnumerator StartDeadAnimation()
    {
        ParticleManager.Instance.Play("Prefabs/Particles/EnemyDead",
            transform.position + (Vector3.up * 0.1f),
            ufoRenderer.material);

        yield return new WaitForSeconds(0.06f);

        OnDead();
    }
}
