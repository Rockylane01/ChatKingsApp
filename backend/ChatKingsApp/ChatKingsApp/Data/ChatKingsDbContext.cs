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
    public DbSet<FriendRequest> FriendRequests => Set<FriendRequest>();
    public DbSet<Friendship> Friendships => Set<Friendship>();
    public DbSet<Chat> Chats => Set<Chat>();
    public DbSet<ChatMember> ChatMembers => Set<ChatMember>();
    public DbSet<Message> Messages => Set<Message>();
    public DbSet<Bet> Bets => Set<Bet>();
    public DbSet<Game> Games => Set<Game>();
    public DbSet<Team> Teams => Set<Team>();
    public DbSet<ChatTeam> ChatTeams => Set<ChatTeam>();
    public DbSet<GameStat> GameStats => Set<GameStat>();
    public DbSet<DailyStrike> DailyStrikes => Set<DailyStrike>();
    public DbSet<BetHistory> BetHistories => Set<BetHistory>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("USERS");
            entity.HasKey(e => e.user_id);
        });

        modelBuilder.Entity<FriendRequest>(entity =>
        {
            entity.ToTable("FRIEND_REQUESTS");
            entity.HasKey(e => e.request_id);
        });

        modelBuilder.Entity<Friendship>(entity =>
        {
            entity.ToTable("FRIENDSHIPS");
            entity.HasKey(e => e.friendship_id);
        });

        modelBuilder.Entity<Chat>(entity =>
        {
            entity.ToTable("CHATS");
            entity.HasKey(e => e.chat_id);
        });

        modelBuilder.Entity<ChatMember>(entity =>
        {
            entity.ToTable("CHAT_MEMBERS");
            entity.HasKey(e => e.member_id);
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.ToTable("MESSAGES");
            entity.HasKey(e => e.message_id);
        });

        modelBuilder.Entity<Bet>(entity =>
        {
            entity.ToTable("BETS");
            entity.HasKey(e => e.bet_id);
        });

        modelBuilder.Entity<Game>(entity =>
        {
            entity.ToTable("GAMES");
            entity.HasKey(e => e.game_id);
        });

        modelBuilder.Entity<Team>(entity =>
        {
            entity.ToTable("TEAMS");
            entity.HasKey(e => e.team_id);
        });

        modelBuilder.Entity<ChatTeam>(entity =>
        {
            entity.ToTable("CHAT_TEAMS");
            entity.HasKey(e => e.chat_team_id);
        });

        modelBuilder.Entity<GameStat>(entity =>
        {
            entity.ToTable("GAME_STATS");
            entity.HasKey(e => e.stat_id);
        });

        modelBuilder.Entity<DailyStrike>(entity =>
        {
            entity.ToTable("DAILY_STRIKES");
            entity.HasKey(e => e.strike_id);
        });

        modelBuilder.Entity<BetHistory>(entity =>
        {
            entity.ToTable("BET_HISTORY");
            entity.HasKey(e => e.history_id);
        });
    }
}

