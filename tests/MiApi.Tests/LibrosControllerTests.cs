using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiApi.Data;
using MiApi.Model;
using MiApi.Repositories;
using MiApi.Services;
using Xunit;

namespace MiApi.Tests;

public class LibrosControllerTests
{
    private static (LibrosController controller, AppDbContext ctx) CrearController()
    {
        var ctx = new AppDbContext(new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);
        var controller = new LibrosController(new LibroService(new LibroRepository(ctx)));
        return (controller, ctx);
    }

    // --- GetTodos ---

    [Fact]
    public void GetTodos_SinLibros_RetornaOkConListaVacia()
    {
        var (ctrl, _) = CrearController();

        ctrl.GetTodos().Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeAssignableTo<List<Libro>>()
            .Which.Should().BeEmpty();
    }

    [Fact]
    public void GetTodos_ConLibros_RetornaOkConTodosLosLibros()
    {
        var (ctrl, ctx) = CrearController();
        ctx.Libros.AddRange(
            new Libro { Titulo = "A", Autor = "X", Anio = 2020 },
            new Libro { Titulo = "B", Autor = "Y", Anio = 2021 }
        );
        ctx.SaveChanges();

        ctrl.GetTodos().Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeAssignableTo<List<Libro>>()
            .Which.Should().HaveCount(2);
    }

    // --- GetPorId ---

    [Fact]
    public void GetPorId_IdExistente_RetornaOkConLibro()
    {
        var (ctrl, ctx) = CrearController();
        ctx.Libros.Add(new Libro { Titulo = "Clean Code", Autor = "Martin", Anio = 2008 });
        ctx.SaveChanges();
        var id = ctx.Libros.First().Id;

        ctrl.GetPorId(id).Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeOfType<Libro>()
            .Which.Titulo.Should().Be("Clean Code");
    }

    [Fact]
    public void GetPorId_IdInexistente_RetornaNotFound()
    {
        var (ctrl, _) = CrearController();

        ctrl.GetPorId(99).Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public void GetPorId_IdInexistente_MensajeContieneElId()
    {
        var (ctrl, _) = CrearController();

        ctrl.GetPorId(42).Should().BeOfType<NotFoundObjectResult>()
            .Which.Value.Should().Be("No existe libro con Id 42");
    }

    // --- Agregar ---

    [Fact]
    public void Agregar_LibroValido_Retorna201Created()
    {
        var (ctrl, _) = CrearController();

        ctrl.Agregar(new Libro { Titulo = "Nuevo", Autor = "Autor", Anio = 2024 })
            .Should().BeOfType<CreatedResult>()
            .Which.StatusCode.Should().Be(201);
    }

    [Fact]
    public void Agregar_LibroValido_LocationContieneIdGenerado()
    {
        var (ctrl, _) = CrearController();

        var resultado = ctrl.Agregar(new Libro { Titulo = "Nuevo", Autor = "Autor", Anio = 2024 })
            .Should().BeOfType<CreatedResult>().Subject;

        resultado.Location.Should().StartWith("/api/libros/");
        resultado.Value.Should().BeOfType<Libro>().Which.Id.Should().BeGreaterThan(0);
    }

    // --- Actualizar ---

    [Fact]
    public void Actualizar_LibroExistente_RetornaOkConDatosActualizados()
    {
        var (ctrl, ctx) = CrearController();
        ctx.Libros.Add(new Libro { Titulo = "Viejo", Autor = "Viejo Autor", Anio = 2000 });
        ctx.SaveChanges();
        var id = ctx.Libros.First().Id;

        ctrl.Actualizar(id, new Libro { Titulo = "Nuevo Título", Autor = "Nuevo Autor", Anio = 2024 })
            .Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeOfType<Libro>()
            .Which.Titulo.Should().Be("Nuevo Título");
    }

    [Fact]
    public void Actualizar_IdInexistente_RetornaNotFound()
    {
        var (ctrl, _) = CrearController();

        ctrl.Actualizar(99, new Libro { Titulo = "X", Autor = "Y", Anio = 2020 })
            .Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public void Actualizar_IdInexistente_MensajeContieneElId()
    {
        var (ctrl, _) = CrearController();

        ctrl.Actualizar(55, new Libro { Titulo = "X", Autor = "Y", Anio = 2020 })
            .Should().BeOfType<NotFoundObjectResult>()
            .Which.Value.Should().Be("No existe libro con Id 55");
    }

    // --- Eliminar ---

    [Fact]
    public void Eliminar_LibroExistente_RetornaOkConMensaje()
    {
        var (ctrl, ctx) = CrearController();
        ctx.Libros.Add(new Libro { Titulo = "Para Borrar", Autor = "Autor", Anio = 2020 });
        ctx.SaveChanges();
        var id = ctx.Libros.First().Id;

        ctrl.Eliminar(id).Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().Be("Libro eliminado");
    }

    [Fact]
    public void Eliminar_IdInexistente_RetornaNotFound()
    {
        var (ctrl, _) = CrearController();

        ctrl.Eliminar(99).Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public void Eliminar_IdInexistente_MensajeContieneElId()
    {
        var (ctrl, _) = CrearController();

        ctrl.Eliminar(77).Should().BeOfType<NotFoundObjectResult>()
            .Which.Value.Should().Be("No existe libro con Id 77");
    }
}
