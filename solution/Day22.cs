using System.Text.RegularExpressions;

class Day22 : IDayCommand
{
  static readonly Point2D<int> RIGHT = new Point2D<int>(1, 0);
  static readonly Point2D<int> LEFT = new Point2D<int>(-1, 0);
  static readonly Point2D<int> UP = new Point2D<int>(0, -1);
  static readonly Point2D<int> DOWN = new Point2D<int>(0, 1);

  private Dictionary<(Point2D<int> point, Point2D<int> direction), (Point2D<int> point, Point2D<int> direction)> EdgeMapping = new();

  public string Execute()
  {
    var lines = new FileReader(22).Read();
    var points = lines.TakeWhile(line => !string.IsNullOrEmpty(line))
                      .SelectMany((line, y) => line.Select((c, x) => (point: new Point2D<int>(x + 1, y + 1), value: c)))
                      .Where(c => c.value != ' ')
                      .ToDictionary(c => c.point, c => c.value);

    var commandRegex = new Regex("([0-9]+)|([A-Z]+)");
    var commands = commandRegex.Matches(lines.Last()).Select(match => match.Value).ToList();


    // Part01
    var currentPositionPart01 = points.Where(p => p.Value == '.').OrderBy(p => p.Key.y).ThenBy(p => p.Key.x).First().Key;
    var currentDirectionPart01 = RIGHT;
    Walk(points, commands, ref currentPositionPart01, ref currentDirectionPart01, CalculateNextPointPart01);
    int directionValuePart01 = GetValueForDirection(currentDirectionPart01);
    var finalResultPart01 = 1000 * currentPositionPart01.y + 4 * currentPositionPart01.x + directionValuePart01;

    // Part02
    PopulateEdgeMapping();
    var currentPositionPart02 = points.Where(p => p.Value == '.').OrderBy(p => p.Key.y).ThenBy(p => p.Key.x).First().Key;
    var currentDirectionPart02 = RIGHT;
    Walk(points, commands, ref currentPositionPart02, ref currentDirectionPart02, CalculateNextPointPart02);

    int directionValuePart02 = GetValueForDirection(currentDirectionPart02);
    var finalResultPart02 = 1000 * currentPositionPart02.y + 4 * currentPositionPart02.x + directionValuePart02;

    return $"The current Result is {finalResultPart01}, for part 02 is {finalResultPart02}";

  }

  private void PopulateEdgeMapping()
  {
    // Build Edge Mapping hardcoded for my input:    
    /*
        111222
        111222
        111222
        333
        333
        333
      444555
      444555
      444555
      666
      666
      666
    */

    for (int i = 1; i <= 50; i++)
    {
      // upper 1 maps to left 6
      EdgeMapping.Add((new Point2D<int>(50 + i, 1), UP), (new Point2D<int>(1, 150 + i), RIGHT));
      EdgeMapping.Add((new Point2D<int>(1, 150 + i), LEFT), (new Point2D<int>(50 + i, 1), DOWN));

      // left 1 maps to left 4
      EdgeMapping.Add((new Point2D<int>(51, i), LEFT), (new Point2D<int>(1, 151 - i), RIGHT));
      EdgeMapping.Add((new Point2D<int>(1, 151 - i), LEFT), (new Point2D<int>(51, i), RIGHT));

      // upper 2 maps to bottom 6
      EdgeMapping.Add((new Point2D<int>(100 + i, 1), UP), (new Point2D<int>(i, 200), UP));
      EdgeMapping.Add((new Point2D<int>(i, 200), DOWN), (new Point2D<int>(100 + i, 1), DOWN));

      // right 2 maps to right 5
      EdgeMapping.Add((new Point2D<int>(150, i), RIGHT), (new Point2D<int>(100, 151 - i), LEFT));
      EdgeMapping.Add((new Point2D<int>(100, 151 - i), RIGHT), (new Point2D<int>(150, i), LEFT));

      // bottom 2 maps to right 3
      EdgeMapping.Add((new Point2D<int>(100 + i, 50), DOWN), (new Point2D<int>(100, 50 + i), LEFT));
      EdgeMapping.Add((new Point2D<int>(100, 50 + i), RIGHT), (new Point2D<int>(100 + i, 50), UP));

      // left 3 maps to upper 4
      EdgeMapping.Add((new Point2D<int>(51, 50 + i), LEFT), (new Point2D<int>(i, 101), DOWN));
      EdgeMapping.Add((new Point2D<int>(i, 101), UP), (new Point2D<int>(51, 50 + i), RIGHT));

      // bottom 5 maps to right 6
      EdgeMapping.Add((new Point2D<int>(50 + i, 150), DOWN), (new Point2D<int>(50, 150 + i), LEFT));
      EdgeMapping.Add((new Point2D<int>(50, 150 + i), RIGHT), (new Point2D<int>(50 + i, 150), UP));
    }
  }

