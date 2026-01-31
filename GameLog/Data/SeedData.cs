using GameLog.Areas.Identity.Data;
using GameLog.Models;

namespace GameLog.Data;

public static class SeedData
{
    public static void SeedGenres(ApplicationDbContext context)
    {
        if (context.Genres.Any())
            return;

        var genres = new[]
        {
            "Action",
            "RPG",
            "Adventure",
            "Shooter",
            "Horror",
            "Sandbox / Open World",
            "Strategy",
            "Simulation"
        };

        context.Genres.AddRange(genres.Select(g => new Genre { Name = g }));
        context.SaveChanges();
    }

    public static void SeedPlatforms(ApplicationDbContext context)
    {
        if (context.Platforms.Any())
            return;

        var platforms = new[]
        {
        "PC",
        "PlayStation",
        "Xbox",
        "Nintendo Switch"
    };

        context.Platforms.AddRange(platforms.Select(p => new Platform { Name = p }));
        context.SaveChanges();
    }

    public static void SeedGames(ApplicationDbContext context)
    {
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

    public static void SeedGameRelations(ApplicationDbContext context)
    {
        if (context.GameGenres.Any() || context.GamePlatforms.Any())
            return;

        // helperi za lakše traženje
        Genre G(string name) => context.Genres.First(g => g.Name == name);
        Platform P(string name) => context.Platforms.First(p => p.Name == name);
        Game GameByTitle(string title) => context.Games.First(g => g.Title == title);

        void Add(string gameTitle, string[] genres, string[] platforms)
        {
            var game = GameByTitle(gameTitle);

            foreach (var g in genres)
            {
                context.GameGenres.Add(new GameGenre
                {
                    GameId = game.Id,
                    GenreId = G(g).Id
                });
            }

            foreach (var p in platforms)
            {
                context.GamePlatforms.Add(new GamePlatform
                {
                    GameId = game.Id,
                    PlatformId = P(p).Id
                });
            }
        }

        // ===== IGRE =====

        Add("God of War Ragnarök",
            new[] { "Action", "Adventure" },
            new[] { "PlayStation" });

        Add("Marvel's Spider-Man 2",
            new[] { "Action", "Adventure" },
            new[] { "PlayStation" });

        Add("Devil May Cry 5",
            new[] { "Action" },
            new[] { "PC", "PlayStation", "Xbox" });

        Add("The Witcher 3: Wild Hunt",
            new[] { "RPG", "Adventure" },
            new[] { "PC", "PlayStation", "Xbox", "Nintendo Switch" });

        Add("Elden Ring",
            new[] { "RPG", "Action" },
            new[] { "PC", "PlayStation", "Xbox" });

        Add("Baldur’s Gate 3",
            new[] { "RPG", "Strategy" },
            new[] { "PC", "PlayStation", "Xbox" });

        Add("Red Dead Redemption 2",
            new[] { "Action", "Adventure" },
            new[] { "PC", "PlayStation", "Xbox" });

        Add("The Last of Us Part I",
            new[] { "Action", "Adventure" },
            new[] { "PlayStation", "PC" });

        Add("Ghost of Tsushima",
            new[] { "Action", "Adventure" },
            new[] { "PlayStation", "PC" });

        Add("Halo Infinite",
            new[] { "Shooter" },
            new[] { "PC", "Xbox" });

        Add("DOOM Eternal",
            new[] { "Shooter", "Action" },
            new[] { "PC", "PlayStation", "Xbox" });

        Add("Call of Duty: Modern Warfare II",
            new[] { "Shooter", "Action" },
            new[] { "PC", "PlayStation", "Xbox" });

        Add("Resident Evil 4",
            new[] { "Horror", "Action" },
            new[] { "PC", "PlayStation", "Xbox" });

        Add("Dead Space",
            new[] { "Horror" },
            new[] { "PC", "PlayStation", "Xbox" });

        Add("Alan Wake 2",
            new[] { "Horror", "Adventure" },
            new[] { "PC", "PlayStation", "Xbox" });

        Add("Minecraft",
            new[] { "Sandbox / Open World" },
            new[] { "PC", "PlayStation", "Xbox", "Nintendo Switch" });

        Add("Grand Theft Auto V",
            new[] { "Action", "Sandbox / Open World" },
            new[] { "PC", "PlayStation", "Xbox" });

        Add("No Man’s Sky",
            new[] { "Sandbox / Open World", "Adventure" },
            new[] { "PC", "PlayStation", "Xbox", "Nintendo Switch" });

        Add("Age of Wonders 4",
            new[] { "Strategy" },
            new[] { "PC" });

        Add("Civilization VI",
            new[] { "Strategy" },
            new[] { "PC", "Nintendo Switch" });

        Add("StarCraft II",
            new[] { "Strategy" },
            new[] { "PC" });

        Add("Microsoft Flight Simulator",
            new[] { "Simulation" },
            new[] { "PC", "Xbox" });

        Add("The Sims 4",
            new[] { "Simulation" },
            new[] { "PC", "PlayStation", "Xbox" });

        Add("Euro Truck Simulator 2",
            new[] { "Simulation" },
            new[] { "PC" });

        context.SaveChanges();
    }

}
