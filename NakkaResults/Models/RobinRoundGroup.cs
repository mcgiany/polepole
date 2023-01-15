namespace NakkaResults.Models;

public class RobinRoundGroup
{
    public int GroupNumber { get; set; }

    public List<GroupMatch> Matches { get; set; } = new List<GroupMatch>();

    public Dictionary<string, GroupPlayerStats> PlayerStats { get; set; } = new Dictionary<string, GroupPlayerStats>();
}

public class GroupMatch
{
    public string Player { get; set; } = null!;

    public string Opponent { get; set; } = null!;

    public decimal Average { get; set; }

    public int PlayerScore { get; set; }

    public int OpponentScore { get; set; }
}

public class GroupPlayerStats
{
    public string Player { get; set; } = null!;

    public int Points { get; set; }

    public int LegDiff { get; set; }

    public decimal BestAvg { get; set; }

    public int Rank { get; set; }

    public int ForcedRank { get; set; }
}