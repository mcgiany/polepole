using Mcgiany.NakkaClient.Entities;
using NakkaResults.Models;

namespace NakkaResults.ViewModels.Home;

public class HomeRoundViewModel
{
    public List<RoundStatItem> RoundStats { get; set; } = new List<RoundStatItem>();

    public NakkaTournament Round { get; set; } = null!;

    public bool Complete { get; set; }

    public string LeagueId { get; set; }
}
