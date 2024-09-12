using Microsoft.EntityFrameworkCore;

namespace TestcontainerExample;

public class Context : DbContext
{
    public Context()
    { }

    public Context(DbContextOptions<Context> options) : base(options)
    { }

    public DbSet<Entity> Entities => Set<Entity>();

    protected override void OnConfiguring(DbContextOptionsBuilder options) => options.UseSqlServer();
}