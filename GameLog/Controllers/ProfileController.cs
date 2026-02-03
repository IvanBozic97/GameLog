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

    // hardcoded pool (safe because files are in wwwroot)
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
            var user = await _userManager.GetUserAsync(User);
            var email = user?.Email ?? "";
            var defaultName = string.IsNullOrWhiteSpace(email)
                ? "User"
                : email.Split('@')[0];

            profile = new UserProfile
            {
                UserId = userId!,
                AvatarFileName = "default-avatar.jpg",
                DisplayName = defaultName
            };

            _context.UserProfiles.Add(profile);
            await _context.SaveChangesAsync();
        }
        else
        {
            // if the profile existed before migration and does not have a DisplayName
            if (string.IsNullOrWhiteSpace(profile.DisplayName))
            {
                var user = await _userManager.GetUserAsync(User);
                var email = user?.Email ?? "";
                profile.DisplayName = string.IsNullOrWhiteSpace(email)
                    ? "User"
                    : email.Split('@')[0];

                await _context.SaveChangesAsync();
            }
        }

        ViewBag.Avatars = AvatarPool;
        return View(profile);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SetAvatar(string avatarFileName)
    {
        // protection: accept only from pool
        if (string.IsNullOrWhiteSpace(avatarFileName) || !AvatarPool.Contains(avatarFileName))
        {
            return BadRequest();
        }

        var userId = _userManager.GetUserId(User);

        var profile = await _context.UserProfiles
            .FirstOrDefaultAsync(p => p.UserId == userId);

        if (profile == null)
        {
            var user = await _userManager.GetUserAsync(User);
            var email = user?.Email ?? "";
            var defaultName = string.IsNullOrWhiteSpace(email)
                ? "User"
                : email.Split('@')[0];

            profile = new UserProfile
            {
                UserId = userId!,
                AvatarFileName = avatarFileName,
                DisplayName = defaultName
            };

            _context.UserProfiles.Add(profile);
        }
        else
        {
            profile.AvatarFileName = avatarFileName;
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SetDisplayName(string displayName)
    {
        displayName = (displayName ?? "").Trim();

        if (displayName.Length < 3 || displayName.Length > 30)
        {
            TempData["ProfileError"] = "Display name must be between 3 and 30 characters.";
            return RedirectToAction(nameof(Index));
        }

        var userId = _userManager.GetUserId(User);

        var profile = await _context.UserProfiles
            .FirstOrDefaultAsync(p => p.UserId == userId);

        if (profile == null)
        {
            // security: if there is no profile, create one
            var email = (await _userManager.GetUserAsync(User))?.Email ?? "";
            var defaultAvatar = "default-avatar.jpg";

            profile = new UserProfile
            {
                UserId = userId!,
                AvatarFileName = defaultAvatar,
                DisplayName = displayName
            };
            _context.UserProfiles.Add(profile);
        }
        else
        {
            profile.DisplayName = displayName;
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

}
