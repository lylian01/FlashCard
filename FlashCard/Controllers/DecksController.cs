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
using System.Net.WebSockets;
using Microsoft.AspNetCore.Mvc.TagHelpers;


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
        public async Task<IActionResult> AdminIndex()
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (userEmail != "admin@gmail.com")
            {
                ModelState.AddModelError("", "Sai thông tin đăng nhập vào admin.");
                return RedirectToAction("Login", "Users");
            }

            var flashcardDbContext = _context.Decks.Include(f => f.User);
            return View(await flashcardDbContext.ToListAsync());
        }
        public async Task<IActionResult> UserIndex()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdString == null)
                return RedirectToAction("Login", "Users");

            int userId = int.Parse(userIdString);

            var decksByUser = await _context.Decks
                .Include(d => d.Flashcards)              
                .Where(d => d.UserId == userId)
                .ToListAsync();

            return View(decksByUser);
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
                return RedirectToAction(nameof(UserIndex));
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

            var deck = await _context.Decks
                .Include(d => d.Flashcards)
                .FirstOrDefaultAsync(d => d.DeckId == id);

            if (deck == null)
            {
                return NotFound();
            }

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
                return RedirectToAction(nameof(UserIndex));
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
                var flashCard = await _context.Flashcards.Where(f => f.DeckId == id).ToListAsync();

                if (flashCard != null)
                {
                    foreach(var fcard in flashCard)
                    {
                        fcard.DeckId = null;
                        
                    }
                }

                _context.Decks.Remove(deck);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(UserIndex));
        }

        private bool DeckExists(int id)
        {
            return _context.Decks.Any(e => e.DeckId == id);
        }
    }
}
