using CwVanDerMeij.Telegram.Bot.AndressBanenes.DAL;
using CwVanDerMeij.Telegram.Bot.Api;
using CwVanDerMeij.Telegram.Bot.Api.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CwVanDerMeij.Telegram.Bot.AndressBanenes
{
    public class AndressBanenesBot
    {
        private BotApi moBotApi;
        private readonly string msConnectionString;

        public AndressBanenesBot(string psBotToken, string psConnectionString)
        {
            moBotApi = new BotApi(psBotToken);
            msConnectionString = psConnectionString;
        }

        public async void HandleUpdate(Update poUpdate)
        {
            //If the update contains a message, store it in the database
            if (poUpdate.Message != null)
            {
                StoreMessage(poUpdate.Message);
            }

            //If the update contains an edited message
            if (poUpdate.EditedMessage != null)
            {
                DAL.Message loOriginalStoredMessage = GetStoredMessage(poUpdate.EditedMessage);

                //If the original message exists in the database
                if (loOriginalStoredMessage != null)
                {
                    await moBotApi.SendMessage(loOriginalStoredMessage.Chat.Id, $@"
Edit detected by user {poUpdate.EditedMessage.From.FirstName}:

{loOriginalStoredMessage.Text}

Was changes into:

{poUpdate.EditedMessage.Text}
                    ", null, null, null, null, null);

                    UpdateStoredMessage(poUpdate.EditedMessage);
                }
            }
        }

        private void UpdateStoredMessage(Api.Models.Message poMessage)
        {
            using (AndressBanenesBotContext loContext = new AndressBanenesBotContext(msConnectionString))
            {
                DAL.Message loStoredMessage = loContext.Message
                    .FirstOrDefault(x => 
                        x.Chat.TelegramChatId == poMessage.Chat.Id 
                        && x.TelegramMessageId == poMessage.MessageId);

                loStoredMessage.Text = poMessage.Text;

                loContext.SaveChanges();
            }
        }

        private DAL.Message GetStoredMessage(Api.Models.Message poMessage)
        {
            using (AndressBanenesBotContext loContext = new AndressBanenesBotContext(msConnectionString))
            {
                return loContext.Message
                    .Include(x => x.Chat)
                    .Include(x => x.User)
                    .FirstOrDefault(x =>
                        x.Chat.TelegramChatId == poMessage.Chat.Id
                        && x.TelegramMessageId == poMessage.MessageId);
            }
        }

        private void StoreMessage(Api.Models.Message poMessage)
        {
            using (AndressBanenesBotContext loContext = new AndressBanenesBotContext(msConnectionString))
            {
                DAL.Chat loChat = loContext.Chat.FirstOrDefault(x => x.TelegramChatId == poMessage.Chat.Id);
                DAL.User loUser = loContext.User.FirstOrDefault(x => x.TelegramUserId == poMessage.From.Id);
                DAL.Message loReplyToMessage = null;

                //If either the chat or the user is new
                if (loChat == null || loUser == null)
                {
                    //If the chat is new, create it and then add it to the context
                    if (loChat == null)
                    {
                        loChat = new DAL.Chat()
                        {
                            TelegramChatId = poMessage.Chat.Id,
                            Username = poMessage.Chat.Username,
                            Title = poMessage.Chat.Title,
                            Type = Enum.GetName(poMessage.Chat.Type.GetType(), poMessage.Chat.Type)
                        };

                        loContext.Chat.Add(loChat);
                    }
                    
                    //If the user is new, create it and then add it to the context
                    if (loUser == null)
                    {
                        loUser = new DAL.User()
                        {
                            TelegramUserId = poMessage.From.Id,
                            FirstName = poMessage.From.FirstName,
                            LastName = poMessage.From.LastName,
                            Username = poMessage.From.Username
                        };

                        loContext.User.Add(loUser);
                    }
                    
                    //Save the changes to the context so the IDs get filled
                    loContext.SaveChanges();
                }

                //If the message is a reply to another message, try getting the original message from the database if it exists in there
                if (poMessage.ReplyToMessage != null)
                {
                    loReplyToMessage = loContext.Message.FirstOrDefault(x => x.TelegramMessageId == poMessage.ReplyToMessage.MessageId && x.ChatId == loChat.Id);
                }

                //Create the new Message object
                DAL.Message loMessage = new DAL.Message()
                {
                    ChatId = loChat.Id,
                    UserId = loUser.Id,
                    ReplyToMessageId = loReplyToMessage?.Id,
                    TelegramMessageId = poMessage.MessageId,
                    Text = poMessage.Text
                };
                
                loContext.Message.Add(loMessage);

                loContext.SaveChanges();
            }
        }
    }
}
