class Day05 : IDayCommand
{
  public string Execute()
  {
    var lines = new FileReader(5).Read().ToList();

    var stacks = ExtractStacks(lines);
    var instructions = ExtractInstructions(lines);

    // Part 01
    instructions.ForEach(instruction => {
      var from = stacks[instruction[1] - 1];
      var to = stacks[instruction[2] - 1];
      Enumerable.Range(0, instruction[0]).ToList().ForEach(_ => {
        to.Push(from.Pop());
      });
    });
    var messagePart01 = string.Join("", stacks.Select(s => s.Peek()));

    // Part 02 
    stacks = ExtractStacks(lines);
    instructions.ForEach(instruction => {
      var from = stacks[instruction[1] - 1];
      var to = stacks[instruction[2] - 1];
      var temporaryStack = new Stack<char>();
      Enumerable.Range(0, instruction[0]).ToList().ForEach(_ => {
        temporaryStack.Push(from.Pop());
      });
      while(temporaryStack.Count > 0)
      {
        to.Push(temporaryStack.Pop());
      }
    });
    var messagePart02 = string.Join("", stacks.Select(s => s.Peek()));

    return $"The message using crane 9000 is {messagePart01} and using using crane 9001 is {messagePart02}";

  }

  private List<int[]> ExtractInstructions(List<string> lines)
  {
    return lines.SkipWhile(line => string.IsNullOrEmpty(line) || line.StartsWith("[") || line.StartsWith(" "))
                .Select(line => line.Replace("move ", "").Replace("from ", "").Replace("to ", "").Split(" "))
                .Select(arrayOfNumbers => arrayOfNumbers.Select(item => int.Parse(item)).ToArray())
                .ToList();
  }

  private List<Stack<char>> ExtractStacks(List<string> lines)
  {
    // Find the number of stacks
    var numberOfStacks = 0;
    lines.Find(line => int.TryParse(line.TrimEnd().Last().ToString(), out numberOfStacks))?.FirstOrDefault();
    
    // Parse the stacks
    var stacks = Enumerable.Range(0, numberOfStacks).Select(_ => new Stack<Char>()).ToList();
    foreach (var line in lines)
    {
      if(string.IsNullOrEmpty(line)) break;
      var onlyCrates = line.Chunk(4).Select(crate => crate[1]).ToArray();
      for (int indexOfCrate = 0; indexOfCrate < onlyCrates.Length; indexOfCrate++)
      {
        if(char.IsAsciiLetterUpper(onlyCrates[indexOfCrate]))
        {
          stacks[indexOfCrate].Push(onlyCrates[indexOfCrate]);
        }
      }
    }
    var resultingStacks = stacks.Select(stack => stack.Aggregate(new Stack<char>(), (reversed, crate) => { reversed.Push(crate); return reversed;})).ToList();
    return resultingStacks;
  }
}
