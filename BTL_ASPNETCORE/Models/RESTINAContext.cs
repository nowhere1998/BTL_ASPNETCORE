using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace BTL_ASPNETCORE.Models
{
    public class RESTINAContext:DbContext
    {
        public RESTINAContext(DbContextOptions<RESTINAContext> options):base(options)
        {
            
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Dish> Dishes { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Chef> Chefs { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
    }
}
