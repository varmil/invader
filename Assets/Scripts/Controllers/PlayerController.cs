using System;
using UnityEngine;

/**
 * 自機の統制
 */
public class PlayerController : MonoBehaviour
{
    // falseならPlayer操作無効
    public bool CanMove { get; set; }

    // 敵を倒したときのcallback（伝播）
    public Action<Enemy> OnEnemyDefeated { get; set; }

    private Player player;

    void Awake()
    {
        player = transform.Find("Player").GetComponent<Player>();
        player.OnEnemyDefeated = (e) => this.OnEnemyDefeated(e);
    }

    void Update()
    {
        if (!player.Alive) return;

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
}
