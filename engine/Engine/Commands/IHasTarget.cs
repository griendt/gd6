using engine.Models;

namespace engine.Engine.Commands;

public interface IHasTarget
{
    public Territory Target { get; set; }
}