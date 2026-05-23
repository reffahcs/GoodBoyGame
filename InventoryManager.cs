// 📁 InventoryManager.cs
using System;
using System.Collections.Generic;
using System.Linq;

public class InventoryItem
{
    // Wraps around your instance data and adds a structural tracking stack count
    public ItemInstance Details { get; set; } = new ItemInstance();
    public int Quantity { get; set; } = 1;
}

public class InventoryManager
{
    // Master storage array for every single unequipped prize won by the player
    public List<InventoryItem> Stash { get; set; } = new List<InventoryItem>();

    /// <summary>
    /// Smoothly adds an incoming item prize to the unequipped backpack collection.
    /// Automatically stacks item instances matching the exact template ID and quality tier.
    /// </summary>
    public void AddItem(ItemInstance item)
    {
        if (item == null)
            return;

        // Find a matching unequipped item stack in your gear backpack
        var existingStack = Stash.Find(i =>
            i.Details.ItemID.Equals(item.ItemID, StringComparison.OrdinalIgnoreCase)
            && i.Details.TierColor.Equals(item.TierColor, StringComparison.OrdinalIgnoreCase)
        );

        if (existingStack != null)
        {
            existingStack.Quantity++;
        }
        else
        {
            Stash.Add(new InventoryItem { Details = item, Quantity = 1 });
        }
    }

    /// <summary>
    /// Removes an item instance completely or reduces its unequipped quantity pool when snapped onto a dog.
    /// </summary>
    public bool RemoveItem(string itemId, string tierColor)
    {
        var existingStack = Stash.Find(i =>
            i.Details.ItemID.Equals(itemId, StringComparison.OrdinalIgnoreCase)
            && i.Details.TierColor.Equals(tierColor, StringComparison.OrdinalIgnoreCase)
        );

        if (existingStack == null)
            return false;

        existingStack.Quantity--;
        if (existingStack.Quantity <= 0)
        {
            Stash.Remove(existingStack);
        }
        return true;
    }

    /// <summary>
    /// Pure diagnostic visual rendering to inspect unequipped gear inventories via debug panels.
    /// </summary>
    public void DisplayInventory()
    {
        Console.Clear();
        Console.WriteLine(
            "=========================================================================="
        );
        Console.WriteLine(
            "📦                    UNEQUIPPED GEAR BACKPACK                           📦"
        );
        Console.WriteLine(
            "=========================================================================="
        );

        if (Stash.Count == 0)
        {
            Console.WriteLine(" Your storage backpack is currently completely empty!");
            return;
        }

        // Categorize by target gear slots to make sorting natural for players
        var sortedStash = Stash
            .OrderBy(i => i.Details.EquipmentType)
            .ThenByDescending(i => i.Details.TierColor);

        int counter = 1;
        foreach (var item in sortedStash)
        {
            Console.WriteLine(
                $" [{counter++}] [{item.Details.TierColor, -9}] {item.Details.ItemName, -22} | Slot: {item.Details.EquipmentType, -7} | Stock: x{item.Quantity}"
            );
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine(
                $"     ↳ Boosts: +{item.Details.AddedHP} HP  |  +{item.Details.AddedAttack} ATK"
            );
            Console.ResetColor();
        }
    }
}
