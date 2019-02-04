using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace pac_man
{
    class Program
    {
        static char[,] map;
        static int pacX = 0, pacY = 0;
        static int DX = 0, DY = 0;
        static int ghostX = 0, ghostY = 0;
        static int GDX = 1, GDY = 0;
        static int[] portA = { 0, 0 }, portB = { 0, 0 };

        static ConsoleKeyInfo key;
        static int points = 0;
        static int allDots = 0;
        static Random rand = new Random();
        static int ghostDir = 0;
        static bool kill = false;
        static void Main(string[] args)
        {
            Console.CursorVisible = false;
            map = GetMap(File.ReadAllLines(@"..\..\map.txt"));
            

            WrightMap();
            

            while (!kill)
            {
                ReadKeys();
                MovePacman();
                if (allDots == points) break;
                System.Threading.Thread.Sleep(100);
                MoveGhost();
                if (ghostX == pacX && ghostY == pacY) kill = true;
            }
            Console.SetCursorPosition(0, map.GetLength(0) + 7);
            if (kill == true) Console.Write("Ты умер.");
            else
                Console.Write("ВЫ ПОБЕДИЛИ!");
        }
        static char[,] GetMap(string[] fileLines)
        {
            char[,] map = new char[fileLines.Length, fileLines[0].Length];
            for (int row = 0; row < map.GetLength(0); row++)
            {
                for (int col = 0; col < map.GetLength(1); col++)
                {
                    map[row, col] = fileLines[row][col];
                }
            }
                
            return map;
        }
        static void WrightMap()
        {
            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    if (map[i, j] == '@')
                    {
                        pacX = j;
                        pacY = i;
                        map[i, j] = ' ';
                    }
                    else if (map[i, j] == ' ')
                    {
                        map[i, j] = '.';
                        allDots++;
                    }
                    else if (map[i, j] == 'M')
                    {
                        ghostX = j;
                        ghostY = i;
                        map[i, j] = ' ';
                    }else if(map[i, j] == 'A')
                    {
                        portA = new int[] {i, j};
                        map[i, j] = ' ';
                    }
                    else if(map[i, j] == 'B')
                    {
                        portB = new int[] {i, j};
                        map[i, j] = ' ';
                    }
                    Console.Write(map[i, j]);
                }

                Console.Write('\n');
            }
        }
        static void ReadKeys()
        {
            if (Console.KeyAvailable)
            {
                key = Console.ReadKey(true);
                switch (key.Key)
                {
                    case ConsoleKey.UpArrow:
                        DX = 0; DY = -1;
                        break;
                    case ConsoleKey.DownArrow:
                        DX = 0; DY = 1;
                        break;
                    case ConsoleKey.LeftArrow:
                        DX = -1; DY = 0;
                        break;
                    case ConsoleKey.RightArrow:
                        DX = 1; DY = 0;
                        break;

                }
            }
        }
        static void MovePacman()
        {
            if (map[pacY + DY, pacX + DX] != '#')
            {

                Console.SetCursorPosition(pacX, pacY);
                Console.Write(' ');
                pacX += DX;
                pacY += DY;
                Console.SetCursorPosition(pacX, pacY);
                Console.Write('@');
                if (map[pacY, pacX] == '.')
                {
                    points++;
                    map[pacY, pacX] = ' ';
                    Console.SetCursorPosition(0, map.GetLength(0) + 5);
                    Console.Write("У вас " + points + " очков");
                }
                if(pacX == portA[1] && pacY == portA[0])
                {
                    pacX = portB[1];
                    pacY = portB[0];
                }
                else if (pacX == portB[1] && pacY == portB[0])
                {
                    pacX = portA[1];
                    pacY = portA[0];
                }
                Console.SetCursorPosition(portB[1], portB[0]);
                Console.Write(' ');
                Console.SetCursorPosition(portA[1], portA[0]);
                Console.Write(' ');
                Console.SetCursorPosition(0, 0);
            }
        }
        static void MoveGhost()
        {
            if (isClosed())
            {
                ChangeGhostVector();
            }
            else
            {
                DrawGhostMove();
            }
        }
        static bool isClosed()
        {
            return
                map[ghostY + GDY, ghostX + GDX] == '#'
                || ghostX+GDX >= map.GetLength(1)-1
                || ghostY + GDY >= map.GetLength(0) - 1;
        }
        static void ChangeGhostVector()
        {
            ghostDir = rand.Next(1, 5);
            switch (ghostDir)
            {
                case 1:
                    GDX = 0; GDY = -1;
                    break;
                case 2:
                    GDX = 0; GDY = 1;
                    break;
                case 3:
                    GDX = -1; GDY = 0;
                    break;
                case 4:
                    GDX = 1; GDY = 0;
                    break;
            }
        }
        static void DrawGhostMove()
        {
            Console.SetCursorPosition(ghostX, ghostY);
            Console.Write(map[ghostY, ghostX]);
            ghostX += GDX;
            ghostY += GDY;
            Console.SetCursorPosition(ghostX, ghostY);
            Console.Write('M');
        }
    }
}