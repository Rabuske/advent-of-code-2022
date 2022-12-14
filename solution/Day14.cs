class Day14 : IDayCommand
{
  public string Execute()
  {
    var lines = new FileReader(14).Read();
    var rocks = Parse(lines);

    var maxY = rocks.Max(p => p.y);

    Func<Point2D<int>, bool> isAtTheBottomPart01 = point => false;
    Func<Point2D<int>, bool> isAtTheBottomPart02 = point => point.y == maxY + 2;
    Func<Point2D<int>, HashSet<Point2D<int>>, bool> shouldStopPart01 = (point, sands) => point.y > maxY;
    Func<Point2D<int>, HashSet<Point2D<int>>, bool> shouldStopPart02 = (point, sands) => sands.Contains(new Point2D<int>(500, 0));
    
    var sandsPart01 = GenerateSands(rocks, isAtTheBottomPart01, shouldStopPart01);
    var sandsPart02 = GenerateSands(rocks, isAtTheBottomPart02, shouldStopPart02);

    return $"{sandsPart01.Count()} units of sand came to a rest without a bottom. With a bottom, that number was {sandsPart02.Count()}";
  }

  private HashSet<Point2D<int>> GenerateSands(HashSet<Point2D<int>> rocks, Func<Point2D<int>, bool> isAtTheBottom, Func<Point2D<int>, HashSet<Point2D<int>>, bool> shouldStop)
  {
    var sands = new HashSet<Point2D<int>>();
    var currentSand = new Point2D<int>(500, 0);
    while (!shouldStop(currentSand, sands))
    {
      // Attempt to move down
      var newSand = currentSand + new Point2D<int>(0, 1);
      if (!rocks.Contains(newSand) && !sands.Contains(newSand) && !isAtTheBottom(newSand))
      {
        currentSand = newSand;
        continue;
      }
      // Attempt to move diagonally left
      newSand = currentSand + new Point2D<int>(-1, 1);
      if (!rocks.Contains(newSand) && !sands.Contains(newSand) && !isAtTheBottom(newSand))
      {
        currentSand = newSand;
        continue;
      }

      // Attempt to move diagonally right
      newSand = currentSand + new Point2D<int>(1, 1);
      if (!rocks.Contains(newSand) && !sands.Contains(newSand) && !isAtTheBottom(newSand))
      {
        currentSand = newSand;
        continue;
      }

      // Sand stops
      sands.Add(currentSand);
      currentSand = new Point2D<int>(500, 0);
    }

    return sands;
  }

  private HashSet<Point2D<int>> Parse(IEnumerable<string> lines)
  {
    var rockPoints = lines.Select(line =>
    {
      var pairs = line.Split(" -> ").Select(pair => pair.Split(",")).ToList();
      return pairs.Select(pairs => new Point2D<int>(int.Parse(pairs[0]), int.Parse(pairs[1])));
    });

    return rockPoints.SelectMany(rockLine =>
    {
      var prevRockPoint = rockLine.First();
      return rockLine.Skip(1).SelectMany(point =>
      {
        var minX = Math.Min(prevRockPoint.x, point.x);
        var minY = Math.Min(prevRockPoint.y, point.y);
        var numberOfX = Math.Abs(prevRockPoint.x - point.x) + 1;
        var numberOfY = Math.Abs(prevRockPoint.y - point.y) + 1;
        var pointsX = Enumerable.Range(minX, numberOfX).Select(i => new Point2D<int>(i, minY));
        var pointsY = Enumerable.Range(minY, numberOfY).Select(i => new Point2D<int>(minX, i));
        prevRockPoint = point;
        return pointsX.Concat(pointsY);
      });
    }).ToHashSet();
  }
}
