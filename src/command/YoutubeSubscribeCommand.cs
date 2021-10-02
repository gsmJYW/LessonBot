using Discord.Commands;
using OthelloBot.src.db;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using IzoneBot.src.model;
using System;
using System.Text;

namespace OthelloBot.src.command
{
    public class YoutubeSubscribeCommand : ModuleBase<ShardedCommandContext>
    {
        [Command("앚튜브구독")]
        public async Task YoutubeSubscribe(params string[] args)
        {
            if (args.Length <= 0)
            {
                await ReplyAsync($"사용법: `{Program.prefix}앚튜브채널 [채널 링크]` `{Program.prefix}앚튜브채널 [채널 링크] 취소`");
                return;
            }
            else
            {
                if (args.Length >= 2 && args[1].Contains("취소"))
                {
                    try
                    {
                        string channel_id = args[0][^24..];

                        if (DB.DeleteYoutubeChannel(channel_id) <= 0)
                        {
                            throw new Exception();
                        }

                        await ReplyAsync("구독 취소 하였습니다.");
                    }
                    catch
                    {
                        await ReplyAsync("구독 취소하지 못 했습니다.");
                    }

                    return;
                }

                try
                {
                    string channel_id = args[0][^24..];

                    var response = await new HttpClient().GetAsync($"https://www.googleapis.com/youtube/v3/channels?part=snippet&id={channel_id}&key={Program.youtube_api_key}");
                    var json = await response.Content.ReadAsStringAsync();

                    var channel = JsonConvert.DeserializeObject<Channel>(json);

                    StringBuilder playlistID = new(channel_id);
                    playlistID[1] = 'U';

                    response = await new HttpClient().GetAsync($"https://www.googleapis.com/youtube/v3/playlistItems?part=snippet&playlistId={playlistID}&key={Program.youtube_api_key}");
                    json = await response.Content.ReadAsStringAsync();

                    Console.WriteLine(json);

                    long last_upload;

                    try
                    {
                        var playlistItems = JsonConvert.DeserializeObject<PlaylistItems>(json);
                        last_upload = playlistItems.items[0].snippet.publishedAt.Ticks;
                    }
                    catch
                    {
                        last_upload = 0;
                    }

                    if (DB.UpdateYoutubeChannel(channel_id, last_upload) <= 0)
                    {
                        throw new Exception();
                    }

                    await ReplyAsync($"**{channel.items[0].snippet.title}** 채널을 구독했습니다.");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    await ReplyAsync($"채널을 구독하지 못했습니다.");
                }
            }
        }
    }
}
