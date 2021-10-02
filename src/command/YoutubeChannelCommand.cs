using Discord.Commands;
using OthelloBot.src.db;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace OthelloBot.src.command
{
    public class YoutubeChannelCommand : ModuleBase<ShardedCommandContext>
    {
        [Command("앚튜브채널")]
        public async Task YoutubeChannel(params string[] args)
        {
            if (args.Length > 0 && args[0].Contains("취소"))
            {
                if (DB.DeleteChannel("youtube") > 0)
                {
                    await ReplyAsync("더 이상 앚튜브 알림을 받지 않습니다.");
                }
                else
                {
                    await ReplyAsync("채널 등록을 취소하지 못했습니다.");
                }

                return;
            }

            if (DB.UpdateChannel(Context.Channel.Id, "youtube") > 0)
            {
                await ReplyAsync($"<#{Context.Channel.Id}>에서 앚튜브 알림을 받습니다.");
            }
            else
            {
                await ReplyAsync("채널을 등록하지 못했습니다.");
            }
        }
    }
}
