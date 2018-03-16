namespace Constants
{
    /// <summary>
    /// ステージ進行に関する定数クラス
    /// </summary>
    public static class Stage
    {
        // Zoneの端座標(X)
        public static readonly float LeftEnd = -10f;
        public static readonly float RightEnd = 10f;

        // （敵集団）1面における最も高い位置の初期Y座標
        public static readonly float FirstLineYPos = 13.5f;
        // 1面進む毎に、初期Y座標がこれだけ下がっていく。
        public static readonly float InvadedYPosPerStage = 0.5f;
        // 下に侵略する際に1回でどの程度移動するか
        public static readonly float InvadedYPosPerMove = 0.5f;
        // これより下に侵略されるとゲームオーバー（exclusive）
        public static readonly float DeadLineOfYPos = 1.0f;

        // （UFO）座標系
        public static readonly float UFOYPos = FirstLineYPos + 2f;
        public static readonly float UFOLeftEnd = LeftEnd - 5f;
        public static readonly float UFORightEnd = RightEnd + 5f;

        // （Invader）緑 --> 青 --> ピンク --> 黄 --> 赤 の変化基準y座標（exclusive）
        // これを下回ったタイミングで色変化させる
        public static readonly float InvaderGreenYPos = FirstLineYPos;
        public static readonly float InvaderBlueYPos = FirstLineYPos - 2f;
        public static readonly float InvaderPinkYPos = FirstLineYPos - 5f;
        public static readonly float InvaderYellowYPos = FirstLineYPos - 8f;
        public static readonly float InvaderRedYPos = FirstLineYPos - 11f;

    }
}
