using UnityEngine;

/**
 * ゲームの進行管理
 */
public class GameProcessManager : SingletonMonoBehaviour<GameProcessManager>
{
    [SerializeField]
    private EnemyController enemyController;

    [SerializeField]
    private PlayerController playerController;

    void Awake()
    {
        playerController.OnEnemyDefeated += (enemy) =>
        {
            // スコア加算
            ScoreStore.Instance.AddScore(enemy.Score);

            // 敵を一時停止（既に停止中の場合は、停止時間を引き伸ばすことはしない）
            if (!enemyController.IsPausing)
            {
                StartCoroutine(enemyController.Pause());
            }
        };
    }

    void Start()
    {
        // enemy process
        enemyController.CreateEnemies();
        StartCoroutine(enemyController.StartCloudMoving());
        StartCoroutine(enemyController.StartFiring());

        // player process
        playerController.CanMove = true;
    }
}
