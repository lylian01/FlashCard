using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using FlashCard.Models; 

[Route("api/[controller]")]
[ApiController]
public class ExportController : ControllerBase
{
    private readonly FlashcardDbContext _context;

    public ExportController(FlashcardDbContext context)
    {
        _context = context;
    }

    [HttpGet("mockapi-format")]
    public async Task<IActionResult> ExportForMockAPI()
    {
        try
        {
            var flashcards = await _context.Flashcards
                .Include(f => f.User)
                .Include(f => f.Deck)
                .Include(f => f.CardPairs)
                .Select(f => new
                {
                    id = f.CardId.ToString(),
                    cardTitle = f.CardTitle,
                    cardDescription = f.CardDescription ?? "",
                    isPublic = f.IsPublic,
                    userId = f.UserId.ToString(),
                    deckId = f.DeckId.HasValue ? f.DeckId.Value.ToString() : null,

                    userName = f.User != null ? f.User.Username : "Unknown",
                    userEmail = f.User != null ? f.User.Email : "",

                    deckName = f.Deck != null ? f.Deck.DeckName : "No Deck",

                    cardPairs = f.CardPairs.Select(cp => new
                    {
                        id = cp.PairId.ToString(),
                        frontCard = cp.FrontCard,
                        backCard = cp.BackCard,
                        flashcardId = cp.FlashcardId.ToString()
                    }).ToList()
                })
                .ToListAsync();

            var decks = await _context.Decks
                .Include(d => d.User)
                .Select(d => new
                {
                    id = d.DeckId.ToString(),
                    deckName = d.DeckName,
                    deckDescription = d.Description ?? "",
                    userId = d.UserId.ToString(),
                    userName = d.User != null ? d.User.Username : "Unknown"
                })
                .ToListAsync();

            var users = await _context.Users
                .Select(u => new
                {
                    id = u.UserId.ToString(),
                    username = u.Username,
                    email = u.Email,
                    password = u.Password
                })
                .ToListAsync();

            var result = new
            {
                flashcards,
                decks,
                users,
                stats = new
                {
                    totalFlashcards = flashcards.Count,
                    totalDecks = decks.Count,
                    totalUsers = users.Count,
                    publicFlashcards = flashcards.Count(f => f.isPublic),
                    exportedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                }
            };

            var options = new JsonSerializerOptions
            {
                WriteIndented = true, //JSON đẹp, nhiều dòng
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase // cardTitle thay vì CardTitle
            };

            return Content(JsonSerializer.Serialize(result, options), "application/json"); // Object → JSON string và Return JSON response
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message, stackTrace = ex.StackTrace });
        }
    }
}