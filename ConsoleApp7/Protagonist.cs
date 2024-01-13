partial class Program // классы, у которых есть наследование и прочие приколы
{
    public class Weapon
    {
        public int W_x;
        public int W_y;

        public int damage = 1;

        public Weapon(int x, int y)
        {
            W_x = x;
            W_y = y;
        }
    }

    public class Arrow : Weapon
    { 
        public Arrow(int x, int y) : base(x, y)
        {
        }
    }

    public class Character
    {
        public int position_x;
        public int position_y;
        public int hp;
        

        public Character(int x, int y, int z)
        {
            position_x = x;
            position_y = y;
            hp = z;
        }
    }
    public class Protagonist : Character
    {
       

        public int count = 0;
        public int map_level = 1;
        public bool HaveWeapon = false;
        public bool YaNenavizhuCats = false;
        public bool prokachka = false;
        public Protagonist(int x, int y, int z) : base(x, y, z) 
        {
            
        }


    }

    public class Masha : Character
    {
        public Masha(int x, int y, int z) : base(x, y, z)

        {

        }

       
        public void Death()
        {
            Console.Clear();
            Console.WriteLine("Маша погибла в подземелье Фефу. Сколько еще душ пожнет это страшное место?");
            Console.ReadLine();
            masha.position_x = 0;
            masha.position_y = 0;
        }
    }

    

    public class Antagonist1 : Character
    {
        public Antagonist1(int x, int y, int z) : base(x, y, z) 
        
        { 

        
        }

        
    }

    public class Antagonist2 : Protagonist
    {
        public Antagonist2(int x, int y, int z) : base(x, y, z)

        {


        }

        
    }

   


}
