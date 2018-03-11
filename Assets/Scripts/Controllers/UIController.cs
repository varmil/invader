using UnityEngine;

/**
 * UIの大元たる親コントローラ
 */
public class UIController : MonoBehaviour
{
    private ScoreView currentScoreView;

    private void Awake()
    {
        currentScoreView = transform.Find("Scores/Score1Value").GetComponent<ScoreView>();

        // set datasource
        currentScoreView.DataSource = ScoreStore.Instance;
    }
}
