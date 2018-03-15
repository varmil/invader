/**
 * this has all other store
 */
public class GlobalStore : Subject
{
    public GlobalStore()
    {
        this.PlayerStore = new PlayerStore();
        this.ScoreStore = new ScoreStore();
    }

    public PlayerStore PlayerStore
    {
        get;
    }

    public ScoreStore ScoreStore
    {
        get;
    }
}
