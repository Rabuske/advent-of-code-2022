class Day06 : IDayCommand
{
  public string Execute()
  {
    var input = new FileReader(06).Read().First();

    var packetChecker = new Queue<char>(Enumerable.Range(0,4).Select(i => input[i]));
    var messageChecker = new Queue<char>(Enumerable.Range(0,14).Select(i => input[i]));
    var firstPacket = 0;
    var firstMessage = 0;
    for (int i = 4; i < input.Length; i++)
    {
      packetChecker.Dequeue();
      packetChecker.Enqueue(input[i]);

      if(i >= 14) {
        messageChecker.Dequeue();
        messageChecker.Enqueue(input[i]);
      }

      if(firstPacket == 0 && packetChecker.Distinct().Count() == 4){
        firstPacket = i + 1;
      };

      if(messageChecker.Distinct().Count() == 14)
      {
        firstMessage = i + 1;
        break;
      }
    }

    return $"The start of the first packet is at {firstPacket} and the first message is {firstMessage}";
  }
}
