using Microsoft.EntityFrameworkCore;
using MiApi.Model;
namespace MiApi.Data;
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // Cada DbSet representa una tabla en la base de datos
    public DbSet<Libro> Libros { get; set; }
}