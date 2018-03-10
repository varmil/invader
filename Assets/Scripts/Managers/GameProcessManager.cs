using UnityEngine;

/**
 * ゲームの進行管理
 */
public class GameProcessManager : MonoBehaviour
{
    private static GameProcessManager _instance;

    public static GameProcessManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameProcessManager>();

                if (_instance == null)
                {
                    _instance = new GameObject("Managers/GameProcessManager").AddComponent<GameProcessManager>();
                }
            }
            return _instance;
        }
    }

    [SerializeField]
    private EnemyController enemyController;

    [SerializeField]
    private PlayerController playerController;

    void Awake()
    {
        playerController.OnEnemyDefeated += (enemy) =>
        {
        };
    }

    void Start()
    {
        // enemy process
        var enemies = enemyController.CreateEnemies();
        StartCoroutine(enemyController.StartCloudMoving());
        StartCoroutine(enemyController.StartFiring());

        // player process
        playerController.CanMove = true;
    }

    void Update()
    {

    }
}
