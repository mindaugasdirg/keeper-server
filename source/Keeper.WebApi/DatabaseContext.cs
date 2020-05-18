using Keeper.WebApi.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Keeper.WebApi
{
    public class DatabaseContext : IdentityDbContext
    {
        public DbSet<Transaction> Transactions { get; set; }

        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {

        }
    }
}