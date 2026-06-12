using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using MiApi.Data;
using MiApi.Model;
using MiApi.Repositories;
using MiApi.Services;
using Xunit;

namespace MiApi.Tests;

public class LibroServiceTests
{
    private static (LibroService service, AppDbContext ctx) CrearServicio()
    {
        var ctx = new AppDbContext(new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);
        return (new LibroService(new LibroRepository(ctx)), ctx);
    }

    // --- ObtenerTodos ---

    [Fact]
    public void ObtenerTodos_SinLibros_RetornaListaVacia()
    {
        var (service, _) = CrearServicio();

        service.ObtenerTodos().Should().BeEmpty();
    }

    [Fact]
    public void ObtenerTodos_ConLibros_RetornaTodosLosLibros()
    {
        var (service, ctx) = CrearServicio();
        ctx.Libros.AddRange(
            new Libro { Titulo = "Libro A", Autor = "Autor A", Anio = 2020 },
            new Libro { Titulo = "Libro B", Autor = "Autor B", Anio = 2021 }
        );
        ctx.SaveChanges();

        service.ObtenerTodos().Should().HaveCount(2);
    }

    // --- ObtenerPorId ---

    [Fact]
    public void ObtenerPorId_IdExistente_RetornaLibro()
    {
        var (service, ctx) = CrearServicio();
        ctx.Libros.Add(new Libro { Titulo = "Clean Code", Autor = "Martin", Anio = 2008 });
        ctx.SaveChanges();
        var id = ctx.Libros.First().Id;

        var resultado = service.ObtenerPorId(id);

        resultado.Should().NotBeNull();
        resultado!.Titulo.Should().Be("Clean Code");
        resultado.Autor.Should().Be("Martin");
    }

    [Fact]
    public void ObtenerPorId_IdInexistente_RetornaNull()
    {
        var (service, _) = CrearServicio();

        service.ObtenerPorId(42).Should().BeNull();
    }

    // --- Agregar ---

    [Fact]
    public void Agregar_LibroNuevo_RetornaLibroConIdAsignado()
    {
        var (service, _) = CrearServicio();
        var libro = new Libro { Titulo = "Nuevo", Autor = "Autor", Anio = 2024 };

        var resultado = service.Agregar(libro);

        resultado.Id.Should().BeGreaterThan(0);
        resultado.Titulo.Should().Be("Nuevo");
    }

    // --- Actualizar ---

    [Fact]
    public void Actualizar_LibroExistente_ActualizaTodosLosCampos()
    {
        var (service, ctx) = CrearServicio();
        ctx.Libros.Add(new Libro { Titulo = "Viejo", Autor = "Viejo Autor", Anio = 2000 });
        ctx.SaveChanges();
        var id = ctx.Libros.First().Id;

        var resultado = service.Actualizar(id, new Libro { Titulo = "Nuevo Título", Autor = "Nuevo Autor", Anio = 2024 });

        resultado.Should().NotBeNull();
        resultado!.Titulo.Should().Be("Nuevo Título");
        resultado.Autor.Should().Be("Nuevo Autor");
        resultado.Anio.Should().Be(2024);
    }

    [Fact]
    public void Actualizar_LibroExistente_NoCreaNuevaEntidad()
    {
        var (service, ctx) = CrearServicio();
        ctx.Libros.Add(new Libro { Titulo = "Viejo", Autor = "Autor", Anio = 2000 });
        ctx.SaveChanges();
        var id = ctx.Libros.First().Id;

        service.Actualizar(id, new Libro { Titulo = "Nuevo", Autor = "Autor", Anio = 2024 });

        ctx.Libros.Should().ContainSingle();
    }

    [Fact]
    public void Actualizar_IdInexistente_RetornaNull()
    {
        var (service, _) = CrearServicio();

        service.Actualizar(99, new Libro { Titulo = "X", Autor = "Y", Anio = 2020 }).Should().BeNull();
    }

    // --- Eliminar ---

    [Fact]
    public void Eliminar_LibroExistente_RetornaTrue()
    {
        var (service, ctx) = CrearServicio();
        ctx.Libros.Add(new Libro { Titulo = "Para Borrar", Autor = "Autor", Anio = 2020 });
        ctx.SaveChanges();
        var id = ctx.Libros.First().Id;

        service.Eliminar(id).Should().BeTrue();
    }

    [Fact]
    public void Eliminar_LibroExistente_LoRemoveDeLaDB()
    {
        var (service, ctx) = CrearServicio();
        ctx.Libros.Add(new Libro { Titulo = "Para Borrar", Autor = "Autor", Anio = 2020 });
        ctx.SaveChanges();
        var id = ctx.Libros.First().Id;

        service.Eliminar(id);

        ctx.Libros.Should().BeEmpty();
    }

    [Fact]
    public void Eliminar_IdInexistente_RetornaFalse()
    {
        var (service, _) = CrearServicio();

        service.Eliminar(99).Should().BeFalse();
    }
}
