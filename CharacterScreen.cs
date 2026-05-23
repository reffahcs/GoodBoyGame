// 📁 CharacterScreen.cs
using System;
using System.Collections.Generic;
using System.Linq;

public class CharacterScreen
{
    public void OpenMenu(
        List<RPGHero> roster,
        List<HeroBlueprint> registry,
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

            // Display a distinct unique list of characters currently owned
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
                InspectCharacterProfile(roster[choiceIdx - 1], registry, ref gold, ref xpChips);
            }
        }
    }

    private void InspectCharacterProfile(
        RPGHero hero,
        List<HeroBlueprint> registry,
        ref int gold,
        ref int xpChips
    )
    {
        bool inspecting = true;
        while (inspecting)
        {
            // Hydrate properties smoothly from standard blueprint definitions
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

            // 📊 DISPLAY STAT VALUES
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($" ❤️ Current Hit Points:  {hero.MaxHP:F0} HP");
            Console.WriteLine($" ⚔️ Core Attack Level:  {hero.Attack:F0} ATK");
            Console.WriteLine($" ✨ Critical Strike:    {hero.CritChance}%");
            Console.ResetColor();
            Console.WriteLine("--------------------------------------------------");

            // 🛡️ DYNAMIC EQUIP STATUS CODES
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine(" [ CURRENT COMPANION GEAR SLOTS ]");
            Console.WriteLine(
                $"  Collar: {(hero.Equipment?.Collar != null ? $"🟢 {hero.Equipment.Collar.ItemName}" : "[Empty]")}"
            );
            Console.WriteLine(
                $"  Jacket: {(hero.Equipment?.Jacket != null ? $"🟢 {hero.Equipment.Jacket.ItemName}" : "[Empty]")}"
            );
            Console.WriteLine(
                $"  Paws:   {(hero.Equipment?.Paws != null ? $"🟢 {hero.Equipment.Paws.ItemName}" : "[Empty]")}"
            );
            Console.WriteLine(
                $"  Toy:    {(hero.Equipment?.Toy != null ? $"🟢 {hero.Equipment.Toy.ItemName}" : "[Empty]")}"
            );
            Console.ResetColor();
            Console.WriteLine("--------------------------------------------------");

            // 🧪 XP REQUIREMENTS
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
            Console.WriteLine(" 🟢 Press [ENTER] to instantly Level Up");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(" 🔴 Press [B] to return to character selection grid");
            Console.ResetColor();
            Console.WriteLine("==================================================");

            // Intercept hotkey clicks directly
            ConsoleKeyInfo key = Console.ReadKey(intercept: true);

            if (key.Key == ConsoleKey.Enter)
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
                    System.Threading.Thread.Sleep(300); // Snappy feedback pause
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
}
