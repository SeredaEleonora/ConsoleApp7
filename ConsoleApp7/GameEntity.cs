partial class Program 
{
    public class GameEntity : GameObject
    {
        private int hp;
        public int Hp
        {
            get
            {
                return hp;
            }
            set
            {
                hp = value;
                if (hp <= 0)
                {
                    Destroy();
                }
            }
        }
        public GameEntity(char symbol, Vector2 position, int hp, bool passable = false)
            : base(symbol, position, passable)
        {
            Hp = hp;
            //actionCounter = 0;
        }

        //на одном форуме посоветовали добавить каунтер событий, чтобы потом использовать его в  апдейте
        //пусть будет
        //public int actionCounter

        public void GetDamage(int damage) => Hp -= damage;
        
        public override void Update()
        {
            base.Update();
            //сначала сама разберусь, зачем это всё нужно, а пока пусть будет
            //if (actionCounter > 0) actionCounter--;
        }
    }


}
