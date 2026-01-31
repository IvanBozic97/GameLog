using GameLog.Areas.Identity.Data;
using GameLog.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
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

            var game = await _context.Games
                .Include(g => g.GameGenres)
                    .ThenInclude(gg => gg.Genre)
                .Include(g => g.GamePlatforms)
                    .ThenInclude(gp => gp.Platform)
                .FirstOrDefaultAsync(m => m.Id == id);

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

            var gameExists = await _context.Games.AnyAsync(g => g.Id == gameId);
            if (!gameExists) return NotFound();

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
        public async Task<IActionResult> Create()
        {
            var vm = new GameEditViewModel
            {
                AllGenres = await _context.Genres.OrderBy(g => g.Name).ToListAsync(),
                AllPlatforms = await _context.Platforms.OrderBy(p => p.Name).ToListAsync()
            };

            return View(vm);
        }

        // POST: Games/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(GameEditViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                vm.AllGenres = await _context.Genres.OrderBy(g => g.Name).ToListAsync();
                vm.AllPlatforms = await _context.Platforms.OrderBy(p => p.Name).ToListAsync();
                return View(vm);
            }

            var game = new Game
            {
                Title = vm.Title,
                ReleaseYear = vm.ReleaseYear,
                ImagePath = vm.ImagePath,
                CreatedAt = DateTime.UtcNow
            };

            foreach (var genreId in vm.SelectedGenreIds.Distinct())
            {
                game.GameGenres.Add(new GameGenre { GenreId = genreId });
            }

            foreach (var platformId in vm.SelectedPlatformIds.Distinct())
            {
                game.GamePlatforms.Add(new GamePlatform { PlatformId = platformId });
            }

            _context.Games.Add(game);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Games/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var game = await _context.Games
                .Include(g => g.GameGenres)
                .Include(g => g.GamePlatforms)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (game == null) return NotFound();

            var vm = new GameEditViewModel
            {
                Id = game.Id,
                Title = game.Title,
                ReleaseYear = game.ReleaseYear,
                ImagePath = game.ImagePath,

                SelectedGenreIds = game.GameGenres.Select(gg => gg.GenreId).ToList(),
                AllGenres = await _context.Genres.OrderBy(g => g.Name).ToListAsync(),

                SelectedPlatformIds = game.GamePlatforms.Select(gp => gp.PlatformId).ToList(),
                AllPlatforms = await _context.Platforms.OrderBy(p => p.Name).ToListAsync()
            };

            return View(vm);
        }

        // POST: Games/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, GameEditViewModel vm)
        {
            if (id != vm.Id) return NotFound();

            if (!ModelState.IsValid)
            {
                vm.AllGenres = await _context.Genres.OrderBy(g => g.Name).ToListAsync();
                vm.AllPlatforms = await _context.Platforms.OrderBy(p => p.Name).ToListAsync();
                return View(vm);
            }

            var game = await _context.Games
                .Include(g => g.GameGenres)
                .Include(g => g.GamePlatforms)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (game == null) return NotFound();

            game.Title = vm.Title;
            game.ReleaseYear = vm.ReleaseYear;
            game.ImagePath = vm.ImagePath;

            // replace genres
            game.GameGenres.Clear();
            foreach (var genreId in vm.SelectedGenreIds.Distinct())
            {
                game.GameGenres.Add(new GameGenre { GameId = game.Id, GenreId = genreId });
            }

            // replace platforms
            game.GamePlatforms.Clear();
            foreach (var platformId in vm.SelectedPlatformIds.Distinct())
            {
                game.GamePlatforms.Add(new GamePlatform { GameId = game.Id, PlatformId = platformId });
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Games/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var game = await _context.Games.FirstOrDefaultAsync(m => m.Id == id);
            if (game == null) return NotFound();

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
