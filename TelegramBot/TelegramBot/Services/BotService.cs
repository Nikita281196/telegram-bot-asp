using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.Services.Traiders;
using TelegramBot.Services.Users;
using Telegram.Bot.Types.Enums;
using Newtonsoft.Json;

namespace TelegramBot.Services
{
    public class BotService
    {
        private static ITelegramBotClient _bot;
        private static ITraiderService _traiderService;
        private static IUserService _userService;

        private const string START_MENU_ITEM = "/start";
        private const string TRAIDER_LIST_MENU_ITEM = "Рейтинг трейдеров";
        private const string SUBSCIBE_ON_TRAIDERS_MENU_ITEM = "Подписка на трейдеров";
        private const string GET_PREMIUM_MENU_ITEM = "Оформить премиум подписку";
        private const string INSTRUCTIONS_FOR_TRADING_MENU_ITEM = "Как торговать";
        public BotService(string token)
        {
            _bot = new TelegramBotClient(token);
            _traiderService = new TraidersService();
            _userService = new UsersService();
        }

        public static void Start()
        {
            var cts = new CancellationTokenSource();
            var cancellationToken = cts.Token;
            var receiverOptions = new ReceiverOptions
            {
                ThrowPendingUpdates = true,
                AllowedUpdates = { }, // receive all update types
            };
            _bot.StartReceiving(
                HandleUpdateAsync,
                HandleErrorAsync,
                receiverOptions,
                cancellationToken
            );
            Console.WriteLine("Запущен бот " + _bot.GetMeAsync().Result.FirstName);
            Console.ReadLine();
        }

        private static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            Console.WriteLine(JsonConvert.SerializeObject(update));

            await MessageProcessingHandler(botClient, update, cancellationToken).ConfigureAwait(false);
        }

        private static async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            Console.WriteLine(JsonConvert.SerializeObject(exception));
        }

        private static async Task MessageProcessingHandler(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            var menu = GetButtons();

            switch (update.Type)
            {
                case UpdateType.Message:
                    var answer = new StringBuilder();
                    switch (update.Message.Text)
                    {
                        case START_MENU_ITEM:
                            _userService.AddUser(new Models.User
                            {
                                Id = update.Message.From.Id,
                                FirstName = update.Message.From.FirstName,
                                LastName = update.Message.From.LastName
                            });
                            await botClient.SendTextMessageAsync(update.Message.Chat, "Добро пожаловать на борт, добрый путник!", replyMarkup: menu, cancellationToken: cancellationToken);
                            break;
                        case TRAIDER_LIST_MENU_ITEM:
                            var traidersList = GetTraidersList();
                            await botClient.SendTextMessageAsync(update.Message.Chat, traidersList, replyMarkup: menu, cancellationToken: cancellationToken);
                            break;
                        case SUBSCIBE_ON_TRAIDERS_MENU_ITEM:
                            answer.AppendLine("Подписка на трейдеров");
                            var callBackMenu = GetCallBackMenu();
                            await botClient.SendTextMessageAsync(update.Message.Chat, answer.ToString(), replyMarkup: callBackMenu, cancellationToken: cancellationToken);
                            break;
                        case GET_PREMIUM_MENU_ITEM:
                            answer.AppendLine("Для активации premium доступа, укажите адрес своей электронной почты:");
                            await botClient.SendTextMessageAsync(update.Message.Chat, answer.ToString(), replyMarkup: menu, cancellationToken: cancellationToken);
                            break;
                        case INSTRUCTIONS_FOR_TRADING_MENU_ITEM:
                            answer.AppendLine("Как торговать:");
                            answer.AppendLine("Чтобы было норм, на все хватало, в минус не уходить, ушел в минус - лох");
                            await botClient.SendTextMessageAsync(update.Message.Chat, answer.ToString(), replyMarkup: menu, cancellationToken: cancellationToken);
                            break;
                        default:
                            if (update.Message.Text.Contains('@') && update.Message.Text.Contains('.'))
                            {
                                _userService.SetPremium(update.Message.From.Id);
                                answer.AppendLine("Premium доступ к боту активирован. Теперь вы можете подписываться на сигналы сразу нескольких трейдеров");
                            }
                            else
                            {
                                answer.AppendLine("Неверный формат почты");
                                
                            }
                            await botClient.SendTextMessageAsync(update.Message.Chat, answer.ToString(), replyMarkup: menu, cancellationToken: cancellationToken);
                            break;
                    }
                    break;
                case UpdateType.CallbackQuery:
                    var traider = _traiderService.GetTraiderById(int.Parse(update.CallbackQuery.Data));
                    var user = _userService.GetById(update.CallbackQuery.From.Id);

                    if (user.TraiderIds.Contains(traider.Id))
                    {
                        _userService.Unsubscribe(user.Id, traider.Id);
                    }
                    else
                    {
                        _userService.Subscribe(user.Id, traider.Id);
                    }

                    var newCallbackMenu = UpdateCallBackMenu(traider.Id, user.Id);
                    botClient.EditMessageReplyMarkupAsync(update.CallbackQuery.Message.Chat, update.CallbackQuery.Message.MessageId, newCallbackMenu);
                    botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id, $"Подписка на {traider.ShortName} оформлена", false);
                    break;
            }
        }

        private static ReplyKeyboardMarkup GetButtons()
        {
            return new ReplyKeyboardMarkup(new List<List<KeyboardButton>>()
            {
                new List<KeyboardButton> { TRAIDER_LIST_MENU_ITEM, SUBSCIBE_ON_TRAIDERS_MENU_ITEM },
                new List<KeyboardButton> { GET_PREMIUM_MENU_ITEM, INSTRUCTIONS_FOR_TRADING_MENU_ITEM }
            })
            {
                ResizeKeyboard = true
            };
        }
        private static InlineKeyboardMarkup UpdateCallBackMenu(int traiderId, long userId)
        {
            var traiders = _traiderService.GetTraiders();
            var user = _userService.GetById(userId);
            var traiderButtons = new List<List<InlineKeyboardButton>>();

            foreach (var traider in traiders)
            {
                if (user.TraiderIds.Contains(traider.Id))
                {
                    traiderButtons.Add(new List<InlineKeyboardButton>()
                    {
                        InlineKeyboardButton.WithCallbackData($"Отписаться от {traider.ShortName}", $"{traider.Id}")
                    });
                }
                else
                {
                    traiderButtons.Add(new List<InlineKeyboardButton>()
                    {
                        InlineKeyboardButton.WithCallbackData($"Подписаться на {traider.ShortName}", $"{traider.Id}")
                    });
                }
            }

            var ikm = new InlineKeyboardMarkup(traiderButtons);
            return ikm;
        }
        private static InlineKeyboardMarkup GetCallBackMenu()
        {
            var traiders = _traiderService.GetTraiders();
            var traiderButtons = new List<List<InlineKeyboardButton>>();
            foreach (var traider in traiders) 
            {
                traiderButtons.Add(new List<InlineKeyboardButton>() 
                {
                    InlineKeyboardButton.WithCallbackData($"Подписаться на {traider.ShortName}", $"{traider.Id}")
                });
            }

            var ikm = new InlineKeyboardMarkup(traiderButtons);
            return ikm;
        }

        private static string GetTraidersList()
        {
            var traiders = _traiderService.GetTraiders();
            var traidersList = new StringBuilder("Список трейдеров:");
            traidersList.AppendLine();
            traidersList.AppendLine();
            foreach (var traider in traiders)
            {
                traidersList.Append($"{traider.ShortName} рейтинг: {traider.Rating} \n");
            }
            return traidersList.ToString();
        }
    }
}
