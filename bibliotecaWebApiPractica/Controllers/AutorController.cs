using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using bibliotecaWebApiPractica.Models;
using Microsoft.EntityFrameworkCore;

namespace bibliotecaWebApiPractica.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AutorController : ControllerBase
    {
        private readonly bibliotecaContext _bibliotecaContexto;

        public AutorController(bibliotecaContext bibliotecaContexto)
        {
            _bibliotecaContexto = bibliotecaContexto;
        }

        [HttpGet]
        [Route("GetAll")]
        public IActionResult Get()
        {
            List<Autor> listaAutores = (from e in _bibliotecaContexto.autor select e).ToList();

            if(listaAutores.Count == 0)
            {
                return NotFound();
            }
            return Ok(listaAutores);
        }

        [HttpGet]
        [Route("GetById/{id}")]
        public IActionResult Get(int id)
        {
            var autor = (from a in _bibliotecaContexto.autor
                          where a.Id == id
                          select new
                          {
                              a.Nombre,
                              Libros = (from l in _bibliotecaContexto.libro where l.AutorId == id select l.Titulo).ToList()
                          }).FirstOrDefault() ;


            //var autor = (from a in _bibliotecaContexto.autor
            //                join l in _bibliotecaContexto.libro
            //                on a.Id equals l.AutorId
            //                where a.Id == id   
            //             select new
            //                {
            //                    a.Nombre,
            //                    l.Titulo
            //                }
            //                );
            
            if (autor == null)
            {
                return NotFound();
            }
             return Ok(autor);
        }

        [HttpPost]
        [Route("Add")]
        public IActionResult GuardarAutor([FromBody] Autor autor)
        {
            try
            {
                _bibliotecaContexto.autor.Add(autor);
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
        public IActionResult ActualizarAutor(int id, [FromBody] Autor autorModificar)
        {
            Autor? autorActual = (from e in _bibliotecaContexto.autor where e.Id == id select e).FirstOrDefault();

            if (autorActual == null)
            {
                return NotFound();
            }

            autorActual.Nombre = autorModificar.Nombre;
            autorActual.Nacionalidad = autorModificar.Nacionalidad;

            _bibliotecaContexto.Entry(autorActual).State = EntityState.Modified;
            _bibliotecaContexto.SaveChanges();

            return Ok(autorModificar);
        }

        [HttpDelete]
        [Route("eliminar/{id}")]

        public IActionResult EliminarAutor(int id)
        {
            Autor? autor = (from e in _bibliotecaContexto.autor where e.Id == id select e).FirstOrDefault();

            if (autor == null)
            {
                return NotFound();
            }
            _bibliotecaContexto.autor.Attach(autor);
            _bibliotecaContexto.autor.Remove(autor);
            _bibliotecaContexto.SaveChanges();
            return Ok(autor);
        }

        [HttpGet]
        [Route("CountBooksByAuthor")]
        public IActionResult CountBooksByAuthor(int id)
        {
            //int cant = _bibliotecaContexto.libro.Count(l => l.AutorId == id);

            int cant = (from e in _bibliotecaContexto.libro where e.AutorId == id select e).Count();

            if (cant == 0)
            {
                return NotFound();
            }
            return Ok(cant);
        }

        [HttpGet]
        [Route("GetMasPublicados")]
        public IActionResult GetMasPublicados()
        {
            var listaAutores = (from a in _bibliotecaContexto.autor
                                        select new
                                        {
                                            a.Nombre,
                                            Libros = (from l in _bibliotecaContexto.libro where l.AutorId == a.Id select l).Count()
                                        }).OrderByDescending(resultado => resultado.Libros).ToList();
            if (listaAutores.Count == 0)
            {
                return NotFound();
            }
            return Ok(listaAutores);
        }



        [HttpGet]
        [Route("AutorTieneLibros/{autorId}")]
        public IActionResult AutorTieneLibros(int autorId)
        {
            bool tieneLibros = _bibliotecaContexto.libro.Any(l => l.AutorId == autorId);

            return Ok(new { AutorId = autorId, TieneLibros = tieneLibros });
        }

        [HttpGet]
        [Route("PrimerLibro")]
        public IActionResult PrimerLibro(int autorId)
        {
            var libro = (from l in _bibliotecaContexto.libro where l.AutorId == autorId select l).OrderBy(res => res.AnioPublicacion).Take(1);

            return Ok(libro);
        }


    }
}
