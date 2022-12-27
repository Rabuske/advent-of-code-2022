class Day23 : IDayCommand
{

  static private List<Func<HashSet<Point2D<int>>, Dictionary<Point2D<int>, Point2D<int>>, List<Point2D<int>>, Point2D<int>, bool>> Directions = new()
  {
    {(elves, proposals, adjacent, elf) => {
      // If there is no Elf in the N, NE, or NW adjacent positions, the Elf proposes moving north one step
      if(adjacent.Where(adj => adj.y < elf.y).All(adj => !elves.Contains(adj)))
      {
        proposals.Add(elf, elf + new Point2D<int>(0, -1));
        return true;
      }
      return false;
    }},
    {(elves, proposals, adjacent, elf) => {
        // If there is no Elf in the S, SE, or SW adjacent positions, the Elf proposes moving south one step.        
        if(adjacent.Where(adj => adj.y > elf.y).All(adj => !elves.Contains(adj)))
        {
          proposals.Add(elf, elf + new Point2D<int>(0, 1));
          return true;
        }
        return false;
    }},
    {(elves, proposals, adjacent, elf) => {
        // If there is no Elf in the W, NW, or SW adjacent positions, the Elf proposes moving west one step
        if(adjacent.Where(adj => adj.x < elf.x).All(adj => !elves.Contains(adj)))
        {
          proposals.Add(elf, elf + new Point2D<int>(-1, 0));
          return true;
        }
        return false;
    }},
    {(elves, proposals, adjacent, elf) => {
        // If there is no Elf in the E, NE, or SE adjacent positions, the Elf proposes moving east one step
        if(adjacent.Where(adj => adj.x > elf.x).All(adj => !elves.Contains(adj)))
        {
          proposals.Add(elf, elf + new Point2D<int>(1, 0));
          return true;
        }
        return false;
    }},
  };

  public string Execute()
  {
    var elves = new FileReader(23).Read()
                                  .SelectMany((line, y) => line.Select((c, x) => c == '#' ? new Point2D<int>(x, y) : new Point2D<int>(int.MinValue, int.MinValue)))
                                  .ToHashSet();
    elves.Remove(new Point2D<int>(int.MinValue, int.MinValue));
    int round = 0;
    var emptySpotsRound10 = 0;


    while (round > int.MinValue)
    {
      // First half
      var proposals = new Dictionary<Point2D<int>, Point2D<int>>();
      foreach (var elf in elves)
      {
        var adjacent = elf.GenerateAdjacent(includeDiagonals: true);
        if (adjacent.All(adj => !elves.Contains(adj)))
        {
          continue;
        }

        for (int directionIndex = round; directionIndex < (round + 4); directionIndex++)
        {
          var generateProposal = Directions[directionIndex % Directions.Count];
          if (generateProposal(elves, proposals, adjacent, elf))
          {
            break;
          }
        }
      }

      // Second Half
      if (proposals.Count() == 0)
      {
        break;
      }
      var groupedProposals = proposals.Values.GroupBy(p => p).ToDictionary(p => p.Key, p => p.Count());
      foreach (var elf in elves.ToList())
      {
        if (!proposals.ContainsKey(elf))
        {
          continue;
        }
        var elfProposal = proposals[elf];
        if (groupedProposals[elfProposal] == 1)
        {
          elves.Remove(elf);
          elves.Add(elfProposal);
        }
      }
      // Calculate the number of empty ground
      if (round == 9)
      {
        var minX = elves.Min(elf => elf.x);
        var maxX = elves.Max(elf => elf.x);
        var minY = elves.Min(elf => elf.y);
        var maxY = elves.Max(elf => elf.y);

        var totalSpots = ((maxX - minX) + 1) * ((maxY - minY) + 1);
        emptySpotsRound10 = totalSpots - elves.Count();
      }
      round++;
      //Print(elves, round);
    }

    return $"After 10 rounds, the number of ground spots is {emptySpotsRound10} and I saved {round + 1} rounds of moving";
  }

  private void Print(HashSet<Point2D<int>> elves, int round = 0)
  {
    var minX = elves.Min(elf => elf.x);
    var maxX = elves.Max(elf => elf.x);
    var minY = elves.Min(elf => elf.y);
    var maxY = elves.Max(elf => elf.y);
    Console.WriteLine();
    Console.WriteLine();
    Console.WriteLine($"----------------- {round}");
    for (int y = minY; y <= maxY; y++)
    {
      for (int x = minX; x <= maxX; x++)
      {
        if (elves.Contains(new Point2D<int>(x, y)))
        {
          Console.Write("#");
        }
        else
        {
          Console.Write(".");
        }
      }
      Console.WriteLine();
    }
  }
}
