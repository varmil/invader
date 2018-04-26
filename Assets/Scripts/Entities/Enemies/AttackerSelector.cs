using System.Collections.Generic;
using System.Linq;

/**
 * 敵集団の中で次に誰がビームするかを決めるロジック
 */
public class AttackerSelector
{
    // 1行に何体の敵がデフォルトで存在するか
    private static readonly int ColumnsNumber = 11;

    private readonly List<EnemyLine> lines;

    public AttackerSelector(List<EnemyLine> lines)
    {
        this.lines = lines;
    }

    // Beam発射可能 == 自インスタンスの真下方向に他の敵がいない
    // 列でみていく
    public IEnumerable<Enemy> GetFireableEnemies()
    {
        return Enumerable.Range(0, ColumnsNumber)
            .Select((i) =>
            {
                var line = this.lines.Aggregate((acc, cur) =>
                {
                    // curのほうが下にあるLine
                    return (cur.Enemies[i].Alive) ? cur : acc;
                });
                return line.Enemies[i];
            })
            // 1列全敵がNotAliveの場合も配列に含まれているので除外
            .Where((enemy) => enemy.Alive);
    }

    // 1体ランダムで選出
    public Enemy GetRandomFireableEnemy()
    {
        return GetFireableEnemies().RandomAt();
    }
}
