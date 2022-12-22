using System.Text.RegularExpressions;
record BluePrint(int id, int oreRobotCostOre, int clayRobotCostOre, int obsidianRobotCostOre, int obsidianRobotCostClay, int geodeRobotCostOre, int geodeRobotCostObsidian);
record Robots(int ore, int clay = 0, int obsidian = 0, int geode = 0);
record Minerals(int ore = 0, int clay = 0, int obsidian = 0, int geode = 0);

class Day19 : IDayCommand
{
  public string Execute()
  {
    var regex = new Regex("([0-9])+");
    var bluePrints = new FileReader(19).Read().Select(line =>
    {
      var matches = regex.Matches(line).Select(m => int.Parse(m.Value)).ToList();
      return new BluePrint(id: matches[0],
              oreRobotCostOre: matches[1],
              clayRobotCostOre: matches[2],
              obsidianRobotCostOre: matches[3],
              obsidianRobotCostClay: matches[4],
              geodeRobotCostOre: matches[5],
              geodeRobotCostObsidian: matches[6]);
    });

    var qualityLevels = bluePrints.Select(bp => bp.id * GetOptimalProduction(bp, 24));

    var geodeMultiplication = bluePrints.Take(3).Select(bp => GetOptimalProduction(bp, 32)).Aggregate(1, (curr, next) => curr * next);

    return $"The sum of the quality levels is {qualityLevels.Sum()}. The multiplication result is {geodeMultiplication}";
  }

