using Mcgiany.NakkaClient.Entities;
using NakkaResults.Models;

namespace NakkaResults.Utils;

public class RobinRound
{
    public static RobinRoundGroup BuildGroup(Dictionary<string, Dictionary<string, GameScore>> rrResult, int groupNumber,
        List<Dictionary<string, int>> robinRoundRank)
    {
        var group = new RobinRoundGroup
        {
            GroupNumber = groupNumber,
        };
        foreach (var player in rrResult)
        {
            foreach (var opponent in player.Value)
            {
                var match = new GroupMatch
                {
                    Player = player.Key,
                    Opponent = opponent.Key,
                    Average = opponent.Value.Average,
                    PlayerScore = opponent.Value.LegsWin
                };
                group.Matches.Add(match);
            }
        }
        foreach (var player in rrResult)
        {
            foreach (var opponent in player.Value)
            {
                var reverseMatch = group.Matches.Where(x => x.Player == opponent.Key && x.Opponent == player.Key).FirstOrDefault();
                if (reverseMatch != null)
                {
                    reverseMatch.OpponentScore = opponent.Value.LegsWin;
                }
            }
        }
        var forcedRanks = new Dictionary<string, int>();
        foreach (var groupRanks in robinRoundRank.Where(x => x != null))
        {
            foreach (var groupRank in groupRanks)
            {
                forcedRanks.Add(groupRank.Key, groupRank.Value);
            }
        }
        group.PlayerStats = group.Matches
            .GroupBy(x => x.Player)
            .ToDictionary(k => k.Key, v => new GroupPlayerStats
            {
                Player = v.Key,
                Points = v.Count(m => m.PlayerScore > m.OpponentScore) * 2,
                LegDiff = v.Sum(l => l.PlayerScore - l.OpponentScore),
                BestAvg = v.Max(a => a.Average),
                ForcedRank = forcedRanks.ContainsKey(v.Key) ? forcedRanks[v.Key] : 0,
            });
        var rank = 1;
        foreach (var player in group.PlayerStats.OrderByDescending(x => x.Value.Points).ThenByDescending(x => x.Value.LegDiff).ThenBy(x => x.Value.ForcedRank))
        {
            player.Value.Rank = rank;
            rank++;
        }

        return group;
    }
}
