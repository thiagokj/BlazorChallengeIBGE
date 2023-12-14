using BlazorChallengeIBGE.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BlazorChallengeIBGE.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
{
  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);

    // Configura o nome da tabela
    modelBuilder.Entity<Locality>().ToTable("Ibge");

    modelBuilder.Entity<Locality>()
        .HasIndex(l => new { l.State, l.City })
        .IsUnique();
  }

  public DbSet<Locality> Localities { get; set; } = null!;
}
