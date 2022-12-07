interface IFileSystemNode
{
  public string Name { get; init; }
  public HashSet<IFileSystemNode> Children { get; init; }
  public long CalculateSize();
  public bool IsDirectory { get; }
  public IFileSystemNode? Parent { get; set; }
}

class Directory : IFileSystemNode
{
  public string Name { get; init; } = "";
  public HashSet<IFileSystemNode> Children { get; init; } = new();
  public IFileSystemNode? Parent { get; set; }
  private long _sizeCache = -1;

  public long CalculateSize()
  {
    if (_sizeCache == -1)
    {
      _sizeCache = Children.Sum(child => child.CalculateSize());
    }
    return _sizeCache;
  }
  public bool IsDirectory => true;
}

class File : IFileSystemNode
{
  public string Name { get; init; } = "";
  public long Size { get; set; }
  public HashSet<IFileSystemNode> Children { get; init; } = new();
  public IFileSystemNode? Parent { get; set; }

  public long CalculateSize() => Size;
  public bool IsDirectory => false;
}

class FileSystem
{
  public IFileSystemNode Root { get; init; } = new Directory() { Name = "/" };
  public HashSet<IFileSystemNode> Nodes = new();
  private IFileSystemNode _currentNode { get; set; }

  public FileSystem()
  {
    _currentNode = Root;
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
      "dir" => (IFileSystemNode)new Directory() { Name = words[1], Parent = _currentNode },
      _ => (IFileSystemNode)new File() { Size = long.Parse(words[0]), Name = words[1], Parent = _currentNode }
    };

    var existingNode = _currentNode.Children.FirstOrDefault(node => node.Name == words[1]);

    if (existingNode is not null)
    {
      newNode = existingNode;
    }

    _currentNode.Children.Add(newNode);
    Nodes.Add(newNode);
  }

  private void _processCurrentDir(string[] words)
  {
    _currentNode = words[2] switch
    {
      "/" => _currentNode = Root,
      ".." => _currentNode.Parent ?? Root,
      _ => _currentNode.Children.FirstOrDefault(child => child.Name == words[2]) ?? _currentNode,
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
      .Where(node => node.IsDirectory)
      .Select(node => node.CalculateSize())
      .ToList();

    var resultPart01 = fileSystem.Nodes
      .Where(node => node.IsDirectory)
      .Select(node => node.CalculateSize())
      .Where(size => size <= 100000)
      .Sum();

    var totalSpaceUsed = fileSystem.Root.CalculateSize();
    var spaceThatNeedsToBeFreed = totalSpaceUsed - (70000000 - 30000000);
    
    var smallestDirectoryToDelete = fileSystem.Nodes
      .Where(node => node.CalculateSize() >= spaceThatNeedsToBeFreed)
      .OrderBy(node => node.CalculateSize())
      .First();
    
    return $"""
            The total size of directories with most 100000 of size is {resultPart01}.
            the smallest dir that needs to be delete has size of {smallestDirectoryToDelete.CalculateSize()}
           """;

  }
}
