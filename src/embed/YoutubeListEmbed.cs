using Discord;
using System.Data;

namespace OthelloBot.src.embed
{
    internal class YoutubeListEmbed : EmbedBuilder
    {
        public YoutubeListEmbed(DataRowCollection rows)
        {
            WithColor(new Color(0xE84F97));

            foreach (DataRow row in rows)
            {
                AddField($"{row["name"]}", $"https://www.youtube.com/channel/{row["id"]}");
            }
        }
    }
}
