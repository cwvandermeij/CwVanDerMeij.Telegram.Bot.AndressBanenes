using System;
using System.Collections.Generic;

namespace CwVanDerMeij.Telegram.Bot.AndressBanenes.DAL
{
    public partial class Message
    {
        public long Id { get; set; }
        public long ChatId { get; set; }
        public long UserId { get; set; }
        public long? ReplyToMessageId { get; set; }
        public int TelegramMessageId { get; set; }
        public string Text { get; set; }

        public virtual Chat Chat { get; set; }
        public virtual Message ReplyToMessage { get; set; }
        public virtual ICollection<Message> InverseReplyToMessage { get; set; }
        public virtual User User { get; set; }
    }
}
