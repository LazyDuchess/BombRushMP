using BombRushMP.Server;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public void Start()
        {
            var builder = WebApplication.CreateBuilder();

            builder.WebHost.UseUrls("http://127.0.0.1:5000");

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy
                        .AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
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

            app.UseCors("AllowAll");

            app.MapGet("/api/players", GetPlayers);

            app.Start();
        }
    }
}
