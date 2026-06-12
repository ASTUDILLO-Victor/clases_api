using MiApi.Data;
using MiApi.Model;
using MiApi.Repositories;
namespace MiApi.Services;
public class LibroService
{
    private readonly LibroRepository _repository;

    public LibroService(LibroRepository repository)
    {
        _repository = repository;
    }

    public List<Libro> ObtenerTodos()
    {
        return _repository.ObtenerTodos();
    }

    public Libro? ObtenerPorId(int id)
    {
        return _repository.ObtenerPorId(id);
    }

    public Libro Agregar(Libro libro)
    {
        _repository.Agregar(libro);
        return libro;
    }

    public Libro? Actualizar(int id, Libro libroActualizado)
    {
        var libro = _repository.ObtenerPorId(id);
        if (libro == null) return null;

        libro.Titulo = libroActualizado.Titulo;
        libro.Autor = libroActualizado.Autor;
        libro.Anio = libroActualizado.Anio;

        _repository.Actualizar(libro);
        return libro;
    }

    public bool Eliminar(int id)
    {
        var libro = _repository.ObtenerPorId(id);
        if (libro == null) return false;

        _repository.Eliminar(libro);
        return true;
    }
}