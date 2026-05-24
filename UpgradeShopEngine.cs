// 📁 UpgradeShopEngine.cs
using System;
using System.Collections.Generic;

public class UpgradeShopEngine
{
    private static readonly Random rand = new Random();
    private List<HeroBlueprint> featuredHeroes = new List<HeroBlueprint>();

    public void OpenMenu(
        List<RPGHero> roster,
        List<HeroBlueprint> heroRegistry,
        ref int tickets,
        ref int gold,
        ref int xpChips
    )
    {
        if (featuredHeroes.Count == 0)
        {
            RefreshFeaturedHeroes(heroRegistry);
        }

        bool inShop = true;
        while (inShop)
        {
            while (Console.KeyAvailable)
            {
                Console.ReadKey(true);
            }

            Console.Clear();
            Console.WriteLine(
                "=========================================================================="
            );
            Console.WriteLine(
                "🌟                       HERO UPGRADE PACK SHOP                           🌟"
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

            if (featuredHeroes.Count == 0)
            {
                Console.WriteLine(" [!] Error: Hero database is empty.");
            }
            else
            {
                for (int i = 0; i < featuredHeroes.Count; i++)
                {
                    Console.WriteLine(
                        $" [{i + 1}] Buy 20x {featuredHeroes[i].Name} (Costs 50 Tickets)"
                    );
                }
            }

            Console.WriteLine(
                "--------------------------------------------------------------------------"
            );
            Console.WriteLine(" [R] Refresh Featured Heroes (Costs 20 Tickets)");
            Console.WriteLine(" [B] Return to Shops");
            Console.WriteLine(
                "=========================================================================="
            );
            Console.Write("Selection: ");

            ConsoleKeyInfo choice = Console.ReadKey(true);

            if (choice.Key == ConsoleKey.B)
                inShop = false;
            else if (choice.Key == ConsoleKey.R)
            {
                if (tickets < 20)
                {
                    Console.WriteLine("\n🚨 Insufficient tickets!");
                    System.Threading.Thread.Sleep(1000);
                }
                else
                {
                    tickets -= 20;
                    RefreshFeaturedHeroes(heroRegistry);
                }
            }
            else
            {
                int selectedIndex = -1;
                if (choice.Key >= ConsoleKey.D1 && choice.Key <= ConsoleKey.D6)
                    selectedIndex = choice.Key - ConsoleKey.D1;
                else if (choice.Key >= ConsoleKey.NumPad1 && choice.Key <= ConsoleKey.NumPad6)
                    selectedIndex = choice.Key - ConsoleKey.NumPad1;

                if (selectedIndex >= 0 && selectedIndex < featuredHeroes.Count)
                {
                    if (tickets < 50)
                    {
                        Console.WriteLine("\n🚨 Need 50 tickets!");
                        System.Threading.Thread.Sleep(1000);
                    }
                    else
                    {
                        tickets -= 50;
                        AwardHeroPack(roster, featuredHeroes[selectedIndex]);
                        Console.ReadKey();
                    }
                }
            }
        }
    }

    private void RefreshFeaturedHeroes(List<HeroBlueprint> heroRegistry)
    {
        featuredHeroes.Clear();
        if (heroRegistry == null || heroRegistry.Count == 0)
            return;
        for (int i = 0; i < 6; i++)
        {
            featuredHeroes.Add(heroRegistry[rand.Next(heroRegistry.Count)]);
        }
    }

    private void AwardHeroPack(List<RPGHero> roster, HeroBlueprint blueprint)
    {
        var existingHero = roster.Find(h =>
            h.Name.Equals(blueprint.Name, StringComparison.OrdinalIgnoreCase)
        );
        if (existingHero != null)
            existingHero.Quantity += 20;
        else
            roster.Add(
                new RPGHero(blueprint.Name, "Stray", 1, blueprint.BaseHP, blueprint.BaseAttack, 20)
            );
        Console.WriteLine($"\n📦 Purchased 20x {blueprint.Name}!");
    }
}
