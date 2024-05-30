using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tetris
{
    public class GameGrid
    {
        private readonly int[,] grid; //класс содержит двумерный прямоугольный массив
        public int Rows { get; }
        public int Columns { get; }

        public int this[int r, int c] //индексатор для доступа к массиву
        {
            get => grid[r, c];
            set => grid[r, c] = value;
        }

        public GameGrid(int rows, int columns) //конструктор принимает кол-во столб и строк в кач-ве параметров
        {
            //в теле сохраняем кол-во строк и столбоцов и инициализируем массив
            Rows = rows;
            Columns = columns;
            grid = new int[rows, columns];
        }

        public bool IsInside (int r, int c) //проверяем находятся ли столб и стр в сетке
        {
            return r >= 0 && r < Rows && c >= 0 && c < Columns;
        }

        public bool IsEmpty(int r, int c) //проверяем ячейку на пустоту
        {
            return IsInside(r, c) && grid[r, c]==0;
        }

        public bool IsRowFull(int r) //проверка на заполнение строки целиком
        {
            for (int c = 0; c < Columns; c++)
            {
                if (grid[r, c]==0)
                {
                    return false;
                }
            }
            return true;
        }
        public bool IsRowEmpty(int r) // проверка на пустоту строки
        {
            for (int c = 0; c < Columns; c++)
            {
                if (grid[r,c]!=0)
                {
                    return false;
                }
            }
            return true;
        }
        private void ClearRow(int r) //метод для очищения строк
        {
            for (int c = 0;c < Columns;c++)
            {
                grid[r,c] = 0;
            }
        }
        
        private void MoveRowDown(int r, int numRows) //перемещение неочищенных строк
        {
            for (int c = 0; c< Columns;c++)
            {
                grid[r+numRows,c] = grid[r,c];
                grid[r,c] = 0;
            }
        }

        public int ClearFullRows() //метод заполнения строк
        {
            int cleared = 0;
            for (int r = Rows-1; r>=0; r--)
            {
                if (IsRowFull(r))
                {
                    ClearRow(r);
                    cleared++;
                }
                else if (cleared > 0)
                {
                    MoveRowDown(r, cleared);
                }
            }
            return cleared; //возвращаем кол-во очищенных строк
        }
    }
}
