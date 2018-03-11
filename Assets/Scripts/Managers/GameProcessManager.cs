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

            // 敵を一時停止（念の為止めてから）
            StopCoroutine(enemyController.Pause());
            StartCoroutine(enemyController.Pause());
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

        // totchka
        //CreateTochka();
    }

    //private void CreateTochka()
    //{
    //    var cubes = Enumerable.Range(0, 20).Select((i) =>
    //    {
    //        var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
    //        return cube;
    //    }).ToList();

    //    cubes[0].transform.position = new Vector3(-0.75f, 0.0f, 0);
    //    cubes[1].transform.position = new Vector3(-0.75f, 0.5f, 0);
    //    cubes[2].transform.position = new Vector3(-0.75f, 1.0f, 0);
    //    cubes[3].transform.position = new Vector3(-0.75f, 1.5f, 0);

    //    cubes[4].transform.position = new Vector3(0.75f, 0.0f, 0);
    //    cubes[5].transform.position = new Vector3(0.75f, 0.5f, 0);
    //    cubes[6].transform.position = new Vector3(0.75f, 1.0f, 0);
    //    cubes[7].transform.position = new Vector3(0.75f, 1.5f, 0);

    //}
}
