using engine.Models;

namespace engine.Engine.Commands;

public interface IHasOrigin
{
    public Territory Origin { get; set; }
}