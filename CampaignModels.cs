using System.Collections.Generic;

public class CampaignStage
{
    public int StageNumber { get; set; }
    public string StageName { get; set; }
    public bool IsBossStage { get; set; }
    public int TicketReward { get; set; }
    public int GoldReward { get; set; }
    public int XPReward { get; set; }
    public List<EnemyBlueprint> Enemies { get; set; } = new List<EnemyBlueprint>();
}

public class EnemyBlueprint
{
    public string Name { get; set; } = "";
    public float MaxHP { get; set; }
    public float Attack { get; set; }
}
