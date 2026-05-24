// 📁 GachaEngine.cs
using System;
using System.Collections.Generic;

public class GachaEngine
{
    private static readonly Random rand = new Random();

    // ==========================================================================
    // 🔮 BANNER 1: CONTINUOUS HERO PULL LOOP
    // ==========================================================================
    public void OpenHeroBanner(
        List<RPGHero> roster,
        List<HeroBlueprint> heroRegistry,
        ref int tickets
    )
    {
        bool keepRolling = true;
        while (keepRolling)
        {
            Console.Clear();
            Console.WriteLine(
                "=========================================================================="
            );
            Console.WriteLine(
                "🔮                         HERO SUMMON BANNER                             🔮"
            );
            Console.WriteLine(
                "=========================================================================="
            );
            Console.WriteLine($" Current Balance: 🎫 Tickets: {tickets}");
            Console.WriteLine(
                "--------------------------------------------------------------------------"
            );

            if (tickets < 1)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(
                    "\n🚨 Insufficient tickets! Clear campaign sectors or check rewards."
                );
                Console.ResetColor();
                System.Threading.Thread.Sleep(1500);
                break;
            }

            tickets--;
            ExecuteHeroRoll(roster, heroRegistry);

            Console.WriteLine("\nPress [Enter] to roll HERO again, or [B] to return to Shops...");

            while (Console.KeyAvailable)
            {
                Console.ReadKey(true);
            }
            ConsoleKeyInfo rollChoice = Console.ReadKey(true);

            if (rollChoice.Key == ConsoleKey.B)
            {
                keepRolling = false;
            }
        }
    }

    // ==========================================================================
    // 🔮 BANNER 2: CONTINUOUS GEAR PULL LOOP
    // ==========================================================================
    public void OpenGearBanner(
        List<GearBlueprint> itemRegistry,
        InventoryManager inventory,
        ref int tickets
    )
    {
        bool keepRolling = true;
        while (keepRolling)
        {
            Console.Clear();
            Console.WriteLine(
                "=========================================================================="
            );
            Console.WriteLine(
                "⚔️                         GEAR SUMMON BANNER                             ⚔️"
            );
            Console.WriteLine(
                "=========================================================================="
            );
            Console.WriteLine($" Current Balance: 🎫 Tickets: {tickets}");
            Console.WriteLine(
                "--------------------------------------------------------------------------"
            );

            if (tickets < 1)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(
                    "\n🚨 Insufficient tickets! Clear campaign sectors or check rewards."
                );
                Console.ResetColor();
                System.Threading.Thread.Sleep(1500);
                break;
            }

            tickets--;
            ExecuteGearRoll(itemRegistry, inventory);

            Console.WriteLine("\nPress [Enter] to roll GEAR again, or [B] to return to Shops...");

            while (Console.KeyAvailable)
            {
                Console.ReadKey(true);
            }
            ConsoleKeyInfo rollChoice = Console.ReadKey(true);

            if (rollChoice.Key == ConsoleKey.B)
            {
                keepRolling = false;
            }
        }
    }

    // ==========================================================================
    // EXECUTION & REWARD LOGIC
    // ==========================================================================
    private void ExecuteHeroRoll(List<RPGHero> roster, List<HeroBlueprint> heroRegistry)
    {
        Console.WriteLine("\n🎬 Initiating Hero summon sequence...");
        System.Threading.Thread.Sleep(500);

        int winRoll = rand.Next(1, 101);
        if (winRoll > 99)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("💨 A swing and a miss! Better luck on the next pull.");
            Console.ResetColor();
            return;
        }

        AwardHeroPrize(roster, heroRegistry);
    }

    private void ExecuteGearRoll(List<GearBlueprint> itemRegistry, InventoryManager inventory)
    {
        Console.WriteLine("\n🎬 Initiating Gear summon sequence...");
        System.Threading.Thread.Sleep(500);

        int winRoll = rand.Next(1, 101);
        if (winRoll > 40)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("💨 A swing and a miss! Better luck on the next pull.");
            Console.ResetColor();
            return;
        }

        if (itemRegistry != null && itemRegistry.Count > 0)
        {
            AwardItemPrize(itemRegistry, inventory);
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("🚨 Error: Gear Registry is empty.");
            Console.ResetColor();
        }
    }

    private void AwardHeroPrize(List<RPGHero> roster, List<HeroBlueprint> heroRegistry)
    {
        string rolledHeroName = "Mutt-zo Ball";
        float rolledHP = 105f;
        float rolledAtk = 13f;

        if (heroRegistry != null && heroRegistry.Count > 0)
        {
            int index = rand.Next(heroRegistry.Count);
            HeroBlueprint rolledBlueprint = heroRegistry[index];

            rolledHeroName = rolledBlueprint.Name;
            rolledHP = rolledBlueprint.BaseHP;
            rolledAtk = rolledBlueprint.BaseAttack;
        }

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"\n🌟 CHARACTER DROP! You pulled: {rolledHeroName}!");
        Console.ResetColor();

        var existingHero = roster.Find(h =>
            h.Name.Equals(rolledHeroName, StringComparison.OrdinalIgnoreCase)
        );
        if (existingHero != null)
        {
            existingHero.Quantity++;
            Console.WriteLine(
                $"   ↳ Stack Updated: Stash count increased to x{existingHero.Quantity}."
            );
        }
        else
        {
            roster.Add(new RPGHero(rolledHeroName, "Stray", 1, rolledHP, rolledAtk, 1));
            Console.WriteLine(
                "   ↳ New Addition: Character successfully added to active roster list."
            );
        }
    }

    private void AwardItemPrize(List<GearBlueprint> itemRegistry, InventoryManager inventory)
    {
        int index = rand.Next(itemRegistry.Count);
        GearBlueprint baseBlueprint = itemRegistry[index];

        ItemInstance rolledGear = ItemEngine.RollItemRarity(baseBlueprint);
        inventory.AddItem(rolledGear);

        switch (rolledGear.TierColor)
        {
            case "Legendary":
                Console.ForegroundColor = ConsoleColor.Yellow;
                break;
            case "Epic":
                Console.ForegroundColor = ConsoleColor.Magenta;
                break;
            case "Rare":
                Console.ForegroundColor = ConsoleColor.Cyan;
                break;
            default:
                Console.ForegroundColor = ConsoleColor.White;
                break;
        }

        Console.WriteLine(
            $"\n🎁 GEAR DROP! You pulled a [{rolledGear.TierColor}] {rolledGear.ItemName} ({rolledGear.EquipmentType})!"
        );
        Console.ResetColor();
        Console.WriteLine(
            $"   ↳ Stats: +{rolledGear.AddedHP} Max HP | +{rolledGear.AddedAttack} Base ATK"
        );

        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine(
            "   ↳ Placement: Sent directly into your unequipped gear backpack storage."
        );
        Console.ResetColor();
    }
}
