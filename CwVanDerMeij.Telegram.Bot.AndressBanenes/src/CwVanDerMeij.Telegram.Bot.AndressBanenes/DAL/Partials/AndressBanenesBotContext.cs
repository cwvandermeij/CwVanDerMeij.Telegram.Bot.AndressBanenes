using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace CwVanDerMeij.Telegram.Bot.AndressBanenes.DAL
{
    public partial class AndressBanenesBotContext : DbContext
    {
        private readonly string msConnectionString;

        public AndressBanenesBotContext(string psConnectionString)
        {
            msConnectionString = psConnectionString;
        }
    }
}
