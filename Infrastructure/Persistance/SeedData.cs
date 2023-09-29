using Domain;
using Domain.Entity;
using Domain.Enum;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistance
{
    public static class SeedData
    {
        public static void SeedProducts(ApplicationDbContext context)
        {
            var productDb = context.Set<Product>();
            if (!productDb.Any())
            {
                // Mobile Phone
                for (int i = 0; i < 50; i++)
                {
                    var mob = new Product
                    {
                        Name = "Samsung S22+",
                        Category = ProductCategory.Electronic,
                        Price = 1453,
                        ProductIdentifier = ProductIdentifier.SamsungS22
                    };
                    productDb.Add(mob);
                }


                // PS5
                for (int i = 0; i < 29; i++)
                {
                    var ps = new Product
                    {
                        Category = ProductCategory.Electronic,
                        Name = "Playstation 5",
                        Price = 1111,
                        ProductIdentifier = ProductIdentifier.Playstation5
                    };
                    productDb.Add(ps);
                }


                // Ball
                for (int i = 0; i < 129; i++)
                {
                    var ball = new Product
                    {
                        ProductIdentifier = ProductIdentifier.FootballBall,
                        Price = 14,
                        Name = "Football Ball",
                        Category = ProductCategory.Sport
                    };
                    productDb.Add(ball);
                }


                context.SaveChanges();
            }
        }

    }
}
