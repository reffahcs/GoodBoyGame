// 📁 GameHub.cs
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class GameHub
{
    private List<RPGHero> playerRoster = new List<RPGHero>();
    private int summonTickets = 10;
    private int playerGold = 500;
    private int playerXPChips = 50;

    private GachaEngine gachaEngine = new GachaEngine();
    private RosterManager rosterManager = new RosterManager();
    private List<HeroBlueprint> jsonRegistry = new List<HeroBlueprint>();

    // 🌟 DETERMINISTIC SUBSYSTEM EXTENSIONS
    private InventoryManager playerInventory = new InventoryManager();
    private List<GearBlueprint> itemRegistry = new List<GearBlueprint>();

    private const string SaveFileName = "savegame.json";
    private const string ItemsFileName = "ItemsDB.json";

    public void Launch()
    {
        jsonRegistry = LoadHeroDatabase();
        itemRegistry = LoadItemDatabase(); // Hydrate static item database at boot

        if (!LoadGame())
        {
            Console.WriteLine("\n🐾 No previous save profile found. Setting up a new journey...");
            HeroBlueprint starterTemplate = jsonRegistry.Find(h => h.Name == "Sherlock Bones");

            if (starterTemplate != null)
            {
                playerRoster.Add(
                    new RPGHero(
                        starterTemplate.Name,
                        "Stray",
                        1,
                        starterTemplate.BaseHP,
                        starterTemplate.BaseAttack,
                        1
                    )
                );
            }
            else
            {
                playerRoster.Add(new RPGHero("Mutt-zo Ball", "Stray", 1, 105f, 13f, 1));
            }

            SaveGame();
            Console.WriteLine(" Press Enter to access the Main Hub...");
            Console.ReadLine();
        }

        bool running = true;
        while (running)
        {
            Console.Clear();
            Console.WriteLine(
                "=========================================================================="
            );
            Console.WriteLine(
                "🐾                    GOOD BOY: SUPERHERO COMPANION HUB                  🐾"
            );
            Console.WriteLine(
                "=========================================================================="
            );
            Console.WriteLine(
                $" [Wallet Reserves]  🎫 Tickets: {this.summonTickets}  |  💰 Gold: {this.playerGold}  |  🧪 XP Chips: {this.playerXPChips}"
            );
            Console.WriteLine(
                "--------------------------------------------------------------------------"
            );
            Console.WriteLine(" [1] Launch PvE Squad Campaign Stages");
            Console.WriteLine(" [2] Enter Local PvP Arena Matches");
            Console.WriteLine(" [3] Open Roster Management (Inspect / Level Up)");
            Console.WriteLine(" [4] Gacha Summon Shop (Spend Tickets)");
            Console.WriteLine(" [5] Exit Game Simulation");
            Console.WriteLine(
                "=========================================================================="
            );
            Console.Write("Choose Sector Destination: ");

            string choice = Console.ReadLine()?.Trim() ?? "";
            switch (choice)
            {
                case "1":
                    // Campaign engine parameter contract intact
                    CampaignEngine campaignEngine = new CampaignEngine();
                    campaignEngine.LaunchCampaign(
                        playerRoster,
                        jsonRegistry,
                        ref summonTickets,
                        ref playerGold,
                        ref playerXPChips
                    );
                    SaveGame();
                    break;

                case "2":
                    Console.Clear();
                    Console.WriteLine("\n⚔️ Entering the Local PvP Arena matches...");
                    System.Threading.Thread.Sleep(1000);
                    SaveGame();
                    break;

                case "3":
                    // Roster Management Entry via RosterManager
                    rosterManager.OpenMenu(playerRoster, ref playerGold, ref playerXPChips);
                    SaveGame();
                    break;

                case "4":
                    // 🌟 Update parameters to cleanly stream live registries down to the engine
                    gachaEngine.OpenMenu(
                        playerRoster,
                        itemRegistry,
                        playerInventory,
                        ref summonTickets,
                        ref playerGold,
                        ref playerXPChips
                    );
                    SaveGame();
                    break;

                case "5":
                    Console.WriteLine("\n💾 Packaging runtime profiles... Exiting simulation.");
                    SaveGame();
                    running = false;
                    break;

                default:
                    Console.WriteLine("\n🚨 Input vector invalid. Re-evaluating terminal loops...");
                    System.Threading.Thread.Sleep(1000);
                    break;
            }
        }
    }

    private void SaveGame()
    {
        try
        {
            PlayerData data = new PlayerData
            {
                Tickets = summonTickets,
                Gold = playerGold,
                XPChips = playerXPChips,
                Roster = playerRoster,
                UnequippedBackpack = playerInventory.Stash,
            };

            string jsonString = JsonSerializer.Serialize(
                data,
                new JsonSerializerOptions { WriteIndented = true }
            );
            File.WriteAllText(SaveFileName, jsonString);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n🚨 Save Profile Processing Failure: {ex.Message}");
        }
    }

    private bool LoadGame()
    {
        try
        {
            if (!File.Exists(SaveFileName))
            {
                return false;
            }

            string jsonString = File.ReadAllText(SaveFileName);
            PlayerData loadedData = JsonSerializer.Deserialize<PlayerData>(jsonString);

            if (loadedData != null)
            {
                this.summonTickets = loadedData.Tickets;
                this.playerGold = loadedData.Gold;
                this.playerXPChips = loadedData.XPChips;
                this.playerRoster = loadedData.Roster ?? new List<RPGHero>();

                // Restore the unequipped stash payload
                this.playerInventory.Stash =
                    loadedData.UnequippedBackpack ?? new List<InventoryItem>();

                // Hydrate runtime fields right after loading from file
                this.playerRoster = LoadAndHydratePlayerSave(this.playerRoster, this.jsonRegistry);

                Console.WriteLine("\n💾 Save file loaded successfully! Welcome back.");
                System.Threading.Thread.Sleep(1000);
                return true;
            }
        }
        catch (Exception)
        {
            return false;
        }
        return false;
    }

    private List<HeroBlueprint> LoadHeroDatabase()
    {
        string filePath = "Heroes.json";
        try
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine(
                    $"\n🚨 File Error: Could not find '{filePath}' in your directory!"
                );
                return new List<HeroBlueprint>();
            }

            string jsonContent = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<List<HeroBlueprint>>(jsonContent)
                ?? new List<HeroBlueprint>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n🚨 JSON Parse Failure: {ex.Message}");
            return new List<HeroBlueprint>();
        }
    }

    private List<GearBlueprint> LoadItemDatabase()
    {
        try
        {
            if (!File.Exists(ItemsFileName))
            {
                return new List<GearBlueprint>();
            }

            string jsonContent = File.ReadAllText(ItemsFileName);
            return JsonSerializer.Deserialize<List<GearBlueprint>>(jsonContent)
                ?? new List<GearBlueprint>();
        }
        catch (Exception)
        {
            return new List<GearBlueprint>();
        }
    }

    public List<RPGHero> LoadAndHydratePlayerSave(
        List<RPGHero> roster,
        List<HeroBlueprint> staticGachaPool
    )
    {
        if (roster == null)
            return new List<RPGHero>();

        foreach (var hero in roster)
        {
            var blueprint = staticGachaPool.Find(b =>
                b.Name.Equals(hero.Name, StringComparison.OrdinalIgnoreCase)
            );

            if (blueprint != null)
            {
                hero.RawBaseHP = blueprint.BaseHP;
                hero.RawBaseAttack = blueprint.BaseAttack;
                hero.CritChance = blueprint.BaseCritChance;
            }
        }

        return roster;
    }
}

public class PlayerData
{
    public int Tickets { get; set; }
    public int Gold { get; set; }
    public int XPChips { get; set; }
    public List<RPGHero> Roster { get; set; } = new List<RPGHero>();
    public List<InventoryItem> UnequippedBackpack { get; set; } = new List<InventoryItem>();
}
