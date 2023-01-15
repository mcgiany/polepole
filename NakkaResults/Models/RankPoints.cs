namespace NakkaResults.Models;

public class RankPoints
{
    public int Rank { get; set; }

    public int Points { get; set; }

    public RankPoints(int rank, int points)
    {
        Rank = rank;
        Points = points;
    }
}
