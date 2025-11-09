using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace FlashCard.Models
{
    public class FlashcardDbContext : DbContext
    {
        public FlashcardDbContext(DbContextOptions<FlashcardDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Deck> Decks { get; set; }
        public DbSet<Flashcard> Flashcards { get; set; }
        public DbSet<CardPair> CardPairs { get; set; }
        public DbSet<Progress> Progresses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // gọi base để EF làm config mặc định
            base.OnModelCreating(modelBuilder);

            // Quan hệ User – Deck (1-n)
            modelBuilder.Entity<Deck>()
                .HasOne(d => d.User)
                .WithMany(u => u.Decks)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade); // Xóa user → xóa luôn deck của user đó

            // Quan hệ User – Flashcard (1-n)
            modelBuilder.Entity<Flashcard>()
                .HasOne(f => f.User)
                .WithMany(u => u.Flashcards)
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Cascade); // Xóa user → xóa flashcard của user đó

            // Quan hệ User – Progress (1-n)
            modelBuilder.Entity<Progress>()
                .HasOne(p => p.User)
                .WithMany(u => u.Progresses)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade); // Xóa user → xóa luôn progress của user đó

            // Quan hệ Deck – Flashcard (1-n)
            modelBuilder.Entity<Flashcard>()
                .HasOne(f => f.Deck)
                .WithMany(d => d.Flashcards)
                .HasForeignKey(f => f.DeckId)
                .OnDelete(DeleteBehavior.NoAction); // Xóa deck → không xóa flashcard

            // Quan hệ Flashcard – Progress (1-n)
            modelBuilder.Entity<Progress>()
                .HasOne(p => p.Flashcard)
                .WithMany()
                .HasForeignKey(p => p.FlashcardId)
                .OnDelete(DeleteBehavior.NoAction); // Xóa flashcard → chỉ set isAvailable = false bằng code
        }

    }
}
