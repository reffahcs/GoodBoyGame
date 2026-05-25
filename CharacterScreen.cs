// 📁 CharacterScreen.cs
using System;
using System.Collections.Generic;
using System.Linq;

public class CharacterScreen
{
    public void OpenMenu(
        List<RPGHero> roster,
        List<HeroBlueprint> registry,
        InventoryManager inventory, // <-- ADDED DEPENDENCY
        ref int gold,
        ref int xpChips
    )
    {
        bool inMenu = true;
        while (inMenu)
        {
            Console.Clear();
            Console.WriteLine(
                "=========================================================================="
            );
            Console.WriteLine(
                "🐕                       PACK CHARACTER PROFILE MANAGEMENT                 🐕"
            );
            Console.WriteLine(
                "=========================================================================="
            );
            Console.WriteLine(
                $" Wallet Balance: 💰 {gold} Gold Coins | 🧪 {xpChips} Trainer XP Chips"
            );
            Console.WriteLine(
                "--------------------------------------------------------------------------"
            );

            if (roster == null || roster.Count == 0)
            {
                Console.WriteLine(
                    " Your pack roster is completely empty! Go summon some companions."
                );
                Console.WriteLine("\n [B] Return to Main Hub Menu");
                Console.WriteLine(
                    "=========================================================================="
                );
                Console.Write("Choice: ");
                string backChoice = Console.ReadLine()?.Trim() ?? "";
                if (backChoice.Equals("b", StringComparison.OrdinalIgnoreCase))
                    inMenu = false;
                continue;
            }

            for (int i = 0; i < roster.Count; i++)
            {
                var h = roster[i];
                Console.WriteLine(
                    $" [{i + 1}] {h.Name, -20} Lv.{h.Level, -4} [{h.Rarity, -11}] Stacks: x{h.Quantity}"
                );
            }

            Console.WriteLine(
                "--------------------------------------------------------------------------"
            );
            Console.WriteLine(" [B] Cancel and Return to Main Hub Menu");
            Console.WriteLine(
                "=========================================================================="
            );
            Console.Write("Select a companion profile to inspect: ");

            string input = Console.ReadLine()?.Trim() ?? "";
            if (input.Equals("b", StringComparison.OrdinalIgnoreCase))
            {
                inMenu = false;
                continue;
            }

            if (
                int.TryParse(input, out int choiceIdx)
                && choiceIdx >= 1
                && choiceIdx <= roster.Count
            )
            {
                InspectCharacterProfile(
                    roster[choiceIdx - 1],
                    registry,
                    inventory,
                    ref gold,
                    ref xpChips
                );
            }
        }
    }

