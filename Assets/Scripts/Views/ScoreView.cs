using UnityEngine;
using UnityEngine.UI;
using FOO = IObserver;

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

            // get initial value
            ValueChanged(dataSource.CurrentScore);
        }
    }

    void Awake()
    {
        label = GetComponent<Text>();
    }

    public void ValueChanged(object value)
    {
        label.text = value.ToString().PadLeft(PaddingWidth, '0');
    }
}
