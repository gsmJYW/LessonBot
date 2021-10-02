using Discord;

namespace OthelloBot.src.embed
{
    internal class HelpEmbed : EmbedBuilder
    {
        public HelpEmbed(string avatarUrl)
        {
            WithColor(new Color(0xE84F97));
            WithThumbnailUrl(avatarUrl);

            AddField($"{Program.prefix}앚시채널", $"해당 명령어를 친 채널에 앚시 알림을 받습니다.\n이미 알림을 받고 있는 채널이 있으면 새로 대체합니다.\n취소를 원하시면 `{Program.prefix}앚시채널 취소`를 입력하세요.");
            AddField($"{Program.prefix}앚튜브채널", $"해당 명령어를 친 채널에 앚튜브 알림을 받습니다.\n이미 알림을 받고 있는 채널이 있으면 새로 대체합니다.\n취소를 원하시면 `{Program.prefix}앚튜브채널 취소`를 입력하세요.");
            AddField($"{Program.prefix}앚튜브구독 [채널 링크]", $"해당 채널 업로드 알림을 받습니다.\n취소를 원하시면 `{Program.prefix}앚튜브구독 [채널 링크] 취소`를 입력하세요.");
            AddField($"{Program.prefix}앚튜브목록", $"구독 중인 채널 목록을 받습니다.");
        }
    }
}
