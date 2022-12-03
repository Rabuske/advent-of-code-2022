class Day03 : IDayCommand
{

  public int GetPriority(char item)
  {
    var priority = item - 96;
    return priority <= 0 ? priority + 58 : priority;
  }

  public string Execute()
  {
    var compartments = new FileReader(3).Read().Select(line => line.Chunk(line.Length / 2).ToList()).ToList();
    var reducedCompartments = compartments
      .Select(compartment => (
        compartment1: compartment[0].ToHashSet(),
        compartment2: compartment[1].ToHashSet()))
      .ToList();

    // Part 01
    var totalPriorityPart01 = reducedCompartments
      .Select(compartment => compartment.compartment1.Where(item => compartment.compartment2.Contains(item)).First())
      .Sum(item => GetPriority(item));

    // Part 02
    var totalPriorityPart02 = reducedCompartments.Chunk(3).Select(sackGroup => {
      var combinedItems1 = sackGroup[0].compartment1.Concat(sackGroup[0].compartment2).ToHashSet();
      var combinedItems2 = sackGroup[1].compartment1.Concat(sackGroup[1].compartment2).ToHashSet();
      var combinedItems3 = sackGroup[2].compartment1.Concat(sackGroup[2].compartment2).ToHashSet();
      return combinedItems1.Where(item => combinedItems2.Contains(item) && combinedItems3.Contains(item))
                           .Sum(item => GetPriority(item));
    }).Sum();

    return $"Total part01 is {totalPriorityPart01} total part 02 is {totalPriorityPart02}";
  }
}
