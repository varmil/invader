namespace Constants
{
    public static class Stage
    {
        // Zoneの端座標(X)
        public static readonly float LeftEnd = -10f;
        public static readonly float RightEnd = 10f;

        // （敵集団）最も高い位置の初期Y座標
        public static readonly float FirstLineYPos = 14f;

        // （UFO）座標系
        public static readonly float UFOYPos = FirstLineYPos + 2f;
        public static readonly float UFOLeftEnd = LeftEnd - 5f;
        public static readonly float UFORightEnd = RightEnd + 5f;
    }
}
