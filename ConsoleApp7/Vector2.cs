partial class Program //классы без наследования + примерно тут я мы дошли до изучения геттеров и сеттеров, бож, ради всего святого, простите, но мне так лень их сюда вписывать....
{
    public struct Vector2
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Vector2(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static Vector2 operator +(Vector2 a, Vector2 b)
        {
            return new Vector2(a.X + b.X, a.Y + b.Y);
        }

        public static Vector2 operator -(Vector2 a, Vector2 b)
        {
            return new Vector2(a.X - b.X, a.Y - b.Y);
        }

    }
}
