namespace NakkaResults.Models;

public class PointRules
{
    public static List<PointTable> PointTables => new List<PointTable>
    {
        new PointTable
        {
            MinNumberOfPlayers = 17,
            MaxNumberOfPlayers = 32,
            RobinPoints = new RobinPoints(20, 10, 10),
            RankPoints = new Dictionary<int, RankPoints>
            {
                { 0, new(0, 0) },
                { 1, new(1, 180) },
                { 2, new(2, 140) },
                { 3, new(3, 100) },
                { 5, new(5, 70) },
                { 9, new(9, 40) },
            },
        },
        new PointTable
        {
            MinNumberOfPlayers = 33,
            MaxNumberOfPlayers = 64,
            RobinPoints = new RobinPoints(40, 20, 10),
            RankPoints = new Dictionary<int, RankPoints>
            {
                { 0, new(0, 0) },
                { 1, new(1, 210) },
                { 2, new(2, 170) },
                { 3, new(3, 130) },
                { 5, new(5, 100) },
                { 9, new(9, 70) },
            },
        },
    };
}
