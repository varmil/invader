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
    // 移動終了後から次の移動までの秒間隔の逆数
    // 敵の残存数が少なくなるほどスピードアップ
    public float MovingInterval
    {
        get
        {
            return .3f * ((enemyCloud.AliveEnemies.Count() - 1) / 55f);
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
        this.enemyCloud = transform.Find("Lines").GetComponent<EnemyCloud>();
    }

    public IEnumerable<IEnumerable<Enemy>> CreateEnemies()
    {
        return this.enemyCloud.CreateEnemies();
    }

    public IEnumerator StartFiring()
    {
        return this.enemyCloud.StartFiring();
    }

    public IEnumerator StartCloudMoving()
    {
        while (true)
        {
            //if (MovingInterval > 0)
            //{
            Debug.Log(MovingInterval);
            yield return new WaitForSeconds(MovingInterval);
            //}

            // Look Line Position to judge move right or left or down
            var nextDir = CalcNextMoveDirection();
            switch (nextDir)
            {
                case MoveDirection.Right:
                    yield return this.enemyCloud.MoveRight();
                    break;
                case MoveDirection.Left:
                    yield return this.enemyCloud.MoveLeft();
                    break;
                case MoveDirection.Down:
                    yield return this.enemyCloud.MoveDown();
                    break;
            }

            // update state
            this.previousMoveDirection = this.currentMoveDirection;
            this.currentMoveDirection = nextDir;
        }
    }

    private MoveDirection CalcNextMoveDirection()
    {
        switch (this.currentMoveDirection)
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
                return (this.previousMoveDirection == MoveDirection.Right)
                    ? MoveDirection.Left
                    : MoveDirection.Right;
            default:
                throw new InvalidOperationException("cannot set MoveDirection");
        }
    }

    private bool IsReachedRightEnd()
    {
        return this.enemyCloud.RightEnd >= Constants.Stage.RightEnd;
    }

    private bool IsReachedLeftEnd()
    {
        return this.enemyCloud.LeftEnd <= Constants.Stage.LeftEnd;
    }
}
