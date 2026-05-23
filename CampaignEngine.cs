// 📁 CampaignEngine.cs
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

// Runtime state wrapper so combat damage doesn't permanently alter your roster database
public class CombatantState
{
    public string Name { get; set; } = "";
    public float CurrentHP { get; set; }
    public float MaxHP { get; set; }
    public float Attack { get; set; }
    public int CritChance { get; set; }
    public bool IsPlayer { get; set; }
}

public class CampaignEngine
{
    private readonly Random rand = new Random();
    private List<CampaignStage> campaignDatabase = new List<CampaignStage>();
    private const string DatabasePath = "campaignDb.json";

    public void LaunchCampaign(
        List<RPGHero> roster,
        List<HeroBlueprint> registry,
        ref int tickets,
        ref int gold,
        ref int xpChips
    )
    {
        if (campaignDatabase == null || campaignDatabase.Count == 0)
        {
            LoadCampaignDatabase();
        }

        if (
            campaignDatabase == null
            || campaignDatabase.Count == 0
            || roster == null
            || roster.Count == 0
        )
        {
            Console.Clear();
            Console.WriteLine("🚨 ERROR: Cannot launch campaign. Check roster or database files.");
            Console.ReadLine();
            return;
        }

        bool runningCampaignSession = true;
        while (runningCampaignSession)
        {
            Console.Clear();
            Console.WriteLine("==================================================");
            Console.WriteLine("🗺️             CAMPAIGN SQUAD SELECTION            🗺️");
            Console.WriteLine("==================================================");
            Console.WriteLine(" Select an active sector battlegrounds to deploy:");
            Console.WriteLine("--------------------------------------------------");

            for (int i = 0; i < campaignDatabase.Count; i++)
            {
                var stage = campaignDatabase[i];
                string tag = stage.IsBossStage ? "[💀 FINAL BOSS]" : "[⚔️ Sector Battle]";
                Console.WriteLine(
                    $" [{i + 1}] Stage {stage.StageNumber}: {stage.StageName, -24} {tag}"
                );
            }
            Console.WriteLine($" [{campaignDatabase.Count + 1}] Return to Main Hub");
            Console.WriteLine("==================================================");
            Console.Write("Choice: ");

            string stageChoice = Console.ReadLine()?.Trim() ?? "";
            CampaignStage? currentStage = null;

            if (
                int.TryParse(stageChoice, out int stageIdx)
                && stageIdx >= 1
                && stageIdx <= campaignDatabase.Count
            )
            {
                currentStage = campaignDatabase[stageIdx - 1];
            }
            else if (
                stageIdx == campaignDatabase.Count + 1
                || stageChoice.Equals("b", StringComparison.OrdinalIgnoreCase)
            )
            {
                runningCampaignSession = false;
                continue;
            }
            else
            {
                Console.WriteLine("🚨 Invalid selection. Press Enter to retry.");
                Console.ReadLine();
                continue;
            }

            // --------------------------------------------------
            // 🐕 TEAM BUILDING SUB-MENU (1-5 COMBATANTS)
            // --------------------------------------------------
            List<CombatantState> playerTeam = new List<CombatantState>();
            bool buildingTeam = true;

            while (buildingTeam)
            {
                List<RPGHero> uniqueSelectableRoster = roster
                    .GroupBy(hero => hero.Name, StringComparer.OrdinalIgnoreCase)
                    .Select(group =>
                        group
                            .OrderByDescending(hero => hero.Level)
                            .ThenByDescending(hero => GetRarityWeight(hero.Rarity))
                            .First()
                    )
                    .ToList();

                Console.Clear();
                Console.WriteLine("==================================================");
                Console.WriteLine(
                    $"⚔️    DEPLOY SQUAD FOR STAGE {currentStage.StageNumber} ({playerTeam.Count}/5 SELECTED)   ⚔️"
                );
                Console.WriteLine("==================================================");
                Console.Write(" Current Lineup: ");
                if (playerTeam.Count == 0)
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.WriteLine("[Empty - Choose 1-5 companions to lock in]");
                    Console.ResetColor();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine(string.Join(", ", playerTeam.Select(p => p.Name)));
                    Console.ResetColor();
                }
                Console.WriteLine("--------------------------------------------------");

                for (int i = 0; i < uniqueSelectableRoster.Count; i++)
                {
                    RPGHero hero = uniqueSelectableRoster[i];
                    bool isAlreadySelected = playerTeam.Any(p =>
                        p.Name.Equals(hero.Name, StringComparison.OrdinalIgnoreCase)
                    );

                    if (isAlreadySelected)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine(
                            $" [{i + 1}] [DEPLOYED]  {hero.Name, -18} Lv.{hero.Level} Rank:[{hero.Rarity}]"
                        );
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.WriteLine(
                            $" [{i + 1}] [        ]  {hero.Name, -18} Lv.{hero.Level} Rank:[{hero.Rarity}]"
                        );
                    }
                }

                Console.WriteLine("--------------------------------------------------");
                if (playerTeam.Count >= 1)
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine(" [F] LOCK IN LINEUP AND LAUNCH ENGAGEMENT MATCH!");
                    Console.ResetColor();
                }
                Console.WriteLine(" [B] Cancel and Return to Maps");
                Console.WriteLine("==================================================");
                Console.Write("Selection: ");