  private static void Walk(Dictionary<Point2D<int>, char> points, List<string> commands, ref Point2D<int> currentPosition, ref Point2D<int> currentDirection, Func<Dictionary<Point2D<int>, char>, Point2D<int>, Point2D<int>, (Point2D<int> nextPoint, Point2D<int> direction)> calculateNextPoint)
  {
    foreach (var command in commands)
    {
      var couldParse = int.TryParse(command, out var steps);
      if (couldParse)
      {
        for (int step = 0; step < steps; step++)
        {
          (var nextPoint, var nextDirection) = calculateNextPoint(points, currentDirection, currentPosition);
          if (points[nextPoint] == '#') // Found wall
          {
            break;
          }
          currentDirection = nextDirection;
          currentPosition = nextPoint;
        }
      }
      else // Could not parse
      {
        switch (command)
        {
          case "R":
            currentDirection = new Point2D<int>(-currentDirection.y, currentDirection.x);
            break;
          case "L":
            currentDirection = new Point2D<int>(currentDirection.y, -currentDirection.x);
            break;
          default:
            break;
        }
      }
    }
  }

  private (Point2D<int> nextPoint, Point2D<int> direction) CalculateNextPointPart01(Dictionary<Point2D<int>, char> points, Point2D<int> currentDirection, Point2D<int> currentPosition)
  {
    var nextPoint = currentPosition + currentDirection;

    if (points.ContainsKey(nextPoint))
    {
      return (nextPoint, currentDirection);
    }

    if (currentDirection == RIGHT)
    {
      var minX = points.Keys.Where(p => p.y == nextPoint.y).Min(p => p.x);
      nextPoint = new Point2D<int>(minX, nextPoint.y);
    }
    if (currentDirection == LEFT)
    {
      var maxX = points.Keys.Where(p => p.y == nextPoint.y).Max(p => p.x);
      nextPoint = new Point2D<int>(maxX, nextPoint.y);
    }
    if (currentDirection == DOWN)
    {
      var minY = points.Keys.Where(p => p.x == nextPoint.x).Min(p => p.y);
      nextPoint = new Point2D<int>(nextPoint.x, minY);
    }
    if (currentDirection == UP)
    {
      var maxY = points.Keys.Where(p => p.x == nextPoint.x).Max(p => p.y);
      nextPoint = new Point2D<int>(nextPoint.x, maxY);
    }

    return (nextPoint, currentDirection);
  }

  private (Point2D<int> nextPoint, Point2D<int> direction) CalculateNextPointPart02(Dictionary<Point2D<int>, char> points, Point2D<int> currentDirection, Point2D<int> currentPosition)
  {
    var nextPoint = currentPosition + currentDirection;
    if (points.ContainsKey(nextPoint))
    {
      return (nextPoint, currentDirection);
    }
    if (EdgeMapping.ContainsKey((currentPosition, currentDirection)))
    {
      var toTest = EdgeMapping[(currentPosition, currentDirection)];
      return EdgeMapping[(currentPosition, currentDirection)];
    }
    return (nextPoint, currentDirection);
  }

  private static int GetValueForDirection(Point2D<int> currentDirection)
  {
    var directionValue = 0;
    if (currentDirection == RIGHT)
    {
      directionValue = 0;
    }
    if (currentDirection == DOWN)
    {
      directionValue = 1;
    }
    if (currentDirection == LEFT)
    {
      directionValue = 2;
    }
    if (currentDirection == UP)
    {
      directionValue = 3;
    }

    return directionValue;
  }
}
