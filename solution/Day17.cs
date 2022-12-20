class Day17 : IDayCommand
{
  public static List<List<Point2D<long>>> shapes = new(){
    new(){new Point2D<long>(2, 0), new Point2D<long>(3, 0), new Point2D<long>(4, 0), new Point2D<long>(5, 0)}, // -
    new(){new Point2D<long>(3, -2), new Point2D<long>(2, -1), new Point2D<long>(3, -1), new Point2D<long>(4, -1), new Point2D<long>(3, 0)}, // +
    new(){new Point2D<long>(4, -2), new Point2D<long>(4, -1), new Point2D<long>(2, 0), new Point2D<long>(3, 0), new Point2D<long>(4, 0)},
    new(){new Point2D<long>(2, -3), new Point2D<long>(2, -2), new Point2D<long>(2, -1), new Point2D<long>(2, 0)},
    new(){new Point2D<long>(2, -1), new Point2D<long>(3, -1), new Point2D<long>(2, 0), new Point2D<long>(3, 0)},
  };

  public string Execute()
  {
    var commands = new FileReader(17).Read().First().ToCharArray();

    // I Guess we'll be working with negative Y
    var lowestY = 0L;
    var lowestYPart01 = 0L;
    var numberOfStoppedRocks = 0L;
    var occupiedPoints = new HashSet<Point2D<long>>();
    var shapeIndex = 0;
    var commandIndex = 0;
    var rockHasStopped = true;
    var currentShape = new List<Point2D<long>>();
    (string floor, int commandIndex, int shapeIndex) stateWhenShapeSpawned = ("", 0, 0);
    var cache = new Dictionary<(string floorShape, int commandIndex, int shapeIndex), (long rockIndex, long lowestY)>();

    while (numberOfStoppedRocks < 1000000000000)
    {
      if (rockHasStopped)
      {
        stateWhenShapeSpawned = (GetFloorShape(lowestY, occupiedPoints), commandIndex, shapeIndex);
        currentShape = SpawnShape(shapeIndex, lowestY);
        rockHasStopped = false;
        shapeIndex = (shapeIndex + 1) % shapes.Count();
      }

      var xIncrement = commands[commandIndex] switch
      {
        '<' => -1,
        '>' => +1,
        _ => 0,
      };

      // Try to move shape horizontally
      var shapeAfterMovingHorizontally = currentShape.Select(p => p + new Point2D<long>(xIncrement, 0));
      if (shapeAfterMovingHorizontally.Min(p => p.x) >= 0 // Doest hit wall left
        && shapeAfterMovingHorizontally.Max(p => p.x) <= 6 // Doesnt hit wall right
        && shapeAfterMovingHorizontally.All(p => !occupiedPoints.Contains(p))) // Doest hit another rock
      {
        currentShape = shapeAfterMovingHorizontally.ToList();
      }

      // Try to move shape vertically
      var shapeAfterMovingDown = currentShape.Select(p => p + new Point2D<long>(0, 1));
      if (shapeAfterMovingDown.Any(p => occupiedPoints.Contains(p) || p.y >= 0)) // Shape cannot be moved down
      {
        currentShape.ForEach(point =>
        {
          occupiedPoints.Add(point);
          lowestY = Math.Min(lowestY, point.y);
        });
        rockHasStopped = true;
        numberOfStoppedRocks += 1;

        if (numberOfStoppedRocks == 2022)
        {
          lowestYPart01 = lowestY;
        }

        // If this shape has been detected in the past, figure out the difference in Y and the number of rocks in the loop 
        // Project the new Y based on the number of remaining rocks
        if (cache.ContainsKey(stateWhenShapeSpawned))
        {
          var detectedLoopInfo = cache[stateWhenShapeSpawned];
          var numberOfRocksInLoop = numberOfStoppedRocks - detectedLoopInfo.rockIndex;
          var missingRocks = 1000000000000 - numberOfStoppedRocks;
          var missingSingleSteps = missingRocks % numberOfRocksInLoop;
          // Since I don't want to account for falling in the middle of the loop, only act when we reach a index in the loop that will match the final number of rocks
          if (missingSingleSteps == 0)
          {
            var yDifference = lowestY - detectedLoopInfo.lowestY;
            var numberOfLoopsToAccount = missingRocks / numberOfRocksInLoop;
            lowestY += yDifference * numberOfLoopsToAccount;
            numberOfStoppedRocks = 1000000000000;
          }
        }
        else
        {
          cache.TryAdd(stateWhenShapeSpawned, (numberOfStoppedRocks, lowestY));
        }
      }
      else
      {
        currentShape = shapeAfterMovingDown.ToList();
      }
      commandIndex = (commandIndex + 1) % commands.Length;
    }

    return $"The tower height after 2022 rocks is {-lowestYPart01} and after a bazillion is {-lowestY}";
  }

  private string GetFloorShape(long lowestY, HashSet<Point2D<long>> occupiedPoints)
  {
    var result = "";
    for (long y = lowestY; y < lowestY + 30; y++) // 30 is just a guess -> I don't want to calculate edges
    {
      for (long x = 0; x < 7; x++)
      {
        result += occupiedPoints.Contains(new Point2D<long>(x, y)) ? x : "";
      }
    }
    return result;
  }

  private List<Point2D<long>> SpawnShape(int shapeIndex, long lowestY)
  {
    var currentShape = shapes[shapeIndex];
    var spawnPointY = lowestY - 4;
    currentShape = currentShape.Select(point => point + new Point2D<long>(0, spawnPointY)).ToList();
    return currentShape;
  }
}