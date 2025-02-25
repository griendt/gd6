using gd6.Models;

namespace gd6.Engine.Commands;

public interface IHasPath
{
    public List<Territory> Path { get; set; }
}