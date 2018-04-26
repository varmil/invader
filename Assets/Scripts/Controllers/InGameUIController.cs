using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections.Generic;

/**
 * UIの大元たる親コントローラ
 */
public class InGameUIController : MonoBehaviour
{
    // all text components
    public IEnumerable<Text> Texts
    {
        get;
        private set;
    }

    private GameObject inGameView;

    private ScoreView currentScoreView;
    private PlayerLifeView playerLifeView;
    private GameObject gameOverView;

    private GlobalStore globalStore;

    void Awake()
    {
        inGameView = transform.Find("InGame").gameObject;
        currentScoreView = inGameView.transform.Find("Header").GetComponent<ScoreView>();
        playerLifeView = inGameView.transform.Find("Footer/Life").GetComponent<PlayerLifeView>();
        gameOverView = inGameView.transform.Find("GameOver").gameObject;

        SetAllTexts();
    }

    void OnDestroy()
    {
        globalStore.ScoreStore.Unsubscribe(currentScoreView);
        globalStore.PlayerStore.Unsubscribe(playerLifeView);
    }

    public void Initialize(GlobalStore store)
    {
        // initial state
        HideGameOver();

        // set datasource
        globalStore = store;
        currentScoreView.DataSource = globalStore.ScoreStore;
        playerLifeView.DataSource = globalStore.PlayerStore;
    }

    public void ShowInGame()
    {
        inGameView.SetActive(true);
    }

    public void HideInGame()
    {
        inGameView.SetActive(false);
    }

    public void ShowGameOver()
    {
        gameOverView.SetActive(true);
    }

    public void HideGameOver()
    {
        gameOverView.SetActive(false);
    }

    private void SetAllTexts()
    {
        Texts = GetComponentsInChildren<Text>();
    }
}
