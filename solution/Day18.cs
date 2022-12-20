class Day18 : IDayCommand
{
  public string Execute()
  {
    var cubes = new FileReader(18).Read()
      .Select(line => line.Split(","))
      .Select(coords => new Point3D<int>(coords[0], coords[1], coords[2]))
      .ToHashSet();

    // Cubes are connected when the Manhattan distance between them is only 1
    var availableSidesPart01 = cubes.Select(cube1 =>
    {
      var touching = cubes.Where(cube2 => cube1.ManhattanDistance(cube2) == 1);
      return 6 - touching.Count();
    }).Sum();

    // Part 02: 
    // 1) Start with a huge cube of "points" that are either air or lava
    var states = new Dictionary<Point3D<int>, char>();
    var edges = new List<Point3D<int>>();
    var minZ = cubes.Min(c => c.z) - 1;
    var maxZ = cubes.Max(c => c.z) + 1;
    var minY = cubes.Min(c => c.y) - 1;
    var maxY = cubes.Max(c => c.y) + 1;
    var minX = cubes.Min(c => c.x) - 1;
    var maxX = cubes.Max(c => c.x) + 1;

    for (int z = minZ; z < maxZ; z++)
    {
      for (int y = minY; y < maxY; y++)
      {
        for (int x = minX; x < maxX; x++)
        {
          var cube = new Point3D<int>(x, y, z);
          if (x == minX || x == maxX || y == minY || y == maxY || z == minZ || z == maxZ)
          {
            edges.Add(cube);
          }
          states.Add(cube, cubes.Contains(cube) ? 'L' : 'A');
        }
      }
    }

    // 2) Pour water from all possible outside edges
    edges.ForEach(edge =>
    {
      Queue<Point3D<int>> toVisit = new();
      toVisit.Enqueue(edge);
      states[edge] = 'W';
      while (toVisit.Count > 0)
      {
        var currentPoint = toVisit.Dequeue();
        if (!states.ContainsKey(currentPoint))
        {
          continue;
        }
        var neighbors = GetAdjacentCoords(currentPoint)
                        .Where(c => states.ContainsKey(c) && states[c] == 'A')
                        .ToList();
        neighbors.ForEach(n => {
          states[n] = 'W';
          toVisit.Enqueue(n);
        });
      }
    });

    // 3) Same as part 01, but only for the sides that are interacting with water
    var availableSidesPart02 = cubes.Select(cube =>
    {
      var touchingLavaOrAir = GetAdjacentCoords(cube).Where(adj => states.ContainsKey(adj) && states[adj] != 'W');
      return 6 - touchingLavaOrAir.Count();
    }).Sum();

    return $"The surface is {availableSidesPart01}, without considering pockets is {availableSidesPart02}";

  }

  private static IEnumerable<Point3D<int>> GetAdjacentCoords(Point3D<int> currentPoint)
  {
    return new List<Point3D<int>>(){
          new Point3D<int>(0, 0, 1),
          new Point3D<int>(0, 0, -1),
          new Point3D<int>(0, 1, 0),
          new Point3D<int>(0, -1, 0),
          new Point3D<int>(1, 0, 0),
          new Point3D<int>(-1, 0, 0),
        }.Select(c => currentPoint + c);
  }
}