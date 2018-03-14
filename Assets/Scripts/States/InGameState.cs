﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InGameState : AppState
{
    public override string SceneName
    {
        get { return "InGameScene"; }
    }

    // 敵撃破時にこの秒数だけ全体が止まる
    private static readonly float PausingSec = 0.3f;
    
    // UFOが出現する間隔（開始から25秒間隔）
    private static readonly float UFOInterval = 25f;
    
    // UFOはインベーダーの数が残り7体以下になると出現しなくなる
    private static readonly float UFONotAppearingThreshold = 7;
    
    private EnemyController enemyController;
    private PlayerController playerController;
    private InGameUIController uiController;
    
    private Coroutine PausingEnemyCoroutine = null;
    private bool pressedEscape = false;
    
    // 自機が死んだ後など、ゲーム全体を停止させるときはtrue
    bool isPausingGame = false;
    
    public override IEnumerator OnEnter()
    {
        yield return StartCoroutine(base.OnEnter());

        // search controllers
        var rootObjects = GetRootObjects().ToArray();
        var entities = rootObjects.First(e => e.name == "Entities");
        var ui = rootObjects.First(e => e.name == "UICanvas");
        enemyController = entities.GetComponentInChildren<EnemyController>();
        playerController = entities.GetComponentInChildren<PlayerController>();
        uiController = ui.GetComponent<InGameUIController>();
        



        // clean this state
        Initialize();




        // enemy process
        StartCoroutine(InitializeEnemies());
        StartCoroutine(InitializeUFO());




        // player process
        playerController.Initialize();
        playerController.OnEnemyDefeated = (enemy) =>
        {
            // スコア加算
            ScoreStore.Instance.AddScore(enemy.Score);
            
            // 敵を一時停止（既に停止中の場合は、停止時間を引き伸ばすことはしない）
            if (!isPausingGame && PausingEnemyCoroutine == null)
            {
                PausingEnemyCoroutine = StartCoroutine(PauseEnemiesShortly());
            }
        };
        
        playerController.OnDeadAnimationStart = () =>
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
        
        playerController.OnDeadAnimationEnd = () =>
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
        MaterialManager.Instance.Add(playerController.Player.GetComponentsInChildren<MeshRenderer>());




        // ui process
        MaterialManager.Instance.Add(uiController.Texts);

        yield return null;
    }

    public override void Tick()
    {
        if (!pressedEscape && Input.GetKeyDown(KeyCode.Escape))
        {
            pressedEscape = true;
            
            // go to title scene
            GameProcessManager.Instance.SetState(GetComponent<TitleState>());
        }   
    }
    
    public override IEnumerator OnLeave()
    {
        yield return StartCoroutine(base.OnLeave());
    }
    
    private void Initialize()
    {
        pressedEscape = false;
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
