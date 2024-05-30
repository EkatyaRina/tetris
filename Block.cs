using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tetris
{
    /// <summary>
    /// Абстрактный класс Блок
    /// </summary>
    public abstract class Block //абстрактный класс, для блоков созданы отдельные классы
    {
        protected abstract Position[][] Tiles { get; }//двумерный массив позиций, содержащий позиции плиток в 4х состояниях вращения
        protected abstract Position StartOffset { get; }//начальное смещение которое определяет где блок появится в сетке
        public abstract int ID {get;}//целочисленный идентификатор для различения блоков
        private int rotationState;// текущее вращение
        private Position offset;//текущее смещение

       /// <summary>
       /// Начальное смещение блока
       /// </summary>
        public Block()
        {
            offset = new Position(StartOffset.Row, StartOffset.Column);
        }

        /// <summary>
        /// Сохранение текущнго смещения и вращения блока
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Position> TilePosition()
        {
            foreach (Position p in Tiles[rotationState]) //перебирает позиции плитки в тек.состоянии поворота и добавляет смещение строки и столбца
            {
                yield return new Position(p.Row + offset.Row, p.Column+offset.Column);
            }
        }

        /// <summary>
        /// Поворот блока на 90' по часовой стрелкe
        /// </summary>
        public void RotateCW()
        {
            rotationState = (rotationState + 1) % Tiles.Length;
        }

        /// <summary>
        /// Поворот блока на 90' против часовой стрелки
        /// </summary>
        public void RotateCCW()
        {
            if (rotationState == 0)
            {
                rotationState = Tiles.Length - 1;
            }
            else
            {
                rotationState--;
            }
        }

        /// <summary>
        /// Перемещение блока на заданное количество строк и столбцов
        /// </summary>
        /// <param name="rows">Количество строк</param>
        /// <param name="columns">Количество столбцов</param>
        public void Move (int rows, int columns)
        {
            offset.Row += rows;
            offset.Column += columns;
        }

        /// <summary>
        /// Сброс вращения и положения блока
        /// </summary>
        public void Reset()
        {
            rotationState = 0;
            offset.Row = StartOffset.Row;
            offset.Column = StartOffset.Column;
        }
    }
}
