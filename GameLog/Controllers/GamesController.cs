using GameLog.Areas.Identity.Data;
using GameLog.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;


namespace GameLog.Controllers
{
    public class GamesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GamesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Games
        public async Task<IActionResult> Index()
        {
            return View(await _context.Games.ToListAsync());
        }

        // GET: Games/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var game = await _context.Games.FirstOrDefaultAsync(m => m.Id == id);
            if (game == null) return NotFound();

            var reviews = await _context.Reviews
                .Where(r => r.GameId == game.Id)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            var vm = new GameDetailsViewModel
            {
                Game = game,
                Reviews = reviews
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> AddReview(int gameId, int rating, string? text)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Challenge();

            // check if game exists
            var gameExists = await _context.Games.AnyAsync(g => g.Id == gameId);
            if (!gameExists) return NotFound();

            // if there is already a review, update it (easier than throwing an error)
            var existing = await _context.Reviews
                .FirstOrDefaultAsync(r => r.GameId == gameId && r.UserId == userId);

            if (existing == null)
            {
                var review = new Review
                {
                    GameId = gameId,
                    UserId = userId,
                    Rating = rating,
                    Text = text,
                    CreatedAt = DateTime.UtcNow
                };
                _context.Reviews.Add(review);
            }
            else
            {
                existing.Rating = rating;
                existing.Text = text;
                existing.CreatedAt = DateTime.UtcNow;
                _context.Reviews.Update(existing);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new { id = gameId });
        }


        // GET: Games/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }


        // POST: Games/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Id,Title,ReleaseYear,CreatedAt")] Game game)
        {
            if (ModelState.IsValid)
            {
                _context.Add(game);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(game);
        }


        // GET: Games/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var game = await _context.Games.FindAsync(id);
            if (game == null)
                return NotFound();

            return View(game);
        }


        // POST: Games/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,ReleaseYear,CreatedAt")] Game game)
        {
            if (id != game.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(game);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GameExists(game.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(game);
        }


        // GET: Games/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var game = await _context.Games
                .FirstOrDefaultAsync(m => m.Id == id);

            if (game == null)
                return NotFound();

            return View(game);
        }


        // POST: Games/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var game = await _context.Games.FindAsync(id);
            if (game != null)
            {
                _context.Games.Remove(game);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool GameExists(int id)
        {
            return _context.Games.Any(e => e.Id == id);
        }


    }
}
