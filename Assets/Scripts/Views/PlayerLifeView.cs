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
            ValueChanged(dataSource.Life);
        }
    }

    void Awake()
    {
        label = GetComponentInChildren<Text>();
    }

    public void ValueChanged(object value)
    {
        label.text = value.ToString();
    }
}