  private int GetOptimalProduction(BluePrint bp, int timeLimitToProcessInMinutes)
  {
    Queue<(int timeInMinutes, Robots robots, Minerals minerals)> toProcess = new();
    var finalResults = new List<int>();
    toProcess.Enqueue((0, new Robots(ore: 1), new Minerals()));
    var processed = new HashSet<(int time, Robots robots, Minerals minerals)>();
    while (toProcess.Count > 0)
    {
      var hasNextRelevantEvent = false;
      var currentState = toProcess.Dequeue();
      if(processed.Contains(currentState))
      {
        continue;
      }
      processed.Add(currentState);
      // Meaningful steps:
      // 1) Compute the next time we would be able to produce a geode robot
      if(currentState.robots.obsidian > 0)
      {
        var missingOresForGeode = Math.Max((double) bp.geodeRobotCostOre - currentState.minerals.ore, 0);
        var timeRequiredForGeodeOre = Math.Ceiling(missingOresForGeode/currentState.robots.ore);   
        
        var missingObsidiansForGeode = Math.Max((double) bp.geodeRobotCostObsidian - currentState.minerals.obsidian, 0);
        var timeRequiredForGeodeObsidian = Math.Ceiling(missingObsidiansForGeode/currentState.robots.obsidian);

        var timeRequiredForRobot = (int) Math.Max(timeRequiredForGeodeOre, timeRequiredForGeodeObsidian) + 1;
        if(currentState.timeInMinutes + timeRequiredForRobot < timeLimitToProcessInMinutes)
        {
          toProcess.Enqueue((
            currentState.timeInMinutes + timeRequiredForRobot,
            currentState.robots with { geode = currentState.robots.geode + 1 },
            currentState.minerals with
            {
              ore = (currentState.minerals.ore + currentState.robots.ore * timeRequiredForRobot) - bp.geodeRobotCostOre,
              clay = (currentState.minerals.clay + currentState.robots.clay * timeRequiredForRobot),
              obsidian = (currentState.minerals.obsidian + currentState.robots.obsidian * timeRequiredForRobot) - bp.geodeRobotCostObsidian,
              geode = (currentState.minerals.geode + currentState.robots.geode * timeRequiredForRobot)
            }
          ));
          hasNextRelevantEvent = true;
        }
      }

      // 2) Compute the next time we would be able to produce an obsidian robot if required
      if(currentState.robots.obsidian < bp.geodeRobotCostObsidian && currentState.robots.clay > 0)
      {
        var missingOresForObsidian = Math.Max((double) bp.obsidianRobotCostOre - currentState.minerals.ore, 0);
        var timeRequiredForObsidianOre = Math.Ceiling(missingOresForObsidian/currentState.robots.ore);   
        
        var missingClayForGeode = Math.Max((double) bp.obsidianRobotCostClay - currentState.minerals.clay, 0);
        var timeRequiredForObsidianClay = Math.Ceiling(missingClayForGeode/currentState.robots.clay);

        var timeRequiredForRobot = (int) Math.Max(timeRequiredForObsidianOre, timeRequiredForObsidianClay) + 1;
        // It is only worth to create a obsidian robot having 3 minutes before the end of the time limit:
        // 1 minute to create robot (already accounted) 1 minute to harvest the additional obsidian 1 minute to create the geode robot 1 minute to harvest the additional geode 
        if(currentState.timeInMinutes + timeRequiredForRobot <= timeLimitToProcessInMinutes - 3)
        {
          toProcess.Enqueue((
            currentState.timeInMinutes + timeRequiredForRobot,
            currentState.robots with { obsidian = currentState.robots.obsidian + 1 },
            currentState.minerals with
            {
              ore = (currentState.minerals.ore + currentState.robots.ore * timeRequiredForRobot) - bp.obsidianRobotCostOre,
              clay = (currentState.minerals.clay + currentState.robots.clay * timeRequiredForRobot) - bp.obsidianRobotCostClay,
              obsidian = (currentState.minerals.obsidian + currentState.robots.obsidian * timeRequiredForRobot),
              geode = (currentState.minerals.geode + currentState.robots.geode * timeRequiredForRobot)
            }
          ));
          hasNextRelevantEvent = true;
        }
      }
      
      // 3) Compute the next time we would be able to produce a clay robot if required
      // Clay is only required to construct obsidian robots, so we check if we are producing either obsidian or clay already
      if(currentState.robots.clay < bp.obsidianRobotCostClay && currentState.robots.obsidian < bp.geodeRobotCostObsidian)
      {
        var missingOresForClay = Math.Max((double) bp.clayRobotCostOre - currentState.minerals.ore, 0);
        var timeRequiredForRobot = (int) Math.Ceiling(missingOresForClay/currentState.robots.ore) + 1;   
        
        // It is only worth to create a clay robot 6 minutes before the end of the time limit:
        // 1 minute to create robot (already accounted) 1 minute to harvest the additional clay 1 minute to create the obsidian robot 1 minute to harvest the additional obsidian 1 minute to create the geode robot 1 minute to harvest the additional geode 
        if(currentState.timeInMinutes + timeRequiredForRobot <= timeLimitToProcessInMinutes - 5)
        {
          toProcess.Enqueue((
            currentState.timeInMinutes + timeRequiredForRobot,
            currentState.robots with { clay = currentState.robots.clay + 1 },
            currentState.minerals with
            {
              ore = (currentState.minerals.ore + currentState.robots.ore * timeRequiredForRobot) - bp.clayRobotCostOre,
              clay = (currentState.minerals.clay + currentState.robots.clay * timeRequiredForRobot),
              obsidian = (currentState.minerals.obsidian + currentState.robots.obsidian * timeRequiredForRobot),
              geode = (currentState.minerals.geode + currentState.robots.geode * timeRequiredForRobot)
            }
          ));
          hasNextRelevantEvent = true;
        }
      }

      // 4) Compute the next time we would be able to produce an ore robot if required
      if(currentState.robots.ore < bp.oreRobotCostOre ||
         ( currentState.robots.ore < bp.obsidianRobotCostOre && currentState.robots.obsidian < bp.geodeRobotCostObsidian) ||
         ( currentState.robots.ore < bp.clayRobotCostOre && currentState.robots.clay < bp.obsidianRobotCostClay) || 
         currentState.robots.ore < bp.geodeRobotCostOre)
      {
        double missingOresForOre = Math.Max(bp.oreRobotCostOre - currentState.minerals.ore, 0);
        var timeRequiredForRobot = (int) Math.Ceiling(missingOresForOre/currentState.robots.ore) + 1;   
        
        // It is only worth to create an ore robot 4 minutes before the end of the time limit:
        // 1 minute to create robot (already accounted) 1 minute to harvest the additional ore 1 minute to create the geode robot 1 minute to harvest geode
        if(currentState.timeInMinutes + timeRequiredForRobot <= timeLimitToProcessInMinutes - 3)
        {
          toProcess.Enqueue((
            currentState.timeInMinutes + timeRequiredForRobot,
            currentState.robots with { ore = currentState.robots.ore + 1 },
            currentState.minerals with
            {
              ore = (currentState.minerals.ore + currentState.robots.ore * timeRequiredForRobot) - bp.oreRobotCostOre,
              clay = (currentState.minerals.clay + currentState.robots.clay * timeRequiredForRobot),
              obsidian = (currentState.minerals.obsidian + currentState.robots.obsidian * timeRequiredForRobot),
              geode = (currentState.minerals.geode + currentState.robots.geode * timeRequiredForRobot)
            }
          ));
          hasNextRelevantEvent = true;
        }
      }

      if(!hasNextRelevantEvent)
      {
        var additionalGeodes = (timeLimitToProcessInMinutes - currentState.timeInMinutes) * currentState.robots.geode;
        finalResults.Add(currentState.minerals.geode + additionalGeodes);
      }

    }
    return finalResults.Max();
  }
}
