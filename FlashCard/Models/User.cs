using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FlashCard.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required, StringLength(100, MinimumLength = 2)]
        public string Username { get; set; } = string.Empty;

        [Required, EmailAddress, StringLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required, StringLength(255)]
        public string Password { get; set; } = string.Empty;


        // Quan hệ
        public ICollection<Deck> Decks { get; set; } = new List<Deck>();
        public ICollection<Flashcard> Flashcards { get; set; } = new List<Flashcard>();
        public ICollection<Progress> Progresses { get; set; } = new List<Progress>();
    }
}
