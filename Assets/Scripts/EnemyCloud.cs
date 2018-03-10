using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/**
 * EnemyZone内に存在する敵の塊
 */
public class EnemyCloud : MonoBehaviour
{
    // 最も高い位置の初期Y座標
    private static readonly float FirstLineYPos = 15f;
    // 敵のbeam発射間隔ベース値
    private static readonly float BaseFiringIntervalSec = 0.8f;

    // 列ごとの移動秒数差
    // 敵の残存数が少なくなるほどスピードアップ
    private float MovingIntervalPerLine
    {
        get
        {
            return .2f * ((AliveEnemies.Count() - 1) / 55f);
        }
    }

    // 横方向の行
    public List<EnemyLine> Lines
    {
        get { return lines; }
    }
    private List<EnemyLine> lines = new List<EnemyLine>(5);

    public IEnumerable<Enemy> AliveEnemies
    {
        get
        {
            return this.Lines.SelectMany(enemyLine =>
            {
                return enemyLine.AliveEnemies;
            });
        }
    }

    public float RightEnd
    {
        get
        {
            //Debug.Log("Cloud RightEnd::" + Mathf.Max(Lines.Select((l) => l.RightEnd).ToArray()).ToString());
            return Mathf.Max(Lines.Select((l) => l.RightEnd).ToArray());
        }
    }

    public float LeftEnd
    {
        get
        {
            //Debug.Log("Cloud LeftEnd::" + Mathf.Min(Lines.Select((e) => e.LeftEnd).ToArray()).ToString());
            return Mathf.Min(Lines.Select((e) => e.LeftEnd).ToArray());
        }
    }

    private BeamManager beamManager;

    void Awake()
    {
        this.lines.Add(transform.Find("1st").GetComponent<EnemyLine>());
        this.lines.Add(transform.Find("2nd").GetComponent<EnemyLine>());
        this.lines.Add(transform.Find("3rd").GetComponent<EnemyLine>());
        this.lines.Add(transform.Find("4th").GetComponent<EnemyLine>());
        this.lines.Add(transform.Find("5th").GetComponent<EnemyLine>());

        SetLinesInitialPosition();

        // instantiate POCO
        this.beamManager = new BeamManager(this.Lines);

    }

    public IEnumerable<Enemy>[] CreateEnemies()
    {
        return Enumerable
            .Range(0, this.Lines.Count)
            .Select((i) => this.lines[i].CreateEnemies())
            .ToArray();
    }

    public IEnumerator MoveRight(float amount)
    {
        return Move(new Vector3(amount, 0f));
    }

    public IEnumerator MoveLeft(float amount)
    {
        return Move(new Vector3(-amount, 0f));
    }

    public IEnumerator MoveDown(float amount)
    {
        return Move(new Vector3(0f, -amount));
    }

    public IEnumerator StartFiring()
    {
        while (true)
        {
            // TODO: randomize
            yield return new WaitForSeconds(BaseFiringIntervalSec);
            var enemy = this.beamManager.GetRandomFireableEnemy();
            enemy.Fire();
            //Debug.Log(string.Join(", ", beamManager.GetFireableEnemies().Select(e => e.Id).ToArray()));
        }
    }

    private IEnumerator Move(Vector3 delta)
    {
        for (int i = this.Lines.Count - 1; i >= 0; i--)
        {
            // ラインが消滅している場合は待たない
            if (this.lines[i].IsAllDead)
                continue;

            this.lines[i].transform.position += delta;
            yield return new WaitForSeconds(MovingIntervalPerLine);
        }
    }

    private void SetLinesInitialPosition()
    {
        for (int i = 0; i < this.lines.Count; i++)
        {
            this.lines[i].transform.position = new Vector3(0f, FirstLineYPos - (i * 1.5f), 0f);
        }
    }
}
