using ChatKingsApp.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatKingsApp.Data;

public class ChatKingsDbContext : DbContext
{
    public ChatKingsDbContext(DbContextOptions options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<FriendRequest> FriendRequests => Set<FriendRequest>();
    public DbSet<Friendship> Friendships => Set<Friendship>();
    public DbSet<Chat> Chats => Set<Chat>();
    public DbSet<ChatMember> ChatMembers => Set<ChatMember>();
    public DbSet<ChatMessage> ChatMessages => Set<ChatMessage>();
    public DbSet<Team> Teams => Set<Team>();
    public DbSet<Game> Games => Set<Game>();
    public DbSet<GameTeamStat> GameTeamStats => Set<GameTeamStat>();
    public DbSet<ChatTeam> ChatTeams => Set<ChatTeam>();
    public DbSet<Prediction> Predictions => Set<Prediction>();
    public DbSet<PredictionOption> PredictionOptions => Set<PredictionOption>();
    public DbSet<Wager> Wagers => Set<Wager>();
    public DbSet<PredictionResolution> PredictionResolutions => Set<PredictionResolution>();
    public DbSet<StrikeEvent> StrikeEvents => Set<StrikeEvent>();
    public DbSet<PointsLedger> PointsLedgers => Set<PointsLedger>();
    public DbSet<ChatLeaderboardSnapshot> ChatLeaderboardSnapshots => Set<ChatLeaderboardSnapshot>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");
            entity.HasKey(e => e.user_id);
        });

        modelBuilder.Entity<FriendRequest>(entity =>
        {
            entity.ToTable("friend_requests");
            entity.HasKey(e => e.request_id);
        });

        modelBuilder.Entity<Friendship>(entity =>
        {
            entity.ToTable("friendships");
            entity.HasKey(e => e.friendship_id);
        });

        modelBuilder.Entity<Chat>(entity =>
        {
            entity.ToTable("chats");
            entity.HasKey(e => e.chat_id);
        });

        modelBuilder.Entity<ChatMember>(entity =>
        {
            entity.ToTable("chat_members");
            entity.HasKey(e => e.chat_member_id);
        });

        modelBuilder.Entity<ChatMessage>(entity =>
        {
            entity.ToTable("chat_messages");
            entity.HasKey(e => e.message_id);
        });

        modelBuilder.Entity<Team>(entity =>
        {
            entity.ToTable("teams");
            entity.HasKey(e => e.team_id);
        });

        modelBuilder.Entity<Game>(entity =>
        {
            entity.ToTable("games");
            entity.HasKey(e => e.game_id);
        });

        modelBuilder.Entity<GameTeamStat>(entity =>
        {
            entity.ToTable("game_team_stats");
            entity.HasKey(e => e.stat_id);
        });

        modelBuilder.Entity<ChatTeam>(entity =>
        {
            entity.ToTable("chat_teams");
            entity.HasKey(e => e.chat_team_id);
        });

        modelBuilder.Entity<Prediction>(entity =>
        {
            entity.ToTable("predictions");
            entity.HasKey(e => e.prediction_id);
        });

        modelBuilder.Entity<PredictionOption>(entity =>
        {
            entity.ToTable("prediction_options");
            entity.HasKey(e => e.option_id);
            entity.HasOne<Prediction>()
                .WithMany(p => p.Options)
                .HasForeignKey(e => e.prediction_id);
        });

        modelBuilder.Entity<Wager>(entity =>
        {
            entity.ToTable("wagers");
            entity.HasKey(e => e.wager_id);
            entity.HasOne(w => w.Prediction)
                .WithMany(p => p.Wagers)
                .HasForeignKey(w => w.prediction_id);
            entity.HasOne(w => w.PredictionOption)
                .WithMany(o => o.Wagers)
                .HasForeignKey(w => w.option_id);
        });

        modelBuilder.Entity<PredictionResolution>(entity =>
        {
            entity.ToTable("prediction_resolutions");
            entity.HasKey(e => e.resolution_id);
        });

        modelBuilder.Entity<StrikeEvent>(entity =>
        {
            entity.ToTable("strike_events");
            entity.HasKey(e => e.strike_event_id);
        });

        modelBuilder.Entity<PointsLedger>(entity =>
        {
            entity.ToTable("points_ledger");
            entity.HasKey(e => e.ledger_id);
        });

        modelBuilder.Entity<ChatLeaderboardSnapshot>(entity =>
        {
            entity.ToTable("chat_leaderboard_snapshots");
            entity.HasKey(e => e.snapshot_id);
        });
    }
}
