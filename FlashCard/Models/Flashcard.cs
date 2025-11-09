using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlashCard.Models
{
    public class Flashcard
    {
        [Key]
        public int CardId { get; set; }
        public string CardTitle { get; set; } = string.Empty;
        public string? CardDescription { get; set; }
        public bool IsPublic { get; set; } = true;


        public int UserId { get; set; }
        public User? User { get; set; }

        // Deck có thể null
        public int? DeckId { get; set; }
        public Deck? Deck { get; set; }


        // Quan hệ ngược lại với Progress
        public ICollection<Progress> Progresses { get; set; } = new List<Progress>();
        // 1 flashcard có nhiều cặp thẻ cardPair
        public List<CardPair> CardPairs { get; set; } = new List<CardPair>();

    }
}
