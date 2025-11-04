using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FlashCard.Models;


using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;


namespace FlashCard.Controllers
{
    [Authorize]
    public class DecksController : Controller
    {
        private readonly FlashcardDbContext _context;

        public DecksController(FlashcardDbContext context)
        {
            _context = context;
        }

        // GET: Decks
        public async Task<IActionResult> Index()
        {
            var flashcardDbContext = _context.Decks.Include(d => d.User);
            return View(await flashcardDbContext.ToListAsync());
        }

        // GET: Decks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var deck = await _context.Decks
                .Include(d => d.User)
                .FirstOrDefaultAsync(m => m.DeckId == id);
            if (deck == null)
            {
                return NotFound();
            }

            return View(deck);
        }

        // GET: Decks/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Decks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DeckId,DeckName,Description")] Deck deck)
        {
            if (ModelState.IsValid)
            {
                var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (int.TryParse(userIdString, out int userId))
                    deck.UserId = userId;

                _context.Add(deck);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            //ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Email", deck.UserId);
            return View(deck);
        }

        // GET: Decks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var deck = await _context.Decks.FindAsync(id);
            if (deck == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Email", deck.UserId);
            return View(deck);
        }

        // POST: Decks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DeckId,DeckName,Description,UserId")] Deck deck)
        {
            if (id != deck.DeckId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(deck);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DeckExists(deck.DeckId))
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
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Email", deck.UserId);
            return View(deck);
        }

        // GET: Decks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var deck = await _context.Decks
                .Include(d => d.User)
                .FirstOrDefaultAsync(m => m.DeckId == id);
            if (deck == null)
            {
                return NotFound();
            }

            return View(deck);
        }

        // POST: Decks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var deck = await _context.Decks.FindAsync(id);
            if (deck != null)
            {
                _context.Decks.Remove(deck);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DeckExists(int id)
        {
            return _context.Decks.Any(e => e.DeckId == id);
        }
    }
}
