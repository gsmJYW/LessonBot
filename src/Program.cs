using System;
using System.Collections.Generic;
using System.Data;
using System.Net.Http;
using System.Reflection;
using System.Text;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using IzoneBot.src.model;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using OthelloBot.src.db;
using Yort.Ntp;

namespace OthelloBot
{
    internal class Program
    { 
        internal static DiscordShardedClient _client;

        private CommandService _commands;
        private IServiceProvider _services;

        public static string prefix, youtube_api_key;
        private static string bot_token, server, port, database, uid, pwd;
        
        private static void Main(string[] args)
        {
            try
            {
                prefix = args[0];
                bot_token = args[1];
                youtube_api_key = args[2];

                server = args[3];
                port = args[4];
                database = args[5];
                uid = args[6];
                pwd = args[7];

                DB.SetConnStr(server, port, database, uid, pwd);
            }
            catch
            {
                Console.WriteLine(
                    @"프로그램 실행 시 다음 매개변수가 필요합니다:
                    [명령어 접두사] [봇 토큰] [유튜브 API 키] [MySQL Server] [MySQL Port] [MySQL Database] [MySQL UID] [MySQL PWD]"
                );
                return;
            }

            Task.Run(async () => await HourNotification());
            Task.Run(async () => await YoutubeNotification());

            new Program()
                .RunBotAsync()
                .GetAwaiter()
                .GetResult();
        }

        public static async Task HourNotification()
        {
            while (true)
            {
                Thread.Sleep((60 - DateTime.Now.Second) * 1000 + (1000 - DateTime.Now.Millisecond));

                RequestTimeResult time;

                while (true)
                {
                    try
                    {
                        time = await new NtpClient().RequestTimeAsync();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                        continue;
                    }
                    break;
                }
                    
                var hour = time.NtpTime.Hour + 9;
                var minute = time.NtpTime.Minute;

                hour %= 12;
                if (hour == 0)
                {
                    hour = 12;
                }

                var name = "";

                if (hour == 1 && minute == 11)
                {
                    name = "채연";
                }
                else if (hour == 2 && minute == 5)
                {
                    name = "민주";
                } 
                else if (hour == 3 && minute == 19)
                {
                    name = "사쿠라";
                }
                else if (hour == 6 && minute == 18)
                {
                    name = "나코";
                }
                else if (hour == 7 && minute == 5)
                {
                    name = "혜원";
                }
                else if (hour == 8 && minute == 1)
                {
                    name = "채원";
                }
                else if (hour == 8 && minute == 31)
                {
                    name = "원영";
                }
                else if (hour == 9 && minute == 1)
                {
                    name = "유진";
                }
                else if (hour == 9 && minute == 27)
                {
                    name = "은비";
                }
                else if (hour == 9 && minute == 29)
                {
                    name = "예나";
                }
                else if (hour == 10 && minute == 6)
                {
                    name = "히토미";
                }
                else if (hour == 10 && minute == 22)
                {
                    name = "유리";
                }
                else if (hour == 10 && minute == 34)
                {
                    name = "데뷔";
                }
                else if (hour == 12 && minute == 1)
                {
                    name = "앚";
                }

                if (name.Length > 0)
                {
                    try
                    {
                        var channel = DB.GetChannel("hour");
                        var channelId = ulong.Parse(channel["id"].ToString());
                        await (_client.GetChannel(channelId) as SocketTextChannel).SendMessageAsync($"지금은 {hour:00}시 {minute:00}분, **{name}시** 입니다!");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                }
            }
        }

        public static async Task YoutubeNotification()
        {
            while (true)
            {
                DataRowCollection rows;

                try
                {
                    rows = DB.GetYoutubeChannels();

                    if (rows.Count == 0)
                    {
                        throw new Exception();
                    }
                }
                catch
                {
                    Thread.Sleep((60 - DateTime.Now.Second) * 1000);
                    continue;
                }

                foreach (DataRow row in rows)
                {
                    Thread.Sleep(24 * 60 * 60 * 1000 / 10000);

                    try
                    {
                        StringBuilder playlistId = new(row["id"].ToString());
                        playlistId[1] = 'U';

                        var response = await new HttpClient().GetAsync($"https://www.googleapis.com/youtube/v3/playlistItems?part=snippet&playlistId={playlistId}&key={youtube_api_key}");
                        var json = await response.Content.ReadAsStringAsync();

                        var playlistItems = JsonConvert.DeserializeObject<PlaylistItems>(json);

                        List<long> tickList = new();

                        foreach (var item in playlistItems.items)
                        {
                            if (item.snippet.publishedAt.Ticks > long.Parse(row["last_upload"].ToString()))
                            {
                                var channel = DB.GetChannel("youtube");
                                var channelId = ulong.Parse(channel["id"].ToString());

                                await (_client.GetChannel(channelId) as SocketTextChannel).SendMessageAsync($"https://youtu.be/{item.snippet.resourceId.videoId}");
                                tickList.Add(item.snippet.publishedAt.Ticks);
                            }
                            else
                            {
                                break;
                            }
                        }

                        if (tickList.Count > 0 && DB.UpdateYoutubeChannel(row["id"].ToString(), tickList[0]) <= 0)
                        {
                            throw new Exception();
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                }
            }
        }

        public async Task RunBotAsync()
        {
            _client = new DiscordShardedClient();
            _commands = new CommandService();
            _services = new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton(_commands)
                .BuildServiceProvider();

            _client.Log += _client_Log;

            await RegisterCommandsAsync();
            await _client.LoginAsync(TokenType.Bot, bot_token);
            await _client.StartAsync();
            await _client.SetGameAsync($"{prefix}도움말");
            await Task.Delay(-1);
        }
        private static Task _client_Log(LogMessage arg)
        {
            Console.WriteLine(arg);
            return Task.CompletedTask;
        }

        public async Task RegisterCommandsAsync()
        {
            _client.MessageReceived += HandleCommandAsync;
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        }

        public async Task HandleCommandAsync(SocketMessage arg)
        {
            var message = arg as SocketUserMessage;
            var context = new ShardedCommandContext(_client, message);

            if (message == null || message.Author.IsBot || message.Channel is not SocketGuildChannel || !(message.Author as SocketGuildUser).GuildPermissions.Administrator)
            {
                return;
            }

            var argPos = 0;

            if (message.HasStringPrefix(prefix, ref argPos))
            {
                if (!(message.Author as SocketGuildUser).GuildPermissions.Administrator)
                {
                    Console.WriteLine("asdf");
                    await message.Channel.SendMessageAsync("**관리자**만 사용할 수 있습니다.");
                    return;
                }

                var result = await _commands.ExecuteAsync(context, argPos, _services);

                if (!result.IsSuccess)
                {
                    var err = result.ErrorReason;
                    Console.WriteLine(err);
                }
            }
        }
    }
}
