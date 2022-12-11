class Operand
{
  public long Value {get; set;} = -1;
  public bool UsesOld {get; set;} = false;
  public long GetValue(long old) 
  {
    if(UsesOld) return old;
    return Value;
  }

  public static Operand GetFromString(string operand)
  {
    if(operand == "old")
    {
      return new Operand(){ UsesOld = true};
    }
    return new Operand(){ Value = long.Parse(operand) };
  }
}

class Monkey
{
  public List<long> Items {get; set;} = new();
  public Func<long, long, long> Operation {get; init;}
  public Operand Operand1 {get; init;}
  public Operand Operand2 {get; init;}
  public Dictionary<bool, int> TestOutcomes {get; set;} = new();
  public long DivisibilityTest {get; set;} = new();
  public long NumberOfInspectedItems {get; set;} = 0;
  public Monkey(Func<long, long, long> operation, string operand1, string operand2)
  {
    Operand1 = Operand.GetFromString(operand1);
    Operand2 = Operand.GetFromString(operand2);
    Operation = operation;
  }
  public void TakeTurn(List<Monkey> monkeys, bool manageWorryLevel, long totalWorryLevel)
  {
    Items.ForEach(item => {
      NumberOfInspectedItems++;
      var newItem = Operation(Operand1.GetValue(item), Operand2.GetValue(item));
      if(manageWorryLevel) 
      {
        newItem = newItem / 3;
      }
      else
      {
        newItem = newItem % totalWorryLevel;
      }
      var targetMonkey = TestOutcomes[newItem % DivisibilityTest == 0];
      monkeys[targetMonkey].Items.Add(newItem);
    });
    Items = new();
  }
}

class Day11 : IDayCommand
{
  Func<long, long, long> Times = (a, b) => a * b; 
  Func<long, long, long> Plus = (a, b) => a + b; 
  private List<Monkey> ParseInput(IEnumerable<string> lines)
  {
    List<Monkey> monkeys = new();
    var monkeyIndex = -1;
    var items = new List<long>();
    var operand1 = string.Empty;
    var operand2 = string.Empty;
    var operation = Times;
    var divisibility = 0;
    var monkeyTestResults = new Dictionary<bool, int> {{true, -1} , {false, -1}};

    foreach (var line in lines)
    {
      if(line.StartsWith("Monkey"))
      { 
        monkeyIndex++;
        continue;
      }
      if(line.StartsWith("  Starting items: "))
      {
        items = line.Replace("  Starting items: ", "")
          .Split(", ")
          .Select(item => long.Parse(item))
          .ToList();
          continue;
      }
      if(line.StartsWith("  Operation:"))
      {
        var partsOfOperation = line.Replace("  Operation: new = ", "").Split(" ");
        operand1 = partsOfOperation[0];
        operand2 = partsOfOperation[2];
        operation = partsOfOperation[1] == "*" ? Times : Plus;
        continue;
      }
      if(line.StartsWith("  Test: divisible by "))
      {
        divisibility = int.Parse(line.Replace("  Test: divisible by ", ""));
        continue;
      }
      if(line.StartsWith("    If true"))
      {
        monkeyTestResults[true] = int.Parse(line.Replace("    If true: throw to monkey ", ""));
        continue;
      }
      if(line.StartsWith("    If false"))
      {
        monkeyTestResults[false] = int.Parse(line.Replace("    If false: throw to monkey ", ""));
        var monkey = new Monkey(operation, operand1, operand2)
        {
          DivisibilityTest = divisibility,
          TestOutcomes = new Dictionary<bool, int>(monkeyTestResults.AsEnumerable()),
          Items = items
        };
        monkeys.Add(monkey);
        continue;
      }
    }

    return monkeys;
  }
  public string Execute()
  {
    // Part 01
    var monkeys = ParseInput(new FileReader(11).Read());
    var totalWorryLevel = monkeys.Select(monkey => monkey.DivisibilityTest).Aggregate(1L, (w1, w2) => w1 * w2);

    for (long round = 0; round < 20; round++)
    {
      monkeys.ForEach(monkey => monkey.TakeTurn(monkeys, manageWorryLevel: true, totalWorryLevel));
    }

    var resultPart01 = monkeys.OrderByDescending(m => m.NumberOfInspectedItems)
      .Take(2)
      .Aggregate(1L , (total, m2) => total * m2.NumberOfInspectedItems);

    // Part 02
    monkeys = ParseInput(new FileReader(11).Read());

    for (long round = 0; round < 10000; round++)
    {
      monkeys.ForEach(monkey => monkey.TakeTurn(monkeys, manageWorryLevel: false, totalWorryLevel));
    }

    var resultPart02 = monkeys.OrderByDescending(m => m.NumberOfInspectedItems)
      .Take(2)
      .Aggregate(1L , (total, m2) => total * m2.NumberOfInspectedItems);

    return $"Monkey Business {resultPart01} is monkey business {resultPart02}";
  }
}
