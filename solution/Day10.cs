class Day10 : IDayCommand
{
  const int WIDTH = 40; 

  public string Execute()
  {
    var instructions = new FileReader(10).Read().Select(line => {
      var items = line.Split(" ");
      return (name: items[0], value: items.Count() > 1? int.Parse(items[1]) : 0);
    }).ToList();

    var screenOutput = "#";

    var registerX = 1;
    var strength = new Dictionary<int, int>() { {20, 0}, {60, 0}, {100, 0}, {140, 0}, {180, 0}, {220, 0} };  
    var cycle = 1;
    var pixel = 0;

    foreach (var instruction in instructions)
    {
      (screenOutput, cycle, pixel) = RunCycle(registerX, strength, cycle, pixel, screenOutput);
      if (instruction.name == "noop") continue;
      registerX += instruction.value;
      (screenOutput, cycle, pixel) = RunCycle(registerX, strength, cycle, pixel, screenOutput);
    }
    screenOutput = string.Join("\n", screenOutput.Chunk(WIDTH).Select(chunk => string.Join("", chunk)));

    return $"""
    The sum of the strengths is {strength.Values.Sum()}
    {screenOutput}
    """;

  }

  private (string screenOutput,int cycle, int pixel) RunCycle(int registerX, Dictionary<int, int> strength, int cycle, int pixel, string screenOutput)
  {
    var newCycle = cycle + 1;
    var newPixel = pixel + 1;
    if (strength.ContainsKey(newCycle))
    {
      strength[newCycle] = newCycle * registerX;
    }
    var column = newPixel % WIDTH;
    var newScreenOutput = screenOutput + (Math.Abs(column - registerX) <= 1 ? "#" : ".");
    return (newScreenOutput, newCycle, newPixel);
  }
}
