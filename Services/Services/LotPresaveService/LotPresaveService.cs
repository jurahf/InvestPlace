using InvestPlaceDB;
using Microsoft.EntityFrameworkCore;
using Services.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Services.Services.LotPresaveService
{
    public class LotPresaveService : ILotPresaveService
    {
        private object lockObject = new object();
        private readonly InvestPlaceContext db;

        public LotPresaveService(InvestPlaceContext db)
        {
            this.db = db;
        }


        public void ClearPresaveLot(ExtendedUserDto user)
        {
            if (user == null)
                return;

            lock (lockObject)
            {
                try
                {
                    LotPresave presave = db.LotPresave
                        .Include(x => x.Seller)
                        .Where(x => x.Seller.Id == user.Id)
                        .FirstOrDefault();

                    if (presave == null)
                        return;

                    db.Remove(presave);
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    // не критично
                }
            }
        }

        public LotDto GetPresaveLot(ExtendedUserDto user)
        {
            if (user == null)
                return null;

            lock (lockObject)
            {
                try
                {
                    LotPresave presave = db.LotPresave
                        .Include(x => x.Seller)
                        .Include(x => x.LotPresaveCategory)
                        .Where(x => x.Seller.Id == user.Id)
                        .FirstOrDefault();

                    if (presave == null)
                        return null;

                    return LotDto.ConvertFromLotPresave(presave);
                }
                catch (Exception ex)
                {
                    // не критично
                }
            }
        }

        public void SavePresaveLot(ExtendedUserDto user, LotDto lot)
        {
            if (user == null || lot == null)
                return;

            lock (lockObject)
            {
                try
                {
                    LotPresave presave = db.LotPresave
                        .Include(x => x.Seller)
                        .Include(x => x.LotPresaveCategory)
                        .Where(x => x.Seller.Id == user.Id)
                        .FirstOrDefault();

                    if (presave == null)
                    {
                        presave = new LotPresave()
                        {
                            SellerId = user.Id,
                            CreateDate = DateTime.Now
                        };
                        db.Add(presave);
                    }

                    presave.ImageLink = lot.ImageLink;
                    presave.LotPresaveCategory.Clear();
                    foreach (var catDto in lot.Categories)
                    {
                        Category cat = db.Category.FirstOrDefault(x => x.Id == catDto.Id);
                        if (cat != null)
                        {
                            LotCategory lc = new LotCategory()
                            {
                                Category = cat,
                                LotPresave = presave
                            };
                            db.Add(lc);
                            presave.LotPresaveCategory.Add(lc);
                        }
                    }
                    presave.Name = lot.Name;
                    presave.Description = lot.Description;
                    presave.Price = lot.Price;
                    presave.SourceLink = lot.SourceLink;

                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    // не критично
                }
            }
        }



    }
}
