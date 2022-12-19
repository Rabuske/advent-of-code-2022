using System.Text.RegularExpressions;

class Day15 : IDayCommand
{
  private const int ROW_NUMBER_PART_01 = 2000000;
  public string Execute()
  {
    var numberExpression = new Regex(@"([-,*]?[0-9])+", RegexOptions.Compiled);
    var sensors = new FileReader(15).Read().Select(line =>
    {
      var matches = numberExpression.Matches(line);
      return (coord: new Point2D<int>(matches[0].Value, matches[1].Value),
              nearestBeacon: new Point2D<int>(matches[2].Value, matches[3].Value));

    }).ToList();

    // For Part 01, we need to figure the what points of the row intersects with the radios of the sensor 
    List<(int start, int end)> limitsForRow = GetLimitsForRow(sensors, ROW_NUMBER_PART_01);
    var numberOfImpossibilitiesPart01 = limitsForRow.Select(limit => (limit.end - limit.start) + 1).Sum();

    // Remove beacons and scanners from the count
    var sensorToReduce = sensors.Where(sensor => sensor.coord.y == ROW_NUMBER_PART_01 && limitsForRow.Any(limit => limit.start <= sensor.coord.x && limit.end >= sensor.coord.x))
                                .Select(sensor => sensor.coord)
                                .ToHashSet()
                                .Count();
    var beaconsToReduce = sensors.Where(sensors => sensors.nearestBeacon.y == ROW_NUMBER_PART_01 && limitsForRow.Any(limit => limit.start <= sensors.nearestBeacon.x && limit.end >= sensors.nearestBeacon.x))
                                .Select(sensor => sensor.nearestBeacon)
                                .ToHashSet()
                                .Count();
    numberOfImpossibilitiesPart01 = numberOfImpossibilitiesPart01 - sensorToReduce - beaconsToReduce;

    // Part 02
    var missingBeaconPosition = GetMissingBeaconPosition(sensors);
    var distressSignal = missingBeaconPosition.x * 4000000L + missingBeaconPosition.y;

    return $"The number of impossibilities for row {ROW_NUMBER_PART_01} is {numberOfImpossibilitiesPart01} and the distress signal is {distressSignal}";
  }

  private Point2D<int> GetMissingBeaconPosition(List<(Point2D<int> coord, Point2D<int> nearestBeacon)> sensors)
  {
    var beaconPosition = new Point2D<int>(0, 0);
    for (int row = 0; row < 4000000; row++)
    {
      var limitsForRow = GetLimitsForRow(sensors, row);
      for (int limitIndex = 0; limitIndex < limitsForRow.Count - 1; limitIndex++)
      {
        if (limitsForRow[limitIndex + 1].start - limitsForRow[limitIndex].end > 1)
        {
          beaconPosition = new Point2D<int>(limitsForRow[limitIndex].end + 1, row);
          return beaconPosition;
        }
      }
    }

    return beaconPosition;
  }

  private List<(int start, int end)> GetLimitsForRow(List<(Point2D<int> coord, Point2D<int> nearestBeacon)> sensors, int rowNumber)
  {
    var limitsForRow = sensors.Select(sensor => GetLimitsForRow(rowNumber, sensor))
                              .Where(limit => limit.shouldConsider)
                              .Select(limit => (start: limit.start.x, end: limit.end.x))
                              .OrderBy(limit => limit.start)
                              .ThenBy(limit => limit.end)
                              .ToList();

    // Solve Intersections
    for (int i = 0; i < limitsForRow.Count(); i++)
    {
      var currentLimit = limitsForRow[i];
      // Find all the intersections
      for (int j = i + 1; j < limitsForRow.Count(); j++)
      {
        if (limitsForRow[j].start <= limitsForRow[i].end)
        {
          limitsForRow[j] = (limitsForRow[i].end + 1, limitsForRow[j].end);
          if (limitsForRow[j].start > limitsForRow[j].end)
          {
            limitsForRow.RemoveAt(j);
            j--;
          }
        }
      }
    }

    return limitsForRow;
  }

  private (Point2D<int> start, Point2D<int> end, bool shouldConsider) GetLimitsForRow(int rowNumber, (Point2D<int> coord, Point2D<int> nearestBeacon) sensor)
  {
    var radius = sensor.coord.ManhattanDistance(sensor.nearestBeacon);
    var yComponent = Math.Abs(sensor.coord.y - rowNumber);

    if (yComponent > radius) return (new Point2D<int>(0, 0), new Point2D<int>(0, 0), false);

    var missingXComponent = radius - yComponent;
    var x1 = sensor.coord.x - missingXComponent;
    var x2 = sensor.coord.x + missingXComponent;
    return (start: new Point2D<int>(Math.Min(x1, x2), rowNumber), end: new Point2D<int>(Math.Max(x2, x1), rowNumber), true);
  }
}