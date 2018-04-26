/**
 * this has all other store
 * singleton
 */
public class GlobalStore : Subject
{
    private static GlobalStore instance = new GlobalStore();

    public static GlobalStore Instance
    {
        get
        {
            return instance;
        }
    }

    private GlobalStore()
    {
        this.PlayerStore = new PlayerStore();
        this.ScoreStore = new ScoreStore();
        this.StageStore = new StageStore();
    }

    public PlayerStore PlayerStore
    {
        get;
    }

    public ScoreStore ScoreStore
    {
        get;
    }

    public StageStore StageStore
    {
        get;
    }
}

