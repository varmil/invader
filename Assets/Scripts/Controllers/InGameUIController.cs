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

    private GlobalStore globalStore;

    void Awake()
    {
        inGameView = transform.Find("InGame").gameObject;
        currentScoreView = inGameView.transform.Find("Header/Score1Value").GetComponent<ScoreView>();
        playerLifeView = inGameView.transform.Find("Footer/Life").GetComponent<PlayerLifeView>();

        SetAllTexts();
    }

    void OnDestroy()
    {
        globalStore.ScoreStore.Unsubscribe(currentScoreView);
        globalStore.PlayerStore.Unsubscribe(playerLifeView);
    }

    public void Initialize(GlobalStore store)
    {
        globalStore = store;

        // set datasource
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

    private void SetAllTexts()
    {
        Texts = GetComponentsInChildren<Text>();
    }
}
