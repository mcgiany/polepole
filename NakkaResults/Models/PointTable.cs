namespace NakkaResults.Models;

public class PointTable
{
    public int MinNumberOfPlayers { get; set; }

    public int MaxNumberOfPlayers { get; set; }

    public Dictionary<int, RankPoints> RankPoints { get; set; } = null!;

    public RobinPoints RobinPoints { get; set; } = null!;
}
