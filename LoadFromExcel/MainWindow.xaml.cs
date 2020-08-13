using InvestPlaceDB;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using Syncfusion.XlsIO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;


namespace LoadFromExcel
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const int RowDataStart = 4;
        private readonly InvestPlaceContext db = null;
        private readonly int PuzzleCostDelimeter = 100;

        public MainWindow()
        {
            InitializeComponent();

            DbContextOptionsBuilder builder = new DbContextOptionsBuilder();
            builder.UseSqlServer("data source=(local);initial catalog=InvestPlaceDatabase;integrated security=True;MultipleActiveResultSets=True;");
            db = new InvestPlaceContext(builder.Options);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Файлы Excel|*.xls;*.xlsx";
            if (dialog.ShowDialog() == true)
            {
                ParceExcelFile(dialog.FileName);
            }
        }

        private void ParceExcelFile(string fileName)
        {
            ExcelEngine excelEngine = null;
            IWorkbook workbook = null;
            try
            {
                excelEngine = new ExcelEngine();
                IApplication application = excelEngine.Excel;
                application.DefaultVersion = ExcelVersion.Excel2010;

                FileStream sampleFile = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                workbook = application.Workbooks.Open(sampleFile);
                IWorksheet worksheet = workbook.Worksheets[0];

                int row = RowDataStart;

                while (true)
                {
                    string rangeStr = worksheet.Range[row, 2].Value;
                    string name = worksheet.Range[row, 3].Value;
                    string desctription = worksheet.Range[row, 4].Value;
                    string imageLink = worksheet.Range[row, 5].Value;
                    string priceStr = worksheet.Range[row, 6].Value;
                    string shopLink = worksheet.Range[row, 7].Value;
                    string parentCategotyStr = worksheet.Range[row, 8].Value;
                    string categoryStr = worksheet.Range[row, 9].Value;

                    if (string.IsNullOrEmpty(name))
                        break;

                    if (db.Lot.Any(x => x.Name == name))
                    {
                        row++;
                        continue;
                    }

                    Category category = FindOrCreateCategory(categoryStr, parentCategotyStr);
                    decimal price = ParcePrice(priceStr);

                    Lot lot = new Lot()
                    {
                        PriceRange = ParceRange(rangeStr, price),
                        Name = name,
                        Description = desctription,
                        ImageLink = imageLink,
                        SourceLink = shopLink,
                        Price = price,
                    };

                    LotCategory lc = new LotCategory()
                    {
                         Lot = lot,
                         Category = category
                    };

                    db.Lot.Add(lot);
                    db.LotCategory.Add(lc);
                    db.SaveChanges();

                    row++;
                }
            }
            finally
            {
                workbook.Close();
                excelEngine.Dispose();
            }
        }


        private string GetString(object value)
        {
            if (value == null)
                return "";
            else
                return value.ToString();
        }

        private int? GetInt(object value)
        {
            if (value == null)
                return null;
            else
            {
                if (int.TryParse(value.ToString(), out int result))
                    return result;
                else
                    return null;
            }
        }


        private PriceRange ParceRange(string rangeStr, decimal price)
        {
            if (string.IsNullOrEmpty(rangeStr))
                return null;

            string[] parts = rangeStr.Split('-');
            int min = int.Parse(parts[0]) + 1;
            int max = int.Parse(parts[1]);

            PriceRange range = db.PriceRange.FirstOrDefault(x => x.Minimum == min && x.Maximum == max);

            if (range == null)
            {
                decimal pricePerPuzzle = price / PuzzleCostDelimeter;
                range = db.PriceRange.FirstOrDefault(x => x.Minimum <= pricePerPuzzle && pricePerPuzzle <= x.Maximum);
            }

            return range;
        }


        private decimal ParcePrice(string priceStr)
        {
            string str = priceStr.Replace(" ", "").Trim('р', 'Р', 'h', 'H', 'p', 'P');

            return decimal.Parse(str);
        }


        private Category FindOrCreateCategory(string catStr, string parentCatStr)
        {
            Category parent = db.Category
                .Include(x => x.InverseParent)
                .FirstOrDefault(x => x.Name.ToLower() == parentCatStr.ToLower());

            if (parent == null)
            {
                parent = new Category()
                {
                    Name = parentCatStr
                };

                db.Category.Add(parent);
            }

            Category cat = parent.InverseParent.FirstOrDefault(x => x.Name.ToLower() == catStr.ToLower());
            if (cat == null)
            {
                cat = new Category()
                {
                    Name = catStr,
                    Parent = parent
                };

                db.Category.Add(cat);
            }

            return cat;
        }


    }
}
