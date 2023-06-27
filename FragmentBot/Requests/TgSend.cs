using System;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
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

        public async Task BotStart(string newBid, string TgName, string bid, string auctionEnd, string bidAddr, string linkUrl, string aucSt)
        {
            bot = new TelegramBotClient(token);

            try
            {
                Conditon set = new Conditon();
                set.BidUp = true;

                bool is404 = CheckUrlFor404(linkUrl);
                if (is404)
                {
                    Console.WriteLine("Страница не найдена (ошибка 404).");
                }
                else
                {
                    if (set.BidUp == true)
                    {
                        await bot.SendTextMessageAsync(
                        chatId: new ChatId(chatId),
                        text: $"A new bid was made on the \n Username: {TgName} \n Current bid: {bid} \n Wallet: {bidAddr} \n\n Auction will end: {auctionEnd}",
                        //parseMode: ParseMode.MarkdownV2,
                        disableNotification: true,
                        replyMarkup: new InlineKeyboardMarkup(
                                InlineKeyboardButton.WithUrl(
                                        text: "Ссылка на кабинет",
                                        url: linkUrl))
                        );
                    }
                    else
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
                }
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

        public async Task TonInToTG(string tonDom, string tonBid, string tonDate, DateTime nowDate, string url)
        {
            bot = new TelegramBotClient(token);
            string modifiedUrl = string.Empty;

            int lastIndex = url.LastIndexOf(".ton");
            if(lastIndex >= 0)
            {
                modifiedUrl = url.Remove(lastIndex, "ton".Length);
            }

            // Проверка url на доступность 404
            bool is404 = CheckUrlFor404(url);
            if (is404)
            {
                Console.WriteLine("Страница не найдена (ошибка 404).");
            }
            else
            {
                Console.WriteLine($"Страница найдена. - {modifiedUrl}");
                await bot.SendTextMessageAsync(
                chatId: new ChatId(chatId),
                text: $"A new bid was made on the \n Ton Domain: {tonDom} \n Current bid: {tonBid} TON \n Date: {nowDate} \n\n Auction will end: {tonDate}",
                        //parseMode: ParseMode.MarkdownV2,
                        disableNotification: true,
                        replyMarkup: new InlineKeyboardMarkup(
                                InlineKeyboardButton.WithUrl(
                                        text: "To personal->",
                                        url: modifiedUrl))
                        );
            }
        }

        public async Task FragmentSold(string num, string ton, string wallet, string time, string soldurl)
        {
            bot = new TelegramBotClient(token);

            // Проверка url на доступность 404
            bool is404 = CheckUrlFor404(soldurl);
            if (is404)
            {
                Console.WriteLine("Страница не найдена (ошибка 404).");
            }
            else
            {
                Console.WriteLine("Страница найдена.");
                await bot.SendTextMessageAsync(
                chatId: new ChatId(chatId),
                text: $"Sold \n Number: {num} \n {ton} TON \n Wallet: {wallet} \n\n Purchased on: {time}",
                        //parseMode: ParseMode.MarkdownV2,
                        disableNotification: true,
                        replyMarkup: new InlineKeyboardMarkup(
                                InlineKeyboardButton.WithUrl(
                text: "To personal->",
                                    url: soldurl))
                    );
            }
        }
        public bool CheckUrlFor404(string url)
        {
            try
            {
                // Создаем объект для выполнения GET-запроса
                var request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";

                // Получаем ответ от сервера
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    // Проверяем статусный код ответа
                    if (response.StatusCode == HttpStatusCode.NotFound)
                    {
                        // Страница не найдена (ошибка 404)
                        return true;
                    }
                    else
                    {
                        // Страница найдена
                        return false;
                    }
                }
            }
            catch (WebException ex)
            {
                // Обработка ошибок подключения или других проблем
                Console.WriteLine($"Ошибка при проверке URL: {ex.Message}");
                return false;
            }
        }
    }
    public class Conditon
    {
        public bool BidUp;
        public void SetBid(bool bid)
        {
            BidUp = bid;
        }
    }
}
