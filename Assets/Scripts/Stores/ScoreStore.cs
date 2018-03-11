/**
 * Singleton && Observableなスコア管理クラス
 */
public class ScoreStore : Subject
{
    // --------------- TEMPLATE ---------------
    private static ScoreStore instance;

    private ScoreStore() { }

    public static ScoreStore Instance
    {
        get
        {
            if (ScoreStore.instance == null)
            {
                ScoreStore.instance = new ScoreStore();
            }
            return ScoreStore.instance;
        }
    }
    // --------------- TEMPLATE END ---------------

    private int hiScore = 0;
    public int HiScore
    {
        get
        {
            return hiScore;
        }

        private set
        {
            hiScore = value;
            Notify();
        }
    }

    private int currentScore = 0;
    public int CurrentScore
    {
        get
        {
            return currentScore;
        }

        private set
        {
            currentScore = value;
            Notify();
        }
    }

    // 加算
    public void AddScore(int value)
    {
        CurrentScore += value;
    }

    // 上書き
    public void UpdateHiScore(int value)
    {
        HiScore = value;
    }
}
