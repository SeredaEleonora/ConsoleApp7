partial class Program // классы, у которых есть наследование и прочие приколы
{
    public class Character : GameObject
    {
        public int Hp {  get; set; }
        

        public Character(Vector2 position, char symbol = 'C', bool isPassable = false, int hp = 20)
            : base(symbol, position, isPassable)
        {
            Hp = hp;
        }
    }

   


}
