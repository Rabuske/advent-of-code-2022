using System.Numerics;
using TwoDigitOperation = System.Func<System.Numerics.BigInteger, System.Numerics.BigInteger, System.Numerics.BigInteger>;

class MonkeyDay21
{
  public String Id {get; set;} = string.Empty;
  public BigInteger YellingContent {get; set;} = int.MinValue; 
  
  public string DependencyLeft {get; set;} = string.Empty;
  public string DependencyRight {get; set;} = string.Empty;
  public Dictionary<string, TwoDigitOperation> Operations {get; set;} = new();

  public virtual bool TryComputeYellingContent(Dictionary<string, MonkeyDay21> monkeys, out BigInteger content)
  {
    content = YellingContent;
    if(YellingContent != int.MinValue)
    {
      return true;
    }
    var canComputeLeft = monkeys[DependencyLeft].TryComputeYellingContent(monkeys, out var valueLeft);
    var canComputeRight = monkeys[DependencyRight].TryComputeYellingContent(monkeys, out var valueRight);
    if(!canComputeLeft || !canComputeRight) return false;
    content = Operations["do"](valueLeft, valueRight);
    return true;
  }

  public virtual BigInteger ComputeMissingValue(BigInteger wantedValue, Dictionary<string, MonkeyDay21> monkeys)
  {
    if(YellingContent != int.MinValue) 
    {
      return YellingContent;
    }

    var canComputeLeft = monkeys[DependencyLeft].TryComputeYellingContent(monkeys, out var valueLeft);
    var canComputeRight = monkeys[DependencyRight].TryComputeYellingContent(monkeys, out var valueRight);

    if(!canComputeLeft)
    {
      var expectedValue = Operations["undoLeft"](wantedValue, valueRight);
      return monkeys[DependencyLeft].ComputeMissingValue(expectedValue, monkeys);
    }

    if(!canComputeRight)
    {
      var expectedValue = Operations["undoRight"](wantedValue, valueLeft);
      return monkeys[DependencyRight].ComputeMissingValue(expectedValue, monkeys);
    }
    return YellingContent; // Should not happen 
  }
}

class FakeMonkey : MonkeyDay21
{
  public override bool TryComputeYellingContent(Dictionary<string, MonkeyDay21> monkeys, out BigInteger content)
  {
    content = 0;
    return false;
  }

  public override BigInteger ComputeMissingValue(BigInteger wantedValue, Dictionary<string, MonkeyDay21> monkeys)
  {
    return wantedValue;
  }
}

class Day21 : IDayCommand
{
  public string Execute()
  {
    Dictionary<string, MonkeyDay21> monkeys = Parse(new FileReader(21).Read());
    

    // Part 01
    monkeys["root"].TryComputeYellingContent(monkeys, out var rootYells);

    // Part 02
    monkeys["humn"] = new FakeMonkey(){};
    var rootMonkey = monkeys["root"];

    var canComputeRight = monkeys[rootMonkey.DependencyRight].TryComputeYellingContent(monkeys, out var knownValueForRootRight);      
    var canComputeLeft = monkeys[rootMonkey.DependencyLeft].TryComputeYellingContent(monkeys, out var knownValueForRootLeft);      
    var knownValue = canComputeRight? knownValueForRootRight : knownValueForRootLeft;
    var unknownPathStartMonkey = canComputeRight? rootMonkey.DependencyLeft : rootMonkey.DependencyRight;
    var missingValue = monkeys[unknownPathStartMonkey].ComputeMissingValue(knownValue, monkeys);

    return $"Root yells {rootYells}. I should yell {missingValue}";
  }

  private static Dictionary<string, MonkeyDay21> Parse(IEnumerable<string> lines)
  {
    return lines.Select(line =>
    {
      var nameAndContent = line.Split(":");
      var couldParse = int.TryParse(nameAndContent[1].Trim(), out var number);
      if (couldParse)
      {
        return new MonkeyDay21() { Id = nameAndContent[0], YellingContent = number };
      }
      var operLeft = nameAndContent[1].Trim()[..4];
      var operRight = nameAndContent[1].Trim()[^4..];

      var sum = new Dictionary<string, TwoDigitOperation>()
      {
        {"do", (left, right) => left + right},
        {"undoLeft", (wanted, right) => wanted - right},
        {"undoRight", (wanted, left) => wanted - left},
      };

      var subtraction = new Dictionary<string, TwoDigitOperation>()
      {
        {"do", (left, right) => left - right},
        {"undoLeft", (wanted, right) => wanted + right },
        {"undoRight", (wanted, left) => left - wanted},
      };      

      var multiplication = new Dictionary<string, TwoDigitOperation>()
      {
        {"do", (left, right) => left * right},
        {"undoLeft", (wanted, right) => wanted / right },
        {"undoRight", (wanted, left) => wanted / left },
      };

      var division = new Dictionary<string, TwoDigitOperation>()
      {
        {"do", (left, right) => left / right},
        {"undoLeft", (wanted, right) => wanted * right },
        {"undoRight", (wanted, left) => left / wanted},
      };                 

      Dictionary<string, TwoDigitOperation> operations = nameAndContent[1].Trim()[5] switch
      {
        '*' => multiplication,
        '+' => sum,
        '-' => subtraction,
        '/' => division,
        _ => throw new NotSupportedException()
      };
      return new MonkeyDay21()
      {
        DependencyLeft = operLeft,
        DependencyRight = operRight,
        Operations = operations,
        Id = nameAndContent[0]
      };
    }).ToDictionary(m => m.Id, m => m);
  }
}
