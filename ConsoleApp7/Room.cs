partial class Program //классы без наследования + примерно тут я мы дошли до изучения геттеров и сеттеров, бож, ради всего святого, простите, но мне так лень их сюда вписывать....
{
    class Room
    {
        // Задаем базовые параметры комнаты
        // она у нас имеет свои размеры и координаты внутри карты
        public int X;
        public int Y;
        public int Width;
        public int Height;
    }

    public class Portal // Портальчик на следующий кровень
    {
        public int X;
        public int Y;

        public Portal(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}
