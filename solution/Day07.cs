interface IFileSystemNode
{
  public string Name { get; init; }
  public long Size {get;}
  public Directory? Parent { get; set; }
}

class Directory : IFileSystemNode
{
  public string Name { get; init; } = "";
  public HashSet<IFileSystemNode> Children { get; init; } = new();
  public Directory? Parent { get; set; }
  public long Size => Children.Sum(child => child.Size);
}

record File(string Name, long Size) : IFileSystemNode
{
  public Directory? Parent { get; set; }

  public long CalculateSize() => Size;
}

class FileSystem
{
  public Directory Root { get; init; } = new Directory() { Name = "/" };
  public HashSet<IFileSystemNode> Nodes = new();
  private Directory _currentDirectory { get; set; }

  public FileSystem()
  {
    _currentDirectory = Root;
  }

  public void ProcessCommand(string command)
  {
    var words = command.Split(" ");
    if (words[0] == "$" && words[1] == "cd")
    {
      _processCurrentDir(words);
    }

    if (words[0] == "$") return;

    _processListResult(words);
  }

  private void _processListResult(string[] words)
  {
    var newNode = words[0] switch
    {
      "dir" => (IFileSystemNode)new Directory() { Name = words[1], Parent = _currentDirectory },
      _ => (IFileSystemNode)new File(words[1], long.Parse(words[0])) { Parent = _currentDirectory }
    };

    var existingNode = _currentDirectory.Children.FirstOrDefault(node => node.Name == words[1]);

    if (existingNode is not null)
    {
      newNode = existingNode;
    }

    _currentDirectory.Children.Add(newNode);
    Nodes.Add(newNode);
  }

  private void _processCurrentDir(string[] words)
  {
    _currentDirectory = words[2] switch
    {
      "/" => _currentDirectory = Root,
      ".." => _currentDirectory.Parent ?? Root,
      _ => (Directory) (_currentDirectory.Children.FirstOrDefault(child => child.Name == words[2] && child is Directory) ?? _currentDirectory),
    };
  }
}

class Day07 : IDayCommand
{
  public string Execute()
  {
    var lines = new FileReader(07).Read().ToList();
    var fileSystem = new FileSystem();

    lines.ForEach(line => fileSystem.ProcessCommand(line));

    var sizes = fileSystem.Nodes
      .Where(node => node is Directory)
      .Select(node => node.Size)
      .ToList();

    var resultPart01 = fileSystem.Nodes
      .Where(node => node is Directory)
      .Select(node => node.Size)
      .Where(size => size <= 100000)
      .Sum();

    var totalSpaceUsed = fileSystem.Root.Size;
    var spaceThatNeedsToBeFreed = totalSpaceUsed - (70000000 - 30000000);
    
    var smallestDirectoryToDelete = fileSystem.Nodes
      .Where(node => node.Size >= spaceThatNeedsToBeFreed)
      .OrderBy(node => node.Size)
      .First();
    
    return $"""
            The total size of directories with most 100000 of size is {resultPart01}.
            the smallest dir that needs to be delete has size of {smallestDirectoryToDelete.Size}
           """;

  }
}
