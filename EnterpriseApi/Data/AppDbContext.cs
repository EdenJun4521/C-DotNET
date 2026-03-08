using EnterpriseApi.Models;
using Microsoft.EntityFrameworkCore;

namespace EnterpriseApi.Data
{
    /// <summary>
    /// 應用程式資料庫內容
    /// </summary>
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<OrderEntity> Orders { get; set; } = null!;
    }
}
