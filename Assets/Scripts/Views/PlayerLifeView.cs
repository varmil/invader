using UnityEngine;
using UnityEngine.UI;

public class PlayerLifeView : MonoBehaviour, IObserver
{
    [SerializeField]
    private Text label;

    private PlayerStore dataSource;
    public PlayerStore DataSource
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
    }

    void IObserver.Update()
    {
        label.text = DataSource.Life.ToString();
    }
}
