// 📁 RosterManager.cs
using System;
using System.Collections.Generic;
using System.Linq;

public class RosterManager
{
    private readonly List<string> rarityScale = new List<string>
    {
        "Stray", // Tier 1 (Max Level: 100)
        "Packmate", // Tier 2 (Max Level: 250)
        "Scout", // Tier 3 (Max Level: 400)
        "Tracker", // Tier 4 (Max Level: 600)
        "Thicc Boi", // Tier 5 (Max Level: 800)
        "Alpha", // Tier 6 (Max Level: 950)
        "Pack Leader", // Tier 7 (Max Tier - Uncapped!)
    };

    private int GetCopiesRequired(string rarity)
    {
        return rarity switch
        {
            "Stray" => 20,
            "Packmate" => 15,
            "Scout" => 10,
            "Tracker" => 8,
            "Thicc Boi" => 5,
            "Alpha" => 2,
            _ => 0,
        };
    }

    // ==========================================================================
    // 🗺️ STEP 2 FLOW: ROSTER MANAGEMENT OVERVIEW (ACCESSED VIA CASE 3)
    // ==========================================================================
    public void OpenMenu(
        List<RPGHero> roster,
        InventoryManager inventory,
        ref int gold,
        ref int xpChips
    )
    {
        bool inRosterMenu = true;
        while (inRosterMenu)
        {
            Console.Clear();
            Console.WriteLine(
                "=========================================================================="
            );
            Console.WriteLine(
                "🐾                       PACK CHARACTER ROSTER                           🐾"
            );
            Console.WriteLine(
                "=========================================================================="
            );

            if (roster == null || roster.Count == 0)
            {
                Console.WriteLine(" Your pack roster is currently empty!");
                Console.WriteLine(
                    "=========================================================================="
                );
                Console.WriteLine(" [B] Return to Main Hub Menu");
                Console.WriteLine(
                    "=========================================================================="
                );
                Console.Write("Choice: ");
                string emptyChoice = Console.ReadLine()?.Trim() ?? "";
                if (emptyChoice.Equals("b", StringComparison.OrdinalIgnoreCase))
                    inRosterMenu = false;
                continue;
            }

            // Group and sort options exactly how your original grid display logic functioned
            List<RPGHero> sortedOptions = roster
                .OrderByDescending(r => r.Level)
                .ThenByDescending(r => GetRarityIndex(r.Rarity))
                .ToList();

            for (int i = 0; i < sortedOptions.Count; i++)
            {
                var h = sortedOptions[i];
                Console.WriteLine(
                    $" [{i + 1}] {h.Name, -20} Lv.{h.Level, -4} [{h.Rarity, -11}] Owned Stacks: x{h.Quantity}"
                );
            }

            Console.WriteLine(
                "=========================================================================="
            );
            Console.WriteLine(
                " [Index #] Select a Companion Card to Open Character Detail Profile Sheet"
            );
            Console.WriteLine(" [M]       Enter Breeding Laboratory (Merge Duplicate Stacks)");
            Console.WriteLine(" [B]       Return to Main Hub Menu");
            Console.WriteLine(
                "=========================================================================="
            );
            Console.Write("Selection: ");

            string input = Console.ReadLine()?.Trim() ?? "";

            if (input.Equals("b", StringComparison.OrdinalIgnoreCase))
            {
                inRosterMenu = false;
                continue;
            }

            if (input.Equals("m", StringComparison.OrdinalIgnoreCase))
            {
                // Route directly into your laboratory logic block
                RunLabMenuFlow(sortedOptions, roster);
                continue;
            }

            if (
                int.TryParse(input, out int choiceIdx)
                && choiceIdx >= 1
                && choiceIdx <= sortedOptions.Count
            )
            {
                // 🗺️ FLOW STEP 3: Go down into the detailed leveling profile page
                OpenHeroDetailProfile(sortedOptions[choiceIdx - 1], ref gold, ref xpChips);
            }
        }
    }

