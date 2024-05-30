using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tetris
{
    /// <summary>
    /// Класс, отвечающий за появление блоков в игре
    /// </summary>
    public class BlockQueue //отвечает за появление блоков в игре
    {
        private readonly Block[] blocks = new Block[]
        {
            new IBlock(),
            new JBlock(),
            new LBlock(),
            new OBlock(),
            new SBlock(),
            new TBlock(),
            new ZBlock()
        };
        private readonly Random rand = new Random();
        /// <summary>
        /// Появлеие ногвого блока  в очереди
        /// </summary>
        public Block NextBlock { get; private set; } //появление нового блока в очереди

        /// <summary>
        /// Инициализация случайного блока 
        /// </summary>
        public BlockQueue()//инициализируем следующий блок случайным блоком
        {
            NextBlock = RandomBlock();
        }

        /// <summary>
        /// Возвращение случайного блока
        /// </summary>
        /// <returns></returns>
        private Block RandomBlock()//возвращение случайного блока
        {
            return blocks[rand.Next(blocks.Length)];
        }

        /// <summary>
        /// Возвращение следюющего блока
        /// </summary>
        /// <returns></returns>
        public Block GetAndUpdate() //возвращает следующий блок и обновляет свойство
        {
            Block block = NextBlock; //меняем блок до тех пор пока не получим дубликат
            do
            {
                NextBlock = RandomBlock();
            }
            while (block.ID == NextBlock.ID);
            return block;
        }
    }
}
