using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using MiApi.Data;
using MiApi.Model;
using MiApi.Repositories;
using Xunit;

namespace MiApi.Tests;

public class LibroRepositoryTests
{
    private static AppDbContext CrearContexto() =>
        new(new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    // --- ObtenerTodos ---

    [Fact]
    public void ObtenerTodos_SinLibros_RetornaListaVacia()
    {
        using var ctx = CrearContexto();
        var repo = new LibroRepository(ctx);

        repo.ObtenerTodos().Should().BeEmpty();
    }

    [Fact]
    public void ObtenerTodos_ConVariosLibros_RetornaTodos()
    {
        using var ctx = CrearContexto();
        ctx.Libros.AddRange(
            new Libro { Titulo = "Libro A", Autor = "Autor A", Anio = 2020 },
            new Libro { Titulo = "Libro B", Autor = "Autor B", Anio = 2021 }
        );
        ctx.SaveChanges();
        var repo = new LibroRepository(ctx);

        repo.ObtenerTodos().Should().HaveCount(2);
    }

    // --- ObtenerPorId ---

    [Fact]
    public void ObtenerPorId_IdExistente_RetornaLibroCorrecto()
    {
        using var ctx = CrearContexto();
        ctx.Libros.Add(new Libro { Titulo = "Clean Code", Autor = "Martin", Anio = 2008 });
        ctx.SaveChanges();
        var id = ctx.Libros.First().Id;
        var repo = new LibroRepository(ctx);

        var resultado = repo.ObtenerPorId(id);

        resultado.Should().NotBeNull();
        resultado!.Titulo.Should().Be("Clean Code");
        resultado.Autor.Should().Be("Martin");
    }

    [Fact]
    public void ObtenerPorId_IdInexistente_RetornaNull()
    {
        using var ctx = CrearContexto();
        var repo = new LibroRepository(ctx);

        repo.ObtenerPorId(999).Should().BeNull();
    }

    // --- Agregar ---

    [Fact]
    public void Agregar_LibroValido_PersisteEnLaDB()
    {
        using var ctx = CrearContexto();
        var repo = new LibroRepository(ctx);
        var libro = new Libro { Titulo = "Pragmatic Programmer", Autor = "Hunt", Anio = 1999 };

        repo.Agregar(libro);

        ctx.Libros.Should().ContainSingle(l => l.Titulo == "Pragmatic Programmer");
    }

    [Fact]
    public void Agregar_LibroValido_AsignaIdAutomaticamente()
    {
        using var ctx = CrearContexto();
        var repo = new LibroRepository(ctx);
        var libro = new Libro { Titulo = "DDD", Autor = "Evans", Anio = 2003 };

        repo.Agregar(libro);

        libro.Id.Should().BeGreaterThan(0);
    }

    // --- Actualizar ---

    [Fact]
    public void Actualizar_PersisteCambiosSinDuplicarEntidad()
    {
        using var ctx = CrearContexto();
        var libro = new Libro { Titulo = "Título Original", Autor = "Autor Original", Anio = 2000 };
        ctx.Libros.Add(libro);
        ctx.SaveChanges();
        var repo = new LibroRepository(ctx);

        libro.Titulo = "Título Actualizado";
        libro.Autor = "Nuevo Autor";
        repo.Actualizar(libro);

        ctx.Libros.Should().ContainSingle();
        ctx.Libros.First().Titulo.Should().Be("Título Actualizado");
        ctx.Libros.First().Autor.Should().Be("Nuevo Autor");
    }

    // --- Eliminar ---

    [Fact]
    public void Eliminar_LibroExistente_LoRemoveDeLaDB()
    {
        using var ctx = CrearContexto();
        var libro = new Libro { Titulo = "Para Borrar", Autor = "X", Anio = 2020 };
        ctx.Libros.Add(libro);
        ctx.SaveChanges();
        var repo = new LibroRepository(ctx);

        repo.Eliminar(libro);

        ctx.Libros.Should().BeEmpty();
    }

    [Fact]
    public void Eliminar_UnoDeVariosLibros_SoloEliminaElIndicado()
    {
        using var ctx = CrearContexto();
        var libro1 = new Libro { Titulo = "Libro 1", Autor = "Autor", Anio = 2020 };
        var libro2 = new Libro { Titulo = "Libro 2", Autor = "Autor", Anio = 2021 };
        ctx.Libros.AddRange(libro1, libro2);
        ctx.SaveChanges();
        var repo = new LibroRepository(ctx);

        repo.Eliminar(libro1);

        ctx.Libros.Should().ContainSingle(l => l.Titulo == "Libro 2");
    }
}
