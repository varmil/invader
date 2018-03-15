/**
 * this has all other store
 */
public class GlobalStore : Subject
{
    // --------------- TEMPLATE ---------------
//    private static GlobalStore instance;
//
//    private GlobalStore()
//    {
//        this.PlayerStore = new PlayerStore();
//        this.ScoreStore = new ScoreStore();
//    }
//
//    public static GlobalStore Instance
//    {
//        get
//        {
//            if (GlobalStore.instance == null)
//            {
//                GlobalStore.instance = new GlobalStore();
//            }
//            return GlobalStore.instance;
//        }
//    }
    // --------------- TEMPLATE END ---------------

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
