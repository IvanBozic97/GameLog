using GameLog.Areas.Identity.Data;
using GameLog.Models;
using Microsoft.EntityFrameworkCore;

namespace GameLog.Data;

public static class SeedData
{
    public static void SeedGames(ApplicationDbContext context)
    {
        // Ako već ima igara – ne seedaj opet
        if (context.Games.Any())
            return;

        var games = new List<Game>
        {
            new() { Title = "God of War Ragnarök", ReleaseYear = 2022, ImagePath = "gow-ragnarok.jpg" },
            new() { Title = "Marvel's Spider-Man 2", ReleaseYear = 2023, ImagePath = "spiderman-2.jpg" },
            new() { Title = "Devil May Cry 5", ReleaseYear = 2019, ImagePath = "dmc5.jpg" },

            new() { Title = "The Witcher 3: Wild Hunt", ReleaseYear = 2015, ImagePath = "witcher-3.jpg" },
            new() { Title = "Elden Ring", ReleaseYear = 2022, ImagePath = "elden-ring.jpg" },
            new() { Title = "Baldur’s Gate 3", ReleaseYear = 2023, ImagePath = "baldurs-gate-3.jpg" },

            new() { Title = "Red Dead Redemption 2", ReleaseYear = 2018, ImagePath = "rdr2.jpg" },
            new() { Title = "The Last of Us Part I", ReleaseYear = 2022, ImagePath = "tlou-part1.jpg" },
            new() { Title = "Ghost of Tsushima", ReleaseYear = 2020, ImagePath = "ghost-of-tsushima.jpg" },

            new() { Title = "Halo Infinite", ReleaseYear = 2021, ImagePath = "halo-infinite.jpg" },
            new() { Title = "DOOM Eternal", ReleaseYear = 2020, ImagePath = "doom-eternal.jpg" },
            new() { Title = "Call of Duty: Modern Warfare II", ReleaseYear = 2022, ImagePath = "cod-mw2.jpg" },

            new() { Title = "Resident Evil 4", ReleaseYear = 2023, ImagePath = "re4-remake.jpg" },
            new() { Title = "Dead Space", ReleaseYear = 2023, ImagePath = "dead-space.jpg" },
            new() { Title = "Alan Wake 2", ReleaseYear = 2023, ImagePath = "alan-wake-2.jpg" },

            new() { Title = "Minecraft", ReleaseYear = 2011, ImagePath = "minecraft.jpg" },
            new() { Title = "Grand Theft Auto V", ReleaseYear = 2013, ImagePath = "gta-v.jpg" },
            new() { Title = "No Man’s Sky", ReleaseYear = 2016, ImagePath = "no-mans-sky.jpg" },

            new() { Title = "Age of Wonders 4", ReleaseYear = 2023, ImagePath = "age-of-wonders-4.jpg" },
            new() { Title = "Civilization VI", ReleaseYear = 2016, ImagePath = "civilization-6.jpg" },
            new() { Title = "StarCraft II", ReleaseYear = 2010, ImagePath = "starcraft-2.jpg" },

            new() { Title = "Microsoft Flight Simulator", ReleaseYear = 2020, ImagePath = "flight-simulator.jpg" },
            new() { Title = "The Sims 4", ReleaseYear = 2014, ImagePath = "sims-4.jpg" },
            new() { Title = "Euro Truck Simulator 2", ReleaseYear = 2012, ImagePath = "ets2.jpg" }
        };

        context.Games.AddRange(games);
        context.SaveChanges();
    }
}
