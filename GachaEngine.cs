// 📁 GachaEngine.cs
using System;
using System.Collections.Generic;

public class GachaEngine
{
    private static readonly Random rand = new Random();

    // ==========================================================================
    // 🔮 THE DETERMINISTIC MASTER ENGAGEMENT LOOP (NO COMPROMISES)
    // ==========================================================================
    public void OpenMenu(
        List<RPGHero> roster,
        List<HeroBlueprint> heroRegistry, // <--- NEW: Dynamic Hero Pool
        List<GearBlueprint> itemRegistry,
        InventoryManager inventory,
        ref int tickets,
        ref int gold,
        ref int xpChips
    )
    {
        bool inGachaMenu = true;
        while (inGachaMenu)
        {
            // Clear buffer to ensure immediate response
            while (Console.KeyAvailable)
            {
                Console.ReadKey(true);
            }

            Console.Clear();
            Console.WriteLine(
                "=========================================================================="
            );
            Console.WriteLine(
                "🔮                         GACHA SUMMON SHOP                              🔮"
            );
            Console.WriteLine(
                "=========================================================================="
            );
            Console.WriteLine(
                $" Current Balances: 🎫 Tickets: {tickets} | 💰 Gold: {gold} | 🧪 XP Chips: {xpChips}"
            );
            Console.WriteLine(
                "--------------------------------------------------------------------------"
            );
            Console.WriteLine(" [1] Perform Single Gacha Pull (Costs 1 Ticket)");
            Console.WriteLine(" [B] Return to Main Hub Menu");
            Console.WriteLine(
                "=========================================================================="
            );
            Console.Write("Selection: ");

            // Immediate reaction without needing Enter
            ConsoleKeyInfo choice = Console.ReadKey(true);

            if (choice.Key == ConsoleKey.B)
            {
                inGachaMenu = false;
            }
            else if (
                choice.Key == ConsoleKey.D1
                || choice.Key == ConsoleKey.NumPad1
                || choice.Key == ConsoleKey.Enter
            )
            {
                bool keepRolling = true;
                while (keepRolling)
                {
                    if (tickets < 1)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(
                            "\n🚨 Insufficient tickets! Clear campaign sectors or check rewards."
                        );
                        Console.ResetColor();
                        System.Threading.Thread.Sleep(1000);
                        break;
                    }

                    tickets--;
                    ExecuteThreeTieredRoll(roster, heroRegistry, itemRegistry, inventory);

                    Console.WriteLine("\nPress [Enter] to roll again, or [B] to return...");

                    while (Console.KeyAvailable)
                    {
                        Console.ReadKey(true);
                    }
                    ConsoleKeyInfo rollChoice = Console.ReadKey(true);

                    if (rollChoice.Key == ConsoleKey.B)
                    {
                        keepRolling = false;
                        inGachaMenu = false;
                    }
                    else if (rollChoice.Key == ConsoleKey.Enter)
                    {
                        Console.WriteLine(
                            "\n--------------------------------------------------------------------------"
                        );
                    }
                    else
                    {
                        keepRolling = false;
                    }
                }
            }
            else
            {
                Console.WriteLine("\n🚨 Input vector invalid.");
                System.Threading.Thread.Sleep(500);
            }
        }
    }

    private void ExecuteThreeTieredRoll(
        List<RPGHero> roster,
        List<HeroBlueprint> heroRegistry,
        List<GearBlueprint> itemRegistry,
        InventoryManager inventory
    )
    {
        Console.WriteLine("\n🎬 Initiating roll sequence...");
        System.Threading.Thread.Sleep(500);

        int winRoll = rand.Next(1, 101);
        if (winRoll > 40)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("💨 A swing and a miss! Better luck on the next pull.");
            Console.ResetColor();
            return;
        }

        if (itemRegistry == null || itemRegistry.Count == 0)
        {
            AwardHeroPrize(roster, heroRegistry);
            return;
        }

        int prizeTypeRoll = rand.Next(1, 3);
        if (prizeTypeRoll == 1)
        {
            AwardHeroPrize(roster, heroRegistry);
        }
        else
        {
            AwardItemPrize(itemRegistry, inventory);
        }
    }

    private void AwardHeroPrize(List<RPGHero> roster, List<HeroBlueprint> heroRegistry)
    {
        string rolledHeroName = "Mutt-zo Ball";
        float rolledHP = 105f;
        float rolledAtk = 13f;

        // Dynamic Roll Logic
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
