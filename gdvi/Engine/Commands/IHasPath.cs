using gdvi.Models;

namespace gdvi.Engine.Commands;

public interface IHasPath
{
    public List<Territory> Path { get; set; }
}