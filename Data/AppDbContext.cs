using Microsoft.EntityFrameworkCore;
using CadeirasAPI.Models;
namespace CadeirasAPI.Data;


public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {

    }

    public DbSet<CadeiraModel> Cadeira { get; set; }
    public DbSet<AlocacaoModel> Alocacao { get; set; }
}

