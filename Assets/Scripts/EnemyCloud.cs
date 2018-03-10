using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/**
 * EnemyZone内に存在する敵の塊
 */
public class EnemyCloud : MonoBehaviour
{
    // 列ごとの移動秒数差
    private static readonly float MovingIntervalPerLine = 0.15f;
    // １回の移動でどの程度X軸方向に動くか
    private static readonly float MovingAmountX = 0.25f;
    // 最も高い位置の初期Y座標
    private static readonly float FirstLineYPos = 15f;
    // 敵のbeam発射間隔ベース値
    private static readonly float BaseFiringIntervalSec = 0.8f;

    // 横方向の行
    public List<Line> Lines
    {
        get { return lines; }
    }
    private List<Line> lines = new List<Line>(5);

    public float RightEnd
    {
        get
        {
            //			Debug.Log ("Cloud RightEnd::" + Mathf.Max (Lines.Select ((l) => l.RightEnd).ToArray()).ToString());
            return Mathf.Max(Lines.Select((l) => l.RightEnd).ToArray());
        }
    }

    public float LeftEnd
    {
        get
        {
            //			Debug.Log ("Cloud LeftEnd::" + Mathf.Min (Lines.Select ((e) => e.LeftEnd).ToArray()).ToString());
            return Mathf.Min(Lines.Select((e) => e.LeftEnd).ToArray());
        }
    }

    private BeamManager beamManager;

    void Awake()
    {
        this.lines.Add(transform.Find("1st").GetComponent<Line>());
        this.lines.Add(transform.Find("2nd").GetComponent<Line>());
        this.lines.Add(transform.Find("3rd").GetComponent<Line>());
        this.lines.Add(transform.Find("4th").GetComponent<Line>());
        this.lines.Add(transform.Find("5th").GetComponent<Line>());

        SetLinesInitialPosition();

        // instantiate POCO
        this.beamManager = new BeamManager(this.Lines);

    }

    void Start()
    {
        // create enemies
        Enumerable
            .Range(0, this.Lines.Count)
            .Select((i) => this.lines[i].CreateEnemies())
            .ToArray();

        // go coroutine
        StartCoroutine(StartFiring());
    }

    public IEnumerator MoveRight()
    {
        for (int i = this.Lines.Count - 1; i >= 0; i--)
        {
            this.lines[i].transform.position += new Vector3(MovingAmountX, 0);
            yield return new WaitForSeconds(MovingIntervalPerLine);
        }
    }

    public IEnumerator MoveLeft()
    {
        for (int i = this.Lines.Count - 1; i >= 0; i--)
        {
            this.lines[i].transform.position += new Vector3(-MovingAmountX, 0f);
            yield return new WaitForSeconds(MovingIntervalPerLine);
        }
    }

    public IEnumerator MoveDown()
    {
        for (int i = this.Lines.Count - 1; i >= 0; i--)
        {
            this.lines[i].transform.position += new Vector3(0f, -1f);
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

    private IEnumerator StartFiring()
    {
        while (true)
        {
            // TODO: randomize
            yield return new WaitForSeconds(BaseFiringIntervalSec);
            var enemy = this.beamManager.GetRandomFireableEnemy();
            enemy.Fire();
            //			Debug.Log (string.Join(", ", beamManager.GetFireableEnemies ().Select(e => e.Id).ToArray()));
        }
    }
}
