/**
 * Observableなスコア管理クラス
 */
public class ScoreStore : Subject
{
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
            Notify(hiScore);
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
            Notify(currentScore);
        }
    }

    public void SetDefault()
    {
        HiScore = 0;
        CurrentScore = 0;
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
