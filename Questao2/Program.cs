using Newtonsoft.Json;

public class Program
{
    public static void Main()
    {
        string teamName = "Paris Saint-Germain";
        int year = 2013;
        int totalGoals = getTotalScoredGoals(teamName, year);

        Console.WriteLine("Team " + teamName + " scored " + totalGoals.ToString() + " goals in " + year);

        teamName = "Chelsea";
        year = 2014;
        totalGoals = getTotalScoredGoals(teamName, year);

        Console.WriteLine("Team " + teamName + " scored " + totalGoals.ToString() + " goals in " + year);

        // Output expected:
        // Team Paris Saint - Germain scored 109 goals in 2013
        // Team Chelsea scored 92 goals in 2014
    }

    public static int getTotalScoredGoals(string team, int year)
    {
        var goalsScoredAsHomeTeam = getTotalScoredGoals(team, year, 1, true);
        var goalsScoredAsAwayTeam = getTotalScoredGoals(team, year, 1, false);

        return goalsScoredAsHomeTeam + goalsScoredAsAwayTeam;
    }

    public static int getTotalScoredGoals(string team, int year, int page, bool isHomeTeam)
    {
        HttpClient client = new HttpClient();
        var url = $"https://jsonmock.hackerrank.com/api/football_matches?year={year}&{(isHomeTeam ? "team1" : "team2")}={team}&page={page}";
        var response = client.SendAsync(new HttpRequestMessage(HttpMethod.Get, url)).Result;

        if (!response.IsSuccessStatusCode)
            throw new Exception("An error occurred while processing your request.");

        var matchesSummary = JsonConvert.DeserializeObject<MatchSummary>(response.Content.ReadAsStringAsync().Result);
        var goalsScored = 0;
        if (matchesSummary != null && matchesSummary.data != null)
        {
            goalsScored += isHomeTeam ? matchesSummary.data.Sum(x => int.Parse(x.team1goals)) : matchesSummary.data.Sum(x => int.Parse(x.team2goals));

            if (matchesSummary.page < matchesSummary.total_pages)
                goalsScored += getTotalScoredGoals(team, year, matchesSummary.page + 1, isHomeTeam);
        }

        return goalsScored;
    }

}

public class Match
{
    public string competition { get; set; }
    public int year { get; set; }
    public string round { get; set; }
    public string team1 { get; set; }
    public string team2 { get; set; }
    public string team1goals { get; set; }
    public string team2goals { get; set; }
}

public class MatchSummary
{
    public int page { get; set; }
    public int per_page { get; set; }
    public int total { get; set; }
    public int total_pages { get; set; }
    public List<Match> data { get; set; }
}