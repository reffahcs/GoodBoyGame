// 📁 Items.cs
using System;
using System.Collections.Generic;

// ==========================================================================
// 🧭 STATIC DATABASE TEMPLATE (Parsed from ItemsDB.json)
// ==========================================================================
public class GearBlueprint
{
    public string ItemID { get; set; } = ""; // Unique template identifier (e.g., "collar_spikes_03")
    public string ItemName { get; set; } = ""; // Base display name (e.g., "Bad Dog Spiked Collar")
    public string EquipmentType { get; set; } = ""; // Target slot: "Collar", "Jacket", "Paws", "Toy"
    public float BaseHPBonus { get; set; }
    public float BaseAttackBonus { get; set; }
}

// ==========================================================================
// 🎁 THE INDIVIDUAL GENERATED ITEM OBJECT (The unique result of your rolls)
// ==========================================================================
public class ItemInstance
{
    public string ItemID { get; set; } = ""; // Keeps a paper-trail back to the origin template ID
    public string ItemName { get; set; } = "";
    public string EquipmentType { get; set; } = ""; // "Collar", "Jacket", "Paws", "Toy"
    public string TierColor { get; set; } = "Common"; // Assigned color tier: "Common", "Rare", "Epic", "Legendary"
    public float AddedHP { get; set; }
    public float AddedAttack { get; set; }
}

// ==========================================================================
// 🎲 THE THREE-TIERED GACHA ITEM CALCULATION ENGINE
// ==========================================================================
public static class ItemEngine
{
    private static readonly Random rand = new Random();

    /// <summary>
    /// Phase 3 of your flow: Evaluates the lucky prize type and rolls its standalone quality tier.
    /// </summary>
    public static ItemInstance RollItemRarity(GearBlueprint blueprint)
    {
        int roll = rand.Next(1, 101);
        string finalTier = "Common";
        float statMultiplier = 1.0f;

        // Custom weight thresholds for your gacha distribution
        if (roll > 95)
        {
            finalTier = "Legendary";
            statMultiplier = 2.5f; // Premium multiplier
        }
        else if (roll > 80)
        {
            finalTier = "Epic";
            statMultiplier = 1.75f;
        }
        else if (roll > 50)
        {
            finalTier = "Rare";
            statMultiplier = 1.3f;
        }

        return new ItemInstance
        {
            ItemID = blueprint.ItemID,
            ItemName = blueprint.ItemName,
            EquipmentType = blueprint.EquipmentType,
            TierColor = finalTier,
            // MathF.Round keeps console stat readouts clean and integer-friendly
            AddedHP = MathF.Round(blueprint.BaseHPBonus * statMultiplier),
            AddedAttack = MathF.Round(blueprint.BaseAttackBonus * statMultiplier),
        };
    }
}
