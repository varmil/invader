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

    private List<Enemy> enemies = new List<Enemy>(EnemyAmountPerLine);

    private GameObject basicEnemyPrefab;

    void Awake()
    {
        basicEnemyPrefab = (GameObject)Resources.Load("Prefabs/Enemy");
    }

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

    public IEnumerable<Enemy> CreateEnemies()
    {
        for (int i = 0; i < EnemyAmountPerLine; i++)
        {
            var position = new Vector3((float)(i * EnemyXSpace - OffsetX), 0f, 0f);
            var obj = (GameObject)Instantiate(basicEnemyPrefab, position, Quaternion.identity);
            obj.transform.SetParent(this.transform, false);

            var enemy = obj.GetComponent<Enemy>();
            enemy.Id = this.name + "-enemy::" + i;
            enemies.Add(enemy);
        }
        return enemies;
    }
}
