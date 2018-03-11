using UnityEngine;

/**
 * UIの大元たる親コントローラ
 */
public class UIController : MonoBehaviour
{
    private ScoreView currentScoreView;
    private PlayerLifeView playerLifeView;

    private void Awake()
    {
        currentScoreView = transform.Find("Scores/Score1Value").GetComponent<ScoreView>();
        playerLifeView = transform.Find("Footer/Life").GetComponent<PlayerLifeView>();

        // set datasource
        currentScoreView.DataSource = ScoreStore.Instance;
        playerLifeView.DataSource = PlayerStore.Instance;
    }
}
