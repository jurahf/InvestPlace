using System;
using System.Collections.Generic;
using System.Text;

namespace Services
{
    public static class EpicSettings
    {
        public const int MaxPuzzleX = 20;

        public const int MaxPuzzleY = 15;


        /// <summary>
        /// Сколько пазлов в одном лоте
        /// </summary>
        public const int PuzzlePerLot = 300;

        /// <summary>
        /// Во сколько раз цена пазла меньше цены лота
        /// </summary>
        public const int PuzzleCostDelimeter = 100;

        /// <summary>
        /// Сколько других товаров надо напокупать, чтобы разместить свой товар
        /// </summary>
        public const int AnotherPuzzlesForNewLot = 3;
    }
}
