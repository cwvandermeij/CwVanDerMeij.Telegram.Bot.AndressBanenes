using System;
using System.Collections.Generic;

namespace CwVanDerMeij.Telegram.Bot.AndressBanenes.DAL
{
    public partial class Chat
    {
        public Chat()
        {
            Message = new HashSet<Message>();
        }

        public long Id { get; set; }
        public long TelegramChatId { get; set; }
        public string Username { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }

        public virtual ICollection<Message> Message { get; set; }
    }
}
