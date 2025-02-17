using bibliotecaWebApiPractica.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace bibliotecaWebApiPractica.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LibroController : ControllerBase
    {
        private readonly bibliotecaContext _bibliotecaContexto;

        public LibroController(bibliotecaContext bibliotecaContexto)
        {
            _bibliotecaContexto = bibliotecaContexto;
        }

        [HttpGet]
        [Route("GetAll")]
        public IActionResult Get()
        {
            List<Libro> listaLibros = (from e in _bibliotecaContexto.libro select e).Skip(5).Take(5).ToList();

            if (listaLibros.Count == 0)
            {
                return NotFound();
            }
            return Ok(listaLibros);
        }

        [HttpGet]
        [Route("GetById/{id}")]
        public IActionResult Get(int id)
        {
            var libro = (from l in _bibliotecaContexto.libro
                         join a in _bibliotecaContexto.autor
                         on l.AutorId equals a.Id
                         where l.Id == id
                         select new
                         {
                             l.Titulo,
                             a.Nombre
                         }).FirstOrDefault();

            if (libro == null)
            {
                return NotFound();
            }
            return Ok(libro);
        }

        [HttpPost]
        [Route("Add")]
        public IActionResult GuardarLibro([FromBody] Libro libro)
        {
            try
            {
                _bibliotecaContexto.libro.Add(libro);
                _bibliotecaContexto.SaveChanges();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        [Route("actualizar/{id}")]
        public IActionResult ActualizarLibro(int id, [FromBody] Libro libroModificar)
        {
            Libro? libroActual = (from e in _bibliotecaContexto.libro where e.Id == id select e).FirstOrDefault();

            if (libroActual == null)
            {
                return NotFound();
            }

            libroActual.Titulo = libroModificar.Titulo;
            libroActual.AnioPublicacion = libroModificar.AnioPublicacion;
            libroActual.AutorId = libroModificar.AutorId;
            libroActual.CategoriaId = libroModificar.CategoriaId;
            libroActual.Resumen = libroModificar.Resumen;

            _bibliotecaContexto.Entry(libroActual).State = EntityState.Modified;
            _bibliotecaContexto.SaveChanges();

            return Ok(libroModificar);
        }

        [HttpDelete]
        [Route("eliminar/{id}")]

        public IActionResult EliminarLibro(int id)
        {
            Libro? libro = (from e in _bibliotecaContexto.libro where e.Id == id select e).FirstOrDefault();

            if (libro == null)
            {
                return NotFound();
            }
            _bibliotecaContexto.libro.Attach(libro);
            _bibliotecaContexto.libro.Remove(libro);
            _bibliotecaContexto.SaveChanges();
            return Ok(libro);
        }

        [HttpGet]
        [Route("GetAfterYear/{year}")]
        public IActionResult GetAfterYear(int year)
        {
            List<Libro> libros = (from l in _bibliotecaContexto.libro
                                  where l.AnioPublicacion > year
                                  select l).ToList();
            if (libros == null)
            {
                return NotFound();
            }
            return Ok(libros);
        }

        [HttpGet]
        [Route("GetBTitle/{titulo}")]
        public IActionResult GetBTitle(string titulo)
        {
            var libro = (from l in _bibliotecaContexto.libro
                         join a in _bibliotecaContexto.autor
                         on l.AutorId equals a.Id
                         where (l.Titulo.Contains(titulo))
                         select new
                         {
                             l.Titulo,
                             a.Nombre
                         }).FirstOrDefault();

            if (libro == null)
            {
                return NotFound();
            }
            return Ok(libro);
        }

        [HttpGet]
        [Route("GetLibrosRecientes")]
        public IActionResult GetLibrosRecientes()
        {
            var librosRecientes = (from l in _bibliotecaContexto.libro
                                   select new
                                   {
                                       l.Titulo,
                                       l.AnioPublicacion
                                   }).OrderByDescending(res => res.AnioPublicacion).ToList();

            if (librosRecientes.Count == 0)
            {
                return NotFound();
            }
            return Ok(librosRecientes);
        }

        [HttpGet]
        [Route("CantLibrosPorAnio")]
        public IActionResult CantLibrosPorAnio()
        {
            var librosRecientes = (from l in _bibliotecaContexto.libro
                                   group l by l.AnioPublicacion into grupo
                                   select new
                                   {
                                       AnioPublicacion = grupo.Key,
                                       Cantidad = grupo.Count()
                                   }).ToList();

            if (!librosRecientes.Any())
            {
                return NotFound();
            }

            return Ok(librosRecientes);
        }

    }
}
