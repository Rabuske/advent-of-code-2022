using System.Numerics;

class Day24 : IDayCommand
{
  public string Execute()
  {

    var lines = new FileReader(24).Read().ToList();
    var initialBlizzard = lines.SelectMany((line, y) => line.Select((c, x) =>
    {
      var direction = c switch
      {
        '>' => new Point2D<int>(1, 0),
        '<' => new Point2D<int>(-1, 0),
        'v' => new Point2D<int>(0, 1),
        '^' => new Point2D<int>(0, -1),
        _ => new Point2D<int>(-1, -1),
      };
      return (position: new Point2D<int>(x, y), direction: direction);
    })).Where(b => b.direction != new Point2D<int>(-1, -1)).ToList();

    var map = lines.SelectMany((line, y) => line.Select((c, x) =>
    {
      return (position: new Point2D<int>(x, y), isWall: c == '#');
    })).ToDictionary(p => p.position, p => p.isWall);

    var minPoint = new Point2D<int>(1, 1);
    var maxPoint = new Point2D<int>(lines[0].Length - 2, lines.Count() - 2);
    var blizzardsGrid = GenerateBlizzardGrid(minPoint, maxPoint, initialBlizzard);

    var initialPosition = minPoint + new Point2D<int>(0, -1);
    var destination = maxPoint + new Point2D<int>(0, 1);

    int moves = CalculateOptimalPath(map, blizzardsGrid, initialPosition, destination, 0);
    int movesPart02 = CalculateOptimalPath(map, blizzardsGrid, destination, initialPosition, moves);
    movesPart02 = CalculateOptimalPath(map, blizzardsGrid, initialPosition, destination, movesPart02);

    return $"It took {moves} moves to reach the destination. To help the elf, it took {movesPart02} moves";
  }

  private static int CalculateOptimalPath(Dictionary<Point2D<int>, bool> map, Dictionary<int, HashSet<Point2D<int>>> blizzardsGrid, Point2D<int> initialPosition, Point2D<int> destination, int currentMoves)
  {
    var queue = new PriorityQueue<(Point2D<int> position, int moves), int>();
    var seen = new HashSet<(Point2D<int> position, int moves)>();
    var moves = currentMoves;
    queue.Enqueue((initialPosition, moves), moves);
    seen.Add((initialPosition, moves));
    while (queue.Count > 0)
    {
      var state = queue.Dequeue();
      if (state.position == destination)
      {
        moves = state.moves;
        break;
      }
      var nextBlizzards = blizzardsGrid[(state.moves + 1) % blizzardsGrid.Count];
      var nextPositions = state.position.GenerateAdjacent(includeItself: true);
      foreach (var nextPosition in nextPositions)
      {
        if (!map.ContainsKey(nextPosition) || map[nextPosition])
        {
          continue;
        }
        if (nextBlizzards.Contains(nextPosition))
        {
          continue;
        }
        if (seen.Contains((nextPosition, (state.moves + 1) % blizzardsGrid.Count)))
        {
          continue;
        }
        seen.Add((nextPosition, (state.moves + 1) % blizzardsGrid.Count));
        queue.Enqueue((nextPosition, state.moves + 1), state.moves + 1); //  + state.position.ManhattanDistance(destination) (optional)
      }
    }

    return moves;
  }

  private List<(Point2D<int> position, Point2D<int> direction)> AdvanceStorm(List<(Point2D<int> position, Point2D<int> direction)> currentBlizzards, Point2D<int> minPoint, Point2D<int> maxPoint)
  {
    return currentBlizzards.Select(bl =>
    {
      var nextPosition = bl.position + bl.direction;
      if (nextPosition.x < minPoint.x)
      {
        nextPosition = new Point2D<int>(maxPoint.x, nextPosition.y);
      }

      if (nextPosition.x > maxPoint.x)
      {
        nextPosition = new Point2D<int>(minPoint.x, nextPosition.y);
      }

      if (nextPosition.y < minPoint.y)
      {
        nextPosition = new Point2D<int>(nextPosition.x, maxPoint.y);
      }

      if (nextPosition.y > maxPoint.y)
      {
        nextPosition = new Point2D<int>(nextPosition.x, minPoint.y);
      }
      return (nextPosition, bl.direction);
    }).ToList();
  }

  private Dictionary<int, HashSet<Point2D<int>>> GenerateBlizzardGrid(Point2D<int> min, Point2D<int> max, List<(Point2D<int> position, Point2D<int> direction)> initialBlizzard)
  {
    var requiredGrids = max.x * max.y / (int) BigInteger.GreatestCommonDivisor(max.x, max.y);
    var blizzards = new Dictionary<int, HashSet<Point2D<int>>>();
    blizzards.Add(0, initialBlizzard.Select(bl => bl.position).ToHashSet());
    var currBlizzard = initialBlizzard;
    for (int t = 1; t < requiredGrids; t++)
    {
      currBlizzard = AdvanceStorm(currBlizzard, min, max);
      blizzards.Add(t, currBlizzard.Select(bl => bl.position).ToHashSet());
    }
    return blizzards;
  }
}
