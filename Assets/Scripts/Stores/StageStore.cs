/**
 * Observableなステージ進行情報管理クラス
 */
public class StageStore : Subject
{
    private int currentStage;
    public int CurrentStage
    {
        get
        {
            return currentStage;
        }

        set
        {
            currentStage = value;
            Notify(currentStage);
        }
    }

    public void SetDefault()
    {
        CurrentStage = 0;
    }

    /// <summary>
    /// go to next stage
    /// </summary>
    public void IncrementStage()
    {
        CurrentStage++;
    }
}
