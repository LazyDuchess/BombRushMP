using AspNet.Security.OAuth.Discord;
using BombRushMP.Server;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using Microsoft.AspNetCore.Authentication.OAuth;
using System.Data;

namespace BombRushMP.ServerApp
{
    public class WebServer
    {
        private BRCServer _server;

        public WebServer(BRCServer server)
        {
            _server = server;
        }

        private async Task<object> GetPlayers()
        {
            return await _server.RunOnMainThreadAsync(() =>
            {
                var playerCount = 0;
                var stages = new Dictionary<int, int>();

                foreach (var ply in _server.Players)
                {
                    if (ply.Value.Invisible) continue;
                    if (ply.Value.ClientState == null) continue;
                    playerCount++;
                    if (stages.TryGetValue(ply.Value.ClientState.Stage, out var plyCount))
                    {
                        stages[ply.Value.ClientState.Stage] = plyCount + 1;
                    }
                    else
                    {
                        stages[ply.Value.ClientState.Stage] = 1;
                    }
                }

                return new
                {
                    count = playerCount,
                    stages
                };
            });
        }

        private async Task OnCreatingTicket(OAuthCreatingTicketContext ctx)
        {
            var id = ctx.Identity?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var username = ctx.Identity?.Name;

            var avatarUrl =
                ctx.Identity?.FindFirst(
                    "urn:discord:avatar:url")?.Value;

            if (id == null) return;

            await using var db = new NpgsqlConnection(_sqlConnectionString);

            await db.OpenAsync();

            await using var existsCommand = new NpgsqlCommand("SELECT discord_id FROM users WHERE discord_id = @id", db);

            existsCommand.Parameters.AddWithValue("id", id);

            var existsResult = await existsCommand.ExecuteScalarAsync();

            if (existsResult != null)
            {
                await using var updateCommand = new NpgsqlCommand("UPDATE users SET discord_username = @username, avatar_url = @avatarUrl, last_login_at = NOW() WHERE discord_id = @id", db);
                updateCommand.Parameters.AddWithValue("id", id);
                updateCommand.Parameters.AddWithValue("username", username ?? "");
                updateCommand.Parameters.AddWithValue("avatarUrl", avatarUrl ?? "");
                await updateCommand.ExecuteNonQueryAsync();
            }
            else
            {
                await using var insertCommand = new NpgsqlCommand("INSERT INTO users (discord_id, discord_username, avatar_url) VALUES (@id, @username, @avatarUrl)", db);
                insertCommand.Parameters.AddWithValue("id", id);
                insertCommand.Parameters.AddWithValue("username", username ?? "");
                insertCommand.Parameters.AddWithValue("avatarUrl", avatarUrl ?? "");
                await insertCommand.ExecuteNonQueryAsync();
            }
        }

        private string _sqlConnectionString;

