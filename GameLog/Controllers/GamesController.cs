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
        public async Task<IActionResult> Index(int? genreId, int? platformId, string? search)
        {
            ViewBag.Genres = await _context.Genres.OrderBy(g => g.Name).ToListAsync();
            ViewBag.Platforms = await _context.Platforms.OrderBy(p => p.Name).ToListAsync();

            ViewBag.SelectedGenreId = genreId;
            ViewBag.SelectedPlatformId = platformId;

            // keep search text in the UI
            ViewBag.Search = search;

            var query = _context.Games.AsQueryable();

            if (genreId.HasValue)
            {
                query = query.Where(g => g.GameGenres.Any(gg => gg.GenreId == genreId.Value));
            }

            if (platformId.HasValue)
            {
                query = query.Where(g => g.GamePlatforms.Any(gp => gp.PlatformId == platformId.Value));
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim();
                query = query.Where(g => g.Title.Contains(term));
            }

            query = query
                .Include(g => g.GameGenres)
                .Include(g => g.GamePlatforms)
                .OrderBy(g => g.Title);

            return View(await query.ToListAsync());
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

            var reviews = await (
                from r in _context.Reviews
                where r.GameId == game.Id
                join up in _context.UserProfiles
                    on r.UserId equals up.UserId into profileJoin
                from up in profileJoin.DefaultIfEmpty()
                orderby r.CreatedAt descending
                select new ReviewDisplayViewModel
                {
                    Id = r.Id,
                    UserId = r.UserId,
                    Rating = r.Rating,
                    Text = r.Text,
                    CreatedAt = r.CreatedAt,

                    DisplayName = up != null && !string.IsNullOrWhiteSpace(up.DisplayName)
                        ? up.DisplayName
                        : "User",

                    AvatarFileName = up != null && !string.IsNullOrWhiteSpace(up.AvatarFileName)
                        ? up.AvatarFileName
                        : "default-avatar.jpg"
                }
            ).ToListAsync();

            var reviewIds = reviews.Select(r => r.Id).ToList();

            var comments = await (
                from c in _context.ReviewComments
                where reviewIds.Contains(c.ReviewId)
                join up in _context.UserProfiles
                    on c.UserId equals up.UserId into profileJoin
                from up in profileJoin.DefaultIfEmpty()
                orderby c.CreatedAt ascending
                select new ReviewCommentDisplayViewModel
                {
                    Id = c.Id,
                    ReviewId = c.ReviewId,
                    UserId = c.UserId,
                    Text = c.Text,
                    CreatedAt = c.CreatedAt,
                    DisplayName = up != null && !string.IsNullOrWhiteSpace(up.DisplayName)
                        ? up.DisplayName
                        : "User",
                    AvatarFileName = up != null && !string.IsNullOrWhiteSpace(up.AvatarFileName)
                        ? up.AvatarFileName
                        : "default-avatar.jpg"
                }
            ).ToListAsync();

            var commentsByReviewId = comments
                .GroupBy(c => c.ReviewId)
                .ToDictionary(g => g.Key, g => g.ToList());

            foreach (var r in reviews)
            {
                if (commentsByReviewId.TryGetValue(r.Id, out var list))
                    r.Comments = list;
            }


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

            // Require display name before allowing reviews
            var profile = await _context.UserProfiles.FirstOrDefaultAsync(p => p.UserId == userId);
            if (profile == null || string.IsNullOrWhiteSpace(profile.DisplayName))
            {
                TempData["ProfileError"] = "Please set a display name before posting a review.";
                return RedirectToAction("Index", "Profile");
            }

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> AddComment(int gameId, int reviewId, string text)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Challenge();

            if (string.IsNullOrWhiteSpace(text) || text.Trim().Length < 1)
            {
                TempData["CommentError"] = "Comment cannot be empty.";
                return RedirectToAction(nameof(Details), new { id = gameId });
            }

            text = text.Trim();

            // (opc.) dodatna sigurnost: provjeri postoji li review
            var reviewExists = await _context.Reviews.AnyAsync(r => r.Id == reviewId && r.GameId == gameId);
            if (!reviewExists) return NotFound();

            var comment = new ReviewComment
            {
                ReviewId = reviewId,
                UserId = userId,
                Text = text,
                CreatedAt = DateTime.UtcNow
            };

            _context.ReviewComments.Add(comment);
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
