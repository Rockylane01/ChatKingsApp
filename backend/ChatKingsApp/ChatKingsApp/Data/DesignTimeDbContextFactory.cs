using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ChatKingsApp.Data;

public sealed class SqliteDesignTimeDbContextFactory : IDesignTimeDbContextFactory<ChatKingsDbContext>
{
    public ChatKingsDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ChatKingsDbContext>();
        var connectionString = Environment.GetEnvironmentVariable("SQLITE_CONNECTION_STRING")
                               ?? "Data Source=ChatKings.db";

        optionsBuilder.UseSqlite(connectionString);
        return new ChatKingsDbContext(optionsBuilder.Options);
    }
}

public sealed class PostgresDesignTimeDbContextFactory : IDesignTimeDbContextFactory<ChatKingsPostgresDbContext>
{
    public ChatKingsPostgresDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ChatKingsPostgresDbContext>();
        var connectionString = Environment.GetEnvironmentVariable("POSTGRES_CONNECTION_STRING")
                               ?? "Host=localhost;Port=5432;Database=chatkingsdb;Username=postgres;Password=postgres";

        optionsBuilder.UseNpgsql(connectionString);
        return new ChatKingsPostgresDbContext(optionsBuilder.Options);
    }
}
