using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlashCard.Models
{
    public class Deck
    {
        [Key]
        public int DeckId { get; set; }
        public string DeckName { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;

        public int UserId { get; set; }
        public User? User { get; set; }

        public ICollection<Flashcard> Flashcards { get; set; } = new List<Flashcard>();
    }
}
