using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace CwVanDerMeij.Telegram.Bot.AndressBanenes.DAL
{
    public partial class AndressBanenesBotContext : DbContext
    {
        public virtual DbSet<Chat> Chat { get; set; }
        public virtual DbSet<Message> Message { get; set; }
        public virtual DbSet<User> User { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(msConnectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Chat>(entity =>
            {
                entity.ToTable("chat");

                entity.HasIndex(e => e.TelegramChatId)
                    .HasName("UQ_telegram_chat_id")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.TelegramChatId).HasColumnName("telegram_chat_id");

                entity.Property(e => e.Title)
                    .HasColumnName("title")
                    .HasMaxLength(255);

                entity.Property(e => e.Type)
                    .HasColumnName("type")
                    .HasMaxLength(255);

                entity.Property(e => e.Username)
                    .HasColumnName("username")
                    .HasMaxLength(255);
            });

            modelBuilder.Entity<Message>(entity =>
            {
                entity.ToTable("message");

                entity.HasIndex(e => new { e.TelegramMessageId, e.ChatId })
                    .HasName("UQ_telegram_message_id__chat_id")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ChatId).HasColumnName("chat_id");

                entity.Property(e => e.ReplyToMessageId).HasColumnName("reply_to_message_id");

                entity.Property(e => e.TelegramMessageId).HasColumnName("telegram_message_id");

                entity.Property(e => e.Text)
                    .IsRequired()
                    .HasColumnName("text");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.Chat)
                    .WithMany(p => p.Message)
                    .HasForeignKey(d => d.ChatId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_message__chat_id__chat__id");

                entity.HasOne(d => d.ReplyToMessage)
                    .WithMany(p => p.InverseReplyToMessage)
                    .HasForeignKey(d => d.ReplyToMessageId)
                    .HasConstraintName("FK_message__reply_to_message_id__message_id");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Message)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_message__user_id__chat__id");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("user");

                entity.HasIndex(e => e.TelegramUserId)
                    .HasName("UQ_telegram_user_id")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasColumnName("first_name")
                    .HasMaxLength(255);

                entity.Property(e => e.LastName)
                    .HasColumnName("last_name")
                    .HasMaxLength(255);

                entity.Property(e => e.TelegramUserId).HasColumnName("telegram_user_id");

                entity.Property(e => e.Username)
                    .HasColumnName("username")
                    .HasMaxLength(255);
            });
        }
    }
}