using ChatKingsApp.Data;
using ChatKingsApp.Services;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

if (!builder.Environment.IsDevelopment())
{
    // Railpack / many PaaS images set PORT (often 3000). Dokploy/Traefik "container
    // port" for this service must match whatever Kestrel binds — see hosting logs.
    // Override with ASPNETCORE_URLS=http://0.0.0.0:8080 if your proxy targets 8080.
    var urls = Environment.GetEnvironmentVariable("ASPNETCORE_URLS");
    if (string.IsNullOrEmpty(urls))
    {
        var port = Environment.GetEnvironmentVariable("PORT") ?? "3000";
        builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
    }
}

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders =
        ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    // Trust only typical reverse-proxy paths (private/container nets). Clearing
    // KnownIPNetworks/KnownProxies without replacing them would trust spoofed
    // X-Forwarded-* from any remote IP.
    options.KnownIPNetworks.Clear();
    options.KnownProxies.Clear();
    foreach (var cidr in new[]
             {
                 "10.0.0.0/8",
                 "172.16.0.0/12",
                 "192.168.0.0/16",
                 "127.0.0.0/8",
                 "::1/128",
                 "fe80::/10",
                 "fc00::/7",
             })
        options.KnownIPNetworks.Add(System.Net.IPNetwork.Parse(cidr));
    options.ForwardLimit = 2;
});

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddHttpClient<NcaamScoreboardService>();
builder.Services.AddCors();
builder.Services.AddDbContext<ChatKingsDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ChatKingsDbContext>();
    db.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseForwardedHeaders();

// TLS terminates at Cloudflare / tunnel; Kestrel is HTTP-only. Redirecting to
// HTTPS here breaks or complicates API calls when X-Forwarded-Proto is http.
if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseCors(policy => policy
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

app.UseAuthorization();

app.MapControllers();

app.Run();
