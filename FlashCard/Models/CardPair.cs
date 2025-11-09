using System.ComponentModel.DataAnnotations;

namespace FlashCard.Models
{
    public class CardPair
    {
        [Key]
        public int PairId { get; set; }
        public string FrontCard { get; set; } = string.Empty;
        public string BackCard { get; set; } = string.Empty;

        public int FlashcardId { get; set; } // Khóa ngoại
        public Flashcard? Flashcard { get; set; }
    }

}
