using System;
using System.Linq;
using System.Security.Cryptography;



partial class Program
{
    // Задаем базовые параметры карты и упаковываем их в массив
    // Двухмерный массив – это прямоугольник (2д фигура)
    static int MAP_WIDTH = 60;
    static int MAP_HEIGHT = 40;
    static string[,] mapData = new string[MAP_HEIGHT, MAP_WIDTH];
    
    static MeleeEnemy[] antagonists1; // вводим наших статических антагонистов, чтобы иметь к ним доступ
    static ArcherEnemy[] antagonists2;
    static Masha masha;
    static Protagonist protagonist;// и протагониста туда же


    static List<Arrow> arrows = new List<Arrow>(); // наши предметы
    static Portal portal;
    static Weapon weapon;
    
    static Random random = new Random(); // и основа основ рогалика, разумеется!
    
    
    static void Main() // В главном методе мы используем другие методы для реализации игрового процесса
    {
        
       // вводим нашу булевую переменную
        bool gameOver = false;

        GenerateMap(); // генерируем подземелье
        Nachalo(); // даем игроку предысторию


        while (!gameOver) // а тут уже вводим цикл геймплея
        {
            Console.Clear(); // чистим карту после каждого хода, чтобы мы играли в рогалик, а не свиток
            DrawMap(); // Рисуем карту с игроком и прочими штучками
            
            ConsoleKeyInfo keyInfo = Console.ReadKey(true); // говорим программке, что надо читать клаву

            switch (keyInfo.Key) // перемещаем игрока в зависимости от нажатой клавиши
            {
                case ConsoleKey.W: // можно еще через две лыжные палки || ввести управление стрелочками или вообще ввести все, что угодно
                    MoveProtagonist(0, -1);// используем отдельный метод для перемещения
                    break;
                case ConsoleKey.S:
                    MoveProtagonist(0, 1);
                    break;
                case ConsoleKey.A:
                    MoveProtagonist(-1, 0);
                    break;
                case ConsoleKey.D:
                    MoveProtagonist(1, 0);
                    break;
                case ConsoleKey.Escape:
                    gameOver = true;
                    break;
                case ConsoleKey.Enter:

                    Udar(); // пишем отдельную штуку для избиения

                    break;
            }
            
            // тут мы перемещаем всех наших антагонистов в рандомном направлении, метод ПоЩам проверяет, укусил ли таракан студента,
            // метод Вестерн определяет, стреляет таракан-лучник или нет (создает стрелы как объект), отдельный метод эти стрелы двигает,
            // ОноЖивое проверяет жив ли еще Студент,
            // Тапок проверяет, взял ли студент тапок и убирает его, если все же взял,
            // ТачкаНаПрокачку проверяет, прокачался ли игрок достаточно, чтобы увеличить наносимый им урон,
            // Легенда проверяет, убил ли Студент достаточно тараканов, чтобы заслужить статус легенды

            MoveAntagonists1(antagonists1, random);
            MoveAntagonists2(antagonists2, random);
            PoSham();
            Vestern();
            ArrowMove();
            OnoZhivoe();
            Tapok();
            TachkaNaProkachku();
            Legend();
            Kvest();
           


        }

        static void Nachalo() // так как ХП игрока генерируется рандомно, тут мы даем ему возможность их увидеть
        {
            Console.Write("Вы безутешный студент, не выдержавший испытания сессией из-за одного проваленного зачета.\n" +
                "В поисках славной смерти вы опустились в подземелье Фефу, чтобы заработать себе статус и славу.\n" +
                "Вы были удивлены, обнаружив цивилизацию тараканов, эволюционировавших до охотников-собирателей. Их стрелы точны, а жвалы остры.\n" +
                $"Но и вы не промах. Подберите тапок '!', пройдите в портал '0', убейте столько, сколько сможете.");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write($" Ваше здоровье {protagonist.hp}, ваш путь – НА ВОСТОК!");
            Console.ForegroundColor = ConsoleColor.White;
            Console.ReadLine();
        }

        static void Tapok() // Если игрок подходит к тапку с одной из 4х сторон, то он автоматически берет его
        {
            if ((protagonist.position_x + 1 == weapon.W_x &&
                protagonist.position_y == weapon.W_y) ||
                (protagonist.position_x - 1 == weapon.W_x &&
                protagonist.position_y == weapon.W_y) ||
                (protagonist.position_y + 1 == weapon.W_y &&
                protagonist.position_x == weapon.W_x) ||
                (protagonist.position_y - 1 == weapon.W_y &&
                protagonist.position_x == weapon.W_x))
            {
                Console.Clear();
                mapData[weapon.W_y, weapon.W_x] = "  ";
                weapon.W_x = 0; // это костыль, чтобы теперь, если протагонист встает на координату тапка, не вылетало сообщение об этом
                weapon.W_y = 0;
                Console.WriteLine("Студент подобрал тапок");
                Console.ReadLine();

                protagonist.HaveWeapon = true;
            }
        }

        static void TachkaNaProkachku() // попытка в развитие персонажа
        {
            if (protagonist.count > 5 && protagonist.prokachka == false)
            {
                Console.Clear();
                weapon.damage++;
                Console.WriteLine("Студент убил достаточно тараканов. Его статус повысился. Теперь он лучше машет тапком и наносит больше урона.");
                Console.ReadLine();
                protagonist.prokachka = true;
            }

        }

        static void GenerateMap()
        {
            // рисуем карту при помощи цикла
            // вложенный цикл заполняет ее по обоим измерениям
            for (int i = 0; i < MAP_HEIGHT; i++)
            {
                for (int j = 0; j < MAP_WIDTH; j++)
                {
                    mapData[i, j] = "##";
                }
            }


            

            int numRooms = random.Next(6, 12); // рандомное кол-во комнат
            Room[] rooms = new Room[numRooms]; // комнаты запихиваем в одномерный массив,
                                               // чтобы позже мы тоже могли по нему проходиться циклами + кол-во комнат известно
                                               // с прошлой строки, так что ничего нам не мешает

            antagonists1 = new MeleeEnemy[numRooms]; // в каждой комнате генерится по 2 таракана
            antagonists2 = new ArcherEnemy[numRooms];

            for (int i = 0; i < numRooms; i++)
            {
                // для каждой комнатки мы определяем размер и координаты, но так, чтобы они влезали
                //+ делаем проверку на вшивость и наложение
                int roomWidth = random.Next(4, 11);
                int roomHeight = random.Next(4, 11);


                int roomX = random.Next(1, MAP_WIDTH - roomWidth - 1);
                int roomY = random.Next(1, MAP_HEIGHT - roomHeight - 1);

                Room newRoom = new Room { X = roomX, Y = roomY, Width = roomWidth, Height = roomHeight };


                bool intersects = false;

                for (int j = 0; j < i; j++)
                {
                    Room otherRoom = rooms[j];

                    if (otherRoom != null &&
                        newRoom.X < otherRoom.X + otherRoom.Width && // а это я подсмотрела у одного чела на ютубе (и это единственная полезная информация за 6 часов его стрима
                        newRoom.X + newRoom.Width > otherRoom.X &&
                        newRoom.Y < otherRoom.Y + otherRoom.Height &&
                        newRoom.Y + newRoom.Height > otherRoom.Y)
                    {
                        intersects = true;
                        break;
                    }
                }

                if (intersects) // если наложение происходит, то мы просто откатываем цикл и делаем все заново
                {
                    i--;
                    continue;
                }


                int AntX_1 = random.Next(roomX, roomX + roomWidth);
                int AntY_1 = random.Next(roomY, roomY + roomHeight);

                int AntX_2 = random.Next(roomX, roomX + roomWidth);
                int AntY_2 = random.Next(roomY, roomY + roomHeight);

                int Health = random.Next(1, 5);

                // если все хорошо, то загружаем данные о нашей комнате в карту
                // и запихитиваем комнату в массив + создаем антагонистов

                ArcherEnemy antagonist_2 = new ArcherEnemy(AntX_2, AntY_2, Health);
                antagonists2[i] = antagonist_2;

                MeleeEnemy antagonist_1 = new MeleeEnemy(AntX_1, AntY_1, Health);
                antagonists1[i] = antagonist_1;

                DrawRoom(newRoom, AntX_1, AntY_1, AntX_2, AntY_2); 
                rooms[i] = newRoom;


            }

            int numOfProRoom = random.Next(0, numRooms); // это у нас будет номер той комнаты, в которую мы поместим игрока
            Room proRoom = rooms[numOfProRoom];

            int numOfMashaRoom = random.Next(0, numRooms);
            Room MashaRoom = rooms[numOfMashaRoom]; // Сюда пихаем Машу

            int proX = random.Next(proRoom.X + 1, proRoom.X + proRoom.Width - 1);
            int proY = random.Next(proRoom.Y + 1, proRoom.Y + proRoom.Height - 1);
            int proH = random.Next(3, 9); // рандомные очки здоровья

            int MashaX = random.Next(MashaRoom.X + 1, MashaRoom.X + MashaRoom.Width - 1);
            int MashaY = random.Next(MashaRoom.Y + 1, MashaRoom.Y + MashaRoom.Height - 1);
            int MashaHp = 5;

            protagonist = new Protagonist(proX, proY, proH); // создаем протагониста
            masha = new Masha(MashaX, MashaY, MashaHp);

            // протягиваем коридор от комнатки до комнатки, исключая последнюю (там уже и так идти некуда)

            for (int i = 0; i < rooms.Length - 1; i++)
            {
                if (rooms[i] != null && rooms[i + 1] != null)
                {
                    Room startRoom = rooms[i]; // выбираем начальную и следующую комнатки
                    Room endRoom = rooms[i + 1];

                    int startX = random.Next(startRoom.X, startRoom.X + startRoom.Width); // выбираем рандомные точки, соединяющие комнаты
                    int startY = random.Next(startRoom.Y, startRoom.Y + startRoom.Height);

                    int endX = random.Next(endRoom.X, endRoom.X + endRoom.Width);
                    int endY = random.Next(endRoom.Y, endRoom.Y + endRoom.Height);

                    DrawCorridor(startX, startY, endX, endY);
                }
            }


            int portalRoomIndex = random.Next(0, numRooms); // делаем портал в следующее подземелье (по сути, это просто точка на карте, наступив на которую, студент генерит новую карту
            Room portalRoom = rooms[portalRoomIndex]; // генерится почти так же, как и протагонист
            int portalX = random.Next(portalRoom.X + 1, portalRoom.X + portalRoom.Width - 1);
            int portalY = random.Next(portalRoom.Y + 1, portalRoom.Y + portalRoom.Height - 1);

            portal = new Portal(portalX, portalY);

            mapData[portal.Y, portal.X] = "О ";

            int WeaponRoomIndex = random.Next(0, numRooms); // тапок генерится почти так же, как портал
            Room WeaponRoom = rooms[WeaponRoomIndex];
            int WeaponRoomX = random.Next(WeaponRoom.X + 1, WeaponRoom.X + WeaponRoom.Width - 1);
            int WeaponRoomY = random.Next(WeaponRoom.Y + 1, WeaponRoom.Y + WeaponRoom.Height - 1);

            weapon = new Weapon(WeaponRoomX, WeaponRoomY);

            mapData[weapon.W_y, weapon.W_x] = " !";

            int VasyaIR = random.Next(0, numRooms);
            Room VasyaRoom = rooms[VasyaIR];
            int V_x = random.Next(VasyaRoom.X + 1, VasyaRoom.X + VasyaRoom.Width - 1);
            int V_y = random.Next(VasyaRoom.Y + 1, VasyaRoom.Y + VasyaRoom.Height - 1);

            mapData[V_y, V_x] = "@ ";
        }


        static void MoveProtagonist(int changeX, int changeY) // Двигательный метод
        {
            int newX = protagonist.position_x + changeX;
            int newY = protagonist.position_y + changeY;
            // Позволяем двигаться только если новая позиция - это пустое пространство

            if(newY  == masha.position_y && newX == masha.position_x)
            {

                Console.Clear();
                Console.WriteLine("Я играла в медленное вычеркивание и не заметила, как меня утащили тараканы. Тут недалеко должен быть Вася, приведи меня к нему!");
                protagonist.YaNenavizhuCats = true;
                Console.ReadLine();

            }
            if (mapData[newY, newX] == "@ ")
            {
                if (protagonist.YaNenavizhuCats == true)
                {
                    protagonist.YaNenavizhuCats = false;
                    Console.Clear();
                    protagonist.hp += 5;
                    Console.WriteLine("Спасибо тебе, добрый друг. Нас уже ничего не спасет, у нас низший статус во всем подземелье, но мы можем отдать тебе Энергетик из СамВозьми. \n Это поможет исцелить раны!");
                    Console.WriteLine($"Вы выпиваете энергетик и чувствуете, как силы возвращаются к вам. Ваше здоровье увеличилось на 5 единиц. Здоровье студента равно {protagonist.hp}.");
                    if (masha.hp > 3) 
                    { Console.WriteLine("У нас мало вещей, которые мы можем подарить тебе, но держи шкуры тараканов. Они увеличат твой статус!"); 
                        protagonist.count += 4; }
                    Console.ReadLine();
                }

                Console.Clear();
                Console.WriteLine("Вы стоите рядом и слышите, как мальчик бубнит себе под нос что-то про вычеркивание нименьшего или наибольшего числа. Вам, мягко говоря, не особо интересно.");
                Console.ReadLine();

            }

            if (mapData[newY, newX] == "  ")
            {
                protagonist.position_x = newX;
                protagonist.position_y = newY;
            }
            if (mapData[newY, newX] == "О ")
            {
                protagonist.map_level++;
                GenerateMap(); // Генерируем новый уровень карты

            }
           
        }

        static void DrawRoom(Room room, int anty, int antx, int anty_2, int antx_2)
        
        {
            // заполняем таким же образом, как заполняли карту, и записываем в нее же наши комнатки и тараканов
            for (int i = room.X; i < room.X + room.Width; i++)
            {
                for (int j = room.Y; j < room.Y + room.Height; j++)
                {
                    mapData[j, i] = "  ";

                    mapData[antx, anty] = " A";

                    mapData[antx_2, anty_2] = "A>";
                }
            }
        }

        static void Udar()
        {
            foreach (MeleeEnemy antagonist1 in antagonists1)
            {
                if (protagonist.HaveWeapon == true && ((protagonist.position_x + 1 == antagonist1.position_x &&
                            protagonist.position_y == antagonist1.position_y) ||
                            (protagonist.position_x - 1 == antagonist1.position_x &&
                            protagonist.position_y == antagonist1.position_y) ||
                            (protagonist.position_x == antagonist1.position_x &&
                            protagonist.position_y == antagonist1.position_y + 1) ||
                            (protagonist.position_x == antagonist1.position_x &&
                            protagonist.position_y == antagonist1.position_y - 1)))
                {

                    antagonist1.hp -= weapon.damage;
                    
                    Console.Clear();
                    Console.WriteLine("Студент замахивается и бьет таракана тапком. Таракан теряет 1 очко здоровья.");
                    Console.ReadLine();
                    if (antagonist1.hp == 0)
                    {
                        mapData[antagonist1.position_y, antagonist1.position_x] = "  ";
                        antagonist1.position_x = 0; // это опять костыль, чтобы убрать таракана с карты (сначала хотела оставить их трупы, а потом поняла, что они проходы закрывают
                        antagonist1.position_y = 0;
                        
                        protagonist.count++;
                        Console.Clear();
                        Console.WriteLine($"Таракан, превозмогая смерть, улыбнулся. Студент избавил подземелье Фефу от {protagonist.count} тараканов."); // тупая отсылка на властелина колец
                        Console.ReadLine();
                    }

                }
            }

            foreach (ArcherEnemy antagonist2 in antagonists2)
            {
                if (protagonist.HaveWeapon == true && ((protagonist.position_x + 1 == antagonist2.position_x &&
                           protagonist.position_y == antagonist2.position_y) ||
                           (protagonist.position_x - 1 == antagonist2.position_x &&
                           protagonist.position_y == antagonist2.position_y) ||
                           (protagonist.position_x == antagonist2.position_x &&
                           protagonist.position_y == antagonist2.position_y + 1) ||
                           (protagonist.position_x == antagonist2.position_x &&
                           protagonist.position_y == antagonist2.position_y - 1)))


                {
                    antagonist2.hp -= weapon.damage; 
                    
                    Console.Clear();
                    Console.WriteLine("Студент замахивается и бьет таракана тапком. Таракан теряет 1 очко здоровья.");
                    Console.ReadLine();
                    
                    if (antagonist2.hp == 0)
                    {
                        mapData[antagonist2.position_y, antagonist2.position_x] = "  ";
                        antagonist2.position_x = 0;
                        antagonist2.position_y = 0;
                        
                        protagonist.count++;
                        Console.Clear();
                        Console.WriteLine($"Таракан, превозмогая смерть, улыбнулся и помер через секунду. Студент избавил подземелье Фефу от {protagonist.count} тараканов.");
                        Console.ReadLine();
                    }

                }
            }
        }
        
        static void DrawCorridor(int startX, int startY, int endX, int endY)
        {
            // протягиваем коридорчики по х и у (берем начальное значение, а оно 100 проц будет минимальным,
            //цикл кончается, когда оно уравнивается с конечным
            for (int x = Math.Min(startX, endX); x <= Math.Max(startX, endX); x++)
            {
                mapData[startY, x] = "  ";
            }


            for (int y = Math.Min(startY, endY); y <= Math.Max(startY, endY); y++)
            {
                mapData[y, endX] = "  ";
            }
        }


        static void DrawMap()
        {
            for (int i = 0; i < MAP_HEIGHT; i++)
            {
                for (int j = 0; j < MAP_WIDTH; j++)
                {
                    // Проверяем, является ли текущая позиция позицией игрока
                    if (i == protagonist.position_y && j == protagonist.position_x && protagonist.HaveWeapon == true)
                    {
                        Console.ForegroundColor = ConsoleColor.Blue; // Люблю голубой), так что если игрок подобрал тапок, то его цвет меняется
                        Console.Write("P ");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    else if (i == protagonist.position_y && j == protagonist.position_x)
                    {
                        
                        Console.Write("P ");
                        
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                        Console.Write(mapData[i, j]);
                        Console.ForegroundColor = ConsoleColor.White;
                    }

                    if (i == masha.position_y && j == masha.position_x)
                    {
                        Console.ForegroundColor = ConsoleColor.Red; 
                        Console.Write("♥");
                        Console.ForegroundColor = ConsoleColor.White;
                    }

                    

                    
                }
                Console.WriteLine();
            }


        }

        

        static void Vestern() // самые быстрые лапки на диком западе)))
        {
            foreach (var antagonist in antagonists2)
            {
                int luck = random.Next(1, 10);

                if (antagonist.position_y == protagonist.position_y && antagonist.position_x < protagonist.position_x)
                {
                    if (antagonist.position_x + 10 < protagonist.position_x && protagonist.position_x < antagonist.position_x + 15 && luck / 2 == 0) //проверка на удачу игрока и прицел таракана
                    {
                        Console.Clear();
                        Console.WriteLine("Таракан натягивает тетиву и целится, но Студент ускользнул.");
                        Console.ReadLine();
                        
                       
                    }
                    else if (antagonist.position_x + 10 > protagonist.position_x) // да, он стреляет только влево, НО я могу еще дофига раз это все скопировать, заменяя иксы и игреки, плюсы и минусы, это не сложно,
                    {                                                             // а помечается антагонист как А>, с луком вправо (да и, если честно, и так сложно играть бывает, лучше уж поверить,
                                                                                  // что все тараканы правши, и оставить в покое игровой баланс
                        Arrow arrow = new Arrow(antagonist.position_x + 1, protagonist.position_y);
                        arrows.Add(arrow);
                        mapData[arrow.W_y, arrow.W_x] = "- ";

                        Console.Clear();
                        Console.WriteLine($"Таракан натягивает тетиву, целится и стреляет.");

                        Console.ReadLine();
                    }
                }


            }
        }


        static void PoSham()
        {
            foreach (var antagonist in antagonists1)
            {
                if (antagonist.position_x == protagonist.position_x && antagonist.position_y == protagonist.position_y)
                {
                    protagonist.hp -= 1; // Протагонист теряет 1 очко при столкновении с тараканом
                    Console.Clear();
                    Console.WriteLine($"Таракан подземелья Фефу кусает студента. Студент теряет очко, пока это только очко здоровья: здоровье студента равно {protagonist.hp}");
                    if (protagonist.YaNenavizhuCats == true)
                    {
                        masha.hp -= 1;
                        Console.WriteLine($"Убегая, таракан задел Машу своими жвалами. Маша тоже теряет очко. Здоровье Маши равно {masha.hp}.");
                    }
                    Console.ReadLine();

                }

            }
        }

        static void OnoZhivoe()
        {
            if (masha.hp <= 0)
            {
                masha.Death();
                masha.hp = 1;
                protagonist.YaNenavizhuCats = false;
            }
            if (protagonist.hp <= 0)
            {
                Console.Clear();
                Console.WriteLine("Студент потерял очко и умер смертью храбрых. Не он первый, не он последний. Подземелья Фефу ждут нового перваша.");
                Console.ReadLine();
                Valhalla(); // метод, выводящий статистику катки
                Console.ReadLine();
                Environment.Exit(0); // пока-пока, если протагонист умер
            }
        }

        static void ArrowMove()
        {
            List<Arrow> arrowsToRemove = new List<Arrow>();

            foreach (Arrow arrow in arrows)
            {   // Проверяем, находится ли следующая позиция стрелы в пределах карты и нет ли там стены или игрока
                if (arrow.W_x + 1 < MAP_WIDTH && mapData[arrow.W_y, arrow.W_x + 1] != "##")
                {
                    // Перемещаем стрелу на одну клеточку вправо и обновляем карту
                    mapData[arrow.W_y, arrow.W_x] = "  ";
                    arrow.W_x += 1;
                    mapData[arrow.W_y, arrow.W_x] = "- ";

                    // Проверяем, попала ли стрела в нашего протагониста
                    if (arrow.W_y == protagonist.position_y && arrow.W_x == protagonist.position_x)
                    {
                        protagonist.hp -= 1;
                        Console.Clear();
                        Console.WriteLine($"В студента прилетела стрела, выпущенная тараканом-лучником. Здоровье студента равно {protagonist.hp}");

                        // стрела попала, итс тайм ту стап
                        arrowsToRemove.Add(arrow);
                        mapData[arrow.W_y, arrow.W_x] = "  ";
                    }
                }
                else
                {
                    // Стрела врезалась в стену или вышла за пределы карты, и мы ее удаляем
                    arrowsToRemove.Add(arrow);
                    mapData[arrow.W_y, arrow.W_x] = "  ";
                }

                
            }


            // Удаляем все стрелы, которые врезались в стену или протагониста
            foreach (Arrow arrowToRemove in arrowsToRemove)
            {
                arrows.Remove(arrowToRemove);
            }



        }


        static void MoveAntagonists2(ArcherEnemy[] antagonists2, Random random)
        {
            foreach (var antagonist2 in antagonists2)
            {
                // Удаляем старую позицию с карты
                mapData[antagonist2.position_y, antagonist2.position_x] = "  ";

                // Случайно выбираем направление движения 
                int direction = random.Next(0, 4);
                switch (direction)
                {
                    case 0: //верх
                        if (antagonist2.position_y > 0 && mapData[antagonist2.position_y - 1, antagonist2.position_x] == "  ")
                            antagonist2.position_y--;
                        break;
                    case 1:// низ
                        if (antagonist2.position_y < MAP_HEIGHT - 1 && mapData[antagonist2.position_y + 1, antagonist2.position_x] == "  ")
                            antagonist2.position_y++;
                        break;
                    case 2: // лево 
                        if (antagonist2.position_x > 0 && mapData[antagonist2.position_y, antagonist2.position_x - 1] == "  ")
                            antagonist2.position_x--;
                        break;
                    case 3: // право
                        if (antagonist2.position_x < MAP_WIDTH - 1 && mapData[antagonist2.position_y, antagonist2.position_x + 1] == "  ")
                            antagonist2.position_x++;
                        break;
                }

                mapData[antagonist2.position_y, antagonist2.position_x] = "A>";
            }



        }
        
        static void MoveAntagonists1(MeleeEnemy[] antagonists1, Random random) // 1 в 1 предыдущий метод
        {

            foreach (var antagonist1 in antagonists1)
            {
                // Удаляем старую позицию антагониста на карте
                mapData[antagonist1.position_y, antagonist1.position_x] = "  ";

                // случайно выбираем направление движения антагониста
                int direction = random.Next(0, 4);
                switch (direction)
                {
                    case 0: // вверх
                        if (antagonist1.position_y > 0 && mapData[antagonist1.position_y - 1, antagonist1.position_x] == "  ")
                            antagonist1.position_y--;
                        break;
                    case 1:// вниз
                        if (antagonist1.position_y < MAP_HEIGHT - 1 && mapData[antagonist1.position_y + 1, antagonist1.position_x] == "  ")
                            antagonist1.position_y++;
                        break;
                    case 2: // влево
                        if (antagonist1.position_x > 0 && mapData[antagonist1.position_y, antagonist1.position_x - 1] == "  ")
                            antagonist1.position_x--;
                        break;
                    case 3: // вправо
                        if (antagonist1.position_x < MAP_WIDTH - 1 && mapData[antagonist1.position_y, antagonist1.position_x + 1] == "  ")
                            antagonist1.position_x++;
                        break;
                }

                mapData[antagonist1.position_y, antagonist1.position_x] = "A ";
            }
        }

        void Legend() // мне просто хочется, чтобы игроку было, к чему стремиться + я поняла, что похорошему студента надо было генерить вне метода генерации карты, потому что перезапуская ее 
            // порталом, мы перезапускаем и игрока, поэтому 20 тараканов, а не 50 (еще есть, куда стремиться, мягко говоря)

        {

            if (protagonist.count > 20)
            {
                Console.Clear();
                Console.WriteLine("Студент убил так много тараканов, что они сбежали из подземелья Фефу в подземелье ВВГУ. Вы благополучно выбрались из подземелья, поздравляем с успешным завершением игры!");
                Valhalla();
                Console.ReadLine();
                gameOver = true;
            }
            
            

        }

        static void Valhalla() // раздача статусов
        {
            Console.WriteLine($"Вы убили {protagonist.count} тараканов.");
            if (protagonist.HaveWeapon)
            {

                Console.WriteLine("Вы подобрали меч.");
            }

            
            Console.WriteLine($"Вы побывали в {protagonist.map_level} подземельях(e).");

            if (protagonist.count > 15)
            {
                Console.WriteLine("Статус: Выпускник. Вы машина для убийств, и этим все сказано. Дальше только переходить с тараканов на индусов.");
            }
            else if (protagonist.count > 10)
            {
                Console.WriteLine("Статус: Старшекурсник. Вы определенно можете выжить везде, поскольку жизнь в Фефу закалила вас лучше любой кутузки."); 
            }
            else if (protagonist.count < 10 && protagonist.HaveWeapon)
            {
                Console.WriteLine("Статус: Второгодка. Вы способны на что-то, но не на многое. Земли Фефу еще не закалили вас, но всё впереди!");
            }
            else if (protagonist.HaveWeapon ==  false)
            {
                Console.WriteLine("Статус: Чепушило. Вы тут и года не протянете. Удачи в касте опущенных.");

            }
            Console.ReadLine();
            

        }

        void Kvest()
        {
            if (protagonist.YaNenavizhuCats == true) 
            {
                masha.position_x = protagonist.position_x;
                masha.position_y = protagonist.position_y;

                
            }
        }

    }

}

