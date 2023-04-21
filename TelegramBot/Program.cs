using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
//using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using LoopedMachineLib;
using FiniteStatetEntropyCodingLib;
//using Telegram.Bots.Types;

namespace TelegramBotExperiments
{

    class Program
    {
        static string cur_command = "/start";
        static ITelegramBotClient bot = new TelegramBotClient(System.IO.File.ReadAllText("token2.txt"));
        public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            // Некоторые действия
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(update));
            if (update.Type == Telegram.Bot.Types.Enums.UpdateType.Message)
            {
                var message = update.Message;

                if (message.Text.ToLower() == "/start")
                {
                    await botClient.SendTextMessageAsync(message.Chat, "Привет!\n" + "Вот список команд :\n" + " /check_machine - проверить, зацикливается ли машина Тьюринга\r\n/fse_coding - энтропийное сжатие строки\r\n/fse_decoding - расжатие строки");
                    return;
                    
                }

                if (message.Text.ToLower() == "/check_machine" || message.Text.ToLower() == "/fse_coding" || message.Text.ToLower() == "/fse_decoding")
                {
                    cur_command = message.Text.ToLower();
                }

               
                if (cur_command == "/check_machine")
                {
                    (bool fl1, string error_mes1) = ParseInput.CheckInput(message.Text);
                    if (message.Text.ToLower() == "/check_machine")
                    {
                        await botClient.SendTextMessageAsync(message.Chat, "*Введите данные о машине Тьюринга в виде :* \n" +
                       "Q - _количество состояний машины_ \n" +
                       "m - _количество строк с описанием машины, после этого следует m строк вида_ : \n" +
                       "q1 a  -> q1 b +1 \n" +
                       "q2 b  -> q1 a +1 \n" +
                       "............................. \n" +
                       "q10 ^  -> q1 a -1 \n" +
                       "a1a2 ... - _входные данные на ленте (без пробелов)_ \n" +
                       "n - _длина ленты_", Telegram.Bot.Types.Enums.ParseMode.Markdown);
                    }
                    else if (fl1)
                    {
                        var res = CheckMachine(message.Text);

                        if (res == 1)
                        {
                            await botClient.SendTextMessageAsync(message.Chat, "*Машина не зацикливается.*", Telegram.Bot.Types.Enums.ParseMode.Markdown);
                        }
                        else if (res == -1)
                        {
                            await botClient.SendTextMessageAsync(message.Chat, "*Машина зацикливается.*", Telegram.Bot.Types.Enums.ParseMode.Markdown);
                        }
                        else
                        {
                            await botClient.SendTextMessageAsync(message.Chat, "*Что-то пошло не так, повторите попытку.*", Telegram.Bot.Types.Enums.ParseMode.Markdown);
                        }

                    }
                    else if (!fl1)
                    {
                        await botClient.SendTextMessageAsync(message.Chat, $"*{error_mes1}*" + "\n" + "Повторите попытку.", Telegram.Bot.Types.Enums.ParseMode.Markdown);
                    }

                }
                
                else if (cur_command == "/fse_coding")
                {
                    (bool fl2, string error_mes2) = InputData.CheckInput1(message.Text);

                    if (message.Text.ToLower() == "/fse_coding")
                    {
                        await botClient.SendTextMessageAsync(message.Chat, "*Введите данные для сжатия в виде :* \n" +
                       "n - _количество cимволов в алфавите_ \n" +
                       "A:3 B:4 C:1 - _символы алфавита и их частоты (сумма частот - степень двойки)_ \n" +
                       "ABBCCA - _строка, которую необходимо сжать_", Telegram.Bot.Types.Enums.ParseMode.Markdown);
                    }
                    else if (fl2)
                    {
                        (int q, string text) = InputData.CodingProg(message.Text);
                        await botClient.SendTextMessageAsync(message.Chat, $"*Финальное состояние :* {q} \n" + $"*Битовая строка :* {text}", Telegram.Bot.Types.Enums.ParseMode.Markdown);
                        cur_command = "empty";
                       


                    }
                    else if (!fl2)
                    {
                        await botClient.SendTextMessageAsync(message.Chat, $"*{error_mes2}*" + "\n" + "Повторите попытку.", Telegram.Bot.Types.Enums.ParseMode.Markdown);
                    }
                }

                else if (cur_command == "/fse_decoding")
                {
                    (bool fl2, string error_mes2) = InputData.CheckInput2(message.Text);

                    if (message.Text.ToLower() == "/fse_decoding")
                    {
                        await botClient.SendTextMessageAsync(message.Chat, "*Введите данные для рассжатия в виде :* \n" +
                       "n - _количество cимволов в алфавите_ \n" +
                       "A:3 B:4 C:1 - _символы алфавита и их частоты (сумма частот - степень двойки)_ \n" +
                       "000100111 - _битовая строка, которую необходимо расжать_ \n" +
                       "q - _финальное состояние_", Telegram.Bot.Types.Enums.ParseMode.Markdown);
                        
                    }
                    else if (fl2)
                    {
                        string text = InputData.DecodingProg(message.Text);
                        await botClient.SendTextMessageAsync(message.Chat, $"*Разжатая строка :* {text}", Telegram.Bot.Types.Enums.ParseMode.Markdown);
                        cur_command = "empty";
                    }
                    else if (!fl2)
                    {
                        await botClient.SendTextMessageAsync(message.Chat, $"*{error_mes2}*" + "\n" + "Повторите попытку.", Telegram.Bot.Types.Enums.ParseMode.Markdown);
                    }
                }
                else if (cur_command == "empty")
                {

                }
                else
                {
                    Console.WriteLine(cur_command);
                    await botClient.SendTextMessageAsync(message.Chat, "О-о-у....");
                }

            }
        }

        public static async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            // Некоторые действия
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(exception));
        }

        static int CheckMachine(string s)
        {
            (int q, string[] tf, string text, int length) = ParseInput.Parse(s);
            var mch = new Machine(q, tf, length);

            (int nc, int np) = (1, 0);

            var res = mch.RunMachine(nc, text, np);

            return res;
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Запущен бот " + bot.GetMeAsync().Result.FirstName);

            var cts = new CancellationTokenSource();
            var cancellationToken = cts.Token;
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = { }, // receive all update types
            };
            bot.StartReceiving(
                HandleUpdateAsync,
                HandleErrorAsync,
                receiverOptions,
                cancellationToken
            );
            Console.ReadLine();
        }
    }
}