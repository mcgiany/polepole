using Mcgiany.NakkaClient.Entities;

namespace NakkaResults.ViewModels.Home;

public class LeagueRoundsViewModel
{
    public NakkaLeague League { get; set; } = null!;

    public List<NakkaRound> RoundList { get; set; } = null!;
}
