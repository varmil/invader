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

    void Start()
    {
        // 初期値設定
        label.text = DataSource.Life.ToString();
    }

    void IObserver.Update()
    {
        label.text = DataSource.Life.ToString();
    }
}
