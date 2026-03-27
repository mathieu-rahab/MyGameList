namespace Mygamelist.DatabaseRepository.Context;

using Microsoft.EntityFrameworkCore;
using Mygamelist.Entity;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
}