using gd6.Models;

namespace gd6.Engine.Commands;

public interface IHasTarget
{
    public Territory Target { get; set; }
}