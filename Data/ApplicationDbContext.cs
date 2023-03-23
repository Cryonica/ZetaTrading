using Microsoft.EntityFrameworkCore;
using ZetaTrading.Exceptions;
using ZetaTrading.Models;

namespace ZetaTrading.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Node> Nodes { get; set; }
        public DbSet<Tree> Trees { get; set; }
        public DbSet<SecureException> ExceptionJournal { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=localhost\SQLEXPRESS;Database=TestJobBD11;Trusted_Connection=True;TrustServerCertificate=True");
        }
        public ApplicationDbContext()
        {

        }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options):base(options)
        {

        }
    }
}
