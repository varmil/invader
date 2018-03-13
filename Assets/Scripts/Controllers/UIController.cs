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

    private ScoreView currentScoreView;
    private PlayerLifeView playerLifeView;

    void Awake()
    {
        currentScoreView = transform.Find("Scores/Score1Value").GetComponent<ScoreView>();
        playerLifeView = transform.Find("Footer/Life").GetComponent<PlayerLifeView>();

        // set datasource
        currentScoreView.DataSource = ScoreStore.Instance;
        playerLifeView.DataSource = PlayerStore.Instance;
    }

    public void Initialize()
    {
        SetAllTexts();
    }

    private void SetAllTexts()
    {
        Texts = GetComponentsInChildren<Text>();
    }
}
