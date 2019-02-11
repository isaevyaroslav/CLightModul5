using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLightModul5
{
    class Task2
    {
        static string mapPath = @"..\..\asteroidMap.txt";
        static char buttonBorder = '#';
        static int menuButtonsWidth = 30;
        static int currentMenuCommand = 0;
        static string[] menuButtons =
        {
            "Начать игру",
            "Режим выживания",
            "Управление",
            "Выход"
        };

        static float resoureceGeneratorPower = 0.1f; 
        static Random random = new Random();
        static ConsoleKeyInfo consoleKey;
        static int viewPortX = 0, viewPortY = 0;
        static char[,] map;
        static int viewPortWidth, viewPortHeight;
        static int worryTime = 2000;
        static int worryTimeCounter = 0;

        const ConsoleColor defaultBgColor = ConsoleColor.Black;
        const ConsoleColor defaultFgColor = ConsoleColor.White;
        static ConsoleColor asteroidColor = ConsoleColor.DarkGray;
        static ConsoleColor asteroidLightColor = ConsoleColor.Gray;
        static ConsoleColor spaceColor = ConsoleColor.Black;
        static ConsoleColor wallsColor = ConsoleColor.DarkBlue;
        static ConsoleColor spaceShipColor = ConsoleColor.DarkCyan;
        static ConsoleColor spaceManColor = ConsoleColor.Red;
        static ConsoleColor lightColor = ConsoleColor.Yellow;
        static char asteroidRaw = '#', spaceRaw = '.', spaceShipRaw = '$', spaceManRaw = '@';
        static char asteroidChar = ' ', spaceChar = ' ',  spaceShipChar = '$', rockChar = 'G';
        static char[] spaceWallsChars = {'o','O','0','#'};
        static char[] spaceManChars = { '>', 'v', '<', '^' };
        static int spaceManTurn = 0;
        static int[] spaceManYX = { 0, 0 };
        static int[,] rocksYX = new int[0,4];
        static float waveTimePow = 0.05f;
        static float waveSpawnsPow = 0.05f;
        static float waveRocksPow = 0.05f;


        static float helpTimer;
        static float rockWaveTimer;
        static int waveRockSpawns;
        static int rocksPerSpawn;
        static int nextWaveRockSpawns;
        static int rockWaveTime;
        static int waveCount;
        static float resources;
        static float resourceGeneratorsCount;


        static bool programExit = false;
        static bool menuExit = false;
        static bool gameStart = true;
        static bool gameWin = false;
        static bool survival = false;



        static void Main(string[] args)
        {
            do
            {
                PlayMenu();
                PlayGame();
            } while (!programExit);
            

            
        }
        static void PlayMenu()
        {
            int[] menuPositionYX;
            do
            {
                
                ShowMenuButtons(2, asteroidLightColor, wallsColor);
                ShowTask("Введите номер кнопки или выберите стрелками.");
                ReadMenuKeys();
            } while (!menuExit);
        }
        static void DrawSpace()
        {
            for (int row = 0; row < Console.WindowHeight; row++)
            {
                for (int col = 0; col < Console.WindowWidth; col++)
                {
                    if(random.Next(10) == 7)
                    {
                        Console.BackgroundColor = defaultBgColor;
                        Console.ForegroundColor = defaultFgColor;
                        Console.SetCursorPosition(col , row);
                        Console.Write(".");
                    }
                }
            }
        }
        static void ShowTask(string taskText)
        {
            int[] taskPosition = new int[]{
                Console.WindowHeight - 2,
                Console.WindowWidth/2 - taskText.Length/2
            };
            WriteText(taskText, taskPosition);
        }
        static void ShowMenuButtons(int margin, ConsoleColor bgColor, ConsoleColor fgColor)
        {
            int[] menuPositionYX = new int[] {
                1,
                    Console.WindowWidth/2-menuButtonsWidth/2
                };
            Console.Clear();
            DrawSpace();
            ConsoleColor currentColor;
            for (int buttonNumber = 0; buttonNumber < menuButtons.GetLength(0); buttonNumber++)
            {
                currentColor = bgColor;
                if (buttonNumber == currentMenuCommand)
                {
                    currentColor = lightColor;
                }
                DrawButton("("+buttonNumber+") "+menuButtons[buttonNumber], menuPositionYX, buttonBorder, currentColor, fgColor);
                menuPositionYX[0] += margin + 5;
            }
        }
        static void ReadMenuKeys()
        {
            Console.CursorVisible = false;
            Console.WriteLine();
            Console.BackgroundColor = defaultBgColor;
            ConsoleKeyInfo consoleKeyInfo = Console.ReadKey();
            if (int.TryParse(consoleKeyInfo.KeyChar.ToString(), out int userCommand)){
                currentMenuCommand = userCommand;
                ExecuteMenuCommand();
            }
            else
            {
                switch (consoleKeyInfo.Key)
                {
                    case ConsoleKey.UpArrow:
                        if (currentMenuCommand - 1 < 0)
                        {
                            currentMenuCommand = menuButtons.Length - 1;
                        }
                        else
                        {
                            currentMenuCommand--;
                        }
                        break;
                    case ConsoleKey.DownArrow:
                        if (currentMenuCommand + 1 > menuButtons.Length - 1)
                        {
                            currentMenuCommand = 0;
                        }
                        else
                        {
                            currentMenuCommand++;
                        }
                        break;
                    case ConsoleKey.Enter:
                        ExecuteMenuCommand();
                        break;
                }
            }
        }
        static void ExecuteMenuCommand()
        {
            ShowMenuButtons(2, asteroidLightColor, wallsColor);
            System.Threading.Thread.Sleep(500);
            switch (currentMenuCommand)
            {
                case 0:
                    StartGame();
                    break;
                case 1:
                    StartGame(true);
                    break;
                case 2:
                    ShowInstructions();
                    break;
                case 3:
                    menuExit = true;
                    gameStart = false;
                    programExit = true;
                    break;
                default:
                    break;
            }
        }
        static void ShowInstructions()
        {
            Console.Clear();
            WriteText("Инструкция по управлению игрой:", new int[] { 2, 10 });
            WriteText("Поле астеройда:", new int[] { 4, 5 });
            WriteText(" ", new int[] { 4, 25 }, wallsColor, asteroidColor);
            WriteText("Отважный астронафт:", new int[] { 6, 5 });
            WriteText(">", new int[] { 6, 25 }, spaceManColor, lightColor);
            WriteText("Строительство стен:", new int[] { 8, 5 });
            WriteText("о -> O -> 0 -> #", new int[] { 8, 25 }, wallsColor, asteroidLightColor);
            WriteText("Генератор ресурсов:", new int[] { 10, 5 });
            WriteText("# -> ", new int[] { 10, 25 }, wallsColor, asteroidLightColor);
            WriteText("$", new int[] { 10, 31 }, spaceShipColor, asteroidLightColor);
            WriteText("Метеоры:", new int[] { 12, 5 });
            WriteText("G", new int[] { 12, 25 }, wallsColor, asteroidLightColor);
            WriteText("Перемещение по астеройду: стрелка вверх, стрелка вправо, стрелка вниз, стрелка влево", new int[] { 14, 5 });
            WriteText("Поворот: вверх - \"w\", влево - \"a\", вниз - \"s\", вправо - \"d\"", new int[] { 14, 5 });
            WriteText("Стройте генераторы ресурсов, чтобы быстрее возводить стены.", new int[] { 16, 5 });
            WriteText("Оставляйте себе выход из стен, чтобы строить их толще.", new int[] { 17, 5 });
            WriteText("Чем толще и мощнее стены тем дольше Вы продержитесь.", new int[] { 18, 5 });
            WriteText("Нажмите любую кнопку, чтобы выйти в меню.", new int[] { 20, 5 });
            Console.Read();
        }
        static void StartGame(bool survivalMode = false)
        {
            if (survivalMode)
            {
                ShowSurvivalHistory();
                helpTimer = 0;
            }
            else
            {
                ShowHelpHistory();
                helpTimer = 150000;
            }
            rockWaveTimer = 30000;
            waveRockSpawns = 5;
            rocksPerSpawn = 1;
            nextWaveRockSpawns = 10;
            rockWaveTime = 30000;
            waveCount = 0;
            resources = 0;
            resourceGeneratorsCount = 1;
            menuExit = false;
            gameStart = true;
            gameWin = false;
            survival = survivalMode;
            rocksYX = new int[0, 4];
            PlayGame();
        }
        static void ShowHelpHistory()
        {
            Console.Clear();
            DrawSpace();
            WriteText("Давным давно, в далёкой далёкой галактике.", new int[] { 2, 10 });
            WriteText("Экспедиция по изучению новых планет для терраформирования", new int[] { 4, 7 });
            WriteText("из-за ошибки в сверхсветовом ускорителе попала в астеройдное поле высокого уровня опасности.", new int[] { 5, 5 });
            WriteText("Проснувшись по тревоге от продолжительной заморозки, астронафт Вэйкрафт", new int[] { 7, 5 });
            WriteText("вывалился из капсулы от сильного сотрясения корабля.", new int[] { 8, 5 });
            WriteText("Вползая на капитанский мостик, он увидел, что вся команда не в сознании,", new int[] { 10, 5 });
            WriteText("а корабль стремительно дрейфует в космосе в сторону огромного астеройда.", new int[] { 11, 5 });
            WriteText("Всё что он успел сделать - это вползти в эвакуационный шлюз и вялыми руками нажать на красную кнопку.", new int[] { 13, 5 });
            WriteText("В этот момент космолёт ударился о твёрдую поверхность космического камня, ", new int[] { 14, 5 });
            WriteText("а спасательный аппарат с Вэйкрафтом, немного отлетев, приземлился рядом с местом катастрофы. ", new int[] { 15, 5 });
            WriteText("Одев скафандр, отважный астронафт обнаружил, что вся команда погибла от разгерметизации. ", new int[] { 17, 5 });
            WriteText("К счастью, генератор ресурсов был цел и ему, как опытному инженеру, удалось наладить его.", new int[] { 18, 5 });
            WriteText("Но тишина не долго \"баловала\". Восстановленный с трудом, радар загорелся красным цветом. ", new int[] { 20, 5 });
            WriteText("От взрыва астеройд набрал большую скорость и летит в метеоритное поле. ", new int[] { 21, 5 });
            WriteText("Вызвав помощь с ближайшей обитаемой планеты, он начал готовиться к обороне...", new int[] { 22, 5 });
            WriteText("Нажмите любую кнопку, чтобы начать игру.", new int[] { 24, 5 });
            Console.ReadKey();
        }
        static void ShowSurvivalHistory()
        {
            Console.Clear();
            DrawSpace();
            WriteText("Давным давно, в далёкой далёкой галактике.", new int[] { 2, 10 });
            WriteText("Экспедиция по изучению новых планет для терраформирования", new int[] { 4, 7 });
            WriteText("из-за ошибки в сверхсветовом ускорителе попала в астеройдное поле высокого уровня опасности.", new int[] { 5, 5 });
            WriteText("Проснувшись по тревоге от продолжительной заморозки, астронафт Вэйкрафт", new int[] { 7, 5 });
            WriteText("вывалился из капсулы от сильного сотрясения корабля.", new int[] { 8, 5 });
            WriteText("Вползая на капитанский мостик, он увидел, что вся команда не в сознании,", new int[] { 10, 5 });
            WriteText("а корабль стремительно дрейфует в космосе в сторону огромного астеройда.", new int[] { 11, 5 });
            WriteText("Всё что он успел сделать - это вползти в эвакуационный шлюз и вялыми руками нажать на красную кнопку.", new int[] { 13, 5 });
            WriteText("В этот момент космолёт ударился о твёрдую поверхность космического камня, ", new int[] { 14, 5 });
            WriteText("а спасательный аппарат с Вэйкрафтом, немного отлетев, приземлился рядом с местом катастрофы. ", new int[] { 15, 5 });
            WriteText("Одев скафандр, отважный астронафт обнаружил, что вся команда погибла от разгерметизации. ", new int[] { 17, 5 });
            WriteText("К счастью, генератор ресурсов был цел и ему, как опытному инженеру, удалось наладить его.", new int[] { 18, 5 });
            WriteText("Но тишина не долго \"баловала\". Восстановленный с трудом, радар загорелся красным цветом. ", new int[] { 20, 5 });
            WriteText("От взрыва астеройд набрал большую скорость и летит в метеоритное поле. Нужно готовиться к обороне...", new int[] { 21, 5 });
            WriteText("Нажмите любую кнопку, чтобы начать игру.", new int[] { 24, 5 });
            Console.ReadKey();
        }
        static void DrawButton(string text, int[] positionYX, char border, 
            ConsoleColor bgColor = defaultBgColor, 
            ConsoleColor fgColor = defaultFgColor)
        {
            int textLength = text.Length;
            int[] textPosition = new int[]
            {
                positionYX[0]+2,
                positionYX[1]+(menuButtonsWidth/2-textLength/2)
            };
            string buttonLine = "";
            string buttonWalls = "";
            for (int charNumber = 0; charNumber <= menuButtonsWidth; charNumber++)
            {
                buttonLine += border;
                if(charNumber == 0 || charNumber == menuButtonsWidth)
                {
                    buttonWalls += border;
                }
                else
                {
                    buttonWalls += " ";
                }
            }
            Console.SetCursorPosition(positionYX[1], positionYX[0]);
            Console.BackgroundColor = bgColor;
            Console.ForegroundColor = fgColor;
            Console.Write(buttonLine);
            Console.SetCursorPosition(positionYX[1], positionYX[0]+1);
            Console.Write(buttonWalls);
            Console.SetCursorPosition(positionYX[1], positionYX[0] + 2);
            Console.Write(buttonWalls);
            Console.SetCursorPosition(positionYX[1], positionYX[0] + 3);
            Console.Write(buttonWalls);
            Console.SetCursorPosition(positionYX[1], positionYX[0] + 4);
            Console.Write(buttonLine);
            WriteText(text, textPosition, fgColor, bgColor);
            Console.SetCursorPosition(0, 0);

        }
        static void PlayGame()
        {
            if (gameStart)
            {
                Console.BackgroundColor = defaultBgColor;
                Console.Clear();
                if (File.Exists(mapPath))
                {
                    map = GetMap(File.ReadAllLines(mapPath));
                }
                else
                {
                    Console.WriteLine("Вы забыли взять файл с картой игры из её папки.\n Перенесите её в корневую папку игры.");
                }
                viewPortWidth = map.GetLength(1);
                viewPortHeight = map.GetLength(0);
                DrawMap(viewPortY, viewPortX);
            }
            while(gameStart)
            {
                System.Threading.Thread.Sleep(50);
                HelpMission();

                MoveRocks();
                SpawnRockWave();

                AddResources();
                ReadKeys();

                WriteRightCol(2);
                HideWorry();
                WrightBox();
            }
            EndGame();
        }

        static char[,] GetMap(string[] fileLines)
        {
            char[,] map = new char[fileLines.Length, fileLines[0].Length];
            for (int row = 0; row < map.GetLength(0); row++)
            {
                for (int col = 0; col < map.GetLength(1); col++)
                {
                    map[row, col] = fileLines[row][col];
                    TrySetAstroManYX(row, col, ref map[row,col]);
                }
            }
            return map;
        }
        static void TrySetAstroManYX(int row, int col, ref char currentChar)
        {
            if(currentChar == spaceManRaw)
            {
                spaceManYX = new int[] {row, col};
            }
        }
        static void DrawMap(int positionY,int positionX, bool raw = false)
        {
            Console.SetCursorPosition(positionY, positionX);
            for (int y = 0; y < map.GetLength(0); y++)
            {
                for (int x = 0; x < map.GetLength(1); x++)
                {
                    if (raw)
                    {
                        Console.Write(map[y,x]);
                    }
                    else
                    {
                        RenderMapChar(y, x);
                    }
                }
                Console.WriteLine();
            }
        }
        static void ReRenderMapChar(int y, int x)
        {
            Console.SetCursorPosition(0, 0);
            RenderMapChar(y, x);
            Console.SetCursorPosition(0, 0);
        }
        static void RenderMapChar(int y, int x)
        {
            if(!IsOutOfMap(new int[] {y, x}))
            {
                char mapChar = map[y, x];

                if (mapChar == spaceRaw && random.Next(20) != 7)
                {
                    TryRenderCharAs(ref mapChar, spaceRaw, spaceChar, spaceColor);
                }
                else if (mapChar == spaceRaw)
                {
                    TryRenderCharAs(ref mapChar, spaceRaw, spaceRaw, spaceColor);
                }
                TryRenderCharAs(ref mapChar, asteroidRaw, asteroidChar, asteroidColor);
                TryRenderCharAs(ref mapChar, spaceShipRaw, spaceShipChar, asteroidLightColor, spaceShipColor);
                TryRenderCharAs(ref mapChar, spaceManRaw, spaceManChars[spaceManTurn], lightColor, spaceManColor);

                if (int.TryParse(mapChar.ToString(), out int spaceWallNumber))
                {
                    TryRenderCharAs(
                        ref mapChar,
                        spaceWallNumber.ToString()[0],
                        spaceWallsChars[spaceWallNumber],
                        asteroidLightColor, wallsColor
                        );
                }
                Console.SetCursorPosition(x, y);
                Console.Write(mapChar);
            }
        }
        static void TryRenderCharAs(ref char currentChar, char rawChar, char renderChar, 
            ConsoleColor backgroundColor = defaultBgColor, 
            ConsoleColor foregroundColor = defaultFgColor
            )
        {
            if (currentChar == rawChar)
            {
                Console.BackgroundColor = backgroundColor;
                Console.ForegroundColor = foregroundColor;
                currentChar = renderChar;
            }
        }
        static void HelpMission()
        {
            if(helpTimer > 0)
            {
                helpTimer -= 100;
            }
            else if(!survival && helpTimer <= 0)
            {
                gameWin = true;
                gameStart = false;
            }
        }
        static void AddResources()
        {
            resources += resourceGeneratorsCount * resoureceGeneratorPower;
        }
        static void ReadKeys()
        {
            Console.CursorVisible = false;
            if (Console.KeyAvailable)
            {
                consoleKey = Console.ReadKey();
                ReadWASD();
                ReadArrows();
                ReadEnter();
                ReadEsc();
            }
        }
        static void ReadWASD()
        {
            switch (consoleKey.Key)
            {
                case ConsoleKey.D:
                    Console.Write("\b \b");
                    moveSpaceMan(0, new int[] { 0, 0 });
                    break;
                case ConsoleKey.S:
                    Console.Write("\b \b");
                    moveSpaceMan(1, new int[] { 0, 0 });
                    break;
                case ConsoleKey.A:
                    Console.Write("\b \b");
                    moveSpaceMan(2, new int[] { 0, 0 });
                    break;
                case ConsoleKey.W:
                    Console.Write("\b \b");
                    moveSpaceMan(3, new int[] { 0, 0 });
                    break;
            }
        }
        static void ReadArrows()
        {
            int[] vectorYX = new int[] { 0, 0 };
            switch (consoleKey.Key)
            {
                case ConsoleKey.RightArrow:
                    vectorYX[1]++;
                    moveSpaceMan(0, vectorYX);
                    break;
                case ConsoleKey.DownArrow:
                    vectorYX[0]++;
                    moveSpaceMan(1, vectorYX);
                    break;
                case ConsoleKey.LeftArrow:
                    vectorYX[1]--;
                    moveSpaceMan(2, vectorYX);
                    break;
                case ConsoleKey.UpArrow:
                    vectorYX[0]--;
                    moveSpaceMan(3, vectorYX);
                    break;
            }
        }
        static void moveSpaceMan(int turn, int[]moveVectorYX)
        {
            spaceManTurn = turn;
            if(moveVectorYX[0] != 0 || moveVectorYX[1] != 0)
            {
                SetManCollision(ref moveVectorYX);
                SetMapChar(ref map, spaceManYX, asteroidRaw);
                RenderSpaceMan();
                spaceManYX[0] += moveVectorYX[0];
                spaceManYX[1] += moveVectorYX[1];
                SetMapChar(ref map, spaceManYX, spaceManRaw);
            }
            RenderSpaceMan();
        }
        static void SetManCollision(ref int[] moveVectorYX)
        {
            char nextChar = map[spaceManYX[0] + moveVectorYX[0], spaceManYX[1] + moveVectorYX[1]];
            if (nextChar != asteroidRaw)
            {
                moveVectorYX = new int[] { 0, 0 };
            }
        }
        static void ReadEnter()
        {
            if(consoleKey.Key == ConsoleKey.Enter)
            {
                int[] buildYX = GetBuildYX();
                TryToBuild(buildYX);
            }
        }
        static int[] GetBuildYX()
        {
            int[] buildVectorYX = new int[] { 0, 0 };
            switch (spaceManTurn)
            {
                case 0:
                    buildVectorYX[1] = 1;
                    break;
                case 1:
                    buildVectorYX[0] = 1;
                    break;
                case 2:
                    buildVectorYX[1] = -1;
                    break;
                case 3:
                    buildVectorYX[0] = -1;
                    break;
            }
            return new int[] { spaceManYX[0] + buildVectorYX[0], spaceManYX[1] + buildVectorYX[1] };
        }
        static void TryToBuild(int[] buildYX)
        {
            char currentChar = map[buildYX[0], buildYX[1]];
            if (int.TryParse(currentChar.ToString(), out int wallNumber))
            {
                Appgrade(wallNumber, buildYX);
            }
            else if (currentChar == asteroidRaw)
            {
                SetMapChar(ref map, buildYX, '0');
            }
            else if (currentChar == spaceShipRaw)
            {
                WriteWorry("У Вас не получилось улучшить генератор ресурсов.");
            }
            else {
                WriteWorry("Вы не можете строить здесь.");
            }
            ReRenderMapChar(buildYX[0], buildYX[1]);
        }
        static void ReadEsc()
        {
            if(consoleKey.Key == ConsoleKey.Escape)
            {
                gameStart = false;
            }
        }
        static void Appgrade(int wallNumber, int[] buildYX)
        {
            wallNumber++;
            if (wallNumber < spaceWallsChars.Length && resources >= wallNumber)
            {
                SetMapChar(ref map, buildYX, (wallNumber).ToString()[0]);
                resources -= wallNumber + 1;
            }
            else if (resources >= 10)
            {
                SetMapChar(ref map, buildYX, spaceShipRaw);
                resourceGeneratorsCount++;
                resources -= 10;
            }
            else
            {
                WriteWorry("У Вас недостаточно ресурсов для постройки.");
            }
        }
        static void SpawnRockWave()
        {
            if(rockWaveTimer <= 0 && waveRockSpawns > 0)
            {
                SpawnRockFlight(rocksPerSpawn);
                waveRockSpawns--;
            }
            else if(rockWaveTimer <= 0)
            {
                waveCount++;
                PowWave();
                rockWaveTimer = rockWaveTime;
                waveRockSpawns = nextWaveRockSpawns;
            }
            else
            {
                rockWaveTimer -= 100;
            }
        }
        static void PowWave()
        {
            nextWaveRockSpawns += Convert.ToInt32(nextWaveRockSpawns * waveCount * waveSpawnsPow) + 1;
            rocksPerSpawn += Convert.ToInt32(rocksPerSpawn * waveCount * waveRocksPow);
            if(rockWaveTime > 3000)
            {
                rockWaveTime -= Convert.ToInt32(rockWaveTime * waveTimePow);
            }
        }
        static void SpawnRockFlight(int rockCount)
        {
            for (int rockNumber = 0; rockNumber <= rockCount; rockNumber++)
            {
                int[] rockYX = new int[rocksYX.GetLength(1)];
                switch (random.Next(4))
                {
                    case 0:
                        rockYX = new int[] { random.Next(map.GetLength(0)-5)+5, 0, random.Next(3)-1, 1 };
                        break;
                    case 1:
                        rockYX = new int[] { 0, random.Next(map.GetLength(1)-5)+5, 1, random.Next(3) - 1 };
                        break;
                        case 2:
                            rockYX = new int[] { map.GetLength(0)-1, random.Next(map.GetLength(1)-5)+5, -1, random.Next(3) - 1 };
                            break;
                        case 3:
                            rockYX = new int[] { random.Next(map.GetLength(0)-5)+5, map.GetLength(1)-1, random.Next(3) - 1, -1 };
                            break;
                }
                AddMatrixRow(ref rocksYX, rockYX);
            }
        }
        static void MoveRocks()
        {
            for (int rockNumber = 0; rockNumber < rocksYX.GetLength(0); rockNumber++)
            {
                ReRenderMapChar(rocksYX[rockNumber, 0], rocksYX[rockNumber, 1]);
                rocksYX[rockNumber, 0] += rocksYX[rockNumber, 2];
                rocksYX[rockNumber, 1] += rocksYX[rockNumber, 3];
                renderRock(rockNumber);
                SetRockCollision(rockNumber);
            }
        }
        static void SetRockCollision(int rockNumber)
        {
            int[] nextCharYX = new int[] {
                rocksYX[rockNumber, 0]+ rocksYX[rockNumber, 2],
                rocksYX[rockNumber, 1] + rocksYX[rockNumber, 3],
            };
            if (IsOutOfMap(nextCharYX))
            {
                int[] lastRockPosition = new int[] { rocksYX[rockNumber, 0], rocksYX[rockNumber, 1] };
                DelMatrixRow(ref rocksYX, rockNumber);
                ReRenderMapChar(lastRockPosition[0], lastRockPosition[1]);
            }
            else
            {
                char nextChar = map[nextCharYX[0], nextCharYX[1]];
                if (int.TryParse(nextChar.ToString(), out int spaceWallNumber))
                {
                    SetWallsCollision(nextCharYX, spaceWallNumber);
                    rocksYX[rockNumber, 2] *= -1;
                    rocksYX[rockNumber, 3] *= -1;
                }
                else if (nextChar == spaceShipRaw)
                {
                    SetShipCollision(nextCharYX);
                    DelMatrixRow(ref rocksYX, rockNumber);
                    resourceGeneratorsCount--;
                }
                else if (nextChar == spaceManRaw)
                {
                    SetMenCollision();
                }
            }
            

        }
        static bool IsOutOfMap(int[] positionYX)
        {
            return
                positionYX[0] <= 0 ||
                positionYX[1] <= 0 ||
                positionYX[0] >= map.GetLength(0)-1 ||
                positionYX[1] >= map.GetLength(1)-1;
        }
        static void SetShipCollision(int[] shipYX)
        {
            SetMapChar(ref map, shipYX, asteroidRaw);
            SetBlow(shipYX);
            ReRenderMapChar(shipYX[0], shipYX[1]);
        }
        static void SetBlow(int[] blowYX)
        {
            for (int row = -1; row < 2; row++)
            {
                for (int col = -1; col < 2; col++)
                {
                    Console.SetCursorPosition(blowYX[1] + col, blowYX[0] + row);
                    Console.BackgroundColor = lightColor;
                    if (row == 0 && col == 0)
                    {
                        Console.BackgroundColor = ConsoleColor.Red;
                    }
                    Console.Write(" ");
                }
            }
            System.Threading.Thread.Sleep(500);
            for (int row = -1; row < 2; row++)
            {
                for (int col = -1; col < 2; col++)
                {
                    Console.SetCursorPosition(blowYX[1] + col, blowYX[0] + row);
                    ReRenderMapChar(blowYX[0] + row, blowYX[1] + col);
                }
            }
        }
        static void SetWallsCollision(int[] wallYX, int wallNumber)
        {
            if(wallNumber == 0)
            {
                SetMapChar(ref map, wallYX, asteroidRaw);
                ReRenderMapChar(wallYX[0], wallYX[1]);
            }
            else
            {
                wallNumber--;
                SetMapChar(ref map, wallYX, wallNumber.ToString()[0]);
                ReRenderMapChar(wallYX[0], wallYX[1]);
            }
        }
        static void SetMenCollision()
        {
            gameStart = false;
            gameWin = false;
        }
        static void renderRock(int rockNumber)
        {
            ReRenderMapChar(rocksYX[rockNumber, 0], rocksYX[rockNumber, 1]);
            Console.SetCursorPosition(rocksYX[rockNumber, 1], rocksYX[rockNumber, 0]);
            Console.Write(rockChar);
            Console.SetCursorPosition(0, 0);
        }
        static void RenderSpaceMan()
        {
            ReRenderMapChar(spaceManYX[0], spaceManYX[1]);
        }
        static void SetMapChar(ref char[,] map, int[] charYX, char setChar)
        {
            map[charYX[0], charYX[1]] = setChar;
        }

        static void AddMatrixRow(ref int[,] matrix, int[] newRow)
        {
            if (newRow.Length >= matrix.GetLength(1))
            {
                int[,] newMatrix = new int[matrix.GetLength(0) + 1, matrix.GetLength(1)];
                for (int row = 0; row < newMatrix.GetLength(0); row++)
                {
                    for (int col = 0; col < newMatrix.GetLength(1); col++)
                    {
                        if (row >= matrix.GetLength(0))
                        {
                            newMatrix[row, col] = newRow[col];
                        }
                        else
                        {
                            newMatrix[row, col] = matrix[row, col];
                        }

                    }
                }
                matrix = newMatrix;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("New row is lower then matrix. \nNew row is not added.");
                Console.ForegroundColor = ConsoleColor.White;
            }
        }


        static void DelMatrixRow(ref int[,] matrix, int rowNumber)
        {
            int[,] newMatrix = new int[matrix.GetLength(0) - 1, matrix.GetLength(1)];
            for (int row = 0; row < matrix.GetLength(0); row++)
            {
                for (int col = 0; col < matrix.GetLength(1); col++)
                {
                    if (row < rowNumber)
                    {
                        newMatrix[row, col] = matrix[row, col];
                    }
                    else if (row > rowNumber)
                    {
                        newMatrix[row - 1, col] = matrix[row, col];
                    }

                }
            }
            matrix = newMatrix;
        }
        static void WrightBox()
        {
            Console.BackgroundColor = defaultBgColor;
            for (int row = 0; row < map.GetLength(0); row++)
            {
                for (int col = 0; col < map.GetLength(1); col++)
                {
                    if(
                        col == map.GetLength(1)-1 || 
                        row == map.GetLength(0) - 1 ||
                        col == 0 ||
                        row == 0
                        )
                    {
                        Console.SetCursorPosition(viewPortX + col, viewPortY + row);
                        Console.Write(" ");
                    }
                    if(row == map.GetLength(0) - 1)
                    {
                        Console.SetCursorPosition(viewPortX + col, viewPortY + row+1);
                        Console.Write(" ");
                    }
                    if(row == map.GetLength(1) - 1)
                    {
                        Console.SetCursorPosition(viewPortX + col+1, viewPortY + row);
                        Console.Write(" ");
                    }
                }
            }
        }
        static void EndGame()
        {
            Console.Clear();
            string bye;
            string task = "Нажмите любую кнопку, чтобы выйти в меню.\n";
            if (programExit)
            {
                bye = "Выход";
                task = "";
            }
            else if (gameWin)
            {
                bye = "Вы спасены!";
            }
            else
            {
                bye = " К сожалению, Вы не выжили.";
            }

            DrawSpace();
            DrawButton(
                bye,
                new int[] { 10, 41 },
                buttonBorder, lightColor, wallsColor
            );
            WriteText(task, new int[] { 20, 35 });
            if (!programExit)
            {
                Console.ReadKey();
            }
        }
        static void WriteWorry(string worry)
        {
            WriteText(
                worry+"                                                                                                        ",
                new int[] {viewPortHeight+3, 3},
                ConsoleColor.Red
                );
            worryTimeCounter += worryTime;
        }
        static void HideWorry()
        {
            if(worryTimeCounter > 0)
            {
                worryTimeCounter -= 100;
            }
            else
            {
                Console.ForegroundColor = defaultFgColor;
                WriteText(
                    "Управление: \"w\",\"a\",\"s\",\"d\" - поворот, стрелки - передвижение, enter - строить, esc - выход в меню." +
                    "                    ",
                    new int[] { viewPortHeight + 3, 3 }
                    );
            }
        }
        static void WriteRightCol(int leftPadding)
        {
            WriteNextWaveTime(leftPadding);
            WriteWaveCount(leftPadding);
            WriteHelpTime(leftPadding);
            WriteResources(leftPadding);
            WritePrice(leftPadding);
        }
        static void WriteNextWaveTime(int leftPadding)
        {
            ConsoleColor currentFgColor = defaultFgColor;
            if(rockWaveTimer < 5)
            {
                currentFgColor = ConsoleColor.Red;
            }
            WriteText(
                "До астеройдов " + Convert.ToInt32(rockWaveTimer / 1000) + "с. ",
                new int[] { 1, viewPortWidth + leftPadding },
                currentFgColor
                );
        }
        static void WriteWaveCount(int leftPadding)
        {
            WriteText(
                "Пережито волн: " + waveCount+" ",
                new int[] { 3, viewPortWidth + leftPadding }
                );
        }
        static void WriteHelpTime(int leftPadding)
        {
            if(helpTimer > 0)
            {
                WriteText(
                    "До спасения: " + Convert.ToInt32(helpTimer / 1000) + "c. ",
                    new int[] { 5, viewPortWidth + leftPadding }
                    );
            }
        }
        static void WriteResources(int leftPadding)
        {
            WriteText(
                "Ресурсы: " + Convert.ToInt32(resources) + "$ ",
                new int[] { 7, viewPortWidth + leftPadding }
                );
        }
        static void WritePrice(int leftPadding)
        {
            WriteText(
                "Вы можете построить:",
                new int[] { 9, viewPortWidth + leftPadding }
                );
            WriteText(
                "Стена "+spaceWallsChars[0]+" = "+1+"$",
                new int[] { 11, viewPortWidth + leftPadding }
                );
            WriteText(
                "Стена " + spaceWallsChars[1]+" = "+2+"$",
                new int[] { 13, viewPortWidth + leftPadding }
                );
            WriteText(
                "Стена " + spaceWallsChars[2]+" = "+3+"$",
                new int[] { 15, viewPortWidth + leftPadding }
                );
            WriteText(
                "Стена " + spaceWallsChars[3]+" = "+4+"$",
                new int[] { 17, viewPortWidth + leftPadding }
                );
            WriteText(
                "Генератор " + spaceShipChar +" = "+10+"$",
                new int[] { 19, viewPortWidth + leftPadding }
                );
        }
        static void WriteText(string text, int[] positionYX, 
            ConsoleColor FgColor = defaultFgColor, 
            ConsoleColor BgColor = defaultBgColor
            )
        {
            Console.BackgroundColor = BgColor;
            Console.ForegroundColor = FgColor;
            Console.SetCursorPosition(positionYX[1], positionYX[0]);
            Console.Write(text);
            Console.ForegroundColor = defaultFgColor;
            Console.BackgroundColor = defaultBgColor;
        }
    }
}