                string teamInput = Console.ReadLine()?.Trim() ?? "";

                if (teamInput.Equals("b", StringComparison.OrdinalIgnoreCase))
                {
                    currentStage = null;
                    buildingTeam = false;
                    break;
                }

                if (teamInput.Equals("f", StringComparison.OrdinalIgnoreCase))
                {
                    if (playerTeam.Count >= 1)
                    {
                        buildingTeam = false;
                        break;
                    }
                    else
                    {
                        Console.WriteLine(
                            "🚨 You must deploy at least 1 hero before attacking! Press Enter."
                        );
                        Console.ReadLine();
                        continue;
                    }
                }

                if (
                    int.TryParse(teamInput, out int choiceIdx)
                    && choiceIdx >= 1
                    && choiceIdx <= uniqueSelectableRoster.Count
                )
                {
                    RPGHero selectedHero = uniqueSelectableRoster[choiceIdx - 1];

                    var existingHero = playerTeam.Find(p =>
                        p.Name.Equals(selectedHero.Name, StringComparison.OrdinalIgnoreCase)
                    );
                    if (existingHero != null)
                    {
                        playerTeam.Remove(existingHero);
                        continue;
                    }

                    if (playerTeam.Count >= 5)
                    {
                        Console.WriteLine(
                            "🚨 Deployment limit reached! Max 5 combatants per encounter row. Press Enter."
                        );
                        Console.ReadLine();
                        continue;
                    }

                    var blueprint = registry.Find(b =>
                        b.Name.Equals(selectedHero.Name, StringComparison.OrdinalIgnoreCase)
                    );
                    if (blueprint != null)
                    {
                        selectedHero.RawBaseHP = blueprint.BaseHP;
                        selectedHero.RawBaseAttack = blueprint.BaseAttack;
                        selectedHero.CritChance = blueprint.BaseCritChance;
                    }

                    playerTeam.Add(
                        new CombatantState
                        {
                            Name = selectedHero.Name,
                            MaxHP = selectedHero.MaxHP,
                            CurrentHP = selectedHero.MaxHP,
                            Attack = selectedHero.Attack,
                            CritChance = selectedHero.CritChance,
                            IsPlayer = true,
                        }
                    );
                }
                else
                {
                    Console.WriteLine("🚨 Invalid choice entry. Press Enter to retry.");
                    Console.ReadLine();
                }
            }

            if (currentStage == null)
                continue;

            // Initialize Enemy Squad Array
            List<CombatantState> enemyTeam = currentStage
                .Enemies.Select(e => new CombatantState
                {
                    Name = e.Name,
                    MaxHP = e.MaxHP,
                    CurrentHP = e.MaxHP,
                    Attack = e.Attack,
                    CritChance = 5,
                    IsPlayer = false,
                })
                .ToList();

            // --------------------------------------------------
            // ⚔️ AUTOMATED SQUAD BRAWL SIMULATION
            // --------------------------------------------------
            Console.Clear();
            Console.WriteLine("==================================================");
            if (currentStage.IsBossStage)
                Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine(
                currentStage.IsBossStage
                    ? "💀         TEAM HYBRID BOSS BRAWL START          💀"
                    : "🔥          SQUAD ENGAGEMENT INITIATING           🔥"
            );
            Console.ResetColor();
            Console.WriteLine("==================================================");
            Console.WriteLine(
                $" PLAYER SQUAD: {string.Join(", ", playerTeam.Select(p => p.Name))}"
            );
            Console.WriteLine($" ENEMY SQUAD:  {string.Join(", ", enemyTeam.Select(e => e.Name))}");
            Console.WriteLine("==================================================");
            Console.WriteLine("Press Enter to COMMENCE AUTOMATED BATTLE POOL...");
            Console.ReadLine();

            int roundCounter = 1;

            // Runs autonomously until a team is entirely wiped out
            while (playerTeam.Any(p => p.CurrentHP > 0) && enemyTeam.Any(e => e.CurrentHP > 0))
            {
                Console.Clear();
                Console.WriteLine($"==================================================");
                Console.WriteLine(
                    $"⚔️                COMBAT ROUND {roundCounter}                  ⚔️"
                );
                Console.WriteLine($"==================================================");

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(" YOUR TEAM STATUS:");
                foreach (var p in playerTeam)
                    Console.WriteLine(
                        p.CurrentHP > 0
                            ? $" 🐕 {p.Name, -18} HP: {p.CurrentHP:F0} / {p.MaxHP}"
                            : $" 💀 {p.Name, -18} [FAINTED]"
                    );

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\n ENEMY SQUAD STATUS:");
                foreach (var e in enemyTeam)
                    Console.WriteLine(
                        e.CurrentHP > 0
                            ? $" 👾 {e.Name, -18} HP: {e.CurrentHP:F0} / {e.MaxHP}"
                            : $" 💀 {e.Name, -18} [DEFEATED]"
                    );
                Console.ResetColor();
                Console.WriteLine("==================================================");

                // Pause slightly so players can assess team health updates
                System.Threading.Thread.Sleep(1000);

                // --- PLAYER ATTACK PHASE ---
                Console.WriteLine("\n⚡ YOUR SQUAD ATTACKS:");
                foreach (var attacker in playerTeam.Where(p => p.CurrentHP > 0))
                {
                    var target = enemyTeam.FirstOrDefault(e => e.CurrentHP > 0);
                    if (target == null)
                        break;

                    float damage = CalculateHit(
                        attacker.Attack,
                        attacker.CritChance,
                        out bool didCrit
                    );
                    target.CurrentHP -= damage;

                    if (didCrit)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine(
                            $"  ✨ [CRIT] {attacker.Name} shredded {target.Name} for {damage:F0} damage!"
                        );
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.WriteLine(
                            $"  💥 {attacker.Name} bit {target.Name} for {damage:F0} damage."
                        );
                    }
                    System.Threading.Thread.Sleep(350); // Fluid feed interval delay
                }

                if (!enemyTeam.Any(e => e.CurrentHP > 0))
                    break;

                // --- ENEMY ATTACK PHASE ---
                Console.WriteLine("\n⚡ ENEMY SQUAD RETALIATES:");
                foreach (var attacker in enemyTeam.Where(e => e.CurrentHP > 0))
                {
                    var target = playerTeam.FirstOrDefault(p => p.CurrentHP > 0);
                    if (target == null)
                        break;

                    float damage = CalculateHit(
                        attacker.Attack,
                        attacker.CritChance,
                        out bool didCrit
                    );
                    target.CurrentHP -= damage;

                    if (didCrit)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.WriteLine(
                            $"  🚨 [CRIT] {attacker.Name} violently gashed {target.Name} for {damage:F0} damage!"
                        );
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.WriteLine(
                            $"  🩸 {attacker.Name} struck {target.Name} for {damage:F0} damage."
                        );
                    }
                    System.Threading.Thread.Sleep(350);
                }

                roundCounter++;
                System.Threading.Thread.Sleep(1000); // Wait a second before automatically rolling over to the next round panel
            }

            // --------------------------------------------------
            // 🏆 OUTCOME MATCH RESOLUTION
            // --------------------------------------------------
            Console.Clear();
            if (playerTeam.Any(p => p.CurrentHP > 0))
            {
                Console.WriteLine("==================================================");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(
                    currentStage.IsBossStage
                        ? "🏆      VICTORY: CHOSEN SQUAD WIPED THE BOSS!      🏆"
                        : "🏆            SECTOR BRACKET CLEARED!              🏆"
                );
                Console.ResetColor();
                Console.WriteLine("==================================================");
                Console.WriteLine(
                    $" You successfully cleared {currentStage.StageName} after {roundCounter} rounds!"
                );
                Console.WriteLine($" Earned +{currentStage.TicketReward} Summon Tickets! 🎫");
                Console.WriteLine($" Earned +{currentStage.GoldReward} Gold Coins! 💰");
                Console.WriteLine($" Earned +{currentStage.XPReward} Trainer XP Chips! 🧪");

                tickets += currentStage.TicketReward;
                gold += currentStage.GoldReward;
                xpChips += currentStage.XPReward;

                Console.WriteLine("==================================================");
                Console.WriteLine(
                    $" Balance: {tickets} Tickets | {gold} Gold | {xpChips} XP Chips"
                );
                Console.WriteLine("==================================================");
                Console.WriteLine("Press Enter to return to Campaign Selection...");
                Console.ReadLine();
            }
            else
            {
                Console.WriteLine("==================================================");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("💀 SQUAD WIPEOUT! All deployed companions fainted. ");
                Console.ResetColor();
                Console.WriteLine("==================================================");
                Console.WriteLine(
                    $" Defeated on Stage: {currentStage.StageName} (Round {roundCounter})"
                );
                Console.WriteLine("==================================================");
                Console.WriteLine("Press Enter to return to the Campaign Map Selection.");
                Console.ReadLine();
            }
        }
    }

    private float CalculateHit(float baseAttack, int baseCritChance, out bool isCritical)
    {
        int critRoll = rand.Next(1, 101);
        isCritical = critRoll <= baseCritChance;

        double varianceMultiplier = rand.NextDouble() * (1.15 - 0.85) + 0.85;
        float finalDamage = baseAttack * (float)varianceMultiplier;

        if (isCritical)
        {
            finalDamage *= 1.5f;
        }

        return Math.Max(1, (float)Math.Round(finalDamage));
    }

    private int GetRarityWeight(string rarity)
    {
        return rarity switch
        {
            "Stray" => 1,
            "Packmate" => 2,
            "Scout" => 3,
            "Tracker" => 4,
            "Thicc Boi" => 5,
            "Alpha" => 6,
            "Pack Leader" => 7,
            _ => 0,
        };
    }

    private void LoadCampaignDatabase()
    {
        try
        {
            if (File.Exists(DatabasePath))
            {
                string jsonText = File.ReadAllText(DatabasePath);
                campaignDatabase =
                    JsonSerializer.Deserialize<List<CampaignStage>>(jsonText)
                    ?? new List<CampaignStage>();
                campaignDatabase = campaignDatabase.OrderBy(s => s.StageNumber).ToList();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Internal JSON Reader Error: {ex.Message}");
            campaignDatabase = new List<CampaignStage>();
        }
    }
}
