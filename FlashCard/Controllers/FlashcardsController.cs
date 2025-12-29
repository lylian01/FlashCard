using FlashCard.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.SqlServer.Server;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using X.PagedList;
using X.PagedList.Extensions;
using System.Net.WebSockets;

namespace FlashCard.Controllers
{
    [Authorize]
    public class FlashcardsController : Controller
    {
        private readonly FlashcardDbContext _context;

       public FlashcardsController(FlashcardDbContext context)
        {
            _context = context;
        }

        // GET: Flashcards
        public async Task<IActionResult> AdminIndex(int? page)
        {
            var pageNumber = page ?? 1;
            var pageSize = 10;
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (userEmail != "admin@gmail.com")
            {
                ModelState.AddModelError("", "Sai thông tin đăng nhập vào admin.");
                return RedirectToAction("Login", "Users");
            }
           
            var flashcardAll = _context.Flashcards
                .Include(f => f.Deck)
                .Include(f => f.CardPairs)
                .Include(f => f.User)
                .OrderBy(x=>x.CardId)
                .ToPagedList(pageNumber,pageSize);
            return View(flashcardAll);
        }
        public async Task<IActionResult> UserIndex()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdString == null)
                return RedirectToAction("Login", "Users");

            int userId = int.Parse(userIdString);

            var flashcardsByUser = await _context.Flashcards
                .Include(f => f.Deck)
                .Include(f => f.CardPairs)
                .Include(f => f.User)
                .Where(f => f.UserId == userId)
                .ToListAsync();

            return View(flashcardsByUser);
        }
        public async Task<IActionResult> Search(string? key)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var fcAllPublicNotUser = _context.Flashcards
                .Include(f => f.CardPairs)
                .Include(f => f.User)
                .Where(f => f.UserId != int.Parse(userIdString) && f.IsPublic == true).AsQueryable();
            var fcUser = _context.Flashcards
                .Include(f => f.CardPairs)
                .Include(f => f.User)
                .Where(f => f.UserId == int.Parse(userIdString)).AsQueryable();

            var flashcardByKey = fcAllPublicNotUser.Union(fcUser); 

            if (!string.IsNullOrEmpty(key))
            {
                flashcardByKey = flashcardByKey
                    .Where(s => s.CardTitle!.ToUpper().Contains(key.ToUpper()));
            }

            var result = await flashcardByKey.Take(20).ToListAsync();
            ViewData["UserId"] = User.FindFirstValue(ClaimTypes.NameIdentifier);

            return View(result);
        }

        [AllowAnonymous]
        // GET: Flashcards/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var flashcard = await _context.Flashcards
                .Include(f => f.Deck)
                .Include(f => f.User)
                .Include(f => f.CardPairs)
                .FirstOrDefaultAsync(m => m.CardId == id);
            if (flashcard == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = User.FindFirstValue(ClaimTypes.NameIdentifier);

            return View(flashcard);
        }

        // GET: Flashcards/Create
        public IActionResult Create()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ViewBag.UserId = userIdString;
            ViewData["DeckId"] = new SelectList(_context.Decks.Where(d => d.UserId == int.Parse(userIdString)), "DeckId", "DeckName");

            var flashcard = new Flashcard();
            flashcard.CardPairs.Add(new CardPair()); // nếu form có ít nhất 1 pair mặc định
            return View(flashcard);
        }

        // POST: Flashcards/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Flashcard flashcard)
        {
            if (ModelState.IsValid)
            {
                _context.Add(flashcard);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(UserIndex));
            }

            ViewData["DeckId"] = new SelectList(_context.Decks, "DeckId", "DeckId", flashcard.DeckId);
            ViewBag.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return View(flashcard);
        }

        // GET: Flashcards/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var flashcard = await _context.Flashcards
                            .Include(f => f.Deck)
                            .Include(f => f.CardPairs)
                            .FirstOrDefaultAsync(f => f.CardId == id);
            if (flashcard == null)
            {
                return NotFound();
            }
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int userId = int.Parse(userIdString);

            ViewData["DeckId"] = new SelectList(_context.Decks.Where(d => d.UserId == userId), "DeckId", "DeckName");
      
            return View(flashcard);
        }

        // POST: Flashcards/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Flashcard flashcard)
        {
            if (id != flashcard.CardId)
            {
                return NotFound();
            }
            
            if (ModelState.IsValid)
            {
                try
                {
                    var existingFlashcard = _context.CardPairs.Where(cp => cp.FlashcardId == flashcard.CardId);
                    _context.CardPairs.RemoveRange(existingFlashcard);

                    foreach (var pair in flashcard.CardPairs)
                    {
                        pair.FlashcardId = flashcard.CardId;
                        _context.Add(pair);
                    }

                    _context.Update(flashcard);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FlashcardExists(flashcard.CardId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(UserIndex));
            }
            ViewData["DeckId"] = new SelectList(_context.Decks, "DeckId", "DeckId", flashcard.DeckId);

            return View(flashcard);
        }

        // GET: Flashcards/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var flashcard = await _context.Flashcards
                .Include(f => f.Deck)
                .Include(f => f.User)
                .FirstOrDefaultAsync(m => m.CardId == id);
            if (flashcard == null)
            {
                return NotFound();
            }

            return View(flashcard);
        }

        // POST: Flashcards/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var flashcard = await _context.Flashcards.FindAsync(id);
            var listCardpair = await _context.CardPairs.Where(cp => cp.FlashcardId == id).ToListAsync();
            if (flashcard != null)
            {
                _context.Flashcards.Remove(flashcard);
                _context.CardPairs.RemoveRange(listCardpair);
            }

            await _context.SaveChangesAsync();
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (userEmail == "admin@gmail.com")
            {
                return RedirectToAction(nameof(AdminIndex));
            }
            return RedirectToAction(nameof(UserIndex));
        }

        private bool FlashcardExists(int id)
        {
            return _context.Flashcards.Any(e => e.CardId == id);
        }
    }
}
