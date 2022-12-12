class Day12 : IDayCommand
{
  public string Execute()
  {
    var startingPositionPart1 = new Node<char>('a');
    var endPosition = new Node<char>('z');
    var nodes = new FileReader(12).Read().Select(line => line.Select(c => 
    c switch {
      'S' => startingPositionPart1,
      'E' => endPosition,
      _ => new Node<char>(c),
    }));
    var map = new Map<char>(nodes);

    // Filter adjacent whose differences is bigger than one
    map.Nodes.ToList().ForEach(node =>
    {
      var toRemove = node.AdjacentNodes.Where(adj => (adj.Value - node.Value) > 1).ToList();
      toRemove.ForEach(nodeToRemove => node.AdjacentNodes.Remove(nodeToRemove));
    });

    // Set the travel cost to a fixed value (1)
    map.Nodes.ToList().ForEach(node => node.GetTravelCost = (current, adjacent) => 1);
    
    // Part 01
    var fewestStepsPathPart1 = map.GetOptimalPath(startingPositionPart1, endPosition).Count() - 1;

    // Part 02
    var startingPositions = map.Nodes.Where(node => node.Value == 'a');
    var fewestStepsPathPart2 = startingPositions.Select(sp => map.GetOptimalPath(sp, endPosition).Count() - 1)
      .Where(count => count > 0)
      .OrderBy(a => a)
      .First();

    return $"The fewest steps required from starting position is {fewestStepsPathPart1} and from any 'a' location is {fewestStepsPathPart2}";
  }
}
