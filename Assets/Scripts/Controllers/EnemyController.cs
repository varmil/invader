using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/**
 * 敵全体の統制
 */
public class EnemyController : MonoBehaviour
{
    // trueなら敵は動かない
    public bool IsPausing
    {
        get { return enemyCloud.IsPausing; }
    }

    public IEnumerable<Enemy> AliveEnemies
    {
        get { return enemyCloud.AliveEnemies; }
    }

    // デッドラインに到達したときのcallback
    public Action OnDeadLineReached { get; set; }

    // 移動終了後から次の移動までの秒間隔の逆数
    // 敵の残存数が少なくなるほどスピードアップ
    private float MovingInterval
    {
        get
        {
            return .3f * ((enemyCloud.AliveEnemies.Count() - 1) / 55f);
        }
    }

    // １回の移動でどの程度X軸方向に動くか
    // 敵の残存数が少なくなるほどスピードアップ（逆数）
    private float MovingAmountX
    {
        get
        {
            return .25f + (.5f * (1f / enemyCloud.AliveEnemies.Count()));
        }
    }

    // Cloudの移動方向
    enum MoveDirection : byte
    {
        Right,
        Left,
        Down
    }

    // UFOの移動方向
    enum UFOMoveDirection : byte
    {
        Right,
        Left,
    }

    private GameObject ufoPrefab;
    private EnemyCloud enemyCloud;

    // 初期は右方向
    private MoveDirection currentMoveDirection = MoveDirection.Right;
    // 下方向へ移動した後の移動方向を決定するのに使用する
    private MoveDirection previousMoveDirection;

    // color container of enemy
    private Dictionary<EnemyColor, Material> materials = new Dictionary<EnemyColor, Material>(5);

    void Awake()
    {
        ufoPrefab = (GameObject)Resources.Load("Prefabs/Enemies/UFO");
        enemyCloud = transform.Find("Lines").GetComponent<EnemyCloud>();

        // set materials
        materials[EnemyColor.Green] = (Material)Resources.Load("Materials/Enemies/Green");
        materials[EnemyColor.Blue] = (Material)Resources.Load("Materials/Enemies/Blue");
        materials[EnemyColor.Pink] = (Material)Resources.Load("Materials/Enemies/Pink");
        materials[EnemyColor.Yellow] = (Material)Resources.Load("Materials/Enemies/Yellow");
        materials[EnemyColor.Red] = (Material)Resources.Load("Materials/Enemies/Red");
    }

    public IEnumerable<Enemy> CreateEnemies(int stageNum)
    {
        // flatten values
        var enemies = enemyCloud.CreateEnemies(stageNum).SelectMany(e => e);
        // set initial color
        ChangeColor();
        return enemies;
    }

    public IEnumerator StartFiring()
    {
        return enemyCloud.StartFiring();
    }

    public IEnumerator StartCloudMoving()
    {
        while (true)
        {
            yield return new WaitForSeconds(MovingInterval);

            // Look Line Position to judge move right or left or down
            var nextDir = CalcNextMoveDirection();
            switch (nextDir)
            {
                case MoveDirection.Right:
                    yield return enemyCloud.MoveRight(MovingAmountX);
                    break;
                case MoveDirection.Left:
                    yield return enemyCloud.MoveLeft(MovingAmountX);
                    break;
                case MoveDirection.Down:
                    yield return enemyCloud.MoveDown(Constants.Stage.InvadedYPosPerMove);

                    // enemy color should be changed depending on current y position
                    ChangeColor();

                    // check whether the enemy reaches dead line or not
                    if (enemyCloud.BottomEnd < Constants.Stage.DeadLineOfYPos)
                    {
                        OnDeadLineReached();
                        yield break;
                    }
                    break;
            }

            // update state
            previousMoveDirection = currentMoveDirection;
            currentMoveDirection = nextDir;
        }
    }

    public UFO MakeUFOAppear()
    {
        var direction = (UFOMoveDirection)UnityEngine.Random.Range(0, 2);
        var pos = (direction == UFOMoveDirection.Right)
            ? new Vector3(Constants.Stage.UFOLeftEnd, Constants.Stage.UFOYPos)
            : new Vector3(Constants.Stage.UFORightEnd, Constants.Stage.UFOYPos);

        var ufo = ObjectPool.Instance.Get(ufoPrefab, pos, Quaternion.identity);
        var component = ufo.GetComponent<UFO>();

        // init
        var movingVector = (direction == UFOMoveDirection.Right) ? Vector3.right : Vector3.left;
        component.Initialize(movingVector);

        // set callback handler
        component.OnGone = () => ObjectPool.Instance.Release(ufo);
        component.OnDead = () => ObjectPool.Instance.Release(ufo);

        component.StartMoving();

        return component;
    }

    public void Pause()
    {
        enemyCloud.IsPausing = true;
    }

    public void Resume()
    {
        enemyCloud.IsPausing = false;
    }

    private void ChangeColor()
    {
        enemyCloud.Lines.Where(l => !l.IsAllDead).ToList().ForEach(l =>
        {
            var currentY = l.transform.position.y;
            Material nextMaterial = null;

            if (currentY < Constants.Stage.InvaderRedYPos)
            {
                nextMaterial = materials[EnemyColor.Red];
            }
            else if (currentY < Constants.Stage.InvaderYellowYPos)
            {
                nextMaterial = materials[EnemyColor.Yellow];
            }
            else if (currentY < Constants.Stage.InvaderPinkYPos)
            {
                nextMaterial = materials[EnemyColor.Pink];
            }
            else if (currentY < Constants.Stage.InvaderBlueYPos)
            {
                nextMaterial = materials[EnemyColor.Blue];
            }
            else
            {
                nextMaterial = materials[EnemyColor.Green];
            }

            // HACK: this changes every time, so should be added some conditions to change color
            l.AliveEnemies.ToList().ForEach(e =>
            {
                var mesh = e.GetComponentInChildren<MeshRenderer>();
                mesh.material = nextMaterial;
            });
        });
    }

    private MoveDirection CalcNextMoveDirection()
    {
        switch (currentMoveDirection)
        {
            // 右端にCloudが達していたらDownへ
            case MoveDirection.Right:
                return (IsReachedRightEnd())
                    ? MoveDirection.Down
                    : MoveDirection.Right;
            // 左端にCloudが達していたらDownへ
            case MoveDirection.Left:
                return (IsReachedLeftEnd())
                    ? MoveDirection.Down
                    : MoveDirection.Left;
            // 一つ前のMoveDirectionを参照して、それと逆方向へ
            case MoveDirection.Down:
                return (previousMoveDirection == MoveDirection.Right)
                    ? MoveDirection.Left
                    : MoveDirection.Right;
            default:
                throw new InvalidOperationException("cannot set MoveDirection");
        }
    }

    private bool IsReachedRightEnd()
    {
        return enemyCloud.RightEnd >= Constants.Stage.RightEnd;
    }

    private bool IsReachedLeftEnd()
    {
        return enemyCloud.LeftEnd <= Constants.Stage.LeftEnd;
    }
}
