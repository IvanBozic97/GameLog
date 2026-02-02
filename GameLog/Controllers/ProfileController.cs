using GameLog.Areas.Identity.Data;
using GameLog.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GameLog.Controllers;

[Authorize]
public class ProfileController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    // hardcoded pool (sigurno jer su to tvoji fileovi u wwwroot)
    private static readonly string[] AvatarPool = new[]
    {
        "avatar-1.jpg","avatar-2.jpg","avatar-3.jpg","avatar-4.jpg",
        "avatar-5.jpg","avatar-6.jpg","avatar-7.jpg","avatar-8.jpg",
        "avatar-9.jpg","avatar-10.jpg","avatar-11.jpg","avatar-12.jpg"
    };


    public ProfileController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var userId = _userManager.GetUserId(User);

        var profile = await _context.UserProfiles
            .FirstOrDefaultAsync(p => p.UserId == userId);

        if (profile == null)
        {
            profile = new UserProfile
            {
                UserId = userId!,
                AvatarFileName = "default-avatar.jpg"
            };
            _context.UserProfiles.Add(profile);
            await _context.SaveChangesAsync();
        }

        ViewBag.Avatars = AvatarPool;
        return View(profile);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SetAvatar(string avatarFileName)
    {
        // zaštita: prihvati samo iz poola
        if (string.IsNullOrWhiteSpace(avatarFileName) || !AvatarPool.Contains(avatarFileName))
        {
            return BadRequest();
        }

        var userId = _userManager.GetUserId(User);

        var profile = await _context.UserProfiles
            .FirstOrDefaultAsync(p => p.UserId == userId);

        if (profile == null)
        {
            profile = new UserProfile { UserId = userId!, AvatarFileName = avatarFileName };
            _context.UserProfiles.Add(profile);
        }
        else
        {
            profile.AvatarFileName = avatarFileName;
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}
