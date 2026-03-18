using ChatKingsApp.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatKingsApp.Data;

public class ChatKingsDbContext : DbContext
{
    public ChatKingsDbContext(DbContextOptions<ChatKingsDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<PublicProfile> PublicProfiles => Set<PublicProfile>();
    public DbSet<Friendship> Friendships => Set<Friendship>();
    public DbSet<ChatThread> ChatThreads => Set<ChatThread>();
    public DbSet<Message> Messages => Set<Message>();
    public DbSet<Game> Games => Set<Game>();
    public DbSet<OpenGame> OpenGames => Set<OpenGame>();
    public DbSet<GameTurn> GameTurns => Set<GameTurn>();
    public DbSet<Team> Teams => Set<Team>();
    public DbSet<GameStat> GameStats => Set<GameStat>();
    public DbSet<ChatTeam> ChatTeams => Set<ChatTeam>();
    public DbSet<BotHistory> BotHistories => Set<BotHistory>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("USERS");
            entity.HasKey(e => e.UserId);
        });

        modelBuilder.Entity<PublicProfile>(entity =>
        {
            entity.ToTable("PUBLIC_PROFILES");
            entity.HasKey(e => e.UserId);

            entity.HasOne(e => e.User)
                .WithOne(u => u.PublicProfile)
                .HasForeignKey<PublicProfile>(e => e.UserId);
        });

        modelBuilder.Entity<Friendship>(entity =>
        {
            entity.ToTable("FRIENDSHIPS");
            entity.HasKey(e => new { e.RequestorUserId, e.AddresseeUserId });

            entity.HasOne(e => e.RequestorUser)
                .WithMany(u => u.FriendshipsRequested)
                .HasForeignKey(e => e.RequestorUserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.AddresseeUser)
                .WithMany(u => u.FriendshipsReceived)
                .HasForeignKey(e => e.AddresseeUserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ChatThread>(entity =>
        {
            entity.ToTable("CHAT_THREADS");
            entity.HasKey(e => e.ChatThreadId);

            entity.HasOne(e => e.Game)
                .WithMany(g => g.ChatThreads)
                .HasForeignKey(e => e.GameId);
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.ToTable("MESSAGES");
            entity.HasKey(e => e.MessageId);

            entity.HasOne(e => e.ChatThread)
                .WithMany(t => t.Messages)
                .HasForeignKey(e => e.ChatThreadId);

            entity.HasOne(e => e.SenderUser)
                .WithMany(u => u.MessagesSent)
                .HasForeignKey(e => e.SenderUserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Game>(entity =>
        {
            entity.ToTable("GAMES");
            entity.HasKey(e => e.GameId);

            entity.HasOne(e => e.HostUser)
                .WithMany(u => u.GamesHosted)
                .HasForeignKey(e => e.HostUserId);
        });

        modelBuilder.Entity<OpenGame>(entity =>
        {
            entity.ToTable("OPEN");
            entity.HasKey(e => e.OpenGameId);

            entity.HasOne(e => e.Game)
                .WithMany(g => g.OpenGames)
                .HasForeignKey(e => e.GameId);

            entity.HasOne(e => e.HostUser)
                .WithMany()
                .HasForeignKey(e => e.HostUserId);
        });

        modelBuilder.Entity<GameTurn>(entity =>
        {
            entity.ToTable("TURNS");
            entity.HasKey(e => e.GameTurnId);

            entity.HasOne(e => e.Game)
                .WithMany(g => g.Turns)
                .HasForeignKey(e => e.GameId);

            entity.HasOne(e => e.ActingUser)
                .WithMany(u => u.GameTurns)
                .HasForeignKey(e => e.ActingUserId);
        });

        modelBuilder.Entity<Team>(entity =>
        {
            entity.ToTable("TEAMS");
            entity.HasKey(e => e.TeamId);

            entity.HasOne(e => e.Game)
                .WithMany(g => g.Teams)
                .HasForeignKey(e => e.GameId);
        });

        modelBuilder.Entity<GameStat>(entity =>
        {
            entity.ToTable("GAME_STATS");
            entity.HasKey(e => e.GameStatId);

            entity.HasOne(e => e.Game)
                .WithMany(g => g.GameStats)
                .HasForeignKey(e => e.GameId);

            entity.HasOne(e => e.Team)
                .WithMany(t => t.GameStats)
                .HasForeignKey(e => e.TeamId);
        });

        modelBuilder.Entity<ChatTeam>(entity =>
        {
            entity.ToTable("CHAT_TEAMS");
            entity.HasKey(e => new { e.ChatThreadId, e.TeamId });

            entity.HasOne(e => e.ChatThread)
                .WithMany(ct => ct.ChatTeams)
                .HasForeignKey(e => e.ChatThreadId);

            entity.HasOne(e => e.Team)
                .WithMany(t => t.ChatTeams)
                .HasForeignKey(e => e.TeamId);
        });

        modelBuilder.Entity<BotHistory>(entity =>
        {
            entity.ToTable("BOT_HISTORY");
            entity.HasKey(e => e.BotHistoryId);

            entity.HasOne(e => e.User)
                .WithMany(u => u.BotHistories)
                .HasForeignKey(e => e.UserId);

            entity.HasOne(e => e.Game)
                .WithMany()
                .HasForeignKey(e => e.GameId);

            entity.HasOne(e => e.GameTurn)
                .WithMany()
                .HasForeignKey(e => e.GameTurnId);
        });
    }
}

