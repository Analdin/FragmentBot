using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace FragmentBot.Requests
{
    public class TelegramAlert
    {
        private static string token = "6227438127:AAEZ3SxWcxYbvqlBer3_PUE0uHuzIpg-eFo";
        private static TelegramBotClient bot;
        private static long chatId = -1001957631663;

        public static async void BotStart(string TgName, string bid, string auctionEnd, string bidAddr)
        {
            bot = new TelegramBotClient(token);
            TeleJob(TgName, bid, auctionEnd, bidAddr);
        }
        public static async void TeleJob(string TgName, string bid, string auctionEnd, string bidAddr)
        {
            try
            {
                GetMessage(TgName, bid, auctionEnd, bidAddr);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Сообщение телеграм: " + ex.Message);
            }
        }
        static async Task GetMessage(string TgName, string bid, string auctionEnd, string bidAddr)
        {
            try
            {
                await bot.SendTextMessageAsync(
                    chatId: new ChatId(chatId),
                    text: $"Имя в ТГ: {TgName}\nСтавка: {bid}\nВремя окончания аукциона: {auctionEnd}\nbid Адрес: {bidAddr}",
                    parseMode: ParseMode.MarkdownV2,
                    disableNotification: true,
                    replyMarkup: new InlineKeyboardMarkup(
                            InlineKeyboardButton.WithUrl(
                                    text: "Ссылка на кабинет",
                                    url: "https://core.telegram.org/bots/api#sendmessage"))
                    );
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetMessage: " + ex.Message);
            }
        }
    }
}
