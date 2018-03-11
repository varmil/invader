using UnityEngine;
using UnityEngine.UI;

public class ScoreView : MonoBehaviour, IObserver
{
    private static readonly int PaddingWidth = 5;

    private Text label;

    private ScoreStore dataSource;
    public ScoreStore DataSource
    {
        get { return dataSource; }
        set
        {
            value.Subscribe(this);
            dataSource = value;
        }
    }

    void Awake()
    {
        label = GetComponent<Text>();
    }

    void Start()
    {
        label.text = DataSource.CurrentScore.ToString().PadLeft(PaddingWidth, '0');
    }

    void IObserver.Update()
    {
        label.text = DataSource.CurrentScore.ToString().PadLeft(PaddingWidth, '0');
    }
}
