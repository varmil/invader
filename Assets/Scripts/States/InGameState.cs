﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

/**
 * READY, RUNNING, GAMEOVER, PAUSE
 * など、さらに細かくStateごとに管理するかどうかは悩みどころ
 */
public class InGameState : AppState, IAppState
{
    public override string SceneName
    {
        get { return "InGameScene"; }
    }

    private EnemyController enemyController;
    private PlayerController playerController;
    private InGameUIController uiController;
    private MaterialController materialController;

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

        // only this class uses MaterialManager
        materialController = new MaterialController();

        // init own class
        Initialize();
        // player process
        InitializePlayerController();
        // enemy process
        InitializeEnemyController();

        // ui process
        uiController.Initialize(GlobalStore.Instance);
        materialController.Add(uiController.Texts);

        yield return null;
    }

    public override IEnumerator OnFadeOutEnd()
    {
        StartStage(GlobalStore.Instance.StageStore.CurrentStage);
        yield return null;
    }

    public override void Tick()
    {
        base.Tick();

        if (!pressedEscape && Input.GetKeyDown(KeyCode.Escape))
        {
            pressedEscape = true;

            // reset current stage info
            GlobalStore.Instance.StageStore.SetDefault();
            // go to title scene
            NextStateSet(GetComponent<TitleState>());
        }
    }

    public override IEnumerator OnLeave()
    {
        yield return StartCoroutine(base.OnLeave());
    }

    /// <summary>
    /// このクラス自体の初期化処理
    /// </summary>
    private void Initialize()
    {
        // reset member
        isPausingGame = false;
        pressedEscape = false;

        // reset store if it is 1st stage
        var globalStore = GlobalStore.Instance;
        if (globalStore.StageStore.CurrentStage == 0)
        {
            globalStore.PlayerStore.SetDefault();
            globalStore.ScoreStore.SetDefaultCurrentScore();

            // TODO: load Hi-Score from DB
        }
    }

    private void InitializePlayerController()
    {
        playerController.Initialize();
        playerController.OnEnemyDefeated = (enemy) =>
        {
            // add score
            GlobalStore.Instance.ScoreStore.AddScore(enemy.Score);

            // go to next stage if enemies are all dead
            if (enemyController.AliveEnemies.Count() == 0)
            {
                // reload the scene
                GlobalStore.Instance.StageStore.IncrementStage();
                NextStateSet(GetComponent<InGameState>());
                return;
            }

            // 敵を一時停止（既に停止中の場合は、停止時間を引き伸ばすことはしない）
            if (!isPausingGame && PausingEnemyCoroutine == null)
            {
                PausingEnemyCoroutine = StartCoroutine(PauseEnemiesShortly());
            }
        };

        playerController.OnDeadAnimationStart = () =>
        {
            PauseGame();
            materialController.ChangeAllColorRed();
        };

        playerController.OnDeadAnimationEnd = () =>
        {
            // reborn
            if (GlobalStore.Instance.PlayerStore.Life > 0)
            {
                GlobalStore.Instance.PlayerStore.DecrementLife();
                materialController.RestoreAllColor();
                StartCoroutine(RebornGame());
            }
            // GameOver
            else
            {
                StartCoroutine(GameOver());
            }
        };
        materialController.Add(playerController.Player.GetComponentsInChildren<MeshRenderer>());
    }

    private void InitializeEnemyController()
    {
        enemyController.OnDeadLineReached = () =>
        {
            PauseGame();
            materialController.ChangeAllColorRed();
            StartCoroutine(GameOver());
        };
    }

    /// <summary>
    /// 指定した面を開始する（0 - 7）
    /// </summary>
    private void StartStage(int stageNum)
    {
        Debug.Log("Start Stage " + GlobalStore.Instance.StageStore.CurrentStage);

        // player process
        playerController.Resume();

        // enemy process
        StartCoroutine(MakeEnemiesAppear(stageNum));
        StartCoroutine(MakeUFOAppear());
    }

    /// <summary>
    /// UFO出現カウンタを開始
    /// </summary>
    private IEnumerator MakeUFOAppear()
    {
        while (true)
        {
            yield return new WaitForSeconds(Constants.Stage.UFOInterval);

            // do not appear while Game is pausing
            while (isPausingGame) yield return null;

            if (enemyController.AliveEnemies.Count() > Constants.Stage.UFONotAppearingThreshold)
            {
                var ufo = enemyController.MakeUFOAppear();
                materialController.Add(ufo.GetComponentInChildren<MeshRenderer>());
            }
        }
    }

    /// <summary>
    /// 敵インスタンスを生成し、移動、攻撃開始。ステージ進行に伴って初期Y座標を変える
    /// </summary>
    private IEnumerator MakeEnemiesAppear(int stageNum)
    {
        var enemies = enemyController.CreateEnemies(stageNum);
        materialController.Add(enemies.Select(e => e.GetComponentInChildren<MeshRenderer>()));

        yield return new WaitForSeconds(0.3f);

        StartCoroutine(enemyController.StartCloudMoving());

        yield return new WaitForSeconds(0.3f);

        StartCoroutine(enemyController.StartFiring());
    }

    /// <summary>
    /// Pause entire the Game
    /// </summary>
    private void PauseGame()
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
    }

    /// <summary>
    /// 敵撃破後の短期一時停止
    /// </summary>
    private IEnumerator PauseEnemiesShortly()
    {
        enemyController.Pause();
        yield return new WaitForSeconds(Constants.Stage.EnemyPausingSec);
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

    private IEnumerator GameOver()
    {
        // restore UI color
        materialController.RestoreTextsColor();
        uiController.ShowGameOver();

        // register Hi-Score to the store if it is new record
        var globalStore = GlobalStore.Instance;
        if (globalStore.ScoreStore.CurrentScore > globalStore.ScoreStore.HiScore)
        {
            globalStore.ScoreStore.UpdateHiScore();
        }

        // reset current stage info
        globalStore.StageStore.SetDefault();

        yield return new WaitForSeconds(0.8f);

        // wait for key press
        while (true)
        {
            if (Input.anyKeyDown)
            {
                // TODO: go to ranking scene
                NextStateSet(GetComponent<TitleState>());
                yield break;
            }

            yield return null;
        }
    }
}
