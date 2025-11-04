using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FlashCard.Models;

namespace FlashCard.Controllers
{
    public class FlashcardsController : Controller
    {
        private readonly FlashcardDbContext _context;

        public FlashcardsController(FlashcardDbContext context)
        {
            _context = context;
        }

        // GET: Flashcards
        public async Task<IActionResult> Index()
        {
            var flashcardDbContext = _context.Flashcards.Include(f => f.Deck).Include(f => f.User);
            return View(await flashcardDbContext.ToListAsync());
        }

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
                .FirstOrDefaultAsync(m => m.CardId == id);
            if (flashcard == null)
            {
                return NotFound();
            }

            return View(flashcard);
        }

        // GET: Flashcards/Create
        public IActionResult Create()
        {
            ViewData["DeckId"] = new SelectList(_context.Decks, "DeckId", "DeckId");
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Email");
            return View();
        }

        // POST: Flashcards/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CardId,FrontFlash,BackFlash,UserId,DeckId")] Flashcard flashcard)
        {
            if (ModelState.IsValid)
            {
                _context.Add(flashcard);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DeckId"] = new SelectList(_context.Decks, "DeckId", "DeckId", flashcard.DeckId);
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Email", flashcard.UserId);
            return View(flashcard);
        }

        // GET: Flashcards/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var flashcard = await _context.Flashcards.FindAsync(id);
            if (flashcard == null)
            {
                return NotFound();
            }
            ViewData["DeckId"] = new SelectList(_context.Decks, "DeckId", "DeckId", flashcard.DeckId);
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Email", flashcard.UserId);
            return View(flashcard);
        }

        // POST: Flashcards/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CardId,FrontFlash,BackFlash,UserId,DeckId")] Flashcard flashcard)
        {
            if (id != flashcard.CardId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
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
                return RedirectToAction(nameof(Index));
            }
            ViewData["DeckId"] = new SelectList(_context.Decks, "DeckId", "DeckId", flashcard.DeckId);
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Email", flashcard.UserId);
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
            if (flashcard != null)
            {
                _context.Flashcards.Remove(flashcard);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FlashcardExists(int id)
        {
            return _context.Flashcards.Any(e => e.CardId == id);
        }
    }
}
