using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GameLog.Areas.Identity.Data;

[Authorize(Roles = "Admin")]
public class AdminUsersController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ApplicationDbContext _context;

    public AdminUsersController(
        UserManager<IdentityUser> userManager,
        ApplicationDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var users = await (
            from u in _userManager.Users
            join p in _context.UserProfiles
                on u.Id equals p.UserId
            select new
            {
                u.Id,
                u.Email,
                p.DisplayName,
                p.IsBanned
            }
        ).ToListAsync();

        return View(users);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleBan(string userId)
    {
        var profile = await _context.UserProfiles.FirstOrDefaultAsync(p => p.UserId == userId);
        if (profile == null) return NotFound();

        profile.IsBanned = !profile.IsBanned;
        _context.UserProfiles.Update(profile);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}
