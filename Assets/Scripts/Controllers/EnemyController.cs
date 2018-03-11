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
    private static readonly float MovingAmountY = 1f;

    // trueなら敵は動かない
    public bool IsPausing
    {
        get { return enemyCloud.IsPausing; }
    }

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

    private EnemyCloud enemyCloud;

    // 初期は右方向
    private MoveDirection currentMoveDirection = MoveDirection.Right;

    // 下方向へ移動した後の移動方向を決定するのに使用する
    private MoveDirection previousMoveDirection;

    void Awake()
    {
        enemyCloud = transform.Find("Lines").GetComponent<EnemyCloud>();
    }

    public IEnumerable<IEnumerable<Enemy>> CreateEnemies()
    {
        return enemyCloud.CreateEnemies();
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
                    yield return enemyCloud.MoveDown(MovingAmountY);
                    break;
            }

            // update state
            previousMoveDirection = currentMoveDirection;
            currentMoveDirection = nextDir;
        }
    }

    public void Pause()
    {
        enemyCloud.IsPausing = true;
    }

    public void Resume()
    {
        enemyCloud.IsPausing = false;
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
