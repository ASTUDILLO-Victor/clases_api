using MiApi.Data;
using MiApi.Model;
namespace MiApi.Repositories;
public class LibroRepository
{
    private readonly AppDbContext _context;

    public LibroRepository(AppDbContext context)
    {
        _context = context;
    }

    public List<Libro> ObtenerTodos()
    {
        return _context.Libros.ToList();
    }

    public Libro? ObtenerPorId(int id)
    {
        return _context.Libros.FirstOrDefault(l => l.Id == id);
    }

    public void Agregar(Libro libro)
    {
        _context.Libros.Add(libro);
        _context.SaveChanges();
    }

    public void Actualizar(Libro libro)
    {
        _context.SaveChanges();
    }

    public void Eliminar(Libro libro)
    {
        _context.Libros.Remove(libro);
        _context.SaveChanges();
    }
}