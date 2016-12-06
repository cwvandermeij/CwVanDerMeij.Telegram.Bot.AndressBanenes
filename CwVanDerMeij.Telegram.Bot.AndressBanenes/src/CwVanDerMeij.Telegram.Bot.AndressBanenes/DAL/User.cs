using System;
using System.Collections.Generic;

namespace CwVanDerMeij.Telegram.Bot.AndressBanenes.DAL
{
    public partial class User
    {
        public User()
        {
            Message = new HashSet<Message>();
        }

        public long Id { get; set; }
        public int TelegramUserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }

        public virtual ICollection<Message> Message { get; set; }
    }
}
