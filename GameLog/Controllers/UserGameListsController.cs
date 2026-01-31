using GameLog.Areas.Identity.Data;
using GameLog.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace GameLog.Controllers;

[Authorize]
public class UserGameListsController : Controller
{
    private readonly ApplicationDbContext _context;

    public UserGameListsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: /UserGameLists
    public async Task<IActionResult> Index(GameStatus? status = null)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Challenge();

        var query = _context.UserGameLists
            .Include(x => x.Game)
            .Where(x => x.UserId == userId);

        if (status.HasValue)
            query = query.Where(x => x.Status == status.Value);

        var items = await query
            .OrderByDescending(x => x.AddedAt)
            .ToListAsync();

        ViewBag.SelectedStatus = status;
        return View(items);
    }

    // POST: /UserGameLists/Upsert
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Upsert(int gameId, GameStatus status)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Challenge();

        var gameExists = await _context.Games.AnyAsync(g => g.Id == gameId);
        if (!gameExists) return NotFound();

        var existing = await _context.UserGameLists
            .FirstOrDefaultAsync(x => x.UserId == userId && x.GameId == gameId);

        if (existing == null)
        {
            _context.UserGameLists.Add(new UserGameList
            {
                UserId = userId,
                GameId = gameId,
                Status = status,
                AddedAt = DateTime.UtcNow
            });
        }
        else
        {
            existing.Status = status;
            _context.UserGameLists.Update(existing);
        }

        await _context.SaveChangesAsync();

        // return user to game details
        return RedirectToAction("Details", "Games", new { id = gameId });
    }

    // POST: /UserGameLists/Remove
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Remove(int gameId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Challenge();

        var existing = await _context.UserGameLists
            .FirstOrDefaultAsync(x => x.UserId == userId && x.GameId == gameId);

        if (existing != null)
        {
            _context.UserGameLists.Remove(existing);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }
}
