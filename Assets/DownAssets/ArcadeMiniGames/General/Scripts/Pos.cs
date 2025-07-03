using System;

namespace ArcadeMiniGames
{
    [Serializable]
    public struct Pos
    {
        public int x;
        public int y;

        public static Pos operator +(Pos a, Pos b) => new Pos { x = a.x + b.x, y = a.y + b.y };

        public static Pos Down { get { return new Pos { x = 0, y = -1 }; } }
        public static Pos Left { get { return new Pos { x = -1, y = 0 }; } }
        public static Pos Right { get { return new Pos { x = 1, y = 0 }; } }
    }
}
