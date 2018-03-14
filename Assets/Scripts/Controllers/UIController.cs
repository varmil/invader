using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections.Generic;

/**
 * UIの大元たる親コントローラ
 */
public class UIController : MonoBehaviour
{
    // all text components
    public IEnumerable<Text> Texts
    {
        get;
        private set;
    }

    // not create the class yet
    private GameObject titleView;
    private GameObject inGameView;

    private ScoreView currentScoreView;
    private PlayerLifeView playerLifeView;

    void Awake()
    {
        titleView = transform.Find("Title").gameObject;
        inGameView = transform.Find("InGame").gameObject;

        currentScoreView = transform.Find("InGame/Header/Score1Value").GetComponent<ScoreView>();
        playerLifeView = transform.Find("InGame/Footer/Life").GetComponent<PlayerLifeView>();

        // set datasource
        currentScoreView.DataSource = ScoreStore.Instance;
        playerLifeView.DataSource = PlayerStore.Instance;

        SetAllTexts();
    }

    public void Initialize()
    {
    }

    public void ShowTitle()
    {
        titleView.SetActive(true);
    }

    public void HideTitle()
    {
        titleView.SetActive(false);
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
