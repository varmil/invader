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
    public Action<Enemy> OnEnemyDefeated { get; set; }

    // 自機の死亡処理が始まった際のcallback（伝播）
    public Action OnDeadAnimationStart { get; set; }

    // 自機の死亡処理が終わった後のcallback（伝播）
    public Action OnDeadAnimationEnd { get; set; }

    private Player player;

    void Awake()
    {
        player = transform.Find("Player").GetComponent<Player>();

        player.OnEnemyDefeated = (e) => this.OnEnemyDefeated(e);

        player.OnDeadAnimationStart = () => this.OnDeadAnimationStart();

        player.OnDeadAnimationEnd = () => this.OnDeadAnimationEnd();
    }

    void Update()
    {
        if (!player.Alive || !CanMove) return;

        if (Input.GetKey(KeyCode.A))
        {
            if (player.transform.position.x > Constants.Stage.LeftEnd)
            {
                player.MoveLeft();
            }
        }

        if (Input.GetKey(KeyCode.D))
        {
            if (player.transform.position.x < Constants.Stage.RightEnd)
            {
                player.MoveRight();
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            player.Fire();
        }
    }

    public void Initialize()
    {
        CanMove = true;
    }

    public void Pause()
    {
        CanMove = false;
    }

    public void Resume()
    {
        CanMove = true;
    }

    public IEnumerator Reborn()
    {
        player.Initialize();
        yield return new WaitForSeconds(0.5f);
    }

}