    private void InspectCharacterProfile(
        RPGHero hero,
        List<HeroBlueprint> registry,
        InventoryManager inventory, // <-- ADDED DEPENDENCY
        ref int gold,
        ref int xpChips
    )
    {
        bool inspecting = true;
        while (inspecting)
        {
            var blueprint = registry.Find(b =>
                b.Name.Equals(hero.Name, StringComparison.OrdinalIgnoreCase)
            );
            if (blueprint != null)
            {
                hero.RawBaseHP = blueprint.BaseHP;
                hero.RawBaseAttack = blueprint.BaseAttack;
            }

            Console.Clear();
            Console.WriteLine("==================================================");
            Console.WriteLine($"🐕        VITAL CHARACTER STATS: {hero.Name.ToUpper()}");
            Console.WriteLine("==================================================");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($" Rarity Classification: [{hero.Rarity}]");
            Console.WriteLine($" Current Level Rating:  Lv. {hero.Level} / {hero.MaxLevelCap}");
            Console.ResetColor();
            Console.WriteLine("--------------------------------------------------");

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($" ❤️ Current Hit Points:  {hero.MaxHP:F0} HP");
            Console.WriteLine($" ⚔️ Core Attack Level:  {hero.Attack:F0} ATK");
            Console.WriteLine($" ✨ Critical Strike:    {hero.CritChance}%");
            Console.ResetColor();
            Console.WriteLine("--------------------------------------------------");

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine(" [ CURRENT COMPANION GEAR SLOTS ]");
            Console.WriteLine(
                $"  Collar: {(hero.Equipment?.Collar != null ? $"🟢 [{hero.Equipment.Collar.TierColor}] {hero.Equipment.Collar.ItemName}" : "[Empty]")}"
            );
            Console.WriteLine(
                $"  Jacket: {(hero.Equipment?.Jacket != null ? $"🟢 [{hero.Equipment.Jacket.TierColor}] {hero.Equipment.Jacket.ItemName}" : "[Empty]")}"
            );
            Console.WriteLine(
                $"  Paws:   {(hero.Equipment?.Paws != null ? $"🟢 [{hero.Equipment.Paws.TierColor}] {hero.Equipment.Paws.ItemName}" : "[Empty]")}"
            );
            Console.WriteLine(
                $"  Toy:    {(hero.Equipment?.Toy != null ? $"🟢 [{hero.Equipment.Toy.TierColor}] {hero.Equipment.Toy.ItemName}" : "[Empty]")}"
            );
            Console.ResetColor();
            Console.WriteLine("--------------------------------------------------");

            int xpCost = hero.XPRequiredForNextLevel;
            int goldCost = hero.GoldCostForNextLevel;

            if (hero.Level >= hero.MaxLevelCap)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(" ⭐ MAX LEVEL REACHED! Promote rarity tier to break cap.");
                Console.ResetColor();
            }
            else
            {
                Console.WriteLine($" Next Level Upgrade Costs:");
                Console.WriteLine($"  🧪 Required Training Chips: {xpCost} (You have: {xpChips})");
                Console.WriteLine($"  💰 Required Gold Reserve:   {goldCost} (You have: {gold})");
            }

            Console.WriteLine("==================================================");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(" 🟢 [ENTER] Instant Level Up");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(" 🛡️ [E] Equip Gear         [A] Auto-Equip Best");
            Console.WriteLine(" 🧹 [U] Unequip Slot       [X] Strip All Gear");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(" 🔴 [B] Return to character selection grid");
            Console.ResetColor();
            Console.WriteLine("==================================================");

            ConsoleKeyInfo key = Console.ReadKey(intercept: true);

            // ==========================================
            // GEAR COMMAND ROUTING
            // ==========================================
            if (key.Key == ConsoleKey.A)
            {
                EquipmentEngine.AutoEquipBestGear(hero, inventory);
                System.Threading.Thread.Sleep(1500);
            }
            else if (key.Key == ConsoleKey.X)
            {
                EquipmentEngine.UnequipAll(hero, inventory);
                System.Threading.Thread.Sleep(1200);
            }
            else if (key.Key == ConsoleKey.U)
            {
                Console.Write("\n Which slot to unequip? [C]ollar, [J]acket, [P]aws, [T]oy: ");
                var slotKey = Console.ReadKey(true);
                string target = DetermineSlotName(slotKey.Key);

                if (target != "")
                    EquipmentEngine.UnequipItem(hero, target, inventory);
                else
                    Console.WriteLine("\n Invalid selection.");

                System.Threading.Thread.Sleep(1000);
            }
            else if (key.Key == ConsoleKey.E)
            {
                Console.Write("\n Which slot to equip? [C]ollar, [J]acket, [P]aws, [T]oy: ");
                var slotKey = Console.ReadKey(true);
                string target = DetermineSlotName(slotKey.Key);

                if (target == "")
                {
                    Console.WriteLine("\n Invalid selection.");
                    System.Threading.Thread.Sleep(800);
                    continue;
                }

                var availableGear = inventory
                    .Stash.Where(i =>
                        i.Details.EquipmentType.Equals(target, StringComparison.OrdinalIgnoreCase)
                    )
                    .ToList();

                if (availableGear.Count == 0)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(
                        $"\n 🚨 No unequipped {target} items found in your backpack."
                    );
                    Console.ResetColor();
                    System.Threading.Thread.Sleep(1200);
                    continue;
                }

                Console.Clear();
                Console.WriteLine($"==================================================");
                Console.WriteLine($"🎒 AVAILABLE {target.ToUpper()} GEAR IN BACKPACK");
                Console.WriteLine($"==================================================");
                for (int i = 0; i < availableGear.Count; i++)
                {
                    var item = availableGear[i].Details;
                    Console.WriteLine(
                        $" [{i + 1}] [{item.TierColor}] {item.ItemName} | +{item.AddedHP} HP | +{item.AddedAttack} ATK | Stock: {availableGear[i].Quantity}"
                    );
                }
                Console.WriteLine("--------------------------------------------------");
                Console.Write(" Select an item number to equip (or B to cancel): ");

                string itemChoice = Console.ReadLine()?.Trim() ?? "";
                if (int.TryParse(itemChoice, out int idx) && idx >= 1 && idx <= availableGear.Count)
                {
                    EquipmentEngine.EquipItem(hero, availableGear[idx - 1].Details, inventory);
                    System.Threading.Thread.Sleep(1200);
                }
            }
            // ==========================================
            // EXISTING LOGIC (LEVEL UP & EXIT)
            // ==========================================
            else if (key.Key == ConsoleKey.Enter)
            {
                if (hero.Level >= hero.MaxLevelCap)
                {
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.WriteLine(
                        "\n🚨 Level cap hit! Fuse multiple stack cards in the Laboratory menu."
                    );
                    Console.ResetColor();
                    System.Threading.Thread.Sleep(1000);
                    continue;
                }

                if (xpChips >= xpCost && gold >= goldCost)
                {
                    xpChips -= xpCost;
                    gold -= goldCost;
                    hero.Level++;

                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine($"\n✨ LEVEL UP! {hero.Name} grew to Level {hero.Level}! ✨");
                    Console.ResetColor();
                    System.Threading.Thread.Sleep(300);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n🚨 Insufficient resources! Complete more campaign maps.");
                    Console.ResetColor();
                    System.Threading.Thread.Sleep(1000);
                }
            }
            else if (key.Key == ConsoleKey.B || key.KeyChar == 'b' || key.KeyChar == 'B')
            {
                inspecting = false;
            }
        }
    }

    private string DetermineSlotName(ConsoleKey key)
    {
        return key switch
        {
            ConsoleKey.C => "Collar",
            ConsoleKey.J => "Jacket",
            ConsoleKey.P => "Paws",
            ConsoleKey.T => "Toy",
            _ => "",
        };
    }
}
