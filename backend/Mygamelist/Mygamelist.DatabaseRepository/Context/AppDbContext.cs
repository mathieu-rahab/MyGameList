namespace Mygamelist.DatabaseRepository.Context;

using Microsoft.EntityFrameworkCore;
using Mygamelist.Entity;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // User
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Pseudo)
            .IsUnique();

        // Friendship
        modelBuilder.Entity<Friendship>()
            .HasKey(f => f.Id);

        modelBuilder.Entity<Friendship>()
            .HasIndex(f => new { f.User1Id, f.User2Id })
            .IsUnique();
        

        //Collection
        modelBuilder.Entity<Collection>()
            .HasKey(c => c.Id);

        // Relation entre utilisateur et collection - Contrainte de clé étrangère
        modelBuilder.Entity<Collection>()
            .HasOne<User>()
            .WithMany(u => u.Collections)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade); // Si l'utilisateur est supprimé, supprimez également ses collections.
        
        // Validation des identifiants de jeu
        modelBuilder.Entity<Collection>()
            .Property(c => c.GamesId);
    }


    public DbSet<User> Users { get; set; }
    public DbSet<Collection> Collections { get; set; }
    public DbSet<Friendship> Friendships { get; set; }
}