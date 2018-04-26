using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/**
 * EnemyCloud内に存在する敵の列（1列）
 */
public class EnemyLine : MonoBehaviour
{
    // １列の敵の数
    private static readonly int EnemyAmountPerLine = 11;
    // 左右に隣り合う敵との距離
    private static readonly float EnemyXSpace = 1.6f;
    // 敵の初期位置をセンタリングするためのオフセット
    private static readonly float OffsetX = EnemyXSpace * (EnemyAmountPerLine - 1) / 2;

    public List<Enemy> Enemies
    {
        get { return enemies; }
    }

    public IEnumerable<Enemy> AliveEnemies
    {
        get { return Enemies.Where(e => e.Alive); }
    }

    public bool IsAllDead
    {
        get { return AliveEnemies.Count() == 0; }
    }

    public int ScorePerEnemy
    {
        get;
        private set;
    }

    private List<Enemy> enemies = new List<Enemy>(EnemyAmountPerLine);
    private GameObject enemyPrefab;

    public float RightEnd
    {
        get
        {
            // 全ての敵が消滅している場合は便宜的にゼロ
            if (IsAllDead) return 0f;

            var myPos = this.transform.position.x;
            var offset = Mathf.Max(AliveEnemies.Select((e) => e.transform.localPosition.x).ToArray());
            return myPos + offset;
        }
    }

    public float LeftEnd
    {
        get
        {
            // 全ての敵が消滅している場合は便宜的にゼロ
            if (IsAllDead) return 0f;

            var myPos = this.transform.position.x;
            var offset = Mathf.Min(AliveEnemies.Select((e) => e.transform.localPosition.x).ToArray());
            return myPos + offset;
        }
    }

    public void Initialize(int scorePerEnemy, GameObject enemyPrefab)
    {
        ScorePerEnemy = scorePerEnemy;
        this.enemyPrefab = enemyPrefab;
    }

    public IEnumerable<Enemy> CreateEnemies()
    {
        for (int i = 0; i < EnemyAmountPerLine; i++)
        {
            var position = new Vector3((float)(i * EnemyXSpace - OffsetX), 0f, 0f);
            var obj = ObjectPool.Instance.Get(this.enemyPrefab, position, Quaternion.identity);
            obj.transform.SetParent(this.transform, false);

            var enemy = obj.GetComponent<Enemy>();
            enemy.Initialize(this.name + "-enemy::" + i, ScorePerEnemy);
            enemy.OnDead = () => ObjectPool.Instance.Release(obj);
            enemies.Add(enemy);
        }
        return enemies;
    }

    // 移動はLineごと行い、個々のEnemyへはイベント通知のみ
    public void Move(Vector3 delta)
    {
        this.transform.position += delta;
        this.Enemies.ForEach(e => e.OnMoved());
    }
}
