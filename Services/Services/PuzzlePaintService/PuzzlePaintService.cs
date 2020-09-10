using InvestPlaceDB;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;

namespace Services.Services.PuzzlePaintService
{
    public class PuzzlePaintService : IPuzzlePaintService
    {
        private readonly IWebHostEnvironment environment;
        private readonly InvestPlaceContext db;
        private readonly List<Image> puzzleImages;

        public PuzzlePaintService(InvestPlaceContext db, IWebHostEnvironment environment)
        {
            this.db = db;
            this.environment = environment;

            puzzleImages = new List<Image>()
            {
                Image.FromFile(@$"{environment.WebRootPath}\images\puzzle-net\1.png"),
                Image.FromFile(@$"{environment.WebRootPath}\images\puzzle-net\2.png"),
                Image.FromFile(@$"{environment.WebRootPath}\images\puzzle-net\3.png"),
                Image.FromFile(@$"{environment.WebRootPath}\images\puzzle-net\4.png"),
                Image.FromFile(@$"{environment.WebRootPath}\images\puzzle-net\5.png"),
                Image.FromFile(@$"{environment.WebRootPath}\images\puzzle-net\6.png"),
                Image.FromFile(@$"{environment.WebRootPath}\images\puzzle-net\7.png"),
                Image.FromFile(@$"{environment.WebRootPath}\images\puzzle-net\8.png"),
                Image.FromFile(@$"{environment.WebRootPath}\images\puzzle-net\9.png"),
                Image.FromFile(@$"{environment.WebRootPath}\images\puzzle-net\10.png"),
                Image.FromFile(@$"{environment.WebRootPath}\images\puzzle-net\11.png"),
                Image.FromFile(@$"{environment.WebRootPath}\images\puzzle-net\12.png"),
                Image.FromFile(@$"{environment.WebRootPath}\images\puzzle-net\13.png"),
                Image.FromFile(@$"{environment.WebRootPath}\images\puzzle-net\14.png"),
            };
        }



        public Stream PuzzleNet(int lotId)
        {
            string netFileName = @$"{environment.WebRootPath}\images\puzzle-net\base.png";
            MemoryStream memory = new MemoryStream();

            Lot lot = db.Lot
                .Include(x => x.Pazzle)
                .FirstOrDefault(x => x.Id == lotId);

            if (lot == null)
                throw new ArgumentNullException("Не найден товар");


            using (Bitmap net = new Bitmap(netFileName))
            {
                using (Graphics graphics = Graphics.FromImage(net))
                {
                    int dx = net.Width / EpicSettings.MaxPuzzleX;
                    int dy = net.Height / EpicSettings.MaxPuzzleY;

                    for (int x = 0; x < EpicSettings.MaxPuzzleX; x++)
                    {
                        for (int y = 0; y < EpicSettings.MaxPuzzleY; y++)
                        {
                            if (!lot.Pazzle.Where(p => p.BuyDate != null).Any(p => p.X == x && p.Y == y))
                            {
                                int puzzleType = GetPuzzleType(x, y);
                                float left = x * dx + CorrectX(puzzleType);
                                float top = y * dy + CorrectY(puzzleType);                       

                                graphics.DrawImage(GetPuzzleImageByCoord(puzzleType), left, top);
                            }
                        }
                    }

                    net.Save(memory, ImageFormat.Png);
                    memory.Position = 0;
                    return memory;
                }
            }
        }

        /// <summary>
        /// Магичесгие вещи, чтобы пазлы соединились замочками
        /// </summary>
        private int CorrectX(int puzzleType)
        {
            switch (puzzleType)
            {
                case 1:
                    return 2;
                case 2:
                    return -15;
                case 3:
                    return -2;
                case 4:
                    return -12;
                case 5:
                    return 3;
                case 6:
                    return -2;
                case 7:
                    return -12;
                case 8:
                    return -0;
                case 9:
                    return 4;
                case 10:
                    return -11;
                case 11:
                    return 2;
                case 12:
                    return -14;
                case 14:
                    return -10;

                default:
                    return 0;
            }
        }

        /// <summary>
        /// Магичесгие вещи, чтобы пазлы соединились замочками
        /// </summary>
        private int CorrectY(int puzzleType)
        {
            switch (puzzleType)
            {
                case 5:
                    return -1;
                case 6:
                    return -17;
                case 7:
                    return -2;
                case 8:
                    return -18;
                case 9:
                    return -17;
                case 10:
                    return -2;
                case 11:
                    return -16;
                case 12:
                    return -1;
                case 13:
                    return -16;
                case 14:
                    return -2;

                default:
                    return 0;
            }
        }

        private Image GetPuzzleImageByCoord(int puzzleType)
        {
            return puzzleImages[puzzleType - 1];
        }

        private int GetPuzzleType(int x, int y)
        {
            int puzzleType = 0;
            if (x == 0)
            {
                if (y == 0)
                    puzzleType = 1;
                else if (y == EpicSettings.MaxPuzzleY - 1)
                    puzzleType = 11;
                else
                    puzzleType = y % 2 == 1 ? 5 : 9;
            }
            else if (x == EpicSettings.MaxPuzzleX - 1)
            {
                if (y == 0)
                    puzzleType = 4;
                else if (y == EpicSettings.MaxPuzzleY - 1)
                    puzzleType = 14;
                else
                    puzzleType = y % 2 == 1 ? 8 : 10;
            }
            else
            {
                if (x % 2 == 1)
                {
                    if (y == 0)
                        puzzleType = 2;
                    else if (y == EpicSettings.MaxPuzzleY - 1)
                        puzzleType = 12;
                    else
                        puzzleType = y % 2 == 1 ? 6 : 7;
                }
                else
                {
                    if (y == 0)
                        puzzleType = 3;
                    else if (y == EpicSettings.MaxPuzzleY - 1)
                        puzzleType = 13;
                    else
                        puzzleType = y % 2 == 1 ? 7 : 6;
                }
            }

            return puzzleType;
        }


    }
}
