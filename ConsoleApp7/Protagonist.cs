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

    

    public class MeleeEnemy : Character
    {
        public MeleeEnemy(int x, int y, int z) : base(x, y, z) 
        
        { 

        
        }

        
    }

    public class ArcherEnemy : Protagonist
    {
        public ArcherEnemy(int x, int y, int z) : base(x, y, z)

        {


        }

        
    }

   


}
