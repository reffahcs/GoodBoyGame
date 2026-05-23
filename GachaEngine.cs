// 📁 GachaEngine.cs
using System;
using System.Collections.Generic;

public class GachaEngine
{
    private static readonly Random rand = new Random();

    // ==========================================================================
    // 🔮 THE DETERMINISTIC MASTER ENGAGEMENT LOOP (DUAL BANNER SYSTEM)
    // ==========================================================================
    public void OpenMenu(
        List<RPGHero> roster,
        List<HeroBlueprint> heroRegistry,
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
            Console.WriteLine(" [1] Summon Hero Companion (Costs 1 Ticket)");
            Console.WriteLine(" [2] Summon Gear & Equipment (Costs 1 Ticket)");
            Console.WriteLine(" [B] Return to Main Hub Menu");
            Console.WriteLine(
                "=========================================================================="
            );
            Console.Write("Selection: ");

            ConsoleKeyInfo choice = Console.ReadKey(true);

            if (choice.Key == ConsoleKey.B)
            {
                inGachaMenu = false;
            }
            // ==========================================
            // BANNER 1: HERO PULL
            // ==========================================
            else if (choice.Key == ConsoleKey.D1 || choice.Key == ConsoleKey.NumPad1)
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
                    ExecuteHeroRoll(roster, heroRegistry);

                    Console.WriteLine("\nPress [Enter] to roll HERO again, or [B] to return...");

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
            // ==========================================
            // BANNER 2: GEAR PULL
            // ==========================================
            else if (choice.Key == ConsoleKey.D2 || choice.Key == ConsoleKey.NumPad2)
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
                    ExecuteGearRoll(itemRegistry, inventory);

                    Console.WriteLine("\nPress [Enter] to roll GEAR again, or [B] to return...");

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

    // ==========================================================================
    // EXECUTION & REWARD LOGIC
    // ==========================================================================
    private void ExecuteHeroRoll(List<RPGHero> roster, List<HeroBlueprint> heroRegistry)
    {
        Console.WriteLine("\n🎬 Initiating Hero summon sequence...");
        System.Threading.Thread.Sleep(500);

        int winRoll = rand.Next(1, 101);
        if (winRoll > 100)
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
        if (winRoll > 25)
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
