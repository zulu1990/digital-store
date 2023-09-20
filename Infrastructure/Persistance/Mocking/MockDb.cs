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
            var mob = new Product
            {
                Id = Guid.NewGuid(),
                Name = "Samsung S22+",
                Category = ProductCategory.Electronic,
                Price = 1453,
                ReminingCount = 35
            };
            Products.Add(mob.Id, mob);

            // PS5
            var ps = new Product
            {
                Id = Guid.NewGuid(),
                Category = ProductCategory.Electronic,
                Name = "Playstation 5",
                Price = 1111,
                ReminingCount = 20
            };
            Products.Add(ps.Id, ps);

            // Ball

            var ball = new Product
            {
                Id = Guid.NewGuid(),
                ReminingCount = 3123,
                Price = 14,
                Name = "Football Ball",
                Category = ProductCategory.Sport
            };
            Products.Add(ball.Id, ball);
        }

    }
}
