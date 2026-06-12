using Microsoft.AspNetCore.Mvc;
using MiApi.Data;
using MiApi.Model;
using MiApi.Repositories;
using MiApi.Services;
[ApiController]
[Route("api/[controller]")]
public class LibrosController : ControllerBase
{
    private readonly LibroService _service;

    public LibrosController(LibroService service)
    {
        _service = service;
    }

    [HttpGet]
    public IActionResult GetTodos()
    {
        return Ok(_service.ObtenerTodos());
    }

    [HttpGet("{id}")]
    public IActionResult GetPorId(int id)
    {
        var libro = _service.ObtenerPorId(id);
        if (libro == null)
            return NotFound($"No existe libro con Id {id}");
        return Ok(libro);
    }

    [HttpPost]
    public IActionResult Agregar([FromBody] Libro libro)
    {
        var nuevo = _service.Agregar(libro);
        return Created($"/api/libros/{nuevo.Id}", nuevo);
    }

    [HttpPut("{id}")]
    public IActionResult Actualizar(int id, [FromBody] Libro libroActualizado)
    {
        var libro = _service.Actualizar(id, libroActualizado);
        if (libro == null)
            return NotFound($"No existe libro con Id        {id}");
        return Ok(libro);
    }

    [HttpDelete("{id}")]
    public IActionResult Eliminar(int id)
    {
        bool eliminado = _service.Eliminar(id);
        if (!eliminado)
            return NotFound($"No existe libro con Id {id}");
        return Ok($"Libro eliminado");
    }
}