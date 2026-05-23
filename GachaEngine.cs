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

            string input = Console.ReadLine()?.Trim() ?? "";

            if (input.Equals("b", StringComparison.OrdinalIgnoreCase))
            {
                inGachaMenu = false;
                continue;
            }

            if (input == "1")
            {
                if (tickets < 1)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(
                        "\n🚨 Insufficient tickets! Clear campaign sectors or check rewards."
                    );
                    Console.ResetColor();
                    System.Threading.Thread.Sleep(1000);
                    continue;
                }

                tickets--;
                ExecuteThreeTieredRoll(roster, itemRegistry, inventory);
                Console.WriteLine("\nPress Enter to continue...");
                Console.ReadLine();
            }
            else
            {
                Console.WriteLine("\n🚨 Input vector invalid. Re-evaluating terminal loops...");
                System.Threading.Thread.Sleep(1000);
            }
        }
    }

    // ==========================================================================
    // 🎰 THREE-TIER PIPELINE: WIN CHECK -> TYPE CHECK -> RARITY STAT SCALE
    // ==========================================================================
    private void ExecuteThreeTieredRoll(
        List<RPGHero> roster,
        List<GearBlueprint> itemRegistry,
        InventoryManager inventory
    )
    {
        Console.WriteLine("\n🎬 Initiating roll sequence...");
        System.Threading.Thread.Sleep(500);

        // FLOW ROLL 1: Evaluate win vector against hardcoded drop rates (e.g., 40% success rate)
        int winRoll = rand.Next(1, 101);
        if (winRoll > 40)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("💨 A swing and a miss! Better luck on the next pull.");
            Console.ResetColor();
            return;
        }

        // FLOW ROLL 2: Separate prize allocation matching category availability
        if (itemRegistry == null || itemRegistry.Count == 0)
        {
            // Forced fallback to character drops if item configuration data is missing
            AwardHeroPrize(roster);
            return;
        }

        int prizeTypeRoll = rand.Next(1, 3); // Evaluates exactly to 1 or 2
        if (prizeTypeRoll == 1)
        {
            AwardHeroPrize(roster);
        }
        else
        {
            AwardItemPrize(itemRegistry, inventory);
        }
    }

    private void AwardHeroPrize(List<RPGHero> roster)
    {
        string rolledHeroName = "Mutt-zo Ball";
        float rolledHP = 105f;
        float rolledAtk = 13f;

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(
            $"\n🌟 CHARACTER DROP! You pulled a duplicate copy of: {rolledHeroName}!"
        );
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

        // FLOW ROLL 3: The ItemEngine applies dynamic rarity coefficients to stats
        ItemInstance rolledGear = ItemEngine.RollItemRarity(baseBlueprint);

        // Push directly to player unequipped inventory stash
        inventory.AddItem(rolledGear);

        // Color coordinate console output matching quality level tiers
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
