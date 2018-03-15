/**
 * this has all other store
 */
public class GlobalStore : Subject
{
    public GlobalStore()
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
