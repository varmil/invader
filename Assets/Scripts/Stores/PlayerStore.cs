/**
 * Observableなスコア管理クラス
 */
public class PlayerStore : Subject
{
    private int life;
    public int Life
    {
        get
        {
            return life;
        }

        set
        {
            life = value;
            Notify(life);
        }
    }

    public void SetDefault()
    {
        Life = Constants.Player.Life;
    }

    public void DecrementLife()
    {
        Life--;
    }
}
