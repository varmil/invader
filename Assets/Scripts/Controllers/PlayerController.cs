using System;
using System.Collections;
using UnityEngine;

/**
 * 自機の統制
 */
public class PlayerController : MonoBehaviour
{
    // falseならPlayer操作無効
    public bool CanMove { get; private set; }

    // 敵を倒したときのcallback（伝播）
    public Action<IEnemy> OnEnemyDefeated { get; set; }

    // 自機の死亡処理が始まった際のcallback（伝播）
    public Action OnDeadAnimationStart { get; set; }

    // 自機の死亡処理が終わった後のcallback（伝播）
    public Action OnDeadAnimationEnd { get; set; }

    public Player Player { get; private set; }

    void Awake()
    {
        Player = transform.Find("Player").GetComponent<Player>();
    }

    void Update()
    {
        if (!Player.Alive || !CanMove)
            return;

        if (Input.GetKey(KeyCode.A))
        {
            if (Player.transform.position.x > Constants.Stage.LeftEnd)
            {
                Player.MoveLeft();
            }
        }

        if (Input.GetKey(KeyCode.D))
        {
            if (Player.transform.position.x < Constants.Stage.RightEnd)
            {
                Player.MoveRight();
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Player.Fire();
        }
    }

    public void Initialize()
    {
        CanMove = false;
        InitializePlayer();
    }

    public void Resume()
    {
        CanMove = true;
    }

    public void Pause()
    {
        CanMove = false;
    }

    public IEnumerator Reborn()
    {
        InitializePlayer();

        // wait for appearing animation
        yield return new WaitForSeconds(0.5f);
    }

    private void InitializePlayer()
    {
        Player.Initialize();

        Player.OnEnemyDefeated = (e) => this.OnEnemyDefeated(e);

        Player.OnDeadAnimationStart = () => this.OnDeadAnimationStart();

        Player.OnDead = () =>
        {
            // SetActive = false にするとコルーチンも消滅するので切り替えでしのぐ
            Player.gameObject.ToggleMeshRenderer(false);
            Player.gameObject.ToggleBoxCollider(false);
        };

        Player.OnDeadAnimationEnd = () => this.OnDeadAnimationEnd();

        // show player
        Player.gameObject.ToggleMeshRenderer(true);
        Player.gameObject.ToggleBoxCollider(true);
    }
}
