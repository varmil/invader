using UnityEngine;
using UnityEngine.UI;

public class PlayerLifeView : MonoBehaviour, IObserver
{
    private Text label;

    private PlayerStore dataSource;
    public PlayerStore DataSource
    {
        get { return dataSource; }
        set
        {
            value.Subscribe(this);
            dataSource = value;

            // get initial value
            SetFromDataSource();
        }
    }

    void Awake()
    {
        label = GetComponentInChildren<Text>();
    }

    public void ValueChanged(object value)
    {
        SetFromDataSource();
    }

    private void SetFromDataSource()
    {
        label.text = dataSource.Life.ToString();
    }
}
