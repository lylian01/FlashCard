using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlashCard.Models
{
    public class Progress
    {
        [Key]
        public int ProgressId { get; set; }

        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public int FlashcardId { get; set; }
        public Flashcard Flashcard { get; set; } = null!;

        public bool IsKnown { get; set; } = false;
        public bool IsAvailable { get; set; } = true;  // <-- đã xóa thì false 

    }
}
