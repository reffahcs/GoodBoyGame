


[ Heroes.json ] (Static Blueprint Registry)
                         │
                         ▼
                ┌─────────────────┐
                │   GameHub.cs    │ ◄───► [ savegame.json ] (Player Save File)
                └────────┬────────┘
                         │
     ┌───────────────────┼───────────────────┐
     ▼                   ▼                   ▼
┌─────────────────┐ ┌─────────────────┐ ┌─────────────────┐
│ CampaignEngine  │ │   GachaEngine   │ │  RosterManager  │
│ (PvE Combat)    │ │ (Summoning Shop)│ │ (Inventory/Lab) │
└─────────────────┘ └─────────────────┘ └─────────────────┘


---

## Technical File Directory

### 1. Core Hub & State Controller (`GameHub.cs`)
Acts as the central state machine and application lifecycle manager. 
* **State Management:** Coordinates active player session variables (`playerRoster`, `summonTickets`, `playerGold`, `playerXPChips`).
* **Persistence Layer:** Manages serialization (`SaveGame`) and deserialization (`LoadGame`) pathways mapping to `savegame.json`.
* **Database Hydration:** Cross-references raw save data with `Heroes.json` at runtime via `LoadAndHydratePlayerSave` to rebind non-serialized base statistics (`RawBaseHP`, `RawBaseAttack`, `CritChance`).

### 2. Combat & Encounter Engine (`CampaignEngine.cs`)
Orchestrates turn-based tactical PvE stages.
* **Selection Logic:** Uses LINQ grouping filters to deduplicate player rosters, enforcing selection visibility on the highest-rarity, highest-level variant of any given hero stack.
* **Combat Mathematics:** Evaluates damage dynamically using a randomized variance algorithm (`CalculateHit`) generating a floating multiplier drifting between $\pm15\%$ ($0.85$ to $1.15$).
* **Critical Strikes:** Rolls individual character `CritChance` values on action execution, augmenting successful rolls with bright `ConsoleColor` status text and a $1.5\times$ ($150\%$) damage amplification multiplier.

### 3. Progression & Evolution Manager (`RosterManager.cs`)
Governs card infrastructure modifications, training, and merge mechanics.
* **Evolution Lab:** Facilitates duplicate hero card consumption ("fusions") to advance characters up a 7-tier rarity hierarchy (`Stray` $\rightarrow$ `Packmate` $\rightarrow$ `Scout` $\rightarrow$ `Tracker` $\rightarrow$ `Thicc Boi` $\rightarrow$ `Alpha` $\rightarrow$ `Pack Leader`).
* **Training Grounds:** Handles resource injections (`Gold` and `XP Chips`) to systematically elevate character levels. Updates are applied universally across all matching duplicate backend card records to preserve character stack integrity.

### 4. Character Data Models (`RPGHero.cs`)
Houses encapsulated entity attributes and structural math formulas.
* **Exponential Scaling:** Implements automated mathematical property getters to scale game systems exponentially:
  * *Next Level XP Requirement:* $100 \times (\text{Level}^{1.5})$
  * *Next Level Gold Cost:* $50 \times (\text{Level}^{1.2})$
* **Dynamic Property Projections:** Automatically scales combat capabilities at runtime via `[JsonIgnore]` property accessors, removing the need for flat-file synchronization:
  * $\text{MaxHP} = \text{Round}(\text{RawBaseHP} \times \text{RarityMultiplier} + (\text{Level} - 1) \times (\text{RawBaseHP} \times 0.10))$
  * $\text{Attack} = \text{Round}(\text{RawBaseAttack} \times \text{RarityMultiplier} + (\text{Level} - 1) \times (\text{RawBaseAttack} \times 0.08))$

### 5. Shared Blueprint Layouts (`CampaignModels.cs`)
Defines shared abstract structures mapped during structured JSON loading pipelines:
* `CampaignStage`: Captures `StageNumber`, `StageName`, loot allocations (`TicketReward`, `GoldReward`, `XPReward`), and a nested list collection of active enemy participants.
* `EnemyBlueprint`: Structural framework mapping basic baseline parameters for opponent profiles (`Name`, `MaxHP`, `Attack`).

---

## Data Schema Profiles

### Player Save Structure (`savegame.json`)
```json
{
  "Tickets": 10,
  "Gold": 500,
  "XPChips": 50,
  "Roster": [
    {
      "Name": "Sherlock Bones",
      "Rarity": "Stray",
      "Level": 1,
      "Quantity": 1,
      "CritChance": 15,
      "CurrentXP": 0
    }
  ]
}
Environment Configuration
Ignoring Local State Records
To prevent local testing profiles from checking into remote version control repositories, verify your project's root .gitignore file contains the local save profile rule:

Plaintext
# Ignore local player save profiles
savegame.json
If the file was committed prior to adding this rule, force-clear tracking from Git's cache via the terminal:

Bash
git rm --cached savegame.json
Building and Compilation
To clean temporary compilation buffers, restore project dependencies, and execute the console application wrapper:

Bash
# Erase internal compiler cache tracking
dotnet clean

# Verify dependencies and compile binaries
dotnet build

# Launch execution environment
dotnet run
"""