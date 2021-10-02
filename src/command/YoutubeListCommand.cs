using Discord.Commands;
using OthelloBot.src.db;
using OthelloBot.src.embed;
using System.Data;

namespace OthelloBot.src.command
{
    public class YoutubeListCommand : ModuleBase<ShardedCommandContext>
    {
        [Command("앚튜브목록")]
        public async Task YoutubeList(params string[] args)
        {
            DataRowCollection rows;

            try
            {
                rows = DB.GetYoutubeChannels();

                if (rows.Count == 0)
                {
                    await ReplyAsync("구독 중인 앚튜버가 없습니다.");
                    return;
                }
            }
            catch
            {
                await ReplyAsync("목록을 불러오는데 실패 하였습니다.");
                return;
            }

            YoutubeListEmbed embed = new(rows);
            await ReplyAsync(embed: embed.Build());
        }
    }
}
