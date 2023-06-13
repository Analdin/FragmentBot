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
        private string token = "6227438127:AAEZ3SxWcxYbvqlBer3_PUE0uHuzIpg-eFo";
        private TelegramBotClient bot;
        private long chatId = -1001957631663;
        public Random rnd = new Random();

        public async Task BotStart(string TgName, string bid, string auctionEnd, string bidAddr, string linkUrl, string aucSt)
        {
            bot = new TelegramBotClient(token);

            try
            {
                await bot.SendTextMessageAsync(
                    chatId: new ChatId(chatId),
                    text: $"Auction started: {aucSt} Username: {TgName} \n Current bid: {bid} \n Wallet: {bidAddr} \n\n Auction will end: {auctionEnd}",
                    //parseMode: ParseMode.MarkdownV2,
                    disableNotification: true,
                    replyMarkup: new InlineKeyboardMarkup(
                            InlineKeyboardButton.WithUrl(
                                    text: "Ссылка на кабинет",
                                    url: linkUrl))
                    );
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка отправки сообщения в ТГ: " + ex.Message);

                if(ex.Message.Contains("Too Many Requests"))
                {
                    Console.WriteLine("Много запросов, пауза 50 сек.");
                    Thread.Sleep(rnd.Next(50000, 60000));
                }
            }
        }
    }
}
