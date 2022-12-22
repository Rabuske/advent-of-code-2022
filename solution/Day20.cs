class Day20 : IDayCommand
{
  public string Execute()
  {
    var originalNumbersPart01 = new FileReader(20).Read().Select((line, index) => (value: long.Parse(line), index: index)).ToList();
    var originalNumbersPart02 = originalNumbersPart01.Select(n => (n.value * 811589153L, n.index)).ToList();
    var orderedListPart01 = new LinkedList<(long value, int originalIndex)>(originalNumbersPart01);
    var orderedListPart02 = new LinkedList<(long value, int originalIndex)>(originalNumbersPart02);
    
    // Part 01
    Encrypt(originalNumbersPart01, orderedListPart01);
    long resultPart01 = CalculateResults(orderedListPart01);
    
    // Part 02
    for (int mixingTime = 0; mixingTime < 10; mixingTime++)
    {
      Encrypt(originalNumbersPart02, orderedListPart02);      
    }
    long resultPart02 = CalculateResults(orderedListPart02);

    return $"The result is {resultPart01}, using the decryption key: {resultPart02}";
  }

  private long CalculateResults(LinkedList<(long value, int originalIndex)> orderedList)
  {
    var n1000th = 0L;
    var n2000th = 0L;
    var n3000th = 0L;
    var zeroValueAndIndex = orderedList.Where(n => n.value == 0).First();
    var zeroElement = orderedList.Find(zeroValueAndIndex) ?? new(zeroValueAndIndex);

    for (int index = 1; index <= 3000; index++)
    {
      zeroElement = zeroElement.Next ?? orderedList.First ?? new(zeroValueAndIndex);
      if (index == 1000)
      {
        n1000th = zeroElement.Value.value;
      }
      if (index == 2000)
      {
        n2000th = zeroElement.Value.value;
      }
      if (index == 3000)
      {
        n3000th = zeroElement.Value.value;
      }
    }
    var result = n1000th + n2000th + n3000th;
    return result;
  }

  private static void Encrypt(List<(long value, int index)> originalNumbers, LinkedList<(long value, int originalIndex)> orderedList)
  {
    for (int index = 0; index < originalNumbers.Count; index++)
    {
      var originalNumber = originalNumbers[index];
      var elementNode = orderedList.Find(originalNumber);
      if(elementNode is null) throw new NullReferenceException();
      var numberOfMoves = originalNumber.value % (originalNumbers.Count - 1);
      if (numberOfMoves == 0) continue;
      if (numberOfMoves < 0)
      {
        var currentElement = elementNode.Previous ?? orderedList.Last ?? new(originalNumber);
        orderedList.Remove(elementNode);
        for (int numberOfRepetitions = 1; numberOfRepetitions < Math.Abs(numberOfMoves); numberOfRepetitions++)
        {
          currentElement = currentElement.Previous ?? orderedList.Last ?? new(originalNumber);
        }
        orderedList.AddBefore(currentElement, elementNode);
      }
      else if (numberOfMoves > 0)
      {
        var currentElement = elementNode.Next ?? orderedList.First ?? new(originalNumber);
        orderedList.Remove(elementNode);
        for (int numberOfRepetitions = 1; numberOfRepetitions < numberOfMoves; numberOfRepetitions++)
        {
          currentElement = currentElement.Next ?? orderedList.First ?? new(originalNumber);
        }
        orderedList.AddAfter(currentElement, elementNode);
      }
    }
  }
}
