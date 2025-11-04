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
    public class ProgressesController : Controller
    {
        private readonly FlashcardDbContext _context;

        public ProgressesController(FlashcardDbContext context)
        {
            _context = context;
        }

        // GET: Progresses
        public async Task<IActionResult> Index()
        {
            var flashcardDbContext = _context.Progresses.Include(p => p.Flashcard).Include(p => p.User);
            return View(await flashcardDbContext.ToListAsync());
        }

        // GET: Progresses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var progress = await _context.Progresses
                .Include(p => p.Flashcard)
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.ProgressId == id);
            if (progress == null)
            {
                return NotFound();
            }

            return View(progress);
        }

        // GET: Progresses/Create
        public IActionResult Create()
        {
            ViewData["FlashcardId"] = new SelectList(_context.Flashcards, "CardId", "CardId");
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Email");
            return View();
        }

        // POST: Progresses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProgressId,UserId,FlashcardId,IsKnown,IsAvailable")] Progress progress)
        {
            if (ModelState.IsValid)
            {
                _context.Add(progress);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["FlashcardId"] = new SelectList(_context.Flashcards, "CardId", "CardId", progress.FlashcardId);
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Email", progress.UserId);
            return View(progress);
        }

        // GET: Progresses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var progress = await _context.Progresses.FindAsync(id);
            if (progress == null)
            {
                return NotFound();
            }
            ViewData["FlashcardId"] = new SelectList(_context.Flashcards, "CardId", "CardId", progress.FlashcardId);
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Email", progress.UserId);
            return View(progress);
        }

        // POST: Progresses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProgressId,UserId,FlashcardId,IsKnown,IsAvailable")] Progress progress)
        {
            if (id != progress.ProgressId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(progress);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProgressExists(progress.ProgressId))
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
            ViewData["FlashcardId"] = new SelectList(_context.Flashcards, "CardId", "CardId", progress.FlashcardId);
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Email", progress.UserId);
            return View(progress);
        }

        // GET: Progresses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var progress = await _context.Progresses
                .Include(p => p.Flashcard)
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.ProgressId == id);
            if (progress == null)
            {
                return NotFound();
            }

            return View(progress);
        }

        // POST: Progresses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var progress = await _context.Progresses.FindAsync(id);
            if (progress != null)
            {
                _context.Progresses.Remove(progress);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProgressExists(int id)
        {
            return _context.Progresses.Any(e => e.ProgressId == id);
        }
    }
}
