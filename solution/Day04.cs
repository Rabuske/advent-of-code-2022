class Day04 : IDayCommand
{
  public string Execute()
  {
    var elves = new FileReader(04).Read()
                    .Select(line => line.Split(",")
                      .Select(item => item.Split("-").Select(number => int.Parse(number)))
                      .Select(range => (min: range.First(), max: range.Last()))
                      .ToList())
                    .ToList();

    var numberOfFullyOverlapping = elves.Where(elfPair => (elfPair[0].min <= elfPair[1].min && elfPair[0].max >= elfPair[1].max) 
                                                        ||(elfPair[1].min <= elfPair[0].min && elfPair[1].max >= elfPair[0].max)).Count();
    
    var numberOfOverlapping = elves.Where(elfPair => !(elfPair[0].max < elfPair[1].min || elfPair[0].min > elfPair[1].max)).Count();

    return $"The number of pair with full overlapping entries is {numberOfFullyOverlapping} the number of partial overlapping is {numberOfOverlapping}";

  }
}
