class PacketComparer : IComparer<List<Object>>
{
  public int Compare(List<object>? first, List<object>? second)
  {
    if (first is null || second is null) return 0;
    int currentIndex = 0;
    while (true)
    {
      if (currentIndex >= first.Count && currentIndex >= second.Count)
      {
        return 0;
      }

      if (currentIndex >= first.Count)
      {
        return -1;
      }

      if (currentIndex >= second.Count)
      {
        return 1;
      }

      if (first[currentIndex] is int && second[currentIndex] is int)
      {
        var firstAsInt = (int)first[currentIndex];
        var secondAsInt = (int)second[currentIndex];
        if (firstAsInt < secondAsInt)
        {
          return -1;
        }
        if (firstAsInt > secondAsInt)
        {
          return 1;
        }
        currentIndex++;
        continue;
      }

      var firstAsList = first[currentIndex];
      var secondAsList = second[currentIndex];

      if (firstAsList is int)
      {
        firstAsList = new List<Object>() { firstAsList };
      }

      if (secondAsList is int)
      {
        secondAsList = new List<Object>() { secondAsList };
      }

      var result = Compare((List<Object>)firstAsList, (List<Object>)secondAsList);
      if (result != 0) return result;
      currentIndex++;
    }
  }
}

class Day13 : IDayCommand
{
  public string Execute()
  {
    var pairsOfPacketsAsStrings = new FileReader(13).Read().Chunk(3);
    var pairs = new List<(List<Object> first, List<Object> second)>();

    var allPackets = new List<List<Object>>();
    foreach (var pairOfPacketAsString in pairsOfPacketsAsStrings)
    {
      var first = Parse(pairOfPacketAsString[0]);
      var second = Parse(pairOfPacketAsString[1]);
      pairs.Add((first, second));
      allPackets.Add(first);
      allPackets.Add(second);
    }

    // Part 01
    var comparer = new PacketComparer();
    var resultPart01 = pairs.Select((pair, index) =>
    {
      if (comparer.Compare(pair.first, pair.second) == -1) return index + 1;
      return 0;
    }).Sum();

    // Part 02
    var divider1 = new List<Object>() { new List<Object>() { 2 } };
    var divider2 = new List<Object>() { new List<Object>() { 6 } };
    allPackets.Add(divider1);
    allPackets.Add(divider2);
    var sorted = allPackets.OrderBy(packet => packet, comparer).ToList();

    var indexDivider1 = sorted.FindIndex(packet => packet == divider1) + 1;
    var indexDivider2 = sorted.FindIndex(packet => packet == divider2) + 1;

    return 
      $"""
      The sum of the indexes of ordered pairs is {resultPart01}. 
      After sorting everything the multiplication of the indexes of the dividers is {indexDivider1 * indexDivider2}
      """;
  }

  private List<Object> Parse(string line)
  {
    var listStack = new Stack<List<Object>>();
    var currentNumberAsString = string.Empty;
    foreach (var c in line)
    {
      switch (c)
      {
        case '[':
          var newList = new List<object>();
          if (listStack.Count > 0)
          {
            listStack.Peek().Add(newList);
          }
          listStack.Push(newList);
          break;
        case ']':
          {
            var currentList = listStack.Pop();
            if (int.TryParse(currentNumberAsString, out var number))
            {
              currentList.Add(number);
              currentNumberAsString = string.Empty;
            }
            if (listStack.Count == 0) return currentList;
          }
          break;
        case ',':
          {
            var currentList = listStack.Peek();
            if (int.TryParse(currentNumberAsString, out var number))
            {
              currentList.Add(number);
              currentNumberAsString = string.Empty;
            }
          }
          break;
        default:
          currentNumberAsString += c;
          break;
      }
    }
    return new();
  }
}