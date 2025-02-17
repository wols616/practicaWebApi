using Microsoft.EntityFrameworkCore;

namespace bibliotecaWebApiPractica.Models
{
    public class bibliotecaContext : DbContext
    {
        public bibliotecaContext(DbContextOptions<bibliotecaContext> options) : base(options) { 
        }

        public DbSet<Libro> libro { get; set; }
        public DbSet<Autor> autor { get; set; }
    }
}
