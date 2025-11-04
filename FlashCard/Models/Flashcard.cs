using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlashCard.Models
{
    public class Flashcard
    {
        [Key]
        public int CardId { get; set; }
        public string FrontFlash { get; set; } = string.Empty;
        public string BackFlash { get; set; } = string.Empty;

        public int UserId { get; set; }
        public User User { get; set; } = null!;

        // Deck có thể null
        public int? DeckId { get; set; }
        public Deck? Deck { get; set; }


        // Quan hệ ngược lại với Progress
        public ICollection<Progress> Progresses { get; set; } = new List<Progress>();


    }
}
