namespace tetris
{
    public class GameState
    {
        private Block currentBlock;//резервное поле для текущего блока
        public Block CurrentBlock
        {
            get => currentBlock;
            private set
            {
                currentBlock = value;
                currentBlock.Reset(); //для установления правильного значения вращения и поворота

                for (int i = 0; i < 2; i++)//установка начальной позиции блока
                {
                    currentBlock.Move(1, 0);

                    if (!BlockFits())
                    {
                        currentBlock.Move(-1, 0);
                    }
                }
            }
        }
        //игрровая сетка и логическое значение конца игры
        public GameGrid GameGrid { get; }
        public BlockQueue BlockQueue { get; }
        public bool GameOver { get; private set; }
        public int Score { get; private set; }
        public Block HeldBlock { get; private set; }
        public bool CanHold { get; private set; }

        //инициализирование сетки и текущего случайного блока
        public GameState()
        {
            GameGrid = new GameGrid(22, 10);
            BlockQueue = new BlockQueue();
            CurrentBlock = BlockQueue.GetAndUpdate();
            CanHold = true;
        }

        //проверка возможности существования текущего блока
        private bool BlockFits()
        {
            foreach (Position p in CurrentBlock.TilePosition())
            {
                if (!GameGrid.IsEmpty(p.Row, p.Column))
                {
                    return false;
                }
            }
            return true;
        }
        //закрепленный блок
        public void HoldBlock()
        {
            if (!CanHold)
            {
                return;
            }
            if (HeldBlock == null)
            {
                HeldBlock = CurrentBlock;
                CurrentBlock = BlockQueue.GetAndUpdate();
            }
            else
            {
                Block tmp = CurrentBlock;
                CurrentBlock = HeldBlock;
                HeldBlock = tmp;
            }
            CanHold = false;
        }
        //проверка возможности поворота фигуры по часовой стрелки
        public void RotateBlockCW()
        {
            CurrentBlock.RotateCW();
            if (!BlockFits())
            {
                CurrentBlock.RotateCCW(); //если положение "неправильное" поворачиваем фигуру обратно
            }
        }

        //проверка возможности поворота фигуры против часовой стрелки
        public void RotateBlockCCW()
        {
            CurrentBlock.RotateCCW();
            if (!BlockFits())
            {
                CurrentBlock.RotateCW(); //если положение "неправильное" поворачиваем фигуру обратно
            }
        }

        //проверка возможности перемещения фигуры Влево/Вправо
        public void MoveBlockLeft()
        {
            CurrentBlock.Move(0, -1);
            if (!BlockFits())
            {
                CurrentBlock.Move(0, 1);
            }
        }
        public void MoveBlockRight()
        {
            CurrentBlock.Move(0, 1);
            if (!BlockFits())
            {
                CurrentBlock.Move(0, -1);
            }
        }

        //окончена ли игра
        public bool IsGameOver()
        {
            return !(GameGrid.IsRowEmpty(0) && GameGrid.IsRowEmpty(1));
        }

        //текущий блок нельзя переместить вниз
        private void PlaceBlock()
        {
            //перебираем позиции тек.блока и устанавливает позиции в игр.сетке равные идентификатору блока
            foreach (Position p in CurrentBlock.TilePosition())
            {
                GameGrid[p.Row, p.Column] = CurrentBlock.ID;
            }

            //очищаем все потанциально заполненные строки и проверяем закончена ли игра
            Score += GameGrid.ClearFullRows();
            if (IsGameOver())
            {
                GameOver = true;
            }
            else
            {
                CurrentBlock = BlockQueue.GetAndUpdate();
                CanHold = true;
            }
        }

        //проверка возможности перемещения фигуры Вверх/вниз
        public void MoveBlockDown()
        {
            CurrentBlock.Move(1, 0);
            if (!BlockFits())
            {
                CurrentBlock.Move(-1, 0);
                PlaceBlock();
            }
        }
       //падение фигуры
        private int TileDropDistance(Position p)
        {
            int drop = 0;
            while (GameGrid.IsEmpty(p.Row + drop + 1, p.Column))
            {
                drop++;
            }
            return drop;
        }
        public int BlockDropDistance()
        {
            int drop = GameGrid.Rows;
            foreach (Position p in CurrentBlock.TilePosition())
            {
                drop = System.Math.Min(drop, TileDropDistance(p));
            }
            return drop;
        }
        public void DropBlock()
        {
            CurrentBlock.Move(BlockDropDistance(), 0);
            PlaceBlock();
        }

    }
}
