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
    // 🗺️ STEP 2 FLOW: ROSTER MANAGEMENT OVERVIEW
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
                RunLabMenuFlow(sortedOptions, roster);
                continue;
            }

            if (
                int.TryParse(input, out int choiceIdx)
                && choiceIdx >= 1
                && choiceIdx <= sortedOptions.Count
            )
            {
                OpenHeroDetailProfile(
                    sortedOptions[choiceIdx - 1],
                    inventory,
                    ref gold,
                    ref xpChips
                );
            }
        }
    }

    // ==========================================================================
    // 🗺️ STEP 3 FLOW: HERO DETAIL SCREEN & 1-TOUCH GEAR MANAGEMENT
    // ==========================================================================
    private void OpenHeroDetailProfile(
        RPGHero hero,
        InventoryManager inventory,
        ref int gold,
        ref int xpChips
    )
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

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($" ❤️ Max Hit Points:     {hero.MaxHP:F0} HP");
            Console.WriteLine($" ⚔️ Attack Level:       {hero.Attack:F0} ATK");
            Console.WriteLine($" ✨ Critical Strike:    {hero.CritChance}%");
            Console.ResetColor();
            Console.WriteLine("--------------------------------------------------");

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine(" [ EQUIPMENT & ARTIFACT SLOTS ]");

            PrintGearSlot("Collar", hero.Equipment?.Collar, "1");
            PrintGearSlot("Jacket", hero.Equipment?.Jacket, "2");
            PrintGearSlot("Paws", hero.Equipment?.Paws, "3");
            PrintGearSlot("Toy", hero.Equipment?.Toy, "4");

            Console.ResetColor();
            Console.WriteLine("--------------------------------------------------");

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
            Console.WriteLine(" 🟢 [ENTER] Instant Level Up");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(" 🛡️ [1-4] Manage Gear Slot   [A] Auto-Equip Best");
            Console.WriteLine(" 🧹 [X] Strip All Gear");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(" 🔴 [B] Return to Pack Roster List");
            Console.ResetColor();
            Console.WriteLine("==================================================");

            // 1-Touch intercept! No Enter key required.
            ConsoleKeyInfo keyStroke = Console.ReadKey(intercept: true);

            if (keyStroke.Key == ConsoleKey.A)
            {
                EquipmentEngine.AutoEquipBestGear(hero, inventory);
                System.Threading.Thread.Sleep(1500);
            }
            else if (keyStroke.Key == ConsoleKey.X)
            {
                EquipmentEngine.UnequipAll(hero, inventory);
                System.Threading.Thread.Sleep(1200);
            }
            // Route to 1-Touch Submenus
            else if (keyStroke.KeyChar == '1')
                OpenSlotManager(hero, "Collar", inventory);
            else if (keyStroke.KeyChar == '2')
                OpenSlotManager(hero, "Jacket", inventory);
            else if (keyStroke.KeyChar == '3')
                OpenSlotManager(hero, "Paws", inventory);
            else if (keyStroke.KeyChar == '4')
                OpenSlotManager(hero, "Toy", inventory);
            else if (keyStroke.Key == ConsoleKey.Enter)
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
                    System.Threading.Thread.Sleep(180);
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
    // ⚡ 1-TOUCH SLOT MANAGER
    // ==========================================================================
    private void OpenSlotManager(RPGHero hero, string slotName, InventoryManager inventory)
    {
        // Sort by pure stats so the best gear naturally populates the 1-9 hotkeys
        var availableGear = inventory
            .Stash.Where(i =>
                i.Details.EquipmentType.Equals(slotName, StringComparison.OrdinalIgnoreCase)
            )
            .OrderByDescending(i => i.Details.AddedHP + i.Details.AddedAttack)
            .ToList();

        Console.Clear();
        Console.WriteLine($"==================================================");
        Console.WriteLine($"🎒 MANAGE {slotName.ToUpper()}");
        Console.WriteLine($"==================================================");

        ItemInstance current = slotName switch
        {
            "Collar" => hero.Equipment?.Collar,
            "Jacket" => hero.Equipment?.Jacket,
            "Paws" => hero.Equipment?.Paws,
            "Toy" => hero.Equipment?.Toy,
            _ => null,
        };

        Console.Write(" Current: ");
        if (current != null)
        {
            Console.ForegroundColor = GetRarityColor(current.TierColor);
            Console.Write("● ");
            Console.ResetColor();
            Console.WriteLine($"[{current.TierColor}] {current.ItemName}");
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("[ Empty ]");
            Console.ResetColor();
        }
        Console.WriteLine("--------------------------------------------------");

        if (availableGear.Count == 0)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($" No unequipped {slotName} items in backpack.");
            Console.ResetColor();
        }
        else
        {
            // Limit loop to 9 to guarantee 1-touch numeric mapping
            for (int i = 0; i < availableGear.Count && i < 9; i++)
            {
                var item = availableGear[i].Details;
                Console.Write($" [{i + 1}] ");

                Console.ForegroundColor = GetRarityColor(item.TierColor);
                Console.Write("● ");
                Console.ResetColor();

                Console.WriteLine(
                    $"[{item.TierColor}] {item.ItemName} | +{item.AddedHP} HP | +{item.AddedAttack} ATK | Stock: {availableGear[i].Quantity}"
                );
            }
        }

        Console.WriteLine("--------------------------------------------------");
        if (current != null)
            Console.WriteLine(" [U] Unequip Slot");
        Console.WriteLine(" [B] Back to Profile");

        // 1-Touch Intercept
        ConsoleKeyInfo key = Console.ReadKey(true);

        if ((key.Key == ConsoleKey.U || key.KeyChar == 'u') && current != null)
        {
            EquipmentEngine.UnequipItem(hero, slotName, inventory);
            System.Threading.Thread.Sleep(800);
        }
        else if (char.IsDigit(key.KeyChar))
        {
            int choice = int.Parse(key.KeyChar.ToString());
            if (choice >= 1 && choice <= availableGear.Count && choice <= 9)
            {
                EquipmentEngine.EquipItem(hero, availableGear[choice - 1].Details, inventory);
                System.Threading.Thread.Sleep(800);
            }
        }
    }

    // ==========================================================================
    // 🎨 UI RENDERING HELPERS
    // ==========================================================================
    private void PrintGearSlot(string slotName, ItemInstance item, string hotkey)
    {
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.Write($"  [{hotkey}] {slotName, -7}: ");

        if (item == null)
        {
            Console.WriteLine("[ Empty ]");
        }
        else
        {
            Console.ForegroundColor = GetRarityColor(item.TierColor);
            Console.Write("● ");
            Console.ResetColor();
            Console.WriteLine($"[{item.TierColor}] {item.ItemName}");
        }
    }

    private ConsoleColor GetRarityColor(string rarity)
    {
        return rarity?.ToLower() switch
        {
            "common" => ConsoleColor.Gray,
            "rare" => ConsoleColor.Blue, // Or swap to ConsoleColor.Cyan if Blue is too dark in your command prompt
            "epic" => ConsoleColor.Magenta,
            "legendary" => ConsoleColor.Yellow,
            _ => ConsoleColor.Gray,
        };
    }

    // ==========================================================================
    // 🧪 BREEDING LABORATORY SYSTEM SUB-CONTEXT
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

        targetHero.Quantity -= costToDeduct;
        if (targetHero.Quantity <= 0)
            mainRoster.Remove(targetHero);

        string nextRarityName = rarityScale[currentRankIndex + 1];
        targetHero.Rarity = nextRarityName;

        var existingStackMatch = mainRoster.Find(r =>
            r.Name.Equals(targetHero.Name, StringComparison.OrdinalIgnoreCase)
            && r.Rarity.Equals(targetHero.Rarity, StringComparison.OrdinalIgnoreCase)
            && r.Level == targetHero.Level
        );

        if (existingStackMatch != null)
            existingStackMatch.Quantity++;
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
