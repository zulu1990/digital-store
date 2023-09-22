using Domain;
using Domain.Entity;
using Domain.Enum;
using System.Drawing;

namespace Infrastructure.Persistance.Mocking
{
    public class MockDb
    {
        public Dictionary<Guid, Product> Products;
        public Dictionary<Guid, User> Users;
        public Dictionary <Guid, Order> Orders;

        public MockDb()
        {
            Products = new Dictionary<Guid, Product>();
            Users = new Dictionary<Guid, User>();
            Orders = new Dictionary<Guid, Order>();

            SeedProducts();
        }

        void SeedProducts()
        {
            // Mobile Phone
            for (int i = 0; i < 50; i++)
            {
                var mob = new Product
                {
                    Id = Guid.NewGuid(),
                    Name = "Samsung S22+",
                    Category = ProductCategory.Electronic,
                    Price = 1453,
                    ProductIdentifier = ProductIdentifier.SamsungS22
                };
                Products.Add(mob.Id, mob);
            }


            // PS5
            for (int i = 0; i < 29; i++)
            {
                var ps = new Product
                {
                    Id = Guid.NewGuid(),
                    Category = ProductCategory.Electronic,
                    Name = "Playstation 5",
                    Price = 1111,
                    ProductIdentifier = ProductIdentifier.Playstation5
                };
                Products.Add(ps.Id, ps);
            }


            // Ball
            for (int i = 0; i< 129; i++)
            {
                var ball = new Product
                {
                    Id = Guid.NewGuid(),
                    ProductIdentifier = ProductIdentifier.FootballBall,
                    Price = 14,
                    Name = "Football Ball",
                    Category = ProductCategory.Sport
                };
                Products.Add(ball.Id, ball);
            }

        }

    }
}
