using Mcgiany.NakkaClient;
using Mcgiany.NakkaClient.Entities;
using Mcgiany.NakkaClient.Enums;
using Mcgiany.NakkaClient.Extensions;
using Microsoft.AspNetCore.Mvc;
using NakkaResults.Models;
using NakkaResults.Utils;
using NakkaResults.ViewModels.Home;
using System.Diagnostics;

namespace NakkaResults.Controllers;

[Route("/")]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IConfiguration _configuration;
    private readonly INakkaClient _nakkaClient;

    public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        _logger = logger;
        _configuration = configuration;
        _nakkaClient = new ApiClient("https://tk2-228-23746.vs.sakura.ne.jp/n01");
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        ViewData["Title"] = "Zoznam sezón";
        var leagueIds = _configuration.GetSection("Leagues").Get<string[]>();
        var leagues = new List<NakkaLeague>();
        foreach (var leagueId in leagueIds)
        {
            var league = await _nakkaClient.GetLeagueAsync(leagueId);
            leagues.Add(league);
        }
        return View(leagues);
    }

    [HttpGet("League/{leagueId}/Rounds")]
    public async Task<IActionResult> LeagueRounds(string leagueId)
    {
        ViewData["Title"] = "Zoznam kôl";
        var league = await _nakkaClient.GetLeagueAsync(leagueId);
        var rounds = await GetRoundList(leagueId);
        var viewModel = new LeagueRoundsViewModel
        {
            League = league,
            RoundList = rounds
        };
        return View(viewModel);
    }

    [HttpGet("Round/{roundId}")]
    public async Task<IActionResult> Round(string roundId)
    {
        ViewData["Title"] = "Poradie v kole";
        var viewModel = new HomeRoundViewModel();
        viewModel.RoundStats = await GetRoundStats(roundId);
        viewModel.Round = await _nakkaClient.GetTournamentAsync(roundId);
        viewModel.LeagueId = viewModel.Round.LeagueId;
        return View(viewModel);
    }

    [HttpGet("Round/{roundId}/Complete")]
    [ResponseCache(Duration = 3600, VaryByQueryKeys = new[] { "roundId" })]
    public async Task<IActionResult> CurrentLeagueStats(string roundId)
    {
        ViewData["Title"] = "Celkové poradie po kole";
        var viewModel = new HomeRoundViewModel();
        viewModel.Complete = true;
        var selectedRound = await _nakkaClient.GetTournamentAsync(roundId);
        viewModel.Round = selectedRound;
        viewModel.LeagueId = selectedRound.LeagueId;
        var rounds = await GetRoundList(selectedRound.LeagueId);
        var allStats = new List<RoundStatItem>();
        foreach (var round in rounds.Where(x => x.TournamentDate <= selectedRound.TournamentDate))
        {
            var roundStats = await GetRoundStats(round.TournamentId);
            allStats.AddRange(roundStats);
        }
        viewModel.RoundStats = allStats.GroupBy(x => x.PlayerName).Select(y => new RoundStatItem
        {
            PlayerName = y.Key,
            TotalScore = y.Sum(z => z.TotalScore),
            TotalDarts = y.Sum(z => z.TotalDarts),
            SetsWin = y.Sum(z => z.SetsWin),
            Sets = y.Sum(z => z.Sets),
            LegsWin = y.Sum(z => z.LegsWin),
            Legs = y.Sum(z => z.Legs),
            Ton80Count = y.Sum(z => z.Ton80Count),
            Ton40Count = y.Sum(z => z.Ton40Count),
            Ton00Count = y.Sum(z => z.Ton00Count),
            HighOut = y.Max(z => z.HighOut),
            BestLeg = y.Min(z => z.BestLeg),
            BestAvg = y.Max(z => z.BestAvg),
            Points = y.Sum(z => z.Points),
            Appearances = y.Count(),
        }).ToList();
        return View("Round", viewModel);
    }

    private async Task<List<NakkaRound>> GetRoundList(string leagueId)
    {
        var allRounds = new List<NakkaRound>();
        int roundCount;
        var skip = 0;
        do
        {
            var rounds = await _nakkaClient.GetSeasonListAsync(leagueId, skip, 10, new[] { NakkaStatus.Completed });
            roundCount = rounds.Count;
            allRounds.AddRange(rounds);
            skip += 10;
        } while (roundCount == 10);
        return allRounds;
    }

    private async Task<List<RoundStatItem>> GetRoundStats(string roundId)
    {
        var roundStats = new List<RoundStatItem>();
        var players = await _nakkaClient.GetTournamentPlayersAsync(roundId);
        var stats = await _nakkaClient.GetTournamentStatsAsync(roundId);
        var round = await _nakkaClient.GetTournamentAsync(roundId);
        var groups = new List<RobinRoundGroup>();
        var groupNumber = 1;
        var tournamentResults = round.GetTournamentResults();
        var tournamentAvg = new Dictionary<string, decimal>();
        foreach (var result in tournamentResults)
        {
            foreach (var player in result)
            {
                foreach(var oponent in player.Value)
                {
                    if (!tournamentAvg.ContainsKey(player.Key) ||
                        tournamentAvg.ContainsKey(player.Key) && tournamentAvg[player.Key] < oponent.Value.Average)
                    {
                        tournamentAvg[player.Key] = oponent.Value.Average;
                    }
                }
            }
        }
        foreach (var rrResult in round.RoundRobinResults)
        {
            var group = RobinRound.BuildGroup(rrResult, groupNumber, round.RobinRoundRank);
            groupNumber++;
            groups.Add(group);
        }
        var minRank = stats.Values.Where(x => x.Rank > 0).Count() / groups.Count();
        var pointTable = PointRules.PointTables.Single(x => x.MinNumberOfPlayers <= players.Count && x.MaxNumberOfPlayers >= players.Count);
        foreach (var player in players)
        {
            var roundStatItem = new RoundStatItem();
            var statsEntry = stats[player.PlayerId];
            var groupStats = groups[statsEntry.Group - 1].PlayerStats[player.PlayerId];
            roundStatItem.PlayerName = player.Name;
            roundStatItem.TotalScore = statsEntry.Score;
            roundStatItem.TotalDarts = statsEntry.Darts;
            roundStatItem.SetsWin = statsEntry.SetsWin;
            roundStatItem.Sets = statsEntry.SetsPlayed;
            roundStatItem.LegsWin = statsEntry.LegsWin;
            roundStatItem.Legs = statsEntry.LegPlayed;
            roundStatItem.Ton80Count = statsEntry.Ton80Count;
            roundStatItem.Ton40Count = statsEntry.Ton40Count;
            roundStatItem.Ton00Count = statsEntry.Ton00Count;
            roundStatItem.HighOut = statsEntry.HighestCheckout;
            roundStatItem.BestLeg = statsEntry.BestLeg;
            var bestAvg = groupStats.BestAvg;
            if (tournamentAvg.ContainsKey(player.PlayerId) && tournamentAvg[player.PlayerId] > bestAvg)
            {
                bestAvg = tournamentAvg[player.PlayerId];
            }
            roundStatItem.BestAvg = bestAvg;
            roundStatItem.Points = pointTable.RankPoints[statsEntry.Rank].Points + groupStats.Points / 2 +
                GetRobinPoints(groupStats.Rank, statsEntry.Rank, minRank, pointTable.RobinPoints);
            roundStats.Add(roundStatItem);
        }
        return roundStats;
    }

    private int GetRobinPoints(int groupRank, int finalRank, int minRank, RobinPoints robinPoints)
    {
        if (finalRank > 0)
        {
            return 0;
        }
        if (groupRank == minRank + 1)
        {
            return robinPoints.FirstNonProgressPoints;
        }
        if (groupRank == minRank + 2)
        {
            return robinPoints.SecondNonProgressPoints;
        }
        if (groupRank >= minRank + 3)
        {
            return robinPoints.OtherNonProgressPoints;
        }
        return 0;
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    [HttpGet("Error")]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
