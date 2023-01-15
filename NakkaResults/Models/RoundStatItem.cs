namespace NakkaResults.Models;

public class RoundStatItem
{
    public string PlayerName { get; set; } = null!;

    public int TotalScore { get; set; }

    public int TotalDarts { get; set; }

    public int SetsWin { get; set; }

    public int Sets { get; set; }

    public int LegsWin { get; set; }

    public int Legs { get; set; }

    public int Ton80Count { get; set; }

    public int Ton40Count { get; set; }

    public int Ton00Count { get; set; }

    public int HighOut { get; set; }

    public int BestLeg { get; set; }

    public decimal BestAvg { get; set; }

    public int Points { get; set; }

    public int Appearances { get; set; }
}
