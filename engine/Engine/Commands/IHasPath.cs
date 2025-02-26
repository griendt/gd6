using engine.Models;

namespace engine.Engine.Commands;

public interface IHasPath
{
    public List<Territory> Path { get; set; }
}