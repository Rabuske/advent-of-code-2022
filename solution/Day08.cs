class Day08 : IDayCommand
{
  public string Execute()
  {
    var map = new FileReader(08).Read()
      .Select(line => line.ToCharArray()
        .Select(n => (int)char.GetNumericValue(n))
        .ToList())
      .ToList();

    var visibleFromEdge = Part01(map);    
    var highestScore = Part02(map);

    return $"The number of visible trees is {visibleFromEdge.Count} and the highest view score is {highestScore}";

    throw new NotImplementedException();
  }

  private int Part02(List<List<int>> map)
  {
    var viewScores = new Dictionary<(int line, int column), (int fromLeft, int fromRight, int fromUp, int fromDown)>();
    for (int line = 0; line < map.Count; line++)
    {
      for (int column = 0; column < map[line].Count; column++)
      {
        // Left
        var visibleLeft = 0;
        for (int columnToCheck = column - 1; columnToCheck >= 0; columnToCheck--)
        {
          visibleLeft++;
          if (map[line][columnToCheck] >= map[line][column])
          {
            break;
          }
        }

        // Right - not very performatic using LINQ
        var visibleRight = 0;
        for (int columnToCheck = column + 1; columnToCheck < map[line].Count; columnToCheck++)
        {
          visibleRight++;
          if (map[line][columnToCheck] >= map[line][column])
          {
            break;
          }
        }

        // Up
        var visibleUp = 0;
        for (int lineToCheck = line - 1; lineToCheck >= 0; lineToCheck--)
        {
          visibleUp++;
          if (map[lineToCheck][column] >= map[line][column])
          {
            break;
          }
        }
        // Down
        var visibleDown = 0;
        for (int lineToCheck = line + 1; lineToCheck < map.Count; lineToCheck++)
        {
          visibleDown++;  
          if (map[lineToCheck][column] >= map[line][column])
          {
            break;
          }
        }
        viewScores.Add((line, column), (visibleLeft, visibleRight, visibleUp, visibleDown));
      }
    }

    var highestScore = viewScores.Values.Select(views => views.fromDown * views.fromUp * views.fromLeft * views.fromRight).Max();
    return highestScore;
  }

  private HashSet<(int line, int column)> Part01(List<List<int>> map)
  {
    var visibleFromEdge = new HashSet<(int line, int column)>();


    // Left and right Edge
    for (int line = 0; line < map.Count; line++)
    {
      var tallestTreeFromLeft = -1;
      var tallestTreeFromRight = -1;
      for (int column = 0; column < map[line].Count; column++)
      {
        if (map[line][column] > tallestTreeFromLeft)
        {
          tallestTreeFromLeft = map[line][column];
          visibleFromEdge.Add((line, column));
        }
      }
      for (int column = map[line].Count - 1; column >= 0; column--)
      {
        if (map[line][column] > tallestTreeFromRight)
        {
          tallestTreeFromRight = map[line][column];
          visibleFromEdge.Add((line, column));
        }
      }
    }

    // Upper Edge and Lower Edges
    for (int column = 0; column < map[0].Count; column++)
    {
      var tallestTreeFromUp = -1;
      var tallestTreeFromDown = -1;
      for (int line = 0; line < map.Count; line++)
      {
        if (map[line][column] > tallestTreeFromUp)
        {
          tallestTreeFromUp = map[line][column];
          visibleFromEdge.Add((line, column));
        }
      }
      for (int line = map.Count - 1; line >= 0; line--)
      {
        if (map[line][column] > tallestTreeFromDown)
        {
          tallestTreeFromDown = map[line][column];
          visibleFromEdge.Add((line, column));
        }
      }
    }

    return visibleFromEdge;
  }
}
