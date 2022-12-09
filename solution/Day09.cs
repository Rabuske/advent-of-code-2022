class Day09 : IDayCommand
{
  public string Execute()
  {
    var movements = new FileReader(09).Read()
      .Select(line => {
        var splitted = line.Split(" ");
        return (direction: splitted[0][0], steps: int.Parse(splitted[1]));
      })
      .ToList();
    
    var knots = Enumerable.Repeat(new Point2D<int>(0,0), 10).ToArray();

    var positionsTailVisitedPart01 = new HashSet<Point2D<int>>();
    var positionsTailVisitedPart02 = new HashSet<Point2D<int>>();

    foreach (var movement in movements)
    {
      var movementDistance = GetMovementDistance(movement.direction);
      for (int step = 0; step < movement.steps; step++)
      {
        // Update Head        
        knots[0] += movementDistance;
        // Update Knots
        for (int i = 1; i < knots.Length; i++)
        {
          var diffX = knots[i-1].x - knots[i].x;
          var diffY = knots[i-1].y - knots[i].y;
          if(Math.Abs(diffX) >= 2 || Math.Abs(diffY) >= 2)
          {
            knots[i] = knots[i] + new Point2D<int>(Math.Sign(diffX), Math.Sign(diffY));
          }          
          if(i == 1)
          {
            positionsTailVisitedPart01.Add(knots[i]);
          }
          if(i == knots.Length - 1)
          {
            positionsTailVisitedPart02.Add(knots[i]);
          }
        }
      }
    }

    return $"The 2nd knot visited {positionsTailVisitedPart01.Count()} positions and the 10th knot visited {positionsTailVisitedPart02.Count()}";
  }

  private void Print(Point2D<int> tail, Point2D<int> head)
  {
    var minX = Math.Min(tail.x, head.x) - 2;
    var maxX = Math.Max(tail.x, head.x) + 2;
    var minY = Math.Min(tail.y, head.y) - 2; 
    var maxY = Math.Max(tail.y, head.y) + 2;

    for (int y = minY; y <= maxY; y++)
    { 
      for (int x = minX; x <= maxX; x++)
      {
        if(head.x == x && head.y == y)
        {
          Console.Write("H"); continue;
        }
        if(tail.x == x && tail.y == y)
        {
          Console.Write("T"); continue;
        }
        Console.Write(".");
      }      
      Console.WriteLine();
    }
    Console.WriteLine();
  }

  private Point2D<int> GetMovementDistance(char direction)
  {
    return direction switch {
      'R' => new Point2D<int>(1, 0),
      'L' => new Point2D<int>(-1, 0),
      'U' => new Point2D<int>(0, -1),
      'D' => new Point2D<int>(0, 1),
       _ => new Point2D<int>(0, 0),
    };
  }
}
