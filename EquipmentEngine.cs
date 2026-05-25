// 📁 EquipmentEngine.cs
using System;
using System.Collections.Generic;
using System.Linq;

public static class EquipmentEngine
{
    // Matches the actual EquipmentType strings in your ItemsDB.json
    private static readonly string[] ValidSlots = { "Collar", "Jacket", "Paws", "Toy" };

    // ==========================================================================
    // ⚙️ MANUAL EQUIP LOGIC
    // ==========================================================================
    public static void EquipItem(RPGHero hero, ItemInstance newItem, InventoryManager inventory)
    {
        // 1. If the slot is already occupied, take the old gear off first
        if (GetSlotItem(hero, newItem.EquipmentType) != null)
        {
            UnequipItem(hero, newItem.EquipmentType, inventory);
        }

        // 2. Put the new item on the hero
        SetSlotItem(hero, newItem.EquipmentType, newItem);

        // 3. Remove it safely from the backpack stack
        inventory.RemoveItem(newItem.ItemID, newItem.TierColor);

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($" ✅ Equipped [{newItem.TierColor}] {newItem.ItemName} to {hero.Name}.");
        Console.ResetColor();
    }

    public static void UnequipItem(RPGHero hero, string slot, InventoryManager inventory)
    {
        ItemInstance oldItem = GetSlotItem(hero, slot);
        if (oldItem != null)
        {
            // Add back to backpack and clear the hero's slot
            inventory.AddItem(oldItem);
            SetSlotItem(hero, slot, null);

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($" ↩️ Unequipped {oldItem.ItemName} from {slot}.");
            Console.ResetColor();
        }
    }

    public static void UnequipAll(RPGHero hero, InventoryManager inventory)
    {
        Console.WriteLine($"\n 🧹 Stripping all gear from {hero.Name}...");
        bool strippedAnything = false;

        foreach (string slot in ValidSlots)
        {
            if (GetSlotItem(hero, slot) != null)
            {
                UnequipItem(hero, slot, inventory);
                strippedAnything = true;
            }
        }

        if (!strippedAnything)
        {
            Console.WriteLine(" Hero is already completely unequipped.");
        }
    }

    // ==========================================================================
    // 🤖 AUTO-EQUIP LOGIC
    // ==========================================================================
    public static void AutoEquipBestGear(RPGHero hero, InventoryManager inventory)
    {
        Console.WriteLine($"\n🔍 Scanning stash to optimize gear for {hero.Name}...");
        bool equippedAnything = false;

        foreach (string slot in ValidSlots)
        {
            // 1. Find all items in the backpack for this specific slot
            var availableItems = inventory
                .Stash.Where(i =>
                    i.Details.EquipmentType.Equals(slot, StringComparison.OrdinalIgnoreCase)
                )
                .Select(i => i.Details) // Extract the ItemInstance from the InventoryItem wrapper
                .ToList();

            if (availableItems.Count == 0)
                continue;

            // 2. Find the highest scoring item in the backpack for this slot
            ItemInstance bestStashItem = availableItems
                .OrderByDescending(i => CalculateGearScore(i))
                .First();
            float bestStashScore = CalculateGearScore(bestStashItem);

            bool shouldSwap = false;
            ItemInstance currentGear = GetSlotItem(hero, slot);

            // 3. Compare it against what they are currently wearing
            if (currentGear == null)
            {
                shouldSwap = true;
            }
            else
            {
                if (bestStashScore > CalculateGearScore(currentGear))
                {
                    shouldSwap = true;
                }
            }

            // 4. Perform the swap
            if (shouldSwap)
            {
                EquipItem(hero, bestStashItem, inventory);
                equippedAnything = true;
            }
        }

        if (!equippedAnything)
            Console.WriteLine(" 🛡️ Hero is already wearing the best available gear.");
        else
            Console.WriteLine(" ✨ Auto-Equip complete!");
    }

    // ==========================================================================
    // 🧮 INTERNAL MATH & HELPERS
    // ==========================================================================
    private static float CalculateGearScore(ItemInstance item)
    {
        float attackWeight = item.AddedAttack * 5f;
        float hpWeight = item.AddedHP * 1f;

        float rarityWeight = item.TierColor switch
        {
            "Rare" => 5f,
            "Epic" => 10f,
            "Legendary" => 15f,
            _ => 0f,
        };

        return attackWeight + hpWeight + rarityWeight;
    }

    // Bridge helpers to interact with your specific HeroGear class properties
    private static ItemInstance? GetSlotItem(RPGHero hero, string slot) =>
        slot.ToLower() switch
        {
            "collar" => hero.Equipment?.Collar,
            "jacket" => hero.Equipment?.Jacket,
            "paws" => hero.Equipment?.Paws,
            "toy" => hero.Equipment?.Toy,
            _ => null,
        };

    private static void SetSlotItem(RPGHero hero, string slot, ItemInstance? item)
    {
        if (hero.Equipment == null)
            hero.Equipment = new HeroGear();
        switch (slot.ToLower())
        {
            case "collar":
                hero.Equipment.Collar = item;
                break;
            case "jacket":
                hero.Equipment.Jacket = item;
                break;
            case "paws":
                hero.Equipment.Paws = item;
                break;
            case "toy":
                hero.Equipment.Toy = item;
                break;
        }
    }
}