        public void Start(string origin, string discordClientId, string discordClientSecret, string discordCallback, string sqlConnectionString)
        {
            _sqlConnectionString = sqlConnectionString;

            var builder = WebApplication.CreateBuilder();

            builder.WebHost.UseUrls("http://127.0.0.1:5000");

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("Frontend", policy =>
                {
                    policy
                        .WithOrigins(origin)
                        .AllowCredentials()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });
            builder.Services.AddAuthorization();
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = DiscordAuthenticationDefaults.AuthenticationScheme;
            }).AddDiscord(options =>
            {
                options.ClientId = discordClientId;
                options.ClientSecret = discordClientSecret;
                options.CallbackPath = discordCallback;
                options.SaveTokens = true;

                options.CorrelationCookie.SameSite = SameSiteMode.Lax;
                options.CorrelationCookie.SecurePolicy = CookieSecurePolicy.Always;

                options.ClaimActions.MapCustomJson("urn:discord:avatar:url", user =>
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "https://cdn.discordapp.com/avatars/{0}/{1}.{2}",
                        user.GetString("id"),
                        user.GetString("avatar"),
                        user.GetString("avatar")!.StartsWith("a_") ? "gif" : "png"));

                options.Scope.Add("identify");

                options.Events.OnCreatingTicket = OnCreatingTicket;

            }).AddCookie(options =>
            {
                options.Cookie.Name = "DiscordAuth";
                options.LoginPath = "/api/login";
                options.LogoutPath = "/api/logout";
                options.ExpireTimeSpan = TimeSpan.FromDays(7);
                options.Cookie.SameSite = SameSiteMode.Lax;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            });

            builder.Services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders =
                    ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            });

            var app = builder.Build();

            app.UseForwardedHeaders();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseCors("Frontend");

            app.MapGet("/api/players", GetPlayers);
            app.MapGet("/api/login", () =>
            {
                var properties = new AuthenticationProperties
                {
                    RedirectUri = $"{origin}/me", // The URL to redirect to after successful authentication
                    IsPersistent = true // Ensures the session persists across requests (the authentication cookie will be stored)
                };

                // Triggers the OAuth challenge and redirects the user to Discord's authorization page
                return Results.Challenge(properties, [DiscordAuthenticationDefaults.AuthenticationScheme]);
            });
            app.MapPost("/api/logout", async (HttpContext ctx) =>
            {
                await ctx.SignOutAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme);

                return Results.Ok();
            });
            app.MapGet("/api/game-token", async (HttpContext ctx) =>
            {
                if (!(ctx.User.Identity?.IsAuthenticated ?? false))
                    return Results.Unauthorized();

                var discordid = ctx.User.FindFirst(
                        ClaimTypes.NameIdentifier)?.Value;

                await using var db = new NpgsqlConnection(_sqlConnectionString);

                await db.OpenAsync();

                await using var tokenCmd = new NpgsqlCommand("SELECT game_token FROM users WHERE discord_id = @id", db);
                tokenCmd.Parameters.AddWithValue("id", discordid);

                var tokenResult = (Guid)await tokenCmd.ExecuteScalarAsync();

                return Results.Json(new
                {
                    token = tokenResult.ToString()
                });

            });
            app.MapPost("/api/game-token/regenerate", async (HttpContext ctx) =>
            {
                if (!(ctx.User.Identity?.IsAuthenticated ?? false))
                    return Results.Unauthorized();

                var discordid = ctx.User.FindFirst(
                        ClaimTypes.NameIdentifier)?.Value;

                await using var db = new NpgsqlConnection(_sqlConnectionString);

                await db.OpenAsync();

                await using var tokenCmd = new NpgsqlCommand("UPDATE users SET game_token = gen_random_uuid() WHERE discord_id = @id RETURNING game_token", db);
                tokenCmd.Parameters.AddWithValue("id", discordid);

                var tokenResult = (Guid)await tokenCmd.ExecuteScalarAsync();

                return Results.Json(new
                {
                    token = tokenResult.ToString()
                });
            });
            app.MapGet("/api/me", async (HttpContext ctx) =>
            {
                if (!(ctx.User.Identity?.IsAuthenticated ?? false))
                    return Results.Unauthorized();

                var discordid = ctx.User.FindFirst(
                        ClaimTypes.NameIdentifier)?.Value;

                await using var db = new NpgsqlConnection(_sqlConnectionString);

                await db.OpenAsync();

                await using var selectCmd = new NpgsqlCommand("SELECT role FROM users WHERE discord_id = @id", db);

                selectCmd.Parameters.AddWithValue("id", discordid);

                var selectResult = await selectCmd.ExecuteScalarAsync() as string;

                return Results.Json(new
                {
                    id = discordid,

                    name = ctx.User.Identity?.Name,

                    avatarUrl = ctx.User.FindFirst(
                        "urn:discord:avatar:url")?.Value,

                    role = selectResult
                });
            });
            app.Start();
        }
    }
}
