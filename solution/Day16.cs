record Valve(string Id, int FlowRate, List<string> Tunnels);

class Day16 : IDayCommand
{
  public string Execute()
  {
    var valves = new FileReader(16).Read().Select(line =>
    {
      var commaSeparated = line.Replace("Valve ", "")
                               .Replace(" has flow rate=", ",")
                               .Replace("; tunnels lead to valves ", ",")
                               .Replace("; tunnel leads to valve ", ",")
                               .Replace(", ", ",").Split(",");
      return new Valve(commaSeparated[0], int.Parse(commaSeparated[1]), commaSeparated.Skip(2).ToList());
    }).ToDictionary(valve => valve.Id, valve => valve);

    // Floyd Warshall Algorithm for finding the distances between nodes:
    var distances = CalculateMinDistance(valves);

    // Now, starting at AA, we need to build the possible states => move to each valve and open them, moving then to the next valve and opening, until all valves are open or we run out of time
    var resultsPart01 = new List<(HashSet<string> path, int totalFlow)>();
    Calc(valves: valves,
         currentValve: "AA",
         remainingTime: 30,
         distances: distances,
         results: resultsPart01,
         currentPath: new HashSet<string>(),
         totalFlow: 0);
    var maxFlowPart01 = resultsPart01.Max(res => res.totalFlow);

    // For part 02, we can run the same logic for 26 minutes. Then, we combine all the results that don't have a common valve => you and the elephant worked on different things

    bool IsDisjoint(HashSet<string> a1, HashSet<string> a2)
    {
      foreach (var item in a1)
      {
        if (a2.Contains(item)) return false;
      }
      return true;
    }

    var resultsPart02 = new List<(HashSet<string> path, int totalFlow)>();
    Calc(valves: valves,
         currentValve: "AA",
         remainingTime: 26,
         distances: distances,
         results: resultsPart02,
         currentPath: new HashSet<string>(),
         totalFlow: 0);
    var maxFlowPart02 = resultsPart02.SelectMany((result1, index) =>
    {
      var disjoint = resultsPart02.Skip(index + 1).Where(result2 => IsDisjoint(result1.path, result2.path));
      return disjoint.Select(result2 => result2.totalFlow + result1.totalFlow);
    }).Max();

    return $"The total flow alone is {maxFlowPart01}, with the elephant is {maxFlowPart02}";
  }

  private void Calc(Dictionary<string, Valve> valves, string currentValve, int remainingTime, Dictionary<(string, string), int> distances, List<(HashSet<string> path, int totalFlow)> results, HashSet<string> currentPath, int totalFlow)
  {
    if (currentPath.Contains(currentValve))
    {
      return; // Valve already open
    }
    // Calculate the cost to open this valve
    var valveInfo = valves[currentValve];
    var newRemainingTime = remainingTime;
    var newTotalFlow = totalFlow;
    var newPath = currentPath;
    if (valveInfo.FlowRate > 0) // Ignore valve AA
    {
      var lastValve = currentPath.LastOrDefault() ?? "AA";
      newRemainingTime = remainingTime - (distances[(currentValve, lastValve)] + 1);
      if (newRemainingTime <= 0)
      {
        return;
      }
      newTotalFlow = totalFlow + newRemainingTime * valveInfo.FlowRate;
      newPath = currentPath.Concat(new List<string>() { currentValve }).ToHashSet();
      results.Add((newPath, newTotalFlow));
    }

    // Process all other valves that have a flow rate
    var valvesToOpen = valves.Values.Where(valve => valve.FlowRate > 0);
    foreach (var valve in valvesToOpen)
    {
      Calc(valves, valve.Id, newRemainingTime, distances, results, newPath, newTotalFlow);
    }
  }

  private Dictionary<(string, string), int> CalculateMinDistance(Dictionary<string, Valve> valves)
  {
    var distances = new Dictionary<(string, string), int>();
    foreach (var valve1 in valves.Values)
    {
      foreach (var valve2 in valves.Values)
      {
        var distance = 9999;
        if (valve1 == valve2)
        {
          distance = 0;
        }
        if (valve1.Tunnels.Contains(valve2.Id))
        {
          distance = 1;
        }
        distances.TryAdd((valve1.Id, valve2.Id), distance);
        distances.TryAdd((valve2.Id, valve1.Id), distance);
      }
    }

    foreach (var valve1 in valves.Keys)
    {
      foreach (var valve2 in valves.Keys)
      {
        foreach (var valve3 in valves.Keys)
        {
          if (distances[(valve1, valve3)] + distances[(valve3, valve2)] < distances[(valve1, valve2)])
          {
            distances[(valve1, valve2)] = distances[(valve1, valve3)] + distances[(valve3, valve2)];
            distances[(valve2, valve1)] = distances[(valve1, valve3)] + distances[(valve3, valve2)];
          }
        }
      }
    }
    return distances;
  }
}
