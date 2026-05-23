using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

public class RPGHero
{
    public string Name { get; set; } = "";
    public string Rarity { get; set; } = "";
    public int Level { get; set; } = 1;
    public int Quantity { get; set; } = 1;
    public int CritChance { get; set; } = 15;

    // 🛡️ GEAR SYSTEM SLOTS
    // Tracks what equipment is snapped onto this specific companion instance.
    public HeroGear Equipment { get; set; } = new HeroGear();

    [JsonIgnore]
    public int MaxLevelCap =>
        Rarity switch
        {
            "Stray" => 100,
            "Packmate" => 250,
            "Scout" => 400,
            "Tracker" => 600,
            "Thicc Boi" => 800,
            "Alpha" => 950,
            "Pack Leader" => int.MaxValue,
            _ => 100,
        };

    [JsonIgnore]
    public float RawBaseHP { get; set; }

    [JsonIgnore]
    public float RawBaseAttack { get; set; }

    // 🌟 STAT ENGINE: Combines level scaling formulas with your gear bonuses dynamically!
    [JsonIgnore]
    public float MaxHP =>
        MathF.Round(RawBaseHP * GetRarityMultiplier() + (Level - 1) * (RawBaseHP * 0.08f))
        + (Equipment?.CollarHPBonus ?? 0f)
        + (Equipment?.JacketHPBonus ?? 0f)
        + (Equipment?.PawsHPBonus ?? 0f)
        + (Equipment?.ToyHPBonus ?? 0f);

    [JsonIgnore]
    public float Attack =>
        MathF.Round(RawBaseAttack * GetRarityMultiplier() + (Level - 1) * (RawBaseAttack * 0.05f))
        + (Equipment?.CollarAtkBonus ?? 0f)
        + (Equipment?.JacketAtkBonus ?? 0f)
        + (Equipment?.PawsAtkBonus ?? 0f)
        + (Equipment?.ToyAtkBonus ?? 0f);

    public int CurrentXP { get; set; } = 0;

    [JsonIgnore]
    public int XPRequiredForNextLevel => (int)Math.Round(100 * Math.Pow(Level, 1.5));

    [JsonIgnore]
    public int GoldCostForNextLevel => (int)Math.Round(50 * Math.Pow(Level, 1.2));

    public RPGHero() { }

    public RPGHero(
        string name,
        string rarity,
        int level,
        float baseHP,
        float baseAttack,
        int quantity
    )
    {
        Name = name;
        Rarity = rarity;
        Level = level;
        RawBaseHP = baseHP;
        RawBaseAttack = baseAttack;
        Quantity = quantity;
    }

    private float GetRarityMultiplier()
    {
        return Rarity switch
        {
            "Stray" => 1.00f,
            "Packmate" => 1.15f,
            "Scout" => 1.32f,
            "Tracker" => 1.52f,
            "Thicc Boi" => 1.75f,
            "Alpha" => 2.01f,
            "Pack Leader" => 2.31f,
            _ => 1.00f,
        };
    }
}

// 🦴 GEAR INVENTORY WRAPPER CLASS
public class HeroGear
{
    // The hero's active slots store an ItemInstance once snapped in place!
    public ItemInstance? Collar { get; set; } = null;
    public ItemInstance? Jacket { get; set; } = null;
    public ItemInstance? Paws { get; set; } = null;
    public ItemInstance? Toy { get; set; } = null;

    [JsonIgnore]
    public float CollarHPBonus => Collar?.AddedHP ?? 0f;

    [JsonIgnore]
    public float CollarAtkBonus => Collar?.AddedAttack ?? 0f;

    [JsonIgnore]
    public float JacketHPBonus => Jacket?.AddedHP ?? 0f;

    [JsonIgnore]
    public float JacketAtkBonus => Jacket?.AddedAttack ?? 0f;

    [JsonIgnore]
    public float PawsHPBonus => Paws?.AddedHP ?? 0f;

    [JsonIgnore]
    public float PawsAtkBonus => Paws?.AddedAttack ?? 0f;

    [JsonIgnore]
    public float ToyHPBonus => Toy?.AddedHP ?? 0f;

    [JsonIgnore]
    public float ToyAtkBonus => Toy?.AddedAttack ?? 0f;
}

// 🧥 INDIVIDUAL ITEM PROPERTIES DATA PATTERN
public class EquippedItem
{
    public string ItemName { get; set; } = "";
    public string EquipmentType { get; set; } = ""; // "Collar", "Jacket", "Paws", "Toy"
    public string TierColor { get; set; } = "Common";
    public float AddedHP { get; set; } = 0f;
    public float AddedAttack { get; set; } = 0f;
}

// ==========================================================================
// 🌟 STATIC DATABASE BLUEPRINT MODEL (RESTORED)
// ==========================================================================
public class HeroBlueprint
{
    public string Name { get; set; } = "";
    public float BaseHP { get; set; }
    public float BaseAttack { get; set; }
    public int BaseCritChance { get; set; } = 15;
}
