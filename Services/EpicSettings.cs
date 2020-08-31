using System;
using System.Collections.Generic;
using System.Text;

namespace Services
{
    public static class EpicSettings
    {
        /// <summary>
        /// Число пазлов по горизонтали в товаре
        /// </summary>
        public const int MaxPuzzleX = 20;

        /// <summary>
        /// Число пазлов по вертикали в товаре
        /// </summary>
        public const int MaxPuzzleY = 15;

        /// <summary>
        /// Число ячеек по горизонтали в поле покупателей
        /// </summary>
        public const int MaxBuyerFieldX = 15;

        /// <summary>
        /// Число ячеек по вертикали в поле покупателей
        /// </summary>
        public const int MaxBuyerFieldY = 20;

        /// <summary>
        /// Число лотов в поле покупателей
        /// </summary>
        public const int LotPerBuyerField = 300;

        /// <summary>
        /// Процент, который возвращается в качестве бонусов тем, кто участвовал в покупке товара но не выиграл
        /// </summary>
        public const decimal BonusPercent = 0.2m;

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
