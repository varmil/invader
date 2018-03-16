using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/**
 * EnemyZone内に存在する敵の塊
 */
public class EnemyCloud : MonoBehaviour
{
    // 敵のbeam発射間隔ベース値
    private static readonly float BaseFiringIntervalSec = 0.75f;

    // 各ライン間の距離
    private static readonly float LineSpaceY = 1.5f;

    // 敵のスコア
    private static readonly int ScoreOfUpper = 30;
    private static readonly int ScoreOfMiddle = 20;
    private static readonly int ScoreOfLower = 10;

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
            return Mathf.Max(Lines.Select(l => l.RightEnd).ToArray());
        }
    }

    public float LeftEnd
    {
        get
        {
            //Debug.Log("Cloud LeftEnd::" + Mathf.Min(Lines.Select((e) => e.LeftEnd).ToArray()).ToString());
            return Mathf.Min(Lines.Select(l => l.LeftEnd).ToArray());
        }
    }

    public float BottomEnd
    {
        get
        {
            // Debug.Log("Cloud BottomEnd::" + this.Lines.Where(l => !l.IsAllDead).Last().transform.position.y.ToString());
            return this.Lines.Where(l => !l.IsAllDead).Last().transform.position.y;
        }
    }

    // 外からsetする。trueなら敵は動かない
    public bool IsPausing
    {
        get;
        set;
    }

    private BeamManager beamManager;
    private GameObject squidPrefab;
    private GameObject crabPrefab;
    private GameObject octopusPrefab;
    private GameObject ufoPrefab;

    void Awake()
    {
        squidPrefab = (GameObject)Resources.Load("Prefabs/Enemies/Squid");
        crabPrefab = (GameObject)Resources.Load("Prefabs/Enemies/Crab");
        octopusPrefab = (GameObject)Resources.Load("Prefabs/Enemies/Octopus");
        ufoPrefab = (GameObject)Resources.Load("Prefabs/Enemies/UFO");

        InitializeLines();

        // instantiate POCO
        this.beamManager = new BeamManager(this.Lines);

    }

    private void InitializeLines()
    {
        var first = transform.Find("1st").GetComponent<EnemyLine>();
        var second = transform.Find("2nd").GetComponent<EnemyLine>();
        var third = transform.Find("3rd").GetComponent<EnemyLine>();
        var fourth = transform.Find("4th").GetComponent<EnemyLine>();
        var fifth = transform.Find("5th").GetComponent<EnemyLine>();

        first.Initialize(ScoreOfUpper, squidPrefab);
        second.Initialize(ScoreOfMiddle, crabPrefab);
        third.Initialize(ScoreOfMiddle, crabPrefab);
        fourth.Initialize(ScoreOfLower, octopusPrefab);
        fifth.Initialize(ScoreOfLower, octopusPrefab);

        this.lines.Add(first);
        this.lines.Add(second);
        this.lines.Add(third);
        this.lines.Add(fourth);
        this.lines.Add(fifth);
    }

    public IEnumerable<Enemy>[] CreateEnemies(int stageNum)
    {
        SetLinesInitialPosition(stageNum);

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
            while (IsPausing)
            {
                yield return null;
            }

            var enemy = this.beamManager.GetRandomFireableEnemy();

            if (enemy != null)
            {
                enemy.Fire();
            }

            // TODO: randomize
            yield return new WaitForSeconds(BaseFiringIntervalSec);
        }
    }

    private IEnumerator Move(Vector3 delta)
    {
        for (int i = this.Lines.Count - 1; i >= 0; i--)
        {
            while (IsPausing)
            {
                yield return null;
            }

            // ラインが消滅している場合は待たない
            if (this.lines[i].IsAllDead)
                continue;

            this.lines[i].Move(delta);
            yield return new WaitForSeconds(MovingIntervalPerLine);
        }
    }

    private void SetLinesInitialPosition(int stageNum)
    {
        var firstLineYPos = Constants.Stage.FirstLineYPos - (Constants.Stage.InvadedYPosPerStage * stageNum);

        for (int i = 0; i < this.lines.Count; i++)
        {
            this.lines[i].transform.position = new Vector3(0f, firstLineYPos - (i * LineSpaceY));
        }
    }
}
