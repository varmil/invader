using System.Collections;
using System.Linq;
using UnityEngine;

/**
 * ゲームの進行管理
 */
public class GameProcessManager : SingletonMonoBehaviour<GameProcessManager>
{
    // 敵撃破時にこの秒数だけ全体が止まる
    private static readonly float PausingSec = 0.3f;

    // UFOが出現する間隔（開始から25秒間隔）
    private static readonly float UFOInterval = 25f;

    // UFOはインベーダーの数が残り7体以下になると出現しなくなる
    private static readonly float UFONotAppearingThreshold = 7;

    [SerializeField]
    private EnemyController enemyController;

    [SerializeField]
    private PlayerController playerController;
    
    [SerializeField]
    private UIController uiController;

    Coroutine PausingEnemyCoroutine = null;

    // 自機が死んだ後など、ゲーム全体を停止させるときはtrue
    bool isPausingGame = false;

    void Awake()
    {
        playerController.OnEnemyDefeated += (enemy) =>
        {
            // スコア加算
            ScoreStore.Instance.AddScore(enemy.Score);

            // 敵を一時停止（既に停止中の場合は、停止時間を引き伸ばすことはしない）
            if (!isPausingGame && PausingEnemyCoroutine == null)
            {
                PausingEnemyCoroutine = StartCoroutine(PauseEnemiesShortly());
            }
        };

        playerController.OnDeadAnimationStart += () =>
        {
            // 強制停止し、敵撃破時のストップモーションも中断
            isPausingGame = true;
            if (PausingEnemyCoroutine != null)
            {
                StopCoroutine(PausingEnemyCoroutine);
                PausingEnemyCoroutine = null;
            }
            playerController.Pause();
            enemyController.Pause();

            // change all material color to red
            MaterialManager.Instance.ChangeAllColorRed();
        };

        playerController.OnDeadAnimationEnd += () =>
        {
            // 復活チェック
            if (PlayerStore.Instance.Life > 0)
            {
                PlayerStore.Instance.Life--;
                MaterialManager.Instance.RestoreAllColor();
                StartCoroutine(RebornGame());
            } else
            {
                // TODO: game over
            }
        };
    }

    void Start()
    {
        // enemy process
        StartCoroutine(InitializeEnemies());
        StartCoroutine(InitializeUFO());

        // player process
        playerController.Initialize();
        MaterialManager.Instance.Add(playerController.Player.GetComponentsInChildren<MeshRenderer>());
        
        // ui process
        uiController.Initialize();
        MaterialManager.Instance.Add(uiController.Texts);
    }

    /// <summary>
    /// 時間制御したいのでコルーチン
    /// </summary>
    private IEnumerator InitializeUFO()
    {
        while (true)
        {
            yield return new WaitForSeconds(UFOInterval);

            if (enemyController.AliveEnemies.Count() > UFONotAppearingThreshold)
            {
                var ufo = enemyController.MakeUFOAppear();
                MaterialManager.Instance.Add(ufo.GetComponentInChildren<MeshRenderer>());
            }
        }
    }

    /// <summary>
    /// 時間制御したいのでコルーチン
    /// </summary>
    private IEnumerator InitializeEnemies()
    {
        var enemies = enemyController.CreateEnemies();
        MaterialManager.Instance.Add(enemies.Select(e => e.GetComponentInChildren<MeshRenderer>()));

        yield return new WaitForSeconds(0.3f);

        StartCoroutine(enemyController.StartCloudMoving());

        yield return new WaitForSeconds(0.3f);

        StartCoroutine(enemyController.StartFiring());
    }

    /// <summary>
    /// 敵撃破後の短期一時停止
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

        isPausingGame = false;
        enemyController.Resume();
    }
}
