using UnityEngine;
using UnityEngine.UI;
using FOO = IObserver;

public class ScoreView : MonoBehaviour, IObserver
{
    private static readonly int PaddingWidth = 5;

    private Text currentScore;
    private Text hiScore;

    private ScoreStore dataSource;
    public ScoreStore DataSource
    {
        get { return dataSource; }
        set
        {
            value.Subscribe(this);
            dataSource = value;

            // set initial value
            SetFromDataSource();
        }
    }

    void Awake()
    {
        currentScore = transform.Find("Score1Value").GetComponent<Text>();
        hiScore = transform.Find("Hi-ScoreValue").GetComponent<Text>();
    }

    public void ValueChanged(object value)
    {
        SetFromDataSource();
    }

    private void SetFromDataSource()
    {
        currentScore.text = dataSource.CurrentScore.ToString().PadLeft(PaddingWidth, '0');
        hiScore.text = dataSource.HiScore.ToString().PadLeft(PaddingWidth, '0');
    }
}
