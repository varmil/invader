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

    public ScoreStore()
    {
        SetDefault();
    }

    public void SetDefault()
    {
        SetDefaultHiScore();
        SetDefaultCurrentScore();
    }

    public void SetDefaultHiScore()
    {
        HiScore = 0;
    }

    public void SetDefaultCurrentScore()
    {
        CurrentScore = 0;
    }

    public void AddScore(int value)
    {
        CurrentScore += value;
    }

    public void UpdateHiScore()
    {
        HiScore = CurrentScore;
    }
}
