/**
 * Singleton && Observableなスコア管理クラス
 */
public class PlayerStore : Subject
{
    // --------------- TEMPLATE ---------------
    private static PlayerStore instance;

    private PlayerStore() { }

    public static PlayerStore Instance
    {
        get
        {
            if (PlayerStore.instance == null)
            {
                PlayerStore.instance = new PlayerStore();
            }
            return PlayerStore.instance;
        }
    }
    // --------------- TEMPLATE END ---------------

    // デフォルト残機は３
    private int life = 3;
    public int Life
    {
        get
        {
            return life;
        }

        set
        {
            life = value;
            Notify();
        }
    }
}
