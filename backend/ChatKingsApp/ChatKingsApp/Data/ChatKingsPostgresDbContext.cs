using Microsoft.EntityFrameworkCore;

namespace ChatKingsApp.Data;

public sealed class ChatKingsPostgresDbContext : ChatKingsDbContext
{
    public ChatKingsPostgresDbContext(DbContextOptions<ChatKingsPostgresDbContext> options)
        : base(options)
    {
    }
}
