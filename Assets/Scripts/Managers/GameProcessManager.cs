using System.Collections;
using UnityEngine;

/**
 * ゲームの進行管理
 */
public class GameProcessManager : SingletonMonoBehaviour<GameProcessManager>
{
    // 敵撃破時にこの秒数だけ全体が止まる
    private static readonly float PausingSec = 0.3f;

    [SerializeField]
    private EnemyController enemyController;

    [SerializeField]
    private PlayerController playerController;

    Coroutine PausingEnemyCoroutine = null;

    // 自機が死んだ後など、ゲーム全体を停止させるときはtrue
    bool IsPausingGame = false;

    void Awake()
    {
        playerController.OnEnemyDefeated += (enemy) =>
        {
            // スコア加算
            ScoreStore.Instance.AddScore(enemy.Score);

            // 敵を一時停止（既に停止中の場合は、停止時間を引き伸ばすことはしない）
            if (!IsPausingGame && PausingEnemyCoroutine == null)
            {
                PausingEnemyCoroutine = StartCoroutine(PauseEnemiesShortly());
            }
        };

        playerController.OnDeadAnimationStart += () =>
        {
            // 強制停止し、敵撃破時のストップモーションも中断
            IsPausingGame = true;
            if (PausingEnemyCoroutine != null)
            {
                StopCoroutine(PausingEnemyCoroutine);
                PausingEnemyCoroutine = null;
            }
            playerController.Pause();
            enemyController.Pause();
        };

        playerController.OnDeadAnimationEnd += () =>
        {
            // 復活チェック
            if (PlayerStore.Instance.Life > 0)
            {
                PlayerStore.Instance.Life--;
                StartCoroutine(RebornGame());
            }
            else
            {
                // TODO: game over
            }
        };
    }

    void Start()
    {
        // enemy process
        StartCoroutine(InitializeEnemies());

        // player process
        playerController.Initialize();
    }

    /// <summary>
    /// 時間制御したいのでコルーチン
    /// </summary>
    private IEnumerator InitializeEnemies()
    {
        enemyController.CreateEnemies();

        yield return new WaitForSeconds(0.3f);

        StartCoroutine(enemyController.StartCloudMoving());

        yield return new WaitForSeconds(0.3f);

        StartCoroutine(enemyController.StartFiring());
    }

    /// <summary>
    /// 敵全体を一時停止
    /// </summary>
    private IEnumerator PauseEnemiesShortly()
    {
        enemyController.Pause();
        yield return new WaitForSeconds(PausingSec);
        enemyController.Resume();

        PausingEnemyCoroutine = null;
    }

    /// <summary>
    /// プレイヤー復活後、敵移動再開
    /// </summary>
    private IEnumerator RebornGame()
    {
        yield return StartCoroutine(playerController.Reborn());
        playerController.Resume();

        // リスキル防止用にプレイヤーの方が若干早く動けるように
        yield return new WaitForSeconds(0.2f);

        IsPausingGame = false;
        enemyController.Resume();
    }
}
