﻿using System;
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

        public async void BotStart(string TgName, string bid, string auctionEnd, string bidAddr)
        {
            bot = new TelegramBotClient(token);

            try
            {
                await bot.SendTextMessageAsync(
                    chatId: new ChatId(chatId),
                    text: $"Имя в ТГ: {TgName} \n Ставка: {bid} \n Время окончания аукциона: {auctionEnd} \n bid Адрес: {bidAddr}",
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
                Console.WriteLine("Ошибка отправки сообщения в ТГ: " + ex.Message);
            }
        }
    }
}