    // ==========================================================================
    // 🗺️ STEP 3 FLOW: HERO DETAIL SCREEN (RAPID ENTER-KEY UPGRADE INTERACTION)
    // ==========================================================================
    private void OpenHeroDetailProfile(RPGHero hero, ref int gold, ref int xpChips)
    {
        bool viewingDetails = true;
        while (viewingDetails)
        {
            Console.Clear();
            Console.WriteLine("==================================================");
            Console.WriteLine($"🐕        HERO PROFILE SHEET: {hero.Name.ToUpper()}");
            Console.WriteLine("==================================================");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($" Rarity Grade:  [{hero.Rarity}]");
            Console.WriteLine($" Current Level:  Lv. {hero.Level} / {hero.MaxLevelCap}");
            Console.ResetColor();
            Console.WriteLine("--------------------------------------------------");

            // 📊 STAT VIEW SHEET
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($" ❤️ Max Hit Points:     {hero.MaxHP:F0} HP");
            Console.WriteLine($" ⚔️ Attack Level:       {hero.Attack:F0} ATK");
            Console.WriteLine($" ✨ Critical Strike:    {hero.CritChance}%");
            Console.ResetColor();
            Console.WriteLine("--------------------------------------------------");

            // 🛡️ FUTURE PROOF TOUCH SCREEN GEAR SLOTS DISPLAY
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine(" [ EQUIPMENT & ARTIFACT SLOTS ]");
            Console.WriteLine("  Collar: [ Empty ]           Paws:   [ Empty ]");
            Console.WriteLine("  Jacket: [ Empty ]           Toy:    [ Empty ]");
            Console.ResetColor();
            Console.WriteLine("--------------------------------------------------");

            // Pull dynamic mathematical pricing right from your scaling formulas inside RPGHero.cs
            int xpCost = hero.XPRequiredForNextLevel;
            int goldCost = hero.GoldCostForNextLevel;

            Console.WriteLine($" Balances: 💰 {gold} Gold Coins | 🧪 {xpChips} XP Chips");

            if (hero.Level >= hero.MaxLevelCap)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(
                    " ⭐ MAX LEVEL REACHED! Merge duplicate cards in Lab to break cap."
                );
                Console.ResetColor();
            }
            else
            {
                Console.WriteLine($" Cost to Upgrade:  💰 {goldCost} Gold | 🧪 {xpCost} XP Chips");
            }

            Console.WriteLine("==================================================");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(" 🟢 Press [ENTER] to instantly Level Up");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(" 🔴 Press [B] to return to Pack Roster List");
            Console.ResetColor();
            Console.WriteLine("==================================================");

            // Snaps keyboard input from buffer instantly without requiring an input line enter
            ConsoleKeyInfo keyStroke = Console.ReadKey(intercept: true);

            if (keyStroke.Key == ConsoleKey.Enter)
            {
                if (hero.Level >= hero.MaxLevelCap)
                {
                    System.Threading.Thread.Sleep(400);
                    continue;
                }

                if (xpChips >= xpCost && gold >= goldCost)
                {
                    xpChips -= xpCost;
                    gold -= goldCost;
                    hero.Level++;

                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine($"\n✨ LEVEL UP! {hero.Name} is now Level {hero.Level}! ✨");
                    Console.ResetColor();
                    System.Threading.Thread.Sleep(180); // Quick snappy feedback pacing
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(
                        "\n🚨 Insufficient currency reserves! Run Campaign sector maps."
                    );
                    Console.ResetColor();
                    System.Threading.Thread.Sleep(800);
                }
            }
            else if (
                keyStroke.Key == ConsoleKey.B
                || keyStroke.KeyChar == 'b'
                || keyStroke.KeyChar == 'B'
            )
            {
                viewingDetails = false;
            }
        }
    }

    // ==========================================================================
    // 🧪 BREEDING LABORATORY SYSTEM SUB-CONTEXT (Your original fusion system)
    // ==========================================================================
    private void RunLabMenuFlow(List<RPGHero> currentSortedView, List<RPGHero> mainRoster)
    {
        Console.Clear();
        Console.WriteLine(
            "=========================================================================="
        );
        Console.WriteLine(
            "🧪                        BREEDING LABORATORY                             🧪"
        );
        Console.WriteLine(
            "=========================================================================="
        );

        for (int i = 0; i < currentSortedView.Count; i++)
        {
            var h = currentSortedView[i];
            Console.WriteLine(
                $" [{i + 1}] {h.Name, -20} Lv.{h.Level, -4} [{h.Rarity, -11}] Stacks: x{h.Quantity}"
            );
        }
        Console.WriteLine(
            "--------------------------------------------------------------------------"
        );
        Console.Write("Select target duplicate card index to mutate/evolve: ");

        string choiceInput = Console.ReadLine()?.Trim() ?? "";
        if (
            !int.TryParse(choiceInput, out int targetIdx)
            || targetIdx < 1
            || targetIdx > currentSortedView.Count
        )
        {
            Console.WriteLine("🚨 Cancelled mutation sequence. Press Enter.");
            Console.ReadLine();
            return;
        }

        RPGHero targetHero = currentSortedView[targetIdx - 1];
        string targetRarity = targetHero.Rarity;
        int costToDeduct = GetCopiesRequired(targetRarity);

        if (targetHero.Quantity < costToDeduct)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(
                $"\n🚨 Fusion Failure: Insufficient fragments! Need x{costToDeduct} copies, you have x{targetHero.Quantity}."
            );
            Console.ResetColor();
            Console.ReadLine();
            return;
        }

        int currentRankIndex = GetRarityIndex(targetRarity);
        if (currentRankIndex >= rarityScale.Count - 1)
        {
            Console.WriteLine(
                "\n🚨 Already achieved maximum Pack Leader configuration tier status!"
            );
            Console.ReadLine();
            return;
        }

        // Process materials out of active collection stack
        targetHero.Quantity -= costToDeduct;
        if (targetHero.Quantity <= 0)
        {
            mainRoster.Remove(targetHero);
        }

        string nextRarityName = rarityScale[currentRankIndex + 1];
        targetHero.Rarity = nextRarityName;

        var existingStackMatch = mainRoster.Find(r =>
            r.Name.Equals(targetHero.Name, StringComparison.OrdinalIgnoreCase)
            && r.Rarity.Equals(targetHero.Rarity, StringComparison.OrdinalIgnoreCase)
            && r.Level == targetHero.Level
        );

        if (existingStackMatch != null)
        {
            existingStackMatch.Quantity++;
        }
        else
        {
            targetHero.Quantity = 1;
            mainRoster.Add(targetHero);
        }

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"\n✨ SUCCESS: Merged x{costToDeduct} [{targetRarity}] duplicates!");
        Console.WriteLine(
            $"🔥 [{targetRarity}] -> [{nextRarityName}] (Level {targetHero.Level} Preserved!)"
        );
        Console.ResetColor();
        Console.ReadLine();
    }

    private int GetRarityIndex(string rarity)
    {
        int index = rarityScale.FindIndex(r =>
            r.Equals(rarity, StringComparison.OrdinalIgnoreCase)
        );
        return index >= 0 ? index : 0;
    }
}
