class Day02 : IDayCommand
{
  public const int ROCK = 1;
  public const int PAPER = 2;
  public const int SCISSOR = 3;
  public const int WIN = 6;
  public const int LOSE = 0;
  public const int DRAW = 3;  

  public Dictionary<string, int> ScoreTablePart01 = new()
  {
    {"A X", DRAW + ROCK},
    {"A Y", WIN  + PAPER},
    {"A Z", LOSE + SCISSOR},
    {"B X", LOSE + ROCK},
    {"B Y", DRAW + PAPER},
    {"B Z", WIN  + SCISSOR},
    {"C X", WIN  + ROCK},
    {"C Y", LOSE + PAPER},
    {"C Z", DRAW + SCISSOR},
  };

  public Dictionary<string, int> ScoreTablePart02 = new()
  {
    {"A X", LOSE + SCISSOR},
    {"A Y", DRAW + ROCK},
    {"A Z", WIN  + PAPER},
    {"B X", LOSE + ROCK},
    {"B Y", DRAW + PAPER},
    {"B Z", WIN  + SCISSOR},
    {"C X", LOSE + PAPER},
    {"C Y", DRAW + SCISSOR},
    {"C Z", WIN  + ROCK},
  };

  public string Execute()
  {
    var rounds = new FileReader(02).Read().ToList();
    var totalPart01 = rounds.Sum(l => ScoreTablePart01[l]);
    var totalPart02 = rounds.Sum(l => ScoreTablePart02[l]);

    return $"The total score for part01 is {totalPart01} for part02 is {totalPart02}";

  }
}