class Day01 : IDayCommand
{
  public string Execute()
  {
    var lines = new FileReader(01).Read();
    List<int> meals = new();

    int currentMeal = 0;
    foreach (var line in lines)
    {
      if(string.IsNullOrEmpty(line))
      {
        meals.Add(currentMeal);
        currentMeal = 0;
        continue;
      }
      currentMeal += int.Parse(line);
    }
    
    meals = meals.OrderByDescending(c => c).ToList();
    var mostCalories = meals.First();
    var topThreeMostCalories = meals.Take(3).Sum();

    return $"The elf carrying most calories carries {mostCalories} cal and the three top carry {topThreeMostCalories} cal";
  }
}